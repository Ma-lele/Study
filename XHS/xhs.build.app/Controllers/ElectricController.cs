using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.ElecMeter;
using XHS.Build.Services.Warning;

namespace XHS.Build.app.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ElectricController : ControllerBase
    {
        private readonly IElecMeterService _electricService;
        private readonly IWarningService _warningService;
        public ElectricController(IElecMeterService electricService, IWarningService warningService)
        {
            _electricService = electricService;
            _warningService = warningService;
        }

        /// <summary>
        /// 获取站点下电表信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetElecMeterList(int siteid)
        {
            DataTable dt = await _electricService.GetElecMeterListBySiteId(siteid);

            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取电表实时监测数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetElecRtdData(int siteid,string emetercode)
        {
            DataTable dt = await _electricService.GetElecRtdData(siteid, emetercode);

            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取电表历史数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetElecHisData(int siteid, string emetercode,DateTime time)
        {
            DataTable dt = await _electricService.GetElecHisData(siteid, emetercode, time);

            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 电表告警 设备占比
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteElecWarnChart(int siteid, DateTime startdate, DateTime enddate)
        {
            DataTable dt = await _electricService.GetSiteElecWarnChart(siteid, startdate, enddate);

            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 电表告警类型占比
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteElecWarnTypeChart(int siteid, DateTime startdate, DateTime enddate)
        {
            DataTable dt = await _electricService.GetSiteElecWarnTypeChart(siteid, startdate, enddate);

            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取电表告警列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteElecWarnList(int siteid, string emetercode, DateTime startdate, DateTime enddate, int pageindex = 1, int pagesize = 10)
        {
            DataTable dt = await _electricService.GetSiteElecWarnList(siteid, emetercode, startdate, enddate, pageindex, pagesize);

            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取电表告警列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteElecLatestWarnList(int siteid, string emetercode)
        {
            DataTable dt = await _electricService.GetSiteElecLatestWarnList(siteid, emetercode);

            return ResponseOutput.Ok(dt);
        }
    }
}
