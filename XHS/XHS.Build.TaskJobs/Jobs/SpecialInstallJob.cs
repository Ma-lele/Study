using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Services.SmsQueue;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SpecialInstallJob : JobBase, IJob
    {
        private readonly ILogger<SpecialInstallJob> _logger;
        private readonly IConfiguration _configuration;
        public SpecialInstallJob(ILogger<SpecialInstallJob> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            try
            {
                string URL_WARN = "http://{0}:9027/Services/WSTemplateMessage.svc";
                string domain = _configuration.GetSection("BatchConsole").GetValue<string>("WcfDomain");
                object result = HpWcfInvoker.ExecuteMethod<IWSTemplateMessage>(string.Format(URL_WARN, domain), "sendSeAlert");

                if (result != null && !Convert.ToInt32(result).Equals(0))
                {
                    //如果发生了发信才写日志
                    //_logParam.Set("operation", string.Format(SRT_ALERT, domain, result));  没有操作  先注释了
                    _logger.LogInformation(string.Format("调用了{0}的特种设备未安装提醒服务！[{1}]", domain, result), false);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message);
            }
        }
    }
}
