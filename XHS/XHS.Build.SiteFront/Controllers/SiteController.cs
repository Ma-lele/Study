using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Site;
using XHS.Build.Common.Helps;
using XHS.Build.SiteFront.Attributes;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 项目管理
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    [Permission]
    public class SiteController : ControllerBase
    {

        private readonly ISiteService _siteService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteService"></param>
        public SiteController(ISiteService siteService, IUser user)
        {
            _siteService = siteService;
            _user = user;
        }

        /// <summary>
        /// 获取用户下监测点
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T000/[action]")]
        public async Task<IResponseOutput> GetSiteList()
        {
           
                DataTable  dataTable = await _siteService.getV2ListByUserId(int.Parse(_user.Id));                
                return ResponseOutput.Ok(dataTable);
        }


        /// <summary>
        /// 设备地图
        /// </summary>
        /// <param name="type">1 扬尘，2 摄像头，3 塔吊，4 升降机，5 卸料平台，6 深基坑，7 高支模，8 临边防护,9 实名制</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T000/[action]")]
        public async Task<IResponseOutput> DeviceMap(int type)
        {
            var result = await _siteService.spV2CommonMap(type, _user.SiteId.ToInt());

            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 切换监测点
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("T000/[action]")]
        public async Task<IResponseOutput> SetSite(string siteid)
        {

            _user.SiteId = siteid;
            return ResponseOutput.Ok();
        }

        /// <summary>
        /// 获取监测点详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T002/[action]")]
        public async Task<IResponseOutput> GetSiteInfo()
        {

            DataTable dataTable = await _siteService.getV2SiteInfo(int.Parse(_user.SiteId));
            return ResponseOutput.Ok(dataTable);
        }

        /// <summary>
        /// 获取监测点五方单位信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T002/[action]")]
        public async Task<IResponseOutput> GetSiteFiveParts()
        {

            DataTable dataTable = await _siteService.getV2SiteFiveParts(int.Parse(_user.SiteId));
            return ResponseOutput.Ok(dataTable);
        }

    }
}
