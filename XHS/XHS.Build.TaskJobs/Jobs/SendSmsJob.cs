using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Services.SmsQueue;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SendSmsJob : JobBase, IJob
    {
        private readonly ILogger<SendSmsJob> _logger;
        private readonly ISmsQueueService _smsQueueService;
        public SendSmsJob(ILogger<SendSmsJob> logger, ISmsQueueService smsQueueService)
        {
            _logger = logger;
            _smsQueueService = smsQueueService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _smsQueueService.SendSmsAll();
        }
    }
}
