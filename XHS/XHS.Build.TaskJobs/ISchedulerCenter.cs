using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.TaskJobs;

namespace XHS.Build.TaskJobs
{
    /// <summary>
    /// 服务调度接口
    /// </summary>
    public interface ISchedulerCenter
    {

        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        Task<IResponseOutput<string>> StartScheduleAsync();
        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        Task<IResponseOutput<string>> StopScheduleAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<IResponseOutput<string>> AddScheduleJobAsync(TasksQz sysSchedule);
        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<IResponseOutput<string>> StopScheduleJobAsync(TasksQz sysSchedule);
        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<IResponseOutput<string>> ResumeJob(TasksQz sysSchedule);
    }
}
