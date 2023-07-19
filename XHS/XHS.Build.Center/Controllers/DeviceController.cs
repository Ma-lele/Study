using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.DeviceBind;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public DeviceController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }

        /// <summary>
        /// 上次实时数据
        /// </summary>
        /// <param name="input">json数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetRtdData(DeviceRtdDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.devicecode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.devicecode, "");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, input);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 离线
        /// </summary>
        /// <param name="devicecode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Offline(string devicecode)
        {
            if (string.IsNullOrEmpty(devicecode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(devicecode,"");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.UrlRequest(netApi, "POST", DBList, new Dictionary<string, object>() { { "devicecode", devicecode } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
    }
}
