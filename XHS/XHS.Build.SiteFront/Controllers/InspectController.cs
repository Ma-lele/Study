using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.Inspection;
using XHS.Build.SiteFront.Attributes;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 安全隐患-检查单
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    [Permission]
    public class InspectController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IInspectionService _inspectionService;


        public InspectController(IUser user, IInspectionService inspectionService)
        {
            _user = user;
            _inspectionService = inspectionService;
        }

        /// <summary>
        /// 检查单-左上角统计
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T051/[action]")]
        public async Task<IResponseOutput> InspectStats(DateTime startDate, DateTime endDate)
        {
            var result = await _inspectionService.GetV2InspectStats(int.Parse(_user.SiteId), startDate, endDate);
            JObject job = JsonTransfer.dataRow2JObject(result.Rows[0]);
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 智慧工地2.0项目端-检查单-每日次数统计
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T051/[action]")]
        public async Task<IResponseOutput> InspectDayCount(DateTime startDate, DateTime endDate)
        {
            var result = await _inspectionService.GetV2InspectDayCount(int.Parse(_user.SiteId), startDate, endDate);
            return ResponseOutput.Ok(result);
        }



        /// <summary>
        /// 智慧工地2.0项目端-检查单-问题类型统计
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T051/[action]")]
        public async Task<IResponseOutput> InspectTypeCount(DateTime startDate, DateTime endDate)
        {
            var result = await _inspectionService.GetV2InspectTypeCount(int.Parse(_user.SiteId), startDate, endDate);
            return ResponseOutput.Ok(result);
        }



        /// <summary>
        /// 检查单-数据详情
        /// </summary>
        /// <param name="inspcode">检查单编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T051/[action]")]
        public async Task<IResponseOutput> InspectOne(string inspcode)
        {
            var result = await _inspectionService.GetV2InspectOne(inspcode);
            JObject job = JsonTransfer.dataRow2JObject(result.Rows[0]);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 检查单-数据列表
        /// </summary>
        /// <param name="INSPID">一页最后一条数据的key</param>
        /// <param name="processstatus">2：等待整改，3：等待确认，4：完成整改</param>
        /// <param name="keyword">单号，模糊查询</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T051/[action]")]
        public IResponseOutput InspectList(int processstatus, string keyword, DateTime startDate, DateTime endDate, int INSPID=0, int pageIndex = 1, int pageSize = 20)
        {
            var result = _inspectionService.GetV2InspectList(int.Parse(_user.SiteId), INSPID, int.Parse(_user.Id), processstatus, keyword, startDate, endDate, pageIndex, pageSize);

            return ResponseOutput.Ok(result);
        }

    }
}
