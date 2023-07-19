using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using System;
using XHS.Build.Services;
using XHS.Build.Services.TaskQz;

namespace XHS.Build.Center.Jobs
{
    /// <summary>
    /// 任务调度 启动服务
    /// </summary>
    public static class JobSetup
    {
        public static void AddJobSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddTransient<JobQuartzSpecial>();//Job使用瞬时依赖注入
            services.AddTransient<JobQuartzSite>();//Job使用瞬时依赖注入
            services.AddTransient<JobQuartzInvade>();//Job使用瞬时依赖注入
            services.AddTransient<JobQuartzMongoLogDelete>();//Job使用瞬时依赖注入
            services.AddSingleton<ISchedulerCenter, SchedulerCenter>();
        }
    }
}
