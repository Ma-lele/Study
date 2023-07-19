using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Response;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.DeepPit;
using XHS.Build.Services.DeviceBind;
using XHS.Build.Services.ElecMeter;
using XHS.Build.Services.HighFormwork;
using XHS.Build.Services.Invade;
using XHS.Build.Services.Unload;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 同步设备信息
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    [NoOprationLog]
    public class DeviceBindController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly IInvadeService _invadeService;
        private readonly IUnloadService _unloadService;
        private readonly IDeepPitService _deepPitService;
        private readonly IHighFormworkService _highFormworkService;
        private readonly IElecMeterService _electricService;
        public DeviceBindController(IDeviceBindService deviceBindService, IElecMeterService electricService, IHighFormworkService highFormworkService, IDeepPitService deepPitService,IInvadeService invadeService, IUnloadService unloadService)
        {
            _deviceBindService = deviceBindService;
            _invadeService = invadeService;
            _unloadService = unloadService;
            _deepPitService = deepPitService;
            _highFormworkService = highFormworkService;
            _electricService = electricService;
        }

        /// <summary>
        /// 获取特种设备信息列表
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Special(DateTime operatedate)
        {
            return await _deviceBindService.GetSpecialEqpApiBindList(operatedate);
        }

        /// <summary>
        /// 获取其他设备信息列表（特种设备以外）
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Site(DateTime operatedate)
        {
            return await _deviceBindService.GetSiteApiBindList(operatedate);
        }

        /// <summary>
        /// 区域入侵
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Invade(DateTime operatedate)
        {
            return ResponseOutput.Ok(await _invadeService.GetDistinctInvadeList(operatedate));
        }

        /// <summary>
        /// 卸料平台
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Unload(DateTime operatedate)
        {
            return ResponseOutput.Ok(await _unloadService.GetDistinctUnloadList(operatedate));
        }


        /// <summary>
        /// 深基坑
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> DeepPit(DateTime operatedate)
        {
            return ResponseOutput.Ok(await _deepPitService.GetDistinctDeepPitList(operatedate));
        }

        /// <summary>
        /// 高支模
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> HighFormwork(DateTime operatedate)
        {
            return ResponseOutput.Ok(await _highFormworkService.GetDistinctHighFormworkList(operatedate));
        }

        /// <summary>
        /// 智慧电表
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ElecMeter(DateTime operatedate)
        {
            return ResponseOutput.Ok(await _electricService.GetDistinctElecMeterList(operatedate));
        }
    }
}
