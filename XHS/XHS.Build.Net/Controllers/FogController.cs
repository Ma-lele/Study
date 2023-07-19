using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Fog;
using static XHS.Build.Common.Helps.HpFog;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 雾炮喷淋
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class FogController : ControllerBase
    {
        private readonly IFogService _fogService;
        public FogController(IFogService fogService)
        {
            _fogService = fogService;
        }

        /// <summary>
        /// 雾泡喷淋开启
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> TurnOn(BnCmd bn)
        {
            if (string.IsNullOrEmpty(bn.fc) || string.IsNullOrEmpty(bn.sw) || bn.sw.ObjToInt() < 1 || bn.sw.ObjToInt() > 4 || bn.delay.ObjToInt() <= 0)
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            HpFog.SendCommand(bn);
            return ResponseOutput.Ok(1);
        }

        /// <summary>
        /// 雾泡喷淋关闭
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> TurnOff(BnCmd bn)
        {
            if (string.IsNullOrEmpty(bn.fc) || string.IsNullOrEmpty(bn.sw) || bn.sw.ObjToInt() < 1 || bn.sw.ObjToInt() > 4)
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            HpFog.SendCommand(bn);
            return ResponseOutput.Ok(1);
        }

        /// <summary>
        /// 雾泡喷淋设备上线
        /// </summary>
        /// <param name="fogcode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Online([Required]string fogcode)
        {
            if (string.IsNullOrEmpty(fogcode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            int result = await _fogService.doCheckin(fogcode);
            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("雾泡喷淋设备上线失败", 0);
        }

        /// <summary>
        /// 雾泡喷淋设备下线
        /// </summary>
        /// <param name="fogcode">设备编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Offline([Required] string fogcode)
        {
            if (string.IsNullOrEmpty(fogcode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            int result = await _fogService.doCheckin(fogcode);
            return result > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("雾泡喷淋设备下线失败", 0);
        }
    }
}
