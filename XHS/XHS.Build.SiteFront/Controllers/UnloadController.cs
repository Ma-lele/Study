using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Unload;
using XHS.Build.Common.Helps;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Util;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 卸料平台
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UnloadController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IUnloadService _unloadService;
        public UnloadController(IUser user, IUnloadService unloadService)
        {
            _user = user;
            _unloadService = unloadService;
        }


        /// <summary>
        /// 卸料平台-左上角统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T033/[action]")]
        public async Task<IResponseOutput> UnloadStats()
        {
            var result = await _unloadService.spV2UnloadStats(_user.SiteId.ToInt());
            JObject job = JsonTransfer.dataRow2JObject(result.Rows[0]);
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 卸料平台-下拉框监测数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T033/[action]")]
        public async Task<IResponseOutput> UnloadSelect()
        {
            var result = await _unloadService.spV2UnloadSelect(_user.SiteId.ToInt());

            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 卸料平台-历史数据
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="searchDate">查找日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T033/[action]")]
        public IResponseOutput UnloadHistory(string unloadid, DateTime searchDate, int pageIndex = 1, int pageSize = 20)
        {
            long total = 0;
            var result = _unloadService.UnloadHistory(unloadid, searchDate, pageIndex, pageSize, ref total);

            JObject jso = new JObject();
            jso.Add("total", total);
            jso.Add("rows", JArray.FromObject(result));
            return ResponseOutput.Ok(jso);
        }


   
    }
}
