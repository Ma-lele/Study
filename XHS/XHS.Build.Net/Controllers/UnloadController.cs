using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Unload;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 卸料平台
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class UnloadController : ControllerBase
    {
        private readonly IUnloadService _unloadService;
        public UnloadController(IUnloadService unloadService)
        {
            _unloadService = unloadService;
        }

        /// <summary>
        /// 实时数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> RealData(UnloadInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.unload_id))
            {
                return ResponseOutput.NotOk("未找到相应设备或状态异常", 0);
            }

            int retDB = await _unloadService.AddRealData(input);
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 报警数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Warn(UnloadWarnInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.unload_id) || input.upstate < 0 || input.upstate > 2)
            {
                return ResponseOutput.NotOk("未找到相应设备或状态异常", 0);
            }

            string unloadid = input.unload_id;
            int retDB = await _unloadService.doWarn(new SugarParameter("@unloadid", unloadid), new SugarParameter("@upstate", input.upstate), new SugarParameter("@weight", input.weight), new SugarParameter("@bias", input.bias), new SugarParameter("@electric", input.electric_quantity), new SugarParameter("@paramjson", JsonConvert.SerializeObject(input)));
            if (retDB > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }
    }
}
