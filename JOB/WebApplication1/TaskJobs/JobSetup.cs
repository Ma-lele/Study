using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using System;
using TaskJobs.Jobs;

namespace TaskJobs
{
    public static class JobSetup
    {
        public static void AddJobSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddTransient<test1>();

            services.AddSingleton<ISchedulerCenter, SchedulerCenterServer>();
            //services.AddTransient<SmartUpLoadAoTu>();
            //services.AddTransient<SmartUpLoadHourYanchengJob>();
            //services.AddTransient<SmartUpLoadGuangliandaJob>();
            //services.AddTransient<SmartUpLoadHourXuweiJob>();
            //services.AddTransient<SmartUpLoadHourRugaoJob>();
            //services.AddTransient<SmartUpLoadHourGuannanJob>();
            //services.AddTransient<SmartUpLoadHourfuningJob>();
            //services.AddTransient<SmartUpLoadHourWuxiJob>();
            //services.AddTransient<SmartUpLoadHourXinwuquJob>();
            //services.AddTransient<SmartUpLoadHourXuzhouJob>();
            //services.AddTransient<SmartUpLoadMinuteGuannanJob>();
            //services.AddTransient<SmartUpLoadMinutefuningJob>();
            //services.AddTransient<SmartUpLoadMinuteXuweiJob>();
            //services.AddTransient<SmartUpLoadMinuteRugaoJob>();
            //services.AddTransient<SmartUpLoadHourjianyeJob>();
            //services.AddTransient<SmartUpLoadMinutejianyeJob>();
            //services.AddTransient<SmartUpLoadMinuteXinwuquJob>();
            //services.AddTransient<SmartUpLoadMinuteYanchengJob>();
            //services.AddTransient<SmartUpLoadMinuteXuzhouJob>();
            //services.AddTransient<XHSMotionDataUpLoadDayJob>();
            //services.AddTransient<SmartUpLoadHuarun>();
            //services.AddTransient<JobQuartzMongoLogDelete>();
            //services.AddTransient<FileCenterUploadJob>();
            //services.AddTransient<FileClearJob>();
            //services.AddTransient<SmartUpLoadWuxiRiskAnalyse>();
            //services.AddTransient<FallProtectionJob>();
            //services.AddTransient<SmartUpLoadYiZhengJob>();
        }
    }
}
