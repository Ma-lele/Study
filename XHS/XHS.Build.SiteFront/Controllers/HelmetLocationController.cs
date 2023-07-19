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
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 人员定位
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HelmetLocationController : ControllerBase
    {
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        string url;
        string user;
        string pwd;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <param name="hpSystemSetting"></param>
        public HelmetLocationController(XHSRealnameToken jwtToken, IHpSystemSetting hpSystemSetting)
        {
            _jwtToken = jwtToken;
            _hpSystemSetting = hpSystemSetting;
            url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            user = _hpSystemSetting.getSettingValue(Const.Setting.S176);
            pwd = _hpSystemSetting.getSettingValue(Const.Setting.S177);
        }
        
        [HttpGet]
        [Route("T012/[action]")]
        public async Task<IResponseOutput> GetUnitStatistics(string city)
        {
            JObject job = new JObject();
            job.Add("City", city);

            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-unit-statisitc", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata);
        }

        [HttpGet]
        [Route("T012/[action]")]
        public async Task<IResponseOutput> GetUn(string city)
        {
            JObject job = new JObject();
            job.Add("City", city);

            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-unit-statisitc", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata);
        }
    }
}
