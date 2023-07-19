using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.AIAirTightAction;
using XHS.Build.Services.AIAirTightAction.Dtos;
using XHS.Build.Services.AIIllegalCarAction;
using XHS.Build.Services.AIIllegalCarAction.Dtos;
using XHS.Build.Services.AISoilAction;
using XHS.Build.Services.AISoilAction.Dtos;
using XHS.Build.Services.AISprayAction;
using XHS.Build.Services.AISprayAction.Dtos;
using XHS.Build.Services.Site;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// AI数据 实时入库
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class AIController : ControllerBase
    {
        private readonly IAIAirTightService _tightService;
        private readonly IAISoilService _soilService;
        private readonly IAISprayService _sprayService;
        private readonly ISiteService _siteService;
        private readonly HpAliSMS _hpAliSMS;
        private readonly IMapper _mapper;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IAIIllegalCarService _illegalCarService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public AIController(IAIAirTightService tightService, IAIIllegalCarService illegalCarService, IAISoilService soilService, IAISprayService sprayService, ISiteService siteService, IHpSystemSetting hpSystemSetting, IMapper mapper)
        {
            _tightService = tightService;
            _illegalCarService = illegalCarService;
            _soilService = soilService;
            _sprayService = sprayService;
            _siteService = siteService;
            _mapper = mapper;
            _hpAliSMS = new HpAliSMS(hpSystemSetting);
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 密闭运输
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("airtight")]
        public async Task<IResponseOutput> PostAirTight(AirTightInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var siteList = await _siteService.Query(a => a.airtightprojid == input.projid);
            if (!siteList.Any())
            {
                return ResponseOutput.NotOk("设备编号不匹配.", -27);
            }
            var entity = _mapper.Map<AIAirTightActionEntity>(input);
            entity.SITEID = siteList[0].SITEID;
            entity.GROUPID = siteList[0].GROUPID;
            int retDB = await _tightService.Add(entity);
            if (retDB > 0)
            {
                //非密闭 发警告
                if (entity.tightstatus == 0)
                {
                    var ProcInput = _mapper.Map<AirTightProcInputDto>(input);
                    var warnId = await _tightService.WarnInsertForAirTight(ProcInput);
                    if (warnId > 0)
                    {
                        _hpAliSMS.SendWarnAIById(warnId, "");
                    }
                }
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 非法车辆进入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("illegalcar")]
        public async Task<IResponseOutput> PostIllegalCar(IllegalCarInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var siteList = await _siteService.Query(a => a.illegalcarprojid == input.projid);
            if (!siteList.Any())
            {
                return ResponseOutput.NotOk("设备编号不匹配.", -27);
            }
            var entity = _mapper.Map<AIIllegalCarActionEntity>(input);
            entity.SITEID = siteList[0].SITEID;
            entity.GROUPID = siteList[0].GROUPID;
            int retDB = await _illegalCarService.Add(entity);
            if (retDB > 0)
            {
                //非发车辆 发警告
                if (entity.status == 1)
                {
                    var ProcInput = _mapper.Map<IllegalCarProcInputDto>(input);
                    var warnId = await _illegalCarService.WarnInsertForIllegalCar(ProcInput);
                    if (warnId > 0)
                    {
                        _hpAliSMS.SendWarnAIById(warnId, "");
                    }
                }
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }


        /// <summary>
        /// 黄土裸露
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("soil")]
        public async Task<IResponseOutput> PostSoil(AISoilInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            input.soilrate = Math.Floor(input.soilrate * 10) / (decimal)10.0;
            var dbEntity = await _soilService.GetSoilLastSoilrate(input.projid);
            if (dbEntity == null)
            {
                return ResponseOutput.NotOk("设备编号不匹配.", -27);
            }
            if (input.soilrate == dbEntity.soilrate)
            {
                return ResponseOutput.Ok(1, "获取裸土覆盖的百分比数值没有变化");
            }
            var entity = _mapper.Map<AISoilActionEntity>(input);
            entity.SITEID = dbEntity.SITEID;
            entity.GROUPID = dbEntity.GROUPID;
            int retDB = await _soilService.Add(entity);
            if (retDB > 0)
            {
                //覆盖率小于指定值 发报警
                var S171 = _hpSystemSetting.getSettingValue(Const.Setting.S171).ToDecimal();
                if (input.soilrate < S171)
                {
                    var ProcInput = _mapper.Map<AISoilProcInputDto>(input);
                    var warnId = await _soilService.WarnInsertForSoil(ProcInput);
                    if (warnId > 0)
                    {
                        _hpAliSMS.SendWarnAIById(warnId, "");
                    }
                }
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 雾炮喷淋
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("spary")]
        public async Task<IResponseOutput> PostSpary(AISparyInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            int retDB = await _sprayService.InsertSparyPROC(input);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }
    }
}
