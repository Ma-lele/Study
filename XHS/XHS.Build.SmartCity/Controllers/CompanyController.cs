using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Company;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 单位统计
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {

        private readonly XHSRealnameToken _jwtToken;
        private readonly ICompanyService _companyService;
        private readonly IHpSystemSetting _hpSystemSetting;
        string url;
        string user;
        string pwd;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyService"></param>
        public CompanyController(ICompanyService companyService, XHSRealnameToken jwtToken, IHpSystemSetting hpSystemSetting)
        {
            _companyService = companyService;
            _jwtToken = jwtToken;
            _hpSystemSetting = hpSystemSetting;
            url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            user = _hpSystemSetting.getSettingValue(Const.Setting.S176);
            pwd = _hpSystemSetting.getSettingValue(Const.Setting.S177);
        }

        /// <summary>
        /// 单位统计详情
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <param name="keyword">查询内容</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页多少条</param>
        /// <param name="companytype">单位类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetUnitStatistics(string city, int pageindex = 1, int pagesize = 20, string district = "", string keyword = "", int companytype = 0)
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            job1.Add("PageIndex", pageindex);
            job1.Add("PageSize", pagesize);
            job1.Add("KeyWord", keyword);
            job1.Add("CompanyType", companytype);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-unit-statisitc", JsonConvert.SerializeObject(job1));
            if (string.IsNullOrEmpty(data1))
            {
                return ResponseOutput.NotOk();
            }
            var job = JObject.Parse(data1);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 单位基本信息
        /// </summary>
        /// <param name="creditCode">统一信用码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetUnitInfo(string creditCode = "")
        {
            JObject job1 = new JObject();
            job1.Add("creditCode", creditCode);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-unit-basice-info", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 根据企业信息表ID和参建类型 获取项目信息
        /// </summary>
        /// <param name="id">企业信息表ID</param>
        /// <param name="companyType">参建类型</param>
        /// <returns></returns>
        [HttpGet]
        public IResponseOutput GetPorjectInfo(string id ,string companyType)
        {
            JObject job1 = new JObject();
            job1.Add("id", id);
            job1.Add("companyType", companyType);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "/api/realname/construction-site-api-app-service-new/{id}/project-info-by-id-type", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 关联项目信息
        /// </summary>
        /// <param name="creditCode">统一信用代码</param>
        /// <param name="keyword">查询内容</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页多少条</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetrRelationProject(string creditCode = "", string keyword = "", int pageindex = 1, int pagesize = 20)
        {
            var data = await _companyService.GetrRelationProjectAsync(creditCode, keyword, pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 处罚信息
        /// </summary>
        /// <param name="creditCode">统一信用代码</param>
        /// <param name="keyword">查询内容</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页多少条</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPunishInfo(string creditCode = "", string keyword = "", int pageindex = 1, int pagesize = 20)
        {
            var data = await _companyService.GetPunishInfoAsync(creditCode, keyword, pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 单位对应数量
        /// </summary>
        /// <param name="city">市名</param>
        /// <param name="district">区名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetUnitCount(string city, string district = "")
        {
            JObject job1 = new JObject();
            job1.Add("City", city);
            job1.Add("Township", district);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-unit-count", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1).GetValue("data");
            return ResponseOutput.Ok(job);
        }



        /// <summary>
        /// 市平台单位统计画面
        /// 这个单位关联的所有项目的projectCode（逗号分隔）
        /// </summary>
        /// <param name="creditCode">单位统一信用代码</param>
        /// <returns></returns>a
        [HttpGet]
        public async Task<IResponseOutput> GetSite(string creditCode)
        {
            JObject job1 = new JObject();
            job1.Add("creditCode", creditCode);
            var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api/city-platform-relative-project-code", JsonConvert.SerializeObject(job1));
            var job = JObject.Parse(data1);
            var data = await _companyService.GetSiteAsync(job.GetValue("data").ToString());
            return ResponseOutput.Ok(data);
        }
    }
}
