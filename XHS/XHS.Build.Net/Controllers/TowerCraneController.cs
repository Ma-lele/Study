using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Common.Util;
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
using XHS.Build.Common.Fleck;
using System.Net.WebSockets;
using XHS.Build.Common.Configs;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 塔吊
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
   	[Authorize]
    [Permission]
    public class TowerCraneController : ControllerBase
    {
        private readonly IElevatorService _elevatorService;
        private readonly ISpecialEqpService _specialEqpService;
        private readonly ISpecialEqp _specialEqp;
        private readonly IConfiguration _configuration;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly HpAliSMS _hpAliSMS;
        private readonly IMapper _mapper;
        private readonly ISpecialEqpAuthHisService _specialEqpAuthHisService;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IOperateLogService _operateLogService;
        private readonly ILogger<TowerCraneController> _logger;
        private readonly IEmployeeCareerService _employeeCareerService;
        private readonly ISpecialEqpWorkDataService _specialEqpWorkDataService;
        private readonly IFleckSpecial _fleckSpecial;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="specialEqp"></param>
        /// <param name="elevatorService"></param>
        /// <param name="configuration"></param>
        /// <param name="hpSystemSetting"></param>
        /// <param name="mapper"></param>
        /// <param name="specialEqpService"></param>
        /// <param name="specialEqpAuthHisService"></param>
        /// <param name="hpFileDoc"></param>
        /// <param name="operateLogService"></param>
        /// <param name="logger"></param>
        /// <param name="employeeCareerService"></param>
        /// <param name="specialEqpWorkDataService"></param>
        public TowerCraneController(ISpecialEqp specialEqp, IElevatorService elevatorService, IConfiguration configuration, IHpSystemSetting hpSystemSetting,
            IMapper mapper, ISpecialEqpService specialEqpService, ISpecialEqpAuthHisService specialEqpAuthHisService, IHpFileDoc hpFileDoc, IFleckSpecial fleckSpecial,
            IOperateLogService operateLogService, ILogger<TowerCraneController> logger, IEmployeeCareerService employeeCareerService, ISpecialEqpWorkDataService specialEqpWorkDataService)
        {
            _fleckSpecial = fleckSpecial;
            _specialEqp = specialEqp;
            _elevatorService = elevatorService;
            _hpAliSMS = new HpAliSMS(hpSystemSetting);
            _mapper = mapper;
            _specialEqpService = specialEqpService;
            _configuration = configuration;
            _specialEqpAuthHisService = specialEqpAuthHisService;
            _hpFileDoc = hpFileDoc;
            _hpSystemSetting = hpSystemSetting;
            _operateLogService = operateLogService;
            _logger = logger;
            _employeeCareerService = employeeCareerService;
            _specialEqpWorkDataService = specialEqpWorkDataService;
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
            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("设备上线失败");
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
            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("设备下线失败");
        }

        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="input">实时数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> RealData(TowerCraneRealDataInput input)
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
            seData.Flag = 1;
            await _operateLogService.AddSpecialEqpDataLog(seData);

            SpecialEqpBean seb = _specialEqp.List[secode];
            short alarmState = Convert.ToInt16(input.Alarm);

            if (seb.waitcount == 0 && alarmState >= _configuration.GetSection("TowerCraneWarnTypeFrom").Value.ObjToInt())
            {
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
            sp.Add("setype", 1);
            sp.Add("alarmstate", input.Alarm);
            sp.Add("ID", input.DriverCardNo);
            sp.Add("hasreport", input.HasReport);
            sp.Add("sedata", JsonConvert.SerializeObject(input));
            sp.NeetReturnValue();
            int retDB = await _elevatorService.doRtdInsert(sp);
            if (retDB > 0)
            {
                if (_fleckSpecial != null && _fleckSpecial.SeCodes.Contains(secode))
                {
                    _fleckSpecial.Distpatch(input);
                }
                 
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 报警数据(废弃)
        /// </summary>
        /// <param name="alarmData">报警数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AlarmData(TowerCraneAlarmInput alarmData)
        {
            if (alarmData == null || string.IsNullOrEmpty(alarmData.SeCode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            string secode = alarmData.SeCode;

            if (!_specialEqp.List.Keys.Contains(secode))
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(-27), -27);
            }

            SpecialEqpBean seb = _specialEqp.List[secode];
            if (seb.waitcount > 0 || seb.warndate.AddMinutes(_configuration.GetSection("TowerCraneWarnDelay").Value.ObjToInt()) > DateTime.Now)
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(-23), -23);
            }

            seb.waitcount = 1;
            seb.warndate = DateTime.Now;
            return ResponseOutput.Ok(1);
        }

        /// <summary>
        /// 设备参数数据
        /// </summary>
        /// <param name="input">参数数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ParamsData(TowerCraneParamsDataInput input)
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
            sp.Add("secode",secode);
            sp.Add("setype", 1);
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
        /// 防倾翻报警诊断
        /// </summary>
        /// <param name="tipOverData">防倾翻报警诊断数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> TipOverData(TipOverDataInput input)
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
            string starttime = Convert.ToDateTime(input.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
            string endtime = Convert.ToDateTime(input.EndTime).ToString("yyyy-MM-dd HH:mm:ss");

            int WARNID = await _elevatorService.doTipOverWarn(secode, starttime, endtime, JsonConvert.SerializeObject(input));
            _hpAliSMS.SendWarnById(WARNID, "");

            if (WARNID > 0)
            {
                return ResponseOutput.Ok(1);
            }
            return ResponseOutput.NotOk("", 0);
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
                var FileOutput = await _hpFileDoc.AddBase64Img(authData.Image64, new FileEntity.FileEntityParam() { filetype = FileEntity.FileType.SeAuth, GROUPID = entity.GROUPID, linkid = DbEntity.SEAHID.ToString(), SITEID = entity.SITEID });
                if (DbEntity != null)
                {
                    if (FileOutput == null)
                    {
                        return ResponseOutput.Ok("信息同步成功，人脸图片同步失败");
                    }
                    else
                    {
                        //bool suc= await _hpFileDoc.doUpdate(FileOutput.FileId + ".jpg", new FileEntity.FileEntityParam() { filetype = FileEntity.FileType.SeAuth, GROUPID = entity.GROUPID, linkid = DbEntity.SEAHID.ToString(), SITEID = entity.SITEID }, "");
                        DbEntity.imagefile = FileOutput.FileId + ".jpg";
                        bool upsuc = await _specialEqpAuthHisService.Update(DbEntity);
                        if (upsuc)
                        {
                            try
                            {
                                var towerDt = await _specialEqpService.GetWXDYSpecialList("1", authData.SeCode);
                                if (towerDt != null && towerDt.Rows.Count == 1 && !string.IsNullOrEmpty(authData.Certification) && !string.IsNullOrEmpty(authData.DriverCardNo) && !string.IsNullOrEmpty(authData.DriverName) && !string.IsNullOrEmpty(authData.Image64))
                                {
                                    var FirstRow = towerDt.Rows[0];
                                    //增加触发推送实时人脸数据
                                    var ACCESS_TOKEN = await _specialEqpService.GetWXDYToken();
                                    if (!string.IsNullOrEmpty(ACCESS_TOKEN))
                                    {
                                        var Careers = await _employeeCareerService.Query(c => c.Papertype == "1" && c.ID == authData.DriverCardNo);
                                        if (Careers.Any())
                                        {
                                            SortedDictionary<string, string> towerParam = new SortedDictionary<string, string>();
                                            StringBuilder sb = new StringBuilder(string.Empty);
                                            string run_status = authData.IsOn.ToString();
                                            towerParam["prj_id"] = Convert.ToString(FirstRow["prj_id"]);
                                            towerParam["prj_name"] = Convert.ToString(FirstRow["prj_name"]);
                                            towerParam["owner_name"] = Convert.ToString(FirstRow["owner_name"]);
                                            towerParam["device_type"] = "塔吊设备";//Convert.ToString(FirstRow["device_type"]);
                                            towerParam["device_id"] = Convert.ToString(FirstRow["secode"]);
                                            towerParam["contract_record_code"] = Convert.ToString(FirstRow["contract_record_code"]);
                                            towerParam["id_card"] = authData.DriverCardNo;
                                            towerParam["run_status"] = run_status;
                                            towerParam["photo"] = authData.Image64;
                                            towerParam["name"] = authData.DriverName;
                                            towerParam["certificate_no"] = authData.Certification;
                                            towerParam["certificate_datetime"] = Convert.ToDateTime(Careers[0].Enddate).ToString("yyyy-MM-dd HH:mm:ss");
                                            towerParam["datetime"] = authData.CheckTime;

                                            foreach (var item in towerParam)
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                                                {
                                                    sb.Append(item.Value);
                                                }
                                            }
                                            sb.Append(ACCESS_TOKEN);
                                            string sign = UEncrypter.SHA256(sb.ToString());

                                            string towerUrl = string.Format("http://{0}/rest/Tower/addWorkData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), "sysfdas2fvdasf33dag", sign);
                                            string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(towerParam), UHttp.CONTENT_TYPE_JSON);
                                            if (!string.IsNullOrEmpty(response))
                                            {
                                                JObject jo = JObject.Parse(response);
                                                if (Convert.ToString(jo["flag"]) != "0000")
                                                {
                                                    _logger.LogInformation("塔吊" + authData.SeCode + "设备" + response);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.LogInformation("塔吊" + authData.SeCode + "设备" + authData.DriverName + authData.DriverCardNo + "未获取到证书信息");
                                        }
                                    }
                                    return ResponseOutput.Ok("信息同步成功");
                                }
                                else
                                {
                                    //少打印点
                                    authData.Image64 = authData.Image64.Length > 100 ? authData.Image64.Substring(0, 100) : authData.Image64;
                                    _logger.LogInformation("塔吊" + authData.SeCode + "基础信息错误:" + JsonConvert.SerializeObject(authData));
                                    return ResponseOutput.Ok("信息同步成功");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("塔吊" + authData.SeCode + "发生错误:" + ex.ToString());
                                return ResponseOutput.Ok("信息同步成功");
                            }
                        }
                        else
                        {
                            await _specialEqpAuthHisService.DeleteById(DbEntity.SEAHID);
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

        /// <summary>
        /// 工作循环
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> WorkData(TowerCraneWorkDataInput input)
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
            SpecialEqpBean seb = _specialEqp.List[secode];
            GCSpecialEqpWorkDataEntity sewde = new GCSpecialEqpWorkDataEntity();
            sewde.GROUPID = seb.GROUPID;
            sewde.secode = secode;
            sewde.setype = seb.setype;
            sewde.ID = input.DriverCardNo;
            sewde.DriverName = input.DriverName;
            sewde.starttime = input.StartTime;
            sewde.endtime = input.EndTime;
            sewde.workdata = JsonConvert.SerializeObject(input);

            var rows = await _specialEqpWorkDataService.Add(sewde);

            return rows > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("添加数据错误");
        }
    }
}
