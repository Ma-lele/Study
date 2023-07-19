using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Dictionary;
using XHS.Build.Services.FallProtection;
using XHS.Build.Services.Warning;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 新临边防护数据
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class FallProtectionController : ControllerBase
    {
        private readonly ILogger<AlertInputDto> _logger;
        private readonly IFallProtectionService _fallProtectionService;
        private readonly IWarningService _warningService;
        private readonly IDictionaryService _dictionaryService;


        public FallProtectionController(ILogger<AlertInputDto> logger, IFallProtectionService fallProtectionService,
            IWarningService warningService, IDictionaryService dictionaryService)
        {
            _logger = logger;
            _fallProtectionService = fallProtectionService;
            _warningService = warningService;
            _dictionaryService = dictionaryService;
        }


        /// <summary>
        /// 接收临边设备数据推送
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ReviceEquipmentData(AlertInputDto input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.deviceId) || string.IsNullOrWhiteSpace(input.alarmId))
            {
                return ResponseOutput.NotOk("参数错误");
            }

            var entity = await _fallProtectionService.FindByCode(input.deviceId);
            if (entity == null || entity.FPDID <= 0)
            {
                return ResponseOutput.NotOk("设备不存在");
            }

            if (input.alarmId == "normal")
            {
                if (!(await _fallProtectionService.SetAlarm(entity.FPDID, "")))
                {
                    _logger.LogError("报警信息更新失败");
                }

                return ResponseOutput.Ok();
            }


            var alarm = (await _dictionaryService
                .Query(ii => ii.datatype == "fpcode" && ii.dcode.ToLower() == input.alarmId.ToLower())).FirstOrDefault();
            if (alarm == null || alarm.DDID <= 0)
            {
                return ResponseOutput.NotOk("AlarmID不存在");
            }

            CCWarning warning = new CCWarning()
            {
                GROUPID = entity.GROUPID,
                SITEID = entity.SITEID,
                devicecode = entity.deviceId,
                type = alarm.sort,
                content = alarm.dataitem
            };

            int result = await _warningService.FallProtectionInsert(warning);
            if (result <= 0)
            {
                _logger.LogError("报警信息入库失败");
                return ResponseOutput.NotOk("报警信息入库失败");
            }

            if (!(await _fallProtectionService.SetAlarm(entity.FPDID, input.alarmId)))
            {
                _logger.LogError("报警信息更新失败");
            }

            return ResponseOutput.Ok();
        }


        /// <summary>
        /// 接收临边设备状态推送
        /// </summary>
        [HttpPost]
        public async Task<IResponseOutput> ReviceEquipmentStatus(FallProtectionStatusDto input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.deviceId))
            {
                return ResponseOutput.NotOk("参数错误");
            }
            var entity = await _fallProtectionService.FindByCode(input.deviceId);
            if (entity == null || entity.FPDID <= 0)
            {
                return ResponseOutput.NotOk("设备不存在");
            }

            entity.battery = input.battery;
            entity.lat = input.lat;
            entity.@long = input.@long;
            entity.onlinestatus = input.onlineStatus;
            entity.operatedate = DateTime.Now;
            entity.lastpushtime = DateTime.Now;
            entity.@operator = "数据推送";

            bool result = await _fallProtectionService.SetDeviceStatus(entity);
            if (!result)
            {
                _logger.LogError("状态更新信息入库失败");
                return ResponseOutput.NotOk("状态更新信息入库失败");
            }

            if (input.battery <= 10)
            {
                CCWarning warning = new CCWarning()
                {
                    GROUPID = entity.GROUPID,
                    SITEID = entity.SITEID,
                    devicecode = entity.deviceId,
                    type = 5,
                    content = "电池电量低"
                };
                int rtnBattery = await _warningService.FallProtectionInsert(warning);
                if (rtnBattery <= 0)
                {
                    _logger.LogError("报警信息入库失败：电池电量低");
                }
            }
            if (input.onlineStatus == 0)
            {
                CCWarning warning = new CCWarning()
                {
                    GROUPID = entity.GROUPID,
                    SITEID = entity.SITEID,
                    devicecode = entity.deviceId,
                    type = 5,
                    content = "设备离线"
                };
                int rtnOnline = await _warningService.FallProtectionInsert(warning);
                if (rtnOnline <= 0)
                {
                    _logger.LogError("报警信息入库失败：设备离线");
                }
            }


            return ResponseOutput.Ok();
        }
    }
}
