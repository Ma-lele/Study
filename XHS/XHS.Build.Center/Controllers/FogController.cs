using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.DeviceBind;
using static XHS.Build.Common.Helps.HpFog;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class FogController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public FogController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }

        /// <summary>
        /// 雾泡喷淋开启
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> TurnOn(BnCmd bn)
        {
            if (string.IsNullOrEmpty(bn.fc) || string.IsNullOrEmpty(bn.sw) || bn.sw.ObjToInt() < 1 || bn.sw.ObjToInt() > 4 || bn.delay.ObjToInt() <= 0)
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(bn.fc,"");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, bn);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 雾泡喷淋关闭
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> TurnOff(BnCmd bn)
        {
            if (string.IsNullOrEmpty(bn.fc) || string.IsNullOrEmpty(bn.sw) || bn.sw.ObjToInt() < 1 || bn.sw.ObjToInt() > 4)
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(bn.fc,"");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, bn);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 雾泡喷淋设备上线
        /// </summary>
        /// <param name="fogcode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Online([Required] string fogcode)
        {
            if (string.IsNullOrEmpty(fogcode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(fogcode,"");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.UrlRequest(netApi, "POST", DBList, new Dictionary<string, object>() { { "fogcode", fogcode } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 雾泡喷淋设备下线
        /// </summary>
        /// <param name="fogcode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Offline([Required] string fogcode)
        {
            if (string.IsNullOrEmpty(fogcode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(fogcode,"");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.UrlRequest(netApi, "POST", DBList, new Dictionary<string, object>() { { "fogcode", fogcode } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
    }
}
