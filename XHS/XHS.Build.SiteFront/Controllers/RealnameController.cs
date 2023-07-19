using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
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
    /// 人员统计/人员二维码
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RealnameController : ControllerBase
    {
        private readonly IUser _user;
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IBaseRepository<BaseEntity> _baseServices;
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
        /// <param name="jwtToken"></param>
        /// <param name="hpSystemSetting"></param>
        public RealnameController(IUser iuser, XHSRealnameToken jwtToken, IHpSystemSetting hpSystemSetting, IBaseRepository<BaseEntity> baseServices)
        {
            _baseServices = baseServices;
            _jwtToken = jwtToken;
            _hpSystemSetting = hpSystemSetting;
            url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            user =  _hpSystemSetting.getSettingValue(Const.Setting.S176);
            pwd = _hpSystemSetting.getSettingValue(Const.Setting.S177);
            _user = iuser;

        }

        /// <summary>
        /// 在册，在岗人员统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T002/[action]")]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetStaffApproach()
        {
            JObject job = new JObject();
            job.Add("projectCode", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/employee-statistics-by-taday", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }

      

        /// <summary>
        /// 人员列表
        /// </summary>
        /// <param name="keyword">姓名/身份证号/所属单位 模糊查</param>
        /// <param name="province">省</param>
        /// <param name="city">市</param>
        /// <param name="township">区/县</param>
        /// <param name="teamname">班组名称</param>
        /// <param name="personstate">必须赋值 人员是否离场状态(1:在岗,0已退场,9全部)</param>
        /// <param name="employeetype">必须赋值 人员类型：0表示管理人员，1表示普通工人 ,9全部</param>
        /// <param name="worktype">工种类型</param>
        /// <param name="pageindex">当前页 从1开始</param>
        /// <param name="pagesize">每页记录数必填</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        [Route("T013/[action]")]
        public async Task<IResponseOutput> GetStaffList(string keyword = "", string province = "", string city = "", string township = "", string teamname = "", int employeetype = 9, string worktype = "", int pageindex = 1, int pagesize = 10, int personstate = 9)
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("keyWord", keyword);
            job.Add("Province", province);
            job.Add("City", city);
            job.Add("Township", township);
            job.Add("TeamName", teamname);
            job.Add("PersonState", personstate);
            job.Add("EmployeeType", employeetype);
            job.Add("WorkType", worktype);
            job.Add("PageIndex", pageindex);
            job.Add("PageSize", pagesize);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/employee-list", JsonConvert.SerializeObject(job));
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
        /// 人员信息
        /// </summary>
        /// <param name="personId">人员编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        [Route("T013/[action]")]
        public async Task<IResponseOutput> GetStaffInfo(string personId = "")
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("Id", personId);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-employee-info", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }
        /// <summary>
        /// 人员考勤明细
        /// </summary>
        /// <param name="personId">人员编号</param>
        /// <param name="month">按月份筛选</param>
        /// <param name="pageindex">当前页 从1开始</param>
        /// <param name="pagesize">每页记录数必填</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        [Route("T013/[action]")]
        public async Task<IResponseOutput> GetStaffAttendance(string personId = "", string month = "", int pageindex = 1, int pagesize = 10)
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("PersonId", personId);
            job.Add("SearchMonth", month);
            job.Add("PageIndex", pageindex);
            job.Add("PageSize", pagesize);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/attendance-clock-list", JsonConvert.SerializeObject(job));
            job.Remove("PageIndex");
            job.Remove("PageSize");
            //考勤时长
            var datatime = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/attendance-clock-statis", JsonConvert.SerializeObject(job));
            job = new JObject();
            if (!string.IsNullOrEmpty(datatime))
            {
                var jobtimedata = JObject.Parse(datatime);
                if (!string.IsNullOrEmpty(jobtimedata.GetValue("data").ToString()))
                {
                    job.Add("duration", (int)jobtimedata["data"]["duration"]);
                }else
                {
                    job.Add("duration", 0);
                }
            }
            else
            {
                job.Add("duration", 0);
            }
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            job.Add("totalCount", jobdata.GetValue("totalCount"));
            job.Add("data", jobdata.GetValue("data"));
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 人员证书信息
        /// </summary>
        /// <param name="personId">人员编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        [Route("T013/[action]")]
        public async Task<IResponseOutput> GetStaffCertificate(string personId = "")
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("PersonId", personId);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-employee-certificate", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 人员教育/培训记录
        /// </summary>
        /// <param name="personId">人员编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        [Route("T013/[action]")]
        public async Task<IResponseOutput> GetStaffEducation(string personId = "")
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("PersonId", personId);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-safety-training", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 培训附件列表
        /// </summary>
        /// <param name="educationId">教育/培训编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetStaffEducationFile(string educationId = "")
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("educationId", educationId);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-training-attachment", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 单位列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetUnitList()
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-companies", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 施工/安管人员考勤统计
        /// </summary>
        /// <param name="EmployeeType">人员类型：0表示管理人员，1表示普通工人，9全部</param>
        /// <param name="month">按月份筛选</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetAttendanceCount(string month = "", int EmployeeType = 9)
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("EmployeeType", EmployeeType);
            job.Add("SearchMonth", month);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-employee-attendance-statistics", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 人员进出场数据列表
        /// </summary>
        /// <param name="keyword">姓名/身份证号/所属单位 模糊查</param>
        /// <param name="province">省</param>
        /// <param name="city">市</param>
        /// <param name="township">区/县</param>
        /// <param name="teamName">班组名称 模糊查</param>
        /// <param name="employeetype">人员类型：0表示管理人员，1表示普通工人，9全部</param>
        /// <param name="month">按月份筛选</param>
        /// <param name="pageindex">当前页 从1开始</param>
        /// <param name="pagesize">每页记录数必填</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetStaffPassOutData(string keyword = "", string province = "", string city = "", string township = "", string teamName = "", int employeetype = 9, string month = "", int pageindex = 1, int pagesize = 10)
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("keyWord", keyword);
            job.Add("Province", province);
            job.Add("City", city);
            job.Add("Township", township);
            job.Add("TeamName", teamName);
            job.Add("EmployeeType", employeetype);
            job.Add("SearchMonth", month);
            job.Add("PageIndex", pageindex);
            job.Add("PageSize", pagesize);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-employee-attendance-list", JsonConvert.SerializeObject(job));
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
        /// 人员行为记录统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetStaffBehaviorCount()
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-award-punish-statistics", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            job = new JObject();
 
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 人员行为记录
        /// </summary>
        /// <param name="keyword">人员名称 模糊查</param>
        /// <param name="worktype">工种类型</param>
        /// <param name="actionyype">行为类型（1:良好记录，2:不良记录，9全部）</param>
        /// <param name="pageindex">当前页 从1开始</param>
        /// <param name="pagesize">每页记录数必填</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetStaffBehaviorRecord(string keyword = "", string worktype = "", int actionyype = 9, int pageindex = 1, int pagesize = 10)
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("EmployeeName", keyword);
            job.Add("WorkType", worktype);
            job.Add("ActionType", actionyype);
            job.Add("PageIndex", pageindex);
            job.Add("PageSize", pagesize);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-award-punish-list", JsonConvert.SerializeObject(job));
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
        /// 班组列表展示
        /// </summary>
        /// <param name="unitcode">社会统一信用代码</param>
        /// <param name="teamname">班组名称 模糊查</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetTeamList(string unitcode = "", string teamname = "")
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("UnifiedSocialCreditCode", unitcode);
            job.Add("TeamName", teamname);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-visitor-team", JsonConvert.SerializeObject(job));
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
        /// 在岗人员二维码统计
        /// </summary>
        /// <param name="employeetype">人员类型：0表示管理人员，1表示普通工人，9全部</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T013/[action]")]
        public async Task<IResponseOutput> GetStaffCodeCount(int employeetype = 9)
        {
            JObject job = new JObject();
            job.Add("ProjectCode", projectcode);
            job.Add("EmployeeType", employeetype);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-onduty-code-statistics", JsonConvert.SerializeObject(job));
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
        [Route("T011/[action]")]
        [Route("T013/[action]")]
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

        /// <summary>
        /// 工种列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        [Route("T013/[action]")]
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
        /// 当前用户下，所有工地的劳务人员总数、男女占比、各年龄段（34岁以下/35~44岁/45~54岁/55岁及以上）占比，各籍贯（省份）的占比，各工种的占比
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetDataCount()
        {
            JObject job = new JObject();
            job.Add("projectCodes", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/projects-employee-statistic", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


        /// <summary>
        /// 单个工地所有人员对应的各班组人数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetteamCount()
        {
            JObject job = new JObject();
            job.Add("projectCode", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/statis-team", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }



       /// <summary>
       /// 工种岗位统计
       /// </summary>
       /// <param name="type">0:施工;1:安管</param>
       /// <returns></returns>
        [HttpGet]
        [Route("T011/[action]")]
        public async Task<IResponseOutput> GetJobCount(int type=0)
        {
            string data = string.Empty;
            JObject job = new JObject();
            job.Add("projectCode", projectcode);
            if(type==0)
            {
                data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/employee-worktype-by-project", JsonConvert.SerializeObject(job));
            }
            else if(type==1)
            {
                data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/employee-admin-by-project", JsonConvert.SerializeObject(job));
            }
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }

        /// <summary>
        /// 单个工地工种数量统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T002/[action]")]
        public async Task<IResponseOutput> GetWorkTypeCount()
        {
            JObject job = new JObject();
            job.Add("projectCode", projectcode);
            var data = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/site-work-type-statistics", JsonConvert.SerializeObject(job));
            if (string.IsNullOrEmpty(data))
            {
                return ResponseOutput.NotOk();
            }
            var jobdata = JObject.Parse(data);
            return ResponseOutput.Ok(jobdata.GetValue("data"));
        }


    }
}
