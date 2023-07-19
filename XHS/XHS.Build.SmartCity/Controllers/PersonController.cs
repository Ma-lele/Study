using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.Board;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 人员管控
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IBoardService _boardService;

        string url;
        string user;
        string pwd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personService"></param>
        public PersonController(IBoardService boardService, XHSRealnameToken jwtToken, IHpSystemSetting hpSystemSetting)
        {
            _jwtToken = jwtToken;
            _hpSystemSetting = hpSystemSetting;
            _boardService = boardService;
            url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            user = _hpSystemSetting.getSettingValue(Const.Setting.S176);
            pwd = _hpSystemSetting.getSettingValue(Const.Setting.S177);
        }


        /// <summary>
        /// 在册人员统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPayroll(string city, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-employee-statistic", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 在岗人员进场统计
        /// </summary>
        /// <param name="City">市名</param>
        /// <param name="district">区名</param>
        /// <param name="type">0:今天 1:昨天</param>
        /// <param name="profession">0:施工人员 1:管理/监理</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPayrollApproach(string City, int type = 0, int profession = 0, string district = "")
        {

            JObject job1 = new JObject();
            job1.Add("City", City);
            job1.Add("Township", district);
            job1.Add("Type", type);
            job1.Add("Profession", profession);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-employee-in", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1);
            return ResponseOutput.Ok(job);

        }

        /// <summary>
        /// 年龄/性别统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAgeSexstatistics(string city, string district = "")
        {

            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);

            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-gender-statistic", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);

        }

        /// <summary>
        /// 人员处罚统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <param name="type">0:周统计 1:月统计</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPersonstatistics(string city, int type = 0, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            job1.Add("Type", type);

            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-employee-punishment", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 违规工种 TOP5
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <param name="type">0:周统计 1:月统计</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetIllegalWorkers(string city, int type = 0, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            job1.Add("Type", type);

            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-worktype-punishment", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 人员户籍统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPersoncensus(string city, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-census-register-statistic", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 劳务人员工种统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetContractPerson(string city, string district = "")
        {

            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-employee-worktype", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 管理人员岗位统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetManagement(string city, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-employee-admin", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 违规工种人员信息
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <param name="count">取前多少条</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetViolationInfo(string city, int count = 4, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            job1.Add("Count", count);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-punishment-worktype", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 指定行政区域内，指定时间区间，每天不良行为和良好行为次数统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPersonRewardpunish(string city, DateTime startDate, DateTime endDate, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            job1.Add("StartDate", startDate);
            job1.Add("EndDate", endDate);

            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-rewardpunish", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 在岗人员教育培训统计
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPersonEducate(string city, DateTime startDate, DateTime endDate, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            job1.Add("StartDate", startDate);
            job1.Add("EndDate", endDate);

            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-educate", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 实名制单点登录url
        /// </summary>
        /// <param name="type">跳转页面类型(0:首页；1:教育；2:奖惩)</param>
        /// <param name="groupid">跳转页面类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetRealNameUrl(int type = 0, int groupid = 0)
        {
            string username = user;
            //username = "admin";
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string data = HttpUtility.UrlEncode(UEncrypter.EncryptByRSA(username + ";" + now, Const.Encryp.PUBLIC_KEY_OTHER));
            // string data1 = UEncrypter.DecryptByRSA(data, Const.Encryp.PRIVATE_KEY_OTHER);
            string realrul = url.Substring(0, url.LastIndexOf(":"));
            return ResponseOutput.Ok(realrul + "/#/login?action=SingleSignOn&type=" + type + "&data=" + data);
        }

        /// <summary>
        /// 实名制看板地址
        /// </summary>
        /// <param name="groupid">分组ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAttendBoardList(int groupid = 0)
        {
            DataTable data = await _boardService.GetAttendBoardListAsync(groupid);
            return ResponseOutput.Ok(data);
        }
    }
}
