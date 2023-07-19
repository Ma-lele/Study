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
using XHS.Build.Services.AIWash;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 车辆未冲洗
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize]
    public class CarWashController : ControllerBase
    {
        private readonly IAiWashService _aiWashService;
        private readonly IUser _user;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aiWashService"></param>
        public CarWashController(IUser user, IAiWashService aiWashService)
        {
            _aiWashService = aiWashService;
            _user = user;
        }

        /// <summary>
        /// 车辆未冲洗列表
        /// </summary>
        /// <param name="month">月份</param>
        /// <param name="platecolor">车牌颜色</param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T062/[action]")]
        public async Task<IResponseOutput> GetRecordList(string month, string platecolor = "", int pageindex = 1, int pagesize = 10)
        {
            var data = await _aiWashService.GetAiWashRecordListAsync(month, platecolor, Convert.ToInt32(_user.SiteId), pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 车冲数据对比
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T062/[action]")]
        public async Task<IResponseOutput> GetDataCompare()
        {
            DataRow data = await _aiWashService.GetAiWashDataCompareAsync(Convert.ToInt32(_user.SiteId));
            JObject job = JsonTransfer.dataRow2JObject(data);

            job["daythan"] = (int)job.GetValue("daythan") > 0 ? "+" + job.GetValue("daythan").ToString() : job.GetValue("daythan");
            job["monththan"] = (int)job.GetValue("monththan") > 0 ? "+" + job.GetValue("monththan").ToString() : job.GetValue("monththan");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 车冲数据统计
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T062/[action]")]
        public async Task<IResponseOutput> GetDataCount(int type)
        {
            var data = await _aiWashService.GetAiWashDataCountAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 车冲时段分析
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T062/[action]")]
        public async Task<IResponseOutput> GetDuringAnalysis(int type)
        {
            var data = await _aiWashService.GetAiWashDuringAnalysisAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }
    }
}
