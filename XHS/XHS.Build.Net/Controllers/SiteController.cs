using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 检测对象
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class SiteController : ControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly IUserKey _userKey;
        private readonly IDeviceCNService _deviceCNService;
        private readonly IHpSystemSetting _hpSystemSetting;
        public IConfiguration _configuration;
        public SiteController(ISiteService siteService, IUserKey userKey, IDeviceCNService deviceCNService, IConfiguration configuration, IHpSystemSetting hpSystemSetting)
        {
            _siteService = siteService;
            _userKey = userKey;
            _deviceCNService = deviceCNService;
            _configuration = configuration;
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 获取监测对象列表
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int index, int size)
        {
            if (index <= 0 || size <= 0 || size > 1000)
            {
                return ResponseOutput.NotOk("请填写正确的参数");
            }
            DataTable dt = await _siteService.getListForApi(_userKey.GroupId, index, size);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取用户下监测对象列表
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListForUser(int userid)
        {
            if (userid <= 0)
            {
                return ResponseOutput.NotOk("请填写正确的参数");
            }
            DataTable dt = await _siteService.getListByUserId(userid);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取检测点下的用户
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetUserList(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请填写正确的参数");
            }

            DataTable dt = await _siteService.getUserForApi(SITEID);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取当前监测对象的扬尘实时数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteRtdData(int index, int size)
        {
            dynamic expObj = new ExpandoObject();
            expObj.result = 0;
            if (index <= 0 || size <= 0 || size > 1000)
            {
                return ResponseOutput.NotOk("请填写正确的参数");
            }

            DataTable dt = await _deviceCNService.getSiteAllRtdApi(_userKey.GroupId, index, size);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取1小时内实时扬尘数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetRtdOneHourData(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请填写正确的参数");
            }

            DataTable dt = await _deviceCNService.getSiteRtdOneHourApi(SITEID);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取分组首页Url
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResponseOutput GetGroupBoard()
        {
            //私钥
            string publicKey = _configuration.GetSection("WebConfig").GetValue<string>("PublicKey");
            string siteDomain = _hpSystemSetting.getSettingValue(Const.Setting.S034);
            JObject jparam = new JObject();
            jparam.Add("username", _userKey.UserId);
            jparam.Add("dt", DateTime.Now);
            string uparam = JsonConvert.SerializeObject(jparam);
            uparam = UEncrypter.EncryptByRSA16(uparam, publicKey);
            string url = string.Format("http://{0}/handler/HLogin.ashx?action=groupboard&data={1}", siteDomain, uparam);
            return ResponseOutput.Ok(new JObject { { "url", url } });
        }

        /// <summary>
        /// 5.1	项目信息上传接口
        /// </summary>
        /// <param name="dto">项目信息</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IResponseOutput> AddSite(SiteDto dto)
        {
            JObject mJObj = new JObject();
            dto.@operator = _userKey.Name;
            int result = await _siteService.saveSite(dto);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -1)
            {
                return ResponseOutput.NotOk("操作失败。(未找到对应的分组。)");
            }
            else if (result == -2)
            {
                return ResponseOutput.NotOk("操作失败。(该项目已上传。)");
            }
            else
            {
                return ResponseOutput.NotOk("操作失败");
            }
        }

    }
}
