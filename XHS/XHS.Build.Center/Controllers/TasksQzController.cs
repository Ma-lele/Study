using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Center.Jobs;
using XHS.Build.Common.Response;
using XHS.Build.Services.TaskQz;

namespace XHS.Build.Center.Controllers
{
    /// <summary>
    /// 控制服务开关
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TasksQzController : ControllerBase
    {
        private readonly ITasksQzServices _tasksQzServices;
        private readonly ISchedulerCenter _schedulerCenter;

        public TasksQzController(ITasksQzServices tasksQzServices, ISchedulerCenter schedulerCenter)
        {
            _tasksQzServices = tasksQzServices;
            _schedulerCenter = schedulerCenter;
        }


        /// <summary>
        /// 启动计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> StartJob(int jobId)
        {
            var data = new ResponseOutput<bool>();

            var model = await _tasksQzServices.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.AddScheduleJobAsync(model);
                if (ResuleModel.success)
                {
                    model.IsStart = true;
                    await _tasksQzServices.Update(model);
                    data.Ok(true);
                }
            }
            return data;

        }
        /// <summary>
        /// 停止一个计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>        
        [HttpGet]
        public async Task<IResponseOutput> StopJob(int jobId)
        {
            var data = new ResponseOutput<bool>();

            var model = await _tasksQzServices.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                if (ResuleModel.success)
                {
                    model.IsStart = false;
                    await _tasksQzServices.Update(model);
                    data.Ok(true);
                }
            }
            return data;

        }
        /// <summary>
        /// 重启一个计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ReCovery(int jobId)
        {
            var data = new ResponseOutput<bool>();

            var model = await _tasksQzServices.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.ResumeJob(model);
                if (ResuleModel.success)
                {
                    model.IsStart = true;
                    await _tasksQzServices.Update(model);
                    data.Ok(true);
                }
            }
            return data;

        }
    }
}
