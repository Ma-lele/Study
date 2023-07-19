using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.DeviceBind;

namespace XHS.Build.Center.Controllers
{
    /// <summary>
    /// 深基坑信息接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class DeepPitController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;

        public DeepPitController(NetToken netToken, IDeviceBindService deviceBindService)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
        }

        /// <summary>
        /// 深基坑结构物数据上传
        /// </summary>
        /// <param name="dto">深基坑结构物Dto</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeepPit(DeepPitInput dto)
        {
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(dto.dpCode, "deeppit");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, dto);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 深基坑实时数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="collectionTime">采集时间</param>
        /// <param name="monitorType">监测项</param>
        /// <param name="warnValue">预警阀值</param>
        /// <param name="alarmValue">报警阀值</param>
        /// <param name="data">数值</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeepPitHistory(DeepPitRtdDto dto)
        {
            if (dto.monitorType<=0 || dto.monitorType>13)
            {
                return ResponseOutput.NotOk("参数不正确(监测项不在范围内）。");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(dto.dpCode, "deeppit");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, dto);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }


        /// <summary>
        /// 深基坑预警数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="warnExplain">报警类型(电量、温度、立杆轴力、水平倾角、立杆倾角、水平位移、模板沉降等)</param>
        /// <param name="warnContent">预警内容</param>
        /// <param name="happenTime">发生时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeepPitAlarmInfo(DeepPitAlarmInfoDto dto)
        {
            if (dto.alarmExplain < 0 || dto.alarmExplain > 13)
            {
                return ResponseOutput.NotOk("参数不正确(报警类型不在范围内）。");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(dto.dpCode, "deeppit");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, dto);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }


    }
}
