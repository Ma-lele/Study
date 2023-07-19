using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Services.DailyJob;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
   public  class CableSyncJob : JobBase, IJob
    {
        private readonly ILogger<CableSyncJob> _logger;
        private readonly IDailyJobService _dailyJobService;
        public CableSyncJob(ILogger<CableSyncJob> logger, IDailyJobService dailyJobService)
        {
            _logger = logger;
            _dailyJobService = dailyJobService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            //同步钢丝绳
            _dailyJobService.CableDoSync();
        }
    }
}
