using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.DeviceBind;
using XHS.Build.Services.OperateLogS;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class TowerCraneController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;
        private readonly IOperateLogService _operateLogService;
        private readonly IMapper _mapper;

        public TowerCraneController(NetToken netToken, IDeviceBindService deviceBindService, IMapper mapper, IOperateLogService  operateLogService)
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
                return ResponseOutput.NotOk("请填写设备信息");
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
                return ResponseOutput.NotOk("请填写设备信息");
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
        /// <param name="realData">实时数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> RealData(TowerCraneRealDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.SeCode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            //var seData = _mapper.Map<SpecialEqpData>(input);
            //seData.Platform = "center";
            //seData.Flag = 1;
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
        /// 报警数据
        /// </summary>
        /// <param name="alarmData">报警数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AlarmData(TowerCraneAlarmInput alarmData)
        {
            if (alarmData == null || string.IsNullOrEmpty(alarmData.SeCode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(alarmData.SeCode, "special");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, alarmData);
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
        /// <param name="paramsData">参数数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ParamsData(TowerCraneParamsDataInput input)
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
        /// 防倾翻报警诊断
        /// </summary>
        /// <param name="tipOverData">防倾翻报警诊断数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> TipOverData(TipOverDataInput input)
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

        /// <summary>
        /// 工作循环
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> WorkData(TowerCraneWorkDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.SeCode))
            {
                return ResponseOutput.NotOk("请输入正确的参数");
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
    }
}
