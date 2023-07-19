using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Cameras;
using XHS.Build.Services.Device;
using XHS.Build.Services.DeviceCN;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 扬尘设备
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceCNService _deviceCNService;
        private readonly IDeviceService _deviceService;
        private readonly ICameraService _cameraService;
        private readonly IUserKey _userKey;
        public DeviceController(IDeviceCNService deviceCNService, IDeviceService deviceService, ICameraService cameraService, IUserKey userKey)
        {
            _deviceCNService = deviceCNService;
            _deviceService = deviceService;
            _cameraService = cameraService;
            _userKey = userKey;
        }

        /// <summary>
        /// 上传实时数据
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
            
            int result = await _deviceCNService.rtdInsert(input);

            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("上传数据失败", 0);
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

            int result = await _deviceService.doCheckout(devicecode, 1);
            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("设备离线失败", 0);
        }

        /// <summary>
        /// 视频上线
        /// </summary>
        /// <param name="deviceId">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> VideoOnline(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            int result = await _cameraService.CamerabonlineUpdateAsync(deviceId, 1);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -22)
            {
                return ResponseOutput.NotOk("上传失败(该设备不存在)", 0);
            }
            else
            {
                return ResponseOutput.NotOk("上传失败", 0);
            }
        }

        /// <summary>
        /// 视频离线
        /// </summary>
        /// <param name="deviceId">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> VideoOffline(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            int result = await _cameraService.CamerabonlineUpdateAsync(deviceId, 0);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -22)
            {
                return ResponseOutput.NotOk("上传失败(该设备不存在)", 0);
            }
            else
            {
                return ResponseOutput.NotOk("上传失败", 0);
            }
        }


        /// <summary>
        /// 5.2	设备信息上传
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddDeviceFacture(DeviceDto dto)
        {
            if (string.IsNullOrEmpty(dto.recordNumber) || dto.deviceType < 0 || dto.deviceType > 14 || string.IsNullOrEmpty(dto.deviceId))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            dto.updater = _userKey.Name;
            int result = await _deviceService.AddDeviceFacture(dto);
            if (result > 0)
            {
                return ResponseOutput.Ok(1, "上传成功。");
            }
            else if (result == -21)
            {
                return ResponseOutput.NotOk("设备上传失败(该设备已存在)", 0);
            }
            else
            {
                return ResponseOutput.NotOk("设备上传失败", 0);
            }
        }


        /// <summary>
        /// 5.2	设备信息删除
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeleteDeviceFacture(DeviceDto dto)
        {
            if (string.IsNullOrEmpty(dto.recordNumber) || dto.deviceType < 0 || dto.deviceType > 14 || string.IsNullOrEmpty(dto.deviceId))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            dto.updater = _userKey.Name;
            int result = await _deviceService.deleteDeviceFacture(dto);
            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("设备删除失败或该设备已删除。", 0);
        }
    }
}
