using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.Danger;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 危大工程
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DangerController : ControllerBase
    {

        private readonly IDangerService _dangerService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dangerService"></param>
        public DangerController(IDangerService dangerService)
        {
            _dangerService = dangerService;
        }

        /// <summary>
        /// 危大项目数量统计
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteCount(int GROUPID = 0)
        {
            var data = await _dangerService.GetSiteCountAsync(GROUPID);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 危大监测设备数量统计
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetDevCount(int GROUPID = 0)
        {
            var data = await _dangerService.GetDevCountAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 告警统计
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">2：一周，3：一个月</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWarnCount(int GROUPID = 0, int type = 2)
        {
            var data = await _dangerService.GetWarnCountAsync(GROUPID, type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 危大告警设备排行
        /// 第一行:本周期；第二行:上周期
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1:今日，2：一周，3：一个月，4：一年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWarnRank(int GROUPID = 0, int type = 1)
        {
            var data = await _dangerService.GetWarnRankAsync(GROUPID, type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 危大告警区域排行
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1:今日，2：一周，3：一个月，4：一年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWarnAreaCount(int GROUPID = 0, int type = 1)
        {
            var data = await _dangerService.GetWarnAreaCountAsync(GROUPID, type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取某种类型的危大设备列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="type">1:塔吊 2:升降机 3:卸料平台 4:深基坑 5:高支模</param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetDevList(int GROUPID, int type, int pageindex, string keyword, int pagesize = 10)
        {
            var data = await _dangerService.GetDevList(GROUPID, type, pageindex, pagesize, keyword);
            return ResponseOutput.Ok(data);
        }
    }
}
