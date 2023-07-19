using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.TaskJobs;
using XHS.Build.Services.TaskQz;
using XHS.Build.TaskJobs;

namespace XHS.Build.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
   
    //[Permission]
    public class TaskJobController : ControllerBase
    {
        private readonly ITasksQzServices _tasksQzServices;
        private readonly ISchedulerCenter _schedulerCenter;

        public TaskJobController(ITasksQzServices tasksQzServices, ISchedulerCenter schedulerCenter)
        {
            _tasksQzServices = tasksQzServices;
            _schedulerCenter = schedulerCenter;
        }

        /// <summary>
        /// 分页获取
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        // GET: api/Buttons/5
        [HttpGet]
        public async Task<IResponseOutput> Get(int page = 1, int size = 10, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }
            Expression<Func<TasksQz, bool>> whereExpression = a => a.IsDeleted != true && (a.Name != null && a.Name.Contains(key));

            var data = await _tasksQzServices.QueryPage(whereExpression, page, size, " Id desc ");

            return ResponseOutput.Ok(data);

        }

        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput<string>> Post([FromBody] TasksQz tasksQz)
        {
            var data = new ResponseOutput<string>();

            var id = (await _tasksQzServices.Add(tasksQz));
            data.Ok(id.ToString());
            return data;
        }


        /// <summary>
        /// 修改计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput<string>> Put([FromBody] TasksQz tasksQz)
        {
            var data = new ResponseOutput<string>();
            if (tasksQz != null && tasksQz.Id > 0)
            {
                var success = await _tasksQzServices.Update(tasksQz);
                if (success)
                {
                    data.Ok(tasksQz.Id.ToString());
                }

            }

            return data;
        }

        /// <summary>
        /// 删除计划
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int jobId)
        {
            var data = new ResponseOutput<bool>();
            var model = await _tasksQzServices.QueryById(jobId);
            if (model != null)
            {
                //先停止计划
                var ResuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                if (ResuleModel.success)
                {
                    model.IsStart = false;
                    model.IsDeleted = true;
                    var success = await _tasksQzServices.Update(model);
                    if (success)
                    {
                        data.Ok(success);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 启动计划任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput<string>> StartJob(int jobId)
        {
            var data = new ResponseOutput<string>();

            var model = await _tasksQzServices.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.AddScheduleJobAsync(model);
                if (ResuleModel.success)
                {
                    model.IsStart = true;
                    var success = await _tasksQzServices.Update(model);
                    if (success)
                    {
                        data.Ok(jobId.ToString());
                    }
                }
                else
                {
                    data.NotOk(ResuleModel.msg);
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
        public async Task<IResponseOutput<string>> StopJob(int jobId)
        {
            var data = new ResponseOutput<string>();

            var model = await _tasksQzServices.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.StopScheduleJobAsync(model);
                if (ResuleModel.success)
                {
                    model.IsStart = false;
                    var success = await _tasksQzServices.Update(model);
                    if (success)
                    {
                        data.Ok(jobId.ToString());
                    }
                }
                else
                {
                    data.NotOk(ResuleModel.msg);
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
        public async Task<IResponseOutput<string>> ReCovery(int jobId)
        {
            var data = new ResponseOutput<string>();

            var model = await _tasksQzServices.QueryById(jobId);
            if (model != null)
            {
                var ResuleModel = await _schedulerCenter.ResumeJob(model);
                if (ResuleModel.success)
                {
                    model.IsStart = true;
                    var success = await _tasksQzServices.Update(model);
                    if (success)
                    {
                        data.Ok(jobId.ToString());
                    }
                }
                else
                {
                    data.NotOk(ResuleModel.msg);
                }
            }
            return data;

        }
    }
}
