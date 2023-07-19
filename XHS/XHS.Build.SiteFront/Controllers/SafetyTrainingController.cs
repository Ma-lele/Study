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
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 教育培训
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SafetyTrainingController : ControllerBase
    {
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IBaseRepository<BaseEntity> _baseServices;
        private readonly IUser _user;

        string url;
        string user;
        string pwd;

        public string projectcode
        {
            get
            {
                return _baseServices.Db.Queryable<GCSiteEntity>().Where(a => a.SITEID == Convert.ToInt32(_user.SiteId)).First().attendprojid;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iuser"></param>
        /// <param name="jwtToken"></param>
        /// <param name="hpSystemSetting"></param>
        /// <param name="baseServices"></param>
        public SafetyTrainingController(IUser iuser, XHSRealnameToken jwtToken, IHpSystemSetting hpSystemSetting, IBaseRepository<BaseEntity> baseServices)
        {
            _user = iuser;
            _baseServices = baseServices;
            _jwtToken = jwtToken;
            _hpSystemSetting = hpSystemSetting;
            url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            user = _hpSystemSetting.getSettingValue(Const.Setting.S176);
            pwd = _hpSystemSetting.getSettingValue(Const.Setting.S177);
        }


        /// <summary>
        /// 在岗施工人员教育/培训数据统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T014/[action]")]
        public async Task<IResponseOutput> GetEducationDataCount()
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);

            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-on-duty-train-statistics", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata);
        }

        /// <summary>
        /// 教育/培训统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T014/[action]")]
        public async Task<IResponseOutput> GetEducationCount(string startTime="",string endTime="")
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("StartTime", startTime);
            job.Add("EndTime", endTime);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-education-train-statistics", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 人员教育/培训列表
        /// </summary>
        /// <param name="keyword">姓名/所属单位 模糊查</param>
        /// <param name="employeetype">人员类型：0表示管理人员，1表示普通工人，9全部</param>
        /// <param name="teamname">班组</param>
        /// <param name="worktype">工种</param>
        /// <param name="pageindex">当前页 从1开始</param>
        /// <param name="pagesize">每页记录数必填</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T014/[action]")]
        public async Task<IResponseOutput> GetStaffEducationList(string keyword="",int employeetype=9,string teamname="",string worktype="",int pageindex=1,int pagesize=10)
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("keyWord", keyword);
            job.Add("EmployeeType", employeetype);
            job.Add("TeamName", teamname);
            job.Add("WorkType", worktype);
            job.Add("PageIndex", pageindex);
            job.Add("PageSize", pagesize);
           
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-education-train-list", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            job = new JObject();
            job.Add("totalCount", jobdata.GetValue("totalCount"));
            job.Add("data", jobdata.GetValue("data"));
            return ResponseOutput.Ok(job);
        }



       /// <summary>
       /// 工种列表
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        [Route("T014/[action]")]
        public async Task<IResponseOutput> GetWorkTypeList()
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-work-type-list", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 班组列表下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T014/[action]")]
        public async Task<IResponseOutput> GetTeamsList()
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-teams-list", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }




    }
}
