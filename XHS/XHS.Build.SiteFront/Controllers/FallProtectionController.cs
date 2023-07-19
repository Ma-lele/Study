using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.FallProtection;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using Newtonsoft.Json.Linq;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 临边防护
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FallProtectionController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IFallProtectionService _fallProtectionService;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fallProtectionService"></param>
        public FallProtectionController(IUser user, IFallProtectionService fallProtectionService)
        {
            _user = user;
            _fallProtectionService = fallProtectionService;
        }


        /// <summary>
        /// 临边防护-左上角统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T083/[action]")]
        public async Task<IResponseOutput> Total()
        {
            var result = await _fallProtectionService.spV2FallProStats(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 告警类型分析
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T083/[action]")]
        public async Task<IResponseOutput> WarningType(DateTime startTime,DateTime endTime)
        {
            var result = await _fallProtectionService.spV2FallProWarnType(_user.SiteId.ToInt(), startTime, endTime);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 设备统计
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="online">在线状态 -1:全部  0:离线  1:在线</param>
        /// <param name="alarm">报警状态 -1:全部  0:正常  1:异常</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T083/[action]")]
        public async Task<IResponseOutput> DeviceStatistic(string keyword, int online = -1, int alarm = -1)
        {
            var result = await _fallProtectionService.spV2FallProDevStats(_user.SiteId.ToInt(), keyword, online, alarm);
            JObject objTotal = new JObject();
            objTotal["normal"] = result.Select("isnull(alarmId,'') = '' ").Count();
            objTotal["alarm"] = result.Select("isnull(alarmId,'') <> '' ").Count();

            return ResponseOutput.Ok(result, objTotal.ToString());
        }

    }
}
