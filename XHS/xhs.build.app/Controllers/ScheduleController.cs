using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Services.Schedule;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        public readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// 获取进度列表
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetList")]
        public async Task<IResponseOutput> GetList(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            try
            {
                DataSet dsSum = await _scheduleService.getSum(SITEID);
                DataSet dsSchd = await _scheduleService.getList(SITEID);
                if (dsSum != null && dsSum.Tables != null)
                {
                    DataTable dt = new DataTableHelp().UniteDataTable(dsSum.Tables[0], dsSchd.Tables[0], "");
                    return ResponseOutput.Ok(dt);
                }
                else
                {
                    DataTable dt = dsSchd.Tables[0].Clone();
                    return ResponseOutput.Ok(dt);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="NODEID">阶段ID</param>
        /// <param name="planstart">计划开始日</param>
        /// <param name="planend">计划结束日</param>
        /// <param name="actstart">实际开始日</param>
        /// <param name="actend">实际结束日</param>
        /// <param name="person">责任人</param>
        /// <param name="reason">原因</param>
        /// <param name="updater">更新者</param>
        /// <returns></returns>
        [HttpPost]
        [Route("setSchedule")]
        public async Task<IResponseOutput> SetSchedule([FromForm] int SITEID, [FromForm] string NODEID, [FromForm] string planstart, [FromForm] string planend, [FromForm] string actstart, [FromForm] string actend, [FromForm] string person, [FromForm] string reason, [FromForm] string updater)
        {
            if (SITEID <= 0 || string.IsNullOrEmpty(NODEID) || string.IsNullOrEmpty(updater))
                return ResponseOutput.NotOk();

            try
            {
                object param = new { updater = updater, SITEID = SITEID, NODEID = NODEID, planstart = planstart, planend = planend, actstart = actstart, actend = actend, person = person, reason = reason };

                int result = await _scheduleService.doUpdate(param);

                return ResponseOutput.Ok<int>(result);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }
    }
}