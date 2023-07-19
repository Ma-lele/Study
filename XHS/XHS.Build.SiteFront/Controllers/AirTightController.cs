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
using XHS.Build.Services.AIAirTightAction;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 车辆密闭运输
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize]
    public class AirTightController : ControllerBase
    {

        private readonly IUser _user;

        private readonly IAIAirTightService _aIAirTightService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aIAirTightService"></param>
        public AirTightController(IUser user, IAIAirTightService aIAirTightService)
        {
            _user = user;
            _aIAirTightService = aIAirTightService;
        }

        /// <summary>
        /// 车辆密闭运输记录列表
        /// </summary>
        /// <param name="month">月份</param>
        /// <param name="keyword">车牌号</param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T063/[action]")]
        public async Task<IResponseOutput> GetRecordList(string month, string keyword="", int pageindex = 1, int pagesize = 10)
        {
            var data = await _aIAirTightService.GetAiAirtightRecordListAsync(month, Convert.ToInt32(_user.SiteId), keyword, pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 车辆密闭运输数据比对
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T063/[action]")]
        public async Task<IResponseOutput> GetDataCompare()
        {
            var data = await _aIAirTightService.GetAiAirtightDataCompareAsync(Convert.ToInt32(_user.SiteId));
            JObject job = JsonTransfer.dataRow2JObject(data);
            job["daythan"] = (int)job.GetValue("daythan") > 0 ? "+" + job.GetValue("daythan").ToString() : job.GetValue("daythan");
            job["monththan"] = (int)job.GetValue("monththan") > 0 ? "+" + job.GetValue("monththan").ToString() : job.GetValue("monththan");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 车辆密闭运输数据统计
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T063/[action]")]
        public async Task<IResponseOutput> GetDataCount(int type)
        {
            var data = await _aIAirTightService.GetAiAirtightDataCountAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 车辆密闭运输时段分析
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T063/[action]")]
        public async Task<IResponseOutput> GetDuringAnalysis(int type)
        {
            var data = await _aIAirTightService.GetAiAirtightDuringAnalysisAsync(Convert.ToInt32(_user.SiteId), type);
            return ResponseOutput.Ok(data);
        }

    }
}
