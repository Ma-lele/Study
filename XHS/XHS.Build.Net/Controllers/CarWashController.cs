using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Warning;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 车辆冲洗
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class CarWashController : ControllerBase
    {
        private readonly IWarningService _warningService;
        public CarWashController(IWarningService warningService)
        {
            _warningService = warningService;
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

            int retDB = await _warningService.doCarWashOffline(parkkey, gatename);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), 0);
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
            SgParams sp = new SgParams();
            sp.Add("parkkey", parkkey);
            sp.Add("gatename", gatename);
            sp.Add("carno", carno);
            sp.Add("img", img);
            sp.Add("video", video);
            sp.NeetReturnValue();
            int retDB = await _warningService.doCarUnWashed(sp);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), 0);
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
            int retDB = await _warningService.CarWashInsert(dto);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), 0);
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

            int retDB = await _warningService.doCarWashOfflineTimeout(parkkey, gatename, 311);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), 0);
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

            int retDB = await _warningService.doCarWashOfflineTimeout(parkkey, gatename, 312);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), 0);
            }
        }
    }
}
