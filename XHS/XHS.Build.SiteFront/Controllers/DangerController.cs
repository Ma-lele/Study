using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.Danger;
using XHS.Build.Common.Helps;
using XHS.Build.Model.ModelDtos;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 危大工程
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DangerController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IDangerService _dangerService;
        
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dangerService"></param>
        public DangerController(IUser user, IDangerService dangerService)
        {
            _user = user;
            _dangerService = dangerService;
        }


        /// <summary>
        /// 危大工程-列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T039/[action]")]
        public async Task<IResponseOutput> List()
        {
            var result = await _dangerService.spV2DangerList(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 危大工程-文件
        /// </summary>
        /// <param name="SDID">危大工程ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T039/[action]")]
        public async Task<IResponseOutput> File(string SDID)
        {
            var result = await _dangerService.spV2DangerFile(_user.SiteId.ToInt(), SDID);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 危大工程-类型统计分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T039/[action]")]
        public async Task<IResponseOutput> TypeStats()
        {
            var result = await _dangerService.spV2DangerTypeStats(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 危大工程-趋势
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T039/[action]")]
        public async Task<IResponseOutput> Trend(int days)
        {
            if (days > 365)
            {
                return ResponseOutput.Ok("日期超限");
            }
            var result = await _dangerService.spV2DangerTrend(_user.SiteId.ToInt(), days);

            return ResponseOutput.Ok(result);
        }

    }
}
