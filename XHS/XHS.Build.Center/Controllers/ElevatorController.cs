using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.DeviceBind;
using XHS.Build.Services.Elevator;
using XHS.Build.Services.OperateLogS;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class ElevatorController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;
        private readonly IOperateLogService _operateLogService;
        private readonly IMapper _mapper;
        public ElevatorController(NetToken netToken, IDeviceBindService deviceBindService, IOperateLogService operateLogService, IMapper mapper)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
            _operateLogService = operateLogService;
            _mapper = mapper;
        }

        /// <summary>
        /// 设备上线
        /// </summary>
        /// <param name="secode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Online(string secode)
        {
            if (string.IsNullOrEmpty(secode))
            {
                return ResponseOutput.NotOk("请选择设备信息");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(secode, "special");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.UrlRequest(netApi, "POST", DBList, new Dictionary<string, object>() { { "secode", secode } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 设备下线
        /// </summary>
        /// <param name="secode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Offline(string secode)
        {
            if (string.IsNullOrEmpty(secode))
            {
                return ResponseOutput.NotOk("请选择设备信息");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(secode, "special");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.UrlRequest(netApi, "POST", DBList, new Dictionary<string, object>() { { "secode", secode } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="input">实时数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> RealData([FromBody] ElevatorRealDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.SeCode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            //var seData= _mapper.Map<SpecialEqpData>(input);
            //seData.Platform = "center";
            //seData.Flag = 2;
            //_operateLogService.AddSpecialEqpDataLog(seData);

            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.SeCode, "special");
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
        /// 设备参数数据
        /// </summary>
        /// <param name="input">参数数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ParamsData([Required] ElevatorParamsDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.SeCode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.SeCode, "special");
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
        /// 人员上机下机刷脸认证
        /// </summary>
        /// <param name="authData"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(204800)]
        public async Task<IResponseOutput> AuthData(AuthData authData)
        {
            if (authData == null || string.IsNullOrEmpty(authData.SeCode))
            {
                return ResponseOutput.NotOk("请输入正确的参数");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(authData.SeCode, "special");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, authData);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
    }
}
