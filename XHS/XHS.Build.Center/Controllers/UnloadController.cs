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
    public class UnloadController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public UnloadController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }

        [HttpPost]
        public async Task<IResponseOutput> RealData(UnloadDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.unload_id))
            {
                return ResponseOutput.NotOk("未找到相应设备或状态异常", 0);
            }

            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.unload_id, "unload");
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

        [HttpPost]
        public async Task<IResponseOutput> Warn(UnloadWarnInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.unload_id) || input.upstate < 0 || input.upstate > 2)
            {
                return ResponseOutput.NotOk("未找到相应设备或状态异常", 0);
            }

            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.unload_id, "unload");
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
    }
}
