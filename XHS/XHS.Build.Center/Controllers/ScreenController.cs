using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.DeviceBind;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ScreenController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public ScreenController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }



        /// <summary>
        /// 大屏
        /// </summary>
        /// <param name="screencode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetNotice(string screencode)
        {
            if (string.IsNullOrEmpty(screencode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCode(screencode);
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.UrlRequest(netApi, "GET", DBList, new Dictionary<string, object>() { { "screencode", screencode } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 大屏
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SyncNotice(ScreenSyncNoticeInput json)
        {
            if (json == null || string.IsNullOrEmpty(json.screencode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCode(json.screencode);
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, json);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
    }
}
