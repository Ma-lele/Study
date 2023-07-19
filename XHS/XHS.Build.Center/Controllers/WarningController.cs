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
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.DeviceBind;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class WarningController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public WarningController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }

        /// <summary>
        /// 安全帽未佩戴
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Helmet(WarnHelmetInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.projid) || string.IsNullOrEmpty(input.location) || input.createtime == null || string.IsNullOrEmpty(input.imgurl) || input.thumblist == null || input.thumblist.Length == 0)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.projid, "helmet");
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
        /// 陌生人
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Stranger(WarnStrangerInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.thumblist == null || jsonData.thumblist.Length == 0)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }

            var DBList = await _deviceBindService.GetDeviceBindByCodeType(jsonData.projid, "stranger");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, jsonData);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
        /// <summary>
        /// 车道识别
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Trespasser(WarnTrespasserInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.thumblist == null || jsonData.thumblist.Length == 0 || jsonData.createtime == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(jsonData.projid, "trespasser");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, jsonData);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
        /// <summary>
        /// 火警
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Fire(WarnFireInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.temperature < 0 || jsonData.createtime == null || jsonData.thumblist == null || jsonData.thumblist.Length == 0)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(jsonData.projid, "fire");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, jsonData);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
        /// <summary>
        /// 卸料平台
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Overload(WarnOverloadInput jsonData)
        {
            if (jsonData == null || jsonData.createtime == null || string.IsNullOrEmpty(jsonData.liftovercode) || jsonData.numofpeople < 0 || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.thumblist == null || jsonData.thumblist.Length == 0 || string.IsNullOrEmpty(jsonData.projid))
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(jsonData.projid, "liftover");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, jsonData);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 区域入侵
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Invade(InvadeWarnInsertInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.invadecode))
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.invadecode, "invade");
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
        /// 反光衣
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ReflectiveVest(WarnReflectiveVestInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.projid) || string.IsNullOrEmpty(input.imgurl) || input.createtime == null || string.IsNullOrEmpty(input.thumburl))
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }

            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.projid, "vest");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/reflectivevest";
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
