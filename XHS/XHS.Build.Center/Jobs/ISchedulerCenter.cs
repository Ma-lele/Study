using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.TaskJobs;

namespace XHS.Build.Center.Jobs
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
        Task<IResponseOutput> StartScheduleAsync();
        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        Task<IResponseOutput> StopScheduleAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<IResponseOutput> AddScheduleJobAsync(TasksQz sysSchedule);
        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<IResponseOutput> StopScheduleJobAsync(TasksQz sysSchedule);
        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<IResponseOutput> ResumeJob(TasksQz sysSchedule);

        /// <summary>
        /// 某个任务状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<TriggerState> GetJobState(TasksQz model);
    }

}
