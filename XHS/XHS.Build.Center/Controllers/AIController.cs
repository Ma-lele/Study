using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.AIAirTightAction.Dtos;
using XHS.Build.Services.AISoilAction.Dtos;
using XHS.Build.Services.AISprayAction.Dtos;
using XHS.Build.Services.DeviceBind;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public AIController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }

        /// <summary>
        /// 密闭运输
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("airtight")]
        public async Task<IResponseOutput> PostAirTight(AirTightInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.projid, "airtight");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/airtight";
                _netToken.JsonRequest(netApi, DBList, input);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 黄土裸露
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("soil")]
        public async Task<IResponseOutput> PostSoil(AISoilInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }

            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.projid, "soil");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/soil";
                _netToken.JsonRequest(netApi, DBList, input);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 雾炮喷淋
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("spary")]
        public async Task<IResponseOutput> PostSpary(AISparyInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.projid, "spray");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/spary";
                _netToken.JsonRequest(netApi, DBList, input);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }


    }
}
