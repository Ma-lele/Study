using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Services.Cable;
using XHS.Build.Services.SystemSetting;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 钢丝绳监测
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CableController : ControllerBase
    {
        private readonly ICableService _cableService;
        public CableController(ICableService cableService)
        {
            _cableService = cableService;
        }


        /// <summary>
        /// 获取监测点下钢丝绳监测设备
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns>实时值数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListForSite(int SITEID)
        {
            return ResponseOutput.Ok(await _cableService.GetListForSite(SITEID));
        }


        /// <summary>
        ///  获取最大损伤数据
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetMaxDamage(string sensorid)
        { 
           return ResponseOutput.Ok(await _cableService.GetMaxDamage(sensorid));
           
        }

        /// <summary>
        /// 获取完整的损伤数据
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAllDamage(string sensorid)
        {
            return ResponseOutput.Ok(await _cableService.GetAllDamage(sensorid));
        }

        /// <summary>
        /// 获取完整断丝预测
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetBroken(string sensorid)
        {
            return ResponseOutput.Ok(await _cableService.GetBroken(sensorid));
        }
    }
}
