using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.AISoilAction;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 裸土覆盖
    /// </summary>
    public class AISoilController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IAISoilService _aISoilService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aiWashService"></param>
        public AISoilController(IUser user, IAISoilService aISoilService)
        {
            _aISoilService = aISoilService;
            _user = user;
        }


        /// <summary>
        /// 裸土覆盖数据列表
        /// </summary>
        /// <param name="month">按月份筛选</param>
        /// <param name="pageindex">分页</param>
        /// <param name="pagesize">分页</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T022/[action]")]
        public async Task<IResponseOutput> GetAiSoilRecordList(string month="", int pageindex=1, int pagesize=10)
        {
            var data = await _aISoilService.GetAiSoilRecordListAsync(Convert.ToInt32(_user.SiteId), month, pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 裸土数据比对
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T022/[action]")]
        public async Task<IResponseOutput> GetAiSoilDataCompare()
        {
            var data = await _aISoilService.GetAiSoilDataCompareAsync(Convert.ToInt32(_user.SiteId));
            JObject job = JsonTransfer.dataRow2JObject(data);
            job["daythan"] = (int)job.GetValue("daythan") > 0 ? "+" + job.GetValue("daythan").ToString() : job.GetValue("daythan");
            job["monththan"] = (int)job.GetValue("monththan") > 0 ? "+" + job.GetValue("monththan").ToString() : job.GetValue("monththan");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 裸土覆盖数据统计
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T022/[action]")]
        public async Task<IResponseOutput> GetAiSoilDataCount(int type=0)
        {
            var data = await _aISoilService.GetAiSoilDataCountAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 裸土覆盖时段分析
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T022/[action]")]
        public async Task<IResponseOutput> GetAiSoilDuringAnalysis(int type=0)
        {
            var data = await _aISoilService.GetAiSoilDuringAnalysisAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }
    }
}
