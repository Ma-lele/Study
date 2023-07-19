using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Services.AIAirTightAction;
using XHS.Build.Services.AIAirTightAction.Dtos;
using XHS.Build.Services.AISoilAction;
using XHS.Build.Services.AISoilAction.Dtos;
using XHS.Build.Services.AISprayAction;
using XHS.Build.Services.AISprayAction.Dtos;
using XHS.Build.Services.Site;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// AI数据
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly IAIAirTightService _tightService;
        private readonly IAISoilService _soilService;
        private readonly IAISprayService _sprayService;
        private readonly ISiteService _siteService;
        /// <summary>
        /// 构造函数
        /// </summary>
        public AIController(IAIAirTightService tightService, IAISoilService soilService, IAISprayService sprayService, ISiteService siteService)
        {
            _tightService = tightService;
            _soilService = soilService;
            _sprayService = sprayService;
            _siteService = siteService;
        }

        /// <summary>
        /// 密闭运输信息取得
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAirTightList(int SITEID, DateTime date)
        {
            DataTable dt = await _tightService.GetAirTightList(SITEID, date);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 黄土裸露信息取得
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSoilList(int SITEID, DateTime date)
        {
            DataTable dt = await _soilService.GetSoilList(SITEID, date);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 雾炮喷淋信息取得
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSparyList(int SITEID, DateTime date)
        {
            DataTable dt = await _sprayService.GetSprayList(SITEID, date);
            return ResponseOutput.Ok(dt);
        }
    }
}
