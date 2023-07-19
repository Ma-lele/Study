using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using System;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
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
            services.AddTransient<BatchJob>();
            services.AddTransient<SmartUpLoadYcCarUndeveloped>();
            services.AddTransient<BolangJob>();
            services.AddTransient<CableSyncJob>();
            services.AddTransient<CityAqiJob>();
            services.AddTransient<CityWeatherJob>();
            services.AddTransient<CameraStateJob>();
            services.AddTransient<DbBackClearJob>();
            services.AddTransient<FogAutoStartJob>();
            services.AddTransient<FogKickerJob>();
            services.AddTransient<SendSmsJob>();
            services.AddTransient<SendWarnJob>();
            services.AddTransient<SiteEmployeeJob>();
            services.AddTransient<CityGetDataDayJob>();
            services.AddTransient<CityGetAqtDataDayJob>();            
            services.AddTransient<DayunMinuteJob>();
            services.AddTransient<SNDJob>();
            services.AddTransient<SpecialInstallJob>();
            services.AddTransient<SuZhouZhuJianJob>(); 
            services.AddTransient<CitySZUpLoadHourJob>();
            services.AddTransient<CitySZUpLoadMinuteJob>();
            services.AddTransient<WXDYJob>();
            services.AddTransient<WXDYJiangYinJob>();
            services.AddTransient<WXDYElevatorOnlineJob>();
            services.AddTransient<CityUpLoadHourJob>();
            services.AddTransient<CityUpLoadMinuteJob>();
            services.AddTransient<SmartUpLoadAoTu>();
            services.AddTransient<SmartUpLoadHourYanchengJob>(); 
            services.AddTransient<SmartUpLoadGuangliandaJob>();
            services.AddTransient<SmartUpLoadHourXuweiJob>();
            services.AddTransient<SmartUpLoadHourRugaoJob>();
            services.AddTransient<SmartUpLoadHourGuannanJob>();
            services.AddTransient<SmartUpLoadHourfuningJob>();
            services.AddTransient<SmartUpLoadHourWuxiJob>();
            services.AddTransient<SmartUpLoadHourXinwuquJob>();
            services.AddTransient<SmartUpLoadHourXuzhouJob>();
            services.AddTransient<SmartUpLoadMinuteGuannanJob>();
            services.AddTransient<SmartUpLoadMinutefuningJob>();
            services.AddTransient<SmartUpLoadMinuteXuweiJob>();
            services.AddTransient<SmartUpLoadMinuteRugaoJob>();
            services.AddTransient<SmartUpLoadHourjianyeJob>();
            services.AddTransient<SmartUpLoadMinutejianyeJob>();
            services.AddTransient<SmartUpLoadMinuteXinwuquJob>();
            services.AddTransient<SmartUpLoadMinuteYanchengJob>();
            services.AddTransient<SmartUpLoadMinuteXuzhouJob>();
            services.AddTransient<XHSMotionDataUpLoadDayJob>();
            services.AddTransient<SmartUpLoadHuarun>();
            services.AddTransient<JobQuartzMongoLogDelete>();
            services.AddTransient<FileCenterUploadJob>();
            services.AddTransient<FileClearJob>();
            services.AddTransient<SmartUpLoadWuxiRiskAnalyse>();
            services.AddSingleton<ISchedulerCenter, SchedulerCenterServer>();
            services.AddTransient<FallProtectionJob>();
            services.AddTransient<SmartUpLoadYiZhengJob>();
        }
    }
}
