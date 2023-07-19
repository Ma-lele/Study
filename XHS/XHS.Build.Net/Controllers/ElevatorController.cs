using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Fleck;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Common.Util;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Elevator;
using XHS.Build.Services.Employee;
using XHS.Build.Services.File;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 升降机
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class ElevatorController : ControllerBase
    {
        private readonly IElevatorService _elevatorService;
        private readonly ISpecialEqp _specialEqp;
        private readonly IConfiguration _configuration;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly HpAliSMS _hpAliSMS;
        private readonly IMapper _mapper;
        private readonly ISpecialEqpService _specialEqpService;
        private readonly ISpecialEqpAuthHisService _specialEqpAuthHisService;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IOperateLogService _operateLogService;
        private readonly IFleckSpecial _fleckSpecial;
        public ElevatorController(IElevatorService elevatorService, IFleckSpecial fleckSpecial, ISpecialEqp specialEqp, IConfiguration configuration, IHpSystemSetting hpSystemSetting, IMapper mapper, ISpecialEqpService specialEqpService, ISpecialEqpAuthHisService specialEqpAuthHisService, IHpFileDoc hpFileDoc, IOperateLogService operateLogService)
        {
            _elevatorService = elevatorService;
            _specialEqp = specialEqp;
            _configuration = configuration;
            _hpSystemSetting = hpSystemSetting;
            _hpAliSMS = new HpAliSMS(hpSystemSetting);
            _mapper = mapper;
            _specialEqpService = specialEqpService;
            _specialEqpAuthHisService = specialEqpAuthHisService;
            _hpFileDoc = hpFileDoc;
            _operateLogService = operateLogService;
            _fleckSpecial = fleckSpecial;
        }

        /// <summary>
        /// 设备上线
        /// </summary>
        /// <param name="secode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Online(string secode)
        {
            if (string.IsNullOrEmpty(secode))
            {
                return ResponseOutput.NotOk("请选择需要上线的设备", 0);
            }
            if (!_specialEqp.List.Keys.Contains(secode))
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(-27), -27);
            }
            var result = await _elevatorService.doCheckin(secode);

            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("设备上线失败", 0);
        }

        /// <summary>
        /// 设备下线
        /// </summary>
        /// <param name="secode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Offline(string secode)
        {
            if (string.IsNullOrEmpty(secode))
            {
                return ResponseOutput.NotOk("请选择需要下线的设备", 0);
            }
            if (!_specialEqp.List.Keys.Contains(secode))
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(-27), -27);
            }

            SpecialEqpBean seb = _specialEqp.List[secode];
            if (seb.waitcount > 0)
            {
                int WARNID = await _elevatorService.doPartInsert(secode, 1);

                seb.waitcount = 0;
                seb.warndate = DateTime.Now;
                _hpAliSMS.SendWarnById(WARNID, "");
            }

            var result = await _elevatorService.doCheckout(secode);

            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("设备下线失败", 0);
        }

        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="input">实时数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> RealData([FromBody] ElevatorRealDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.SeCode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            string secode = input.SeCode;

            if (!_specialEqp.List.Keys.Contains(secode))
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(-27), -27);
            }

            var seData = _mapper.Map<SpecialEqpData>(input);
            seData.Platform = "net";
            seData.Flag = 2;
            await _operateLogService.AddSpecialEqpDataLog(seData);

            SpecialEqpBean seb = _specialEqp.List[secode];
            short alarmState = Convert.ToInt16(input.AlarmState);
            // 重量限位:1,人数限位:2,倾角限位:512 只报警这3种
            if (seb.waitcount == 0 && ((((alarmState > 0 && alarmState <= 2) || alarmState >= 512)))
                && seb.warndate.AddMinutes(_configuration.GetSection("ElevatorWarnDelay").Value.ObjToInt()) < DateTime.Now)
            //if ((alarmState > 0 || seb.waitcount > 0) && seb.warndate.AddMinutes(ElevatorWarnDelay) < DateTime.Now)
            {//报警状态,并且距离上次报警时间满足时间间隔
             //第一次发出报警
                seb.waitcount = 1;
                seb.warndate = DateTime.Now;
            }
            else if (seb.waitcount > 0)
                seb.waitcount++;

            if (seb.waitcount >= Convert.ToInt32(_hpSystemSetting.getSettingValue(Const.Setting.S065)) - 10
                       || (seb.waitcount > 0 && seb.warndate.AddMinutes(5) < DateTime.Now))
            {//积攒到一定数量的实时数据,或等待超过5分钟了,就开始报警.
                int WARNID = await _elevatorService.doPartInsert(secode, 1);
                seb.waitcount = 0;
                _hpAliSMS.SendWarnById(WARNID, "");
            }
            SgParams sp = new SgParams();
            sp.Add("GROUPID", seb.GROUPID);
            sp.Add("secode", secode);
            sp.Add("setype",2);
            sp.Add("alarmstate", input.AlarmState);
            sp.Add("ID", input.DriverCardNo);
            sp.Add("hasreport", input.HasReport);
            sp.Add("sedata", JsonConvert.SerializeObject(input));
            sp.NeetReturnValue();
            int retProcedure = await _elevatorService.doRtdInsert(sp);
            if (retProcedure > 0)
            {
                if (_fleckSpecial != null && _fleckSpecial.SeCodes.Contains(secode))
                {
                    _fleckSpecial.Distpatch(input);
                }
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retProcedure), retProcedure);
            }
        }

        /// <summary>
        /// 设备参数数据
        /// </summary>
        /// <param name="input">参数数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ParamsData([Required] ElevatorParamsDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.SeCode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            string secode = input.SeCode;
            if (!_specialEqp.List.Keys.Contains(secode))
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(-27), -27);
            }
            SgParams sp = new SgParams();
            sp.Add("secode", secode);
            sp.Add("setype", 2);
            sp.Add("paramjson", JsonConvert.SerializeObject(input));
            sp.NeetReturnValue();
            int retDB = await _elevatorService.doParamUpdate(sp);
            if (retDB > 0)
            {
                //图片存本地
                //if (!string.IsNullOrEmpty(input.DriverImg))
                //{
                //    ImgHelper.Base64ToImage(input.DriverImg, _hpSystemSetting.getSettingValue(""), input.DriverCardNo);
                //}
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 人员上机下机刷脸认证
        /// </summary>
        /// <param name="authData"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(204800)]
        public async Task<IResponseOutput> AuthData(AuthData authData)
        {
            if (authData == null || string.IsNullOrEmpty(authData.SeCode))
            {
                return ResponseOutput.NotOk("请输入正确的参数");
            }
            var Devices = await _specialEqpService.Query(a => a.secode == authData.SeCode);
            if (Devices.Any())
            {
                var entity = _mapper.Map<GCSpecialEqpAuthHisEntity>(authData);
                entity.GROUPID = Devices[0].GROUPID;
                entity.SITEID = Devices[0].SITEID;
                entity.updatedate = DateTime.Now;
                entity.imagefile = "";
                var DbEntity = await _specialEqpAuthHisService.AddEntity(entity);
                var FileOutput = await _hpFileDoc.AddBase64Img(authData.Image64);
                if (DbEntity != null)
                {
                    if (FileOutput == null)
                    {
                        return ResponseOutput.Ok("信息同步成功，人脸图片同步失败");
                    }
                    else
                    {
                        bool suc = await _hpFileDoc.doUpdate(FileOutput.FileId + ".jpg", new FileEntity.FileEntityParam() { filetype = FileEntity.FileType.SeAuth, GROUPID = entity.GROUPID, linkid = DbEntity.SEAHID.ToString(), SITEID = entity.SITEID }, "");
                        DbEntity.imagefile = FileOutput.FileId + ".jpg";
                        bool upsuc = await _specialEqpAuthHisService.Update(DbEntity);
                        if (suc)
                        {
                            return ResponseOutput.Ok("信息同步成功");
                        }
                        else
                        {
                            return ResponseOutput.Ok("信息同步成功，人脸图片同步失败");
                        }

                    }

                }
                else
                {
                    return ResponseOutput.NotOk("信息同步发生错误，请联系");
                }
            }
            else
            {
                return ResponseOutput.NotOk("设备未同步");
            }
        }

    }
}
