using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Defence;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 临边围挡
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DefenceController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IDefenceService _defenceService;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="defenceService"></param>
        /// <param name="user"></param>
        public DefenceController(IDefenceService defenceService, IUser user)
        {
            _user = user;
            _defenceService = defenceService;
        }


        /// <summary>
        /// 临边围挡-设备统计列表
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="dfstatus">设备状态0:闭合 1::断线 2:拆除 3:主机拆除 4:围挡缺失</param>
        /// <param name="bsheild">警戒状态0:撤防 1:布防</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T082/[action]")]
        public async Task<IResponseOutput> EquipmentStatistic(string keyword, int dfstatus = -1, int bsheild = -1)
        {
            var result = await _defenceService.spV2DefenceStats(
                keyword
                , dfstatus
                , bsheild
                , _user.SiteId.ToInt()
                );

             
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 临边围挡-左上角统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T082/[action]")]
        public async Task<IResponseOutput> Total()
        {
            var result = await _defenceService.spV2DefenceTotal(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }

    }
}
