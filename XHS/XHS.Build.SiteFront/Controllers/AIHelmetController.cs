using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.Helmet;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 人员安全帽未佩戴
    /// </summary>
    [Route("api")]
    [ApiController]
    [Authorize]
    public class AIHelmetController : ControllerBase
    {
        private readonly IUser _user;

        private readonly IHelmetService _helmetService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="helmetService"></param>
        public AIHelmetController(IUser user, IHelmetService helmetService)
        {
            _user = user;
            _helmetService = helmetService;
        }

        /// <summary>
        /// 安全帽未佩戴列表
        /// </summary>
        /// <param name="keyword">点位信息</param>
        /// <param name="month">月份</param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T015/[action]")]
        public async Task<IResponseOutput> GetAiHelmetRecordList(string keyword = "", string month = "", int pageindex = 1, int pagesize = 10)
        {
            var data = await _helmetService.GetAiHelmetRecordListAsync(keyword, month, Convert.ToInt32(_user.SiteId), pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全帽未佩戴数据对比
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T015/[action]")]
        public async Task<IResponseOutput> GetAiHelmetDataCompare()
        {
            var data = await _helmetService.GetAiHelmetDataCompareAsync(Convert.ToInt32(_user.SiteId));
            JObject job = JsonTransfer.dataRow2JObject(data);
            job["daythan"] = (int)job.GetValue("daythan") > 0 ? "+" + job.GetValue("daythan").ToString() : job.GetValue("daythan");
            job["monththan"] = (int)job.GetValue("monththan") > 0 ? "+" + job.GetValue("monththan").ToString() : job.GetValue("monththan");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 安全帽未佩戴数据统计
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T015/[action]")]
        public async Task<IResponseOutput> GetAiHelmetDataCount(int type = 0)
        {
            var data = await _helmetService.GetAiHelmetDataCountAsync(type, Convert.ToInt32(_user.SiteId));
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全帽未佩戴时段分析
        /// </summary>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T015/[action]")]
        public async Task<IResponseOutput> GetAiHelmetDuringAnalysis(int type)
        {
            var data = await _helmetService.GetAiHelmetDuringAnalysisAsync(type,Convert.ToInt32(_user.SiteId));
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全帽未佩戴位置列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T015/[action]")]
        public async Task<IResponseOutput> GetAiHelmentLocationList()
        {
            var data = await _helmetService.GetAiHelmentLocationListAsync(Convert.ToInt32(_user.SiteId));
            return ResponseOutput.Ok(data);
        }
    }
}
