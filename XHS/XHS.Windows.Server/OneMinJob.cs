using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XHS.Windows.Server
{
    public class OneMinJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            LogHelper.Info("每一分钟执行一次");

            var requestUrl = ConfigurationManager.AppSettings["onemin"];
            if (!string.IsNullOrEmpty(requestUrl))
            {
                HttpNetRequest.HttpGet(requestUrl);
            }

            return Task.FromResult(true);
        }
    }
}
