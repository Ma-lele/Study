using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Services.TaskQz;

namespace XHS.Build.TaskJobs
{
    /// <summary>
    /// Quartz 启动服务
    /// </summary>
    public static class QuartzJobMildd
    {
        public static void UseQuartzJobMildd(this IApplicationBuilder app, ITasksQzServices tasksQzServices, ISchedulerCenter schedulerCenter)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                var allQzServices = tasksQzServices.Query(a=>a.IsStart==true).Result;
                foreach (var item in allQzServices)
                {
                    if (item.IsStart)
                    {
                        var ResuleModel = schedulerCenter.AddScheduleJobAsync(item).Result;
                        if (ResuleModel.success)
                        {
                            Console.WriteLine($"QuartzNetJob{item.Name}启动成功！");
                        }
                        else
                        {
                            Console.WriteLine($"QuartzNetJob{item.Name}启动失败！错误信息：{ResuleModel.msg}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
