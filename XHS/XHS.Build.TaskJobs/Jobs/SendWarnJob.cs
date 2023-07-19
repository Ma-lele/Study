using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SendWarnJob : JobBase, IJob
    {
        private readonly ILogger<SendWarnJob> _logger;
        private readonly HpAliSMS _hpAliSMS;
        public SendWarnJob(ILogger<SendWarnJob> logger, IHpSystemSetting hpSystemSetting)
        {
            _logger = logger;
            _hpAliSMS = new HpAliSMS(hpSystemSetting);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _hpAliSMS.SendWarnAll("");
        }
    }
}
