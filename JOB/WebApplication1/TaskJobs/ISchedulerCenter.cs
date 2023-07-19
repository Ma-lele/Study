using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskJobs
{
    public interface ISchedulerCenter
    {
        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        Task<string> StartScheduleAsync();
        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        Task<string> StopScheduleAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<string> AddScheduleJobAsync(TasksQz sysSchedule);
        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<string> StopScheduleJobAsync(TasksQz sysSchedule);
        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<string> ResumeJob(TasksQz sysSchedule);
    }
}
