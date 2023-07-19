using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.AIIllegalCarAction;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 非法车辆进入
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize]
    public class IllegalCarController : ControllerBase
    {
        private readonly IUser _user;

        private readonly IAIIllegalCarService _aIIllegalCarService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aIIllegalCarService"></param>
        public IllegalCarController(IUser user, IAIIllegalCarService aIIllegalCarService)
        {
            _aIIllegalCarService = aIIllegalCarService;
            _user = user;
        }

        /// <summary>
        /// 非法车辆进入列表
        /// </summary>
        /// <param name="month">月份</param>
        /// <param name="keyword">车牌号</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pagesize">每页条数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T061/[action]")]
        public async Task<IResponseOutput> GetRecordList(string month, string keyword = "", int pageindex = 1, int pagesize = 10)
        {
            var data = await _aIIllegalCarService.GetAiIllegalRecordListAsync(month, keyword, Convert.ToInt32(_user.SiteId), pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 非法车辆数据对比
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T061/[action]")]
        public async Task<IResponseOutput> GetDataCompare()
        {
            var data = await _aIIllegalCarService.GetAiIllegalDataCompareAsync(Convert.ToInt32(_user.SiteId));
            JObject job = JsonTransfer.dataRow2JObject(data);
            job["daythan"] = (int)job.GetValue("daythan") > 0 ? "+" + job.GetValue("daythan").ToString() : job.GetValue("daythan");
            job["monththan"] = (int)job.GetValue("monththan") > 0 ? "+" + job.GetValue("monththan").ToString() : job.GetValue("monththan");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 非法车辆统计
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T061/[action]")]
        public async Task<IResponseOutput> GetDataCount(int type)
        {
            var data = await _aIIllegalCarService.GetAiIllegalDataCountAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 非法车辆时段分析
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T061/[action]")]
        public async Task<IResponseOutput> GetDuringAnalysis(int type)
        {
            var data = await _aIIllegalCarService.GetAiIllegalDuringAnalysisAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }

    }
}
