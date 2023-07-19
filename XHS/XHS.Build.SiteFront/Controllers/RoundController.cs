using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Helps;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Util;
using XHS.Build.Services.Round;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 随手拍
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoundController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IRoundService _roundService;
        public RoundController(IUser user, IRoundService roundService)
        {
            _user = user;
            _roundService = roundService;
        }


        /// <summary>
        /// 随手拍-左上角统计
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T052/[action]")]
        public async Task<IResponseOutput> RoundStats(DateTime startDate, DateTime endDate)
        {
            var result = await _roundService.GetV2RoundStats(_user.SiteId.ToInt(), startDate, endDate);
            JObject job = JsonTransfer.dataRow2JObject(result.Rows[0]);
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 智慧工地2.0项目端-随手拍-每日次数统计
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T052/[action]")]
        public async Task<IResponseOutput> RoundDayCount(DateTime startDate, DateTime endDate)
        {
            var result = await _roundService.GetV2RoundDayCount(_user.SiteId.ToInt(), startDate, endDate);
            return ResponseOutput.Ok(result);
        }



        /// <summary>
        /// 智慧工地2.0项目端-随手拍-问题类型统计
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T002/[action]")]
        [Route("T052/[action]")]
        public async Task<IResponseOutput> RoundTypeCount(DateTime startDate, DateTime endDate)
        {
            var result = await _roundService.GetV2RoundTypeCount(_user.SiteId.ToInt(), startDate, endDate);
            return ResponseOutput.Ok(result);
        }



        /// <summary>
        /// 随手拍-数据详情
        /// </summary>
        /// <param name="ROUNDID">编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T052/[action]")]
        public async Task<IResponseOutput> RoundOne(int ROUNDID)
        {
            var result = await _roundService.GetV2RoundOne(ROUNDID);
            JObject job = JsonTransfer.dataRow2JObject(result.Rows[0]);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 随手拍-数据列表
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="keyword">关键字</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T052/[action]")]
        public IResponseOutput RoundList(int status,string keyword, DateTime startDate, DateTime endDate, int pageIndex = 1, int pageSize = 20)
        {
            var result = _roundService.GetV2RoundList(_user.SiteId.ToInt(), status, keyword, startDate, endDate, pageIndex, pageSize);

            return ResponseOutput.Ok(result);
        }
   
    }
}
