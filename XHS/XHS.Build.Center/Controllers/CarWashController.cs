using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.DeviceBind;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class CarWashController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public CarWashController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }
        /// <summary>
        /// 车辆冲洗设备下线报警
        /// </summary>
        /// <param name="parkkey">停车场编号</param>
        /// <param name="gatename">车道名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AlarmOffline([FromForm] string parkkey, [FromForm] string gatename)
        {
            if (string.IsNullOrEmpty(parkkey) || string.IsNullOrEmpty(gatename))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(parkkey, "carwash");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                //这里 多个循环部分成功部分失败如何处理
                _netToken.FormRequest(netApi, DBList, new Dictionary<string, object>() { { "parkkey", parkkey }, { "gatename", gatename } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
        /// <summary>
        /// 车辆未冲洗报警
        /// </summary>
        /// <param name="parkkey">停车场编号</param>
        /// <param name="gatename">车道名称</param>
        /// <param name="carno">车牌</param>
        /// <param name="img">照片地址</param>
        /// <param name="video">视频地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AlarmUnWashed([FromForm] string parkkey, [FromForm] string gatename, [FromForm] string carno, [FromForm] string img, [FromForm] string video)
        {
            if (string.IsNullOrEmpty(parkkey) || string.IsNullOrEmpty(gatename) || string.IsNullOrEmpty(carno))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            var DBList = await _deviceBindService.GetDeviceBindByCodeType(parkkey, "carwash");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.FormRequest(netApi, DBList, new Dictionary<string, object>() { { "parkkey", parkkey }, { "gatename", gatename }, { "carno", carno }, { "img", img }, { "video", video } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }


        /// <summary>
        /// 车辆冲洗
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Wash(CarWashInsertDto dto)
        {
            if (dto == null)
            {
                return ResponseOutput.NotOk("请填写车辆冲洗信息");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(dto.cwcode, "carwash");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, dto);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 车辆冲洗设备下线超时2级报警
        /// </summary>
        /// <param name="parkkey">停车场编号</param>
        /// <param name="gatename">车道名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AlarmOfflineTimeout2([FromForm] string parkkey, [FromForm] string gatename)
        {
            if (string.IsNullOrEmpty(parkkey) || string.IsNullOrEmpty(gatename))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(parkkey, "carwash");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.FormRequest(netApi, DBList, new Dictionary<string, object>() { { "parkkey", parkkey }, { "gatename", gatename } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
        /// <summary>
        /// 车辆冲洗设备下线超时3级报警
        /// </summary>
        /// <param name="parkkey">停车场编号</param>
        /// <param name="gatename">车道名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AlarmOfflineTimeout3([FromForm] string parkkey, [FromForm] string gatename)
        {
            if (string.IsNullOrEmpty(parkkey) || string.IsNullOrEmpty(gatename))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(parkkey, "carwash");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.FormRequest(netApi, DBList, new Dictionary<string, object>() { { "parkkey", parkkey }, { "gatename", gatename } });
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
    }
}
