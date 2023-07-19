using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Services.FallProtection;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class FallProtectionJob : JobBase, IJob
    {
        private readonly ILogger<FallProtectionJob> _logger;
        private readonly IFallProtectionService _fallProtectionService; 

        public FallProtectionJob(ILogger<FallProtectionJob> logger, IFallProtectionService fallProtectionService)
        {
            _logger = logger;
            _fallProtectionService = fallProtectionService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            var result = await _fallProtectionService.FindnSetOffline();
            if (result > 0)
            {
                _logger.LogInformation($"{DateTime.Now}临边防护离线检测到{result}台离线设备");
            }
        }
    }
}
