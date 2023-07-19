using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XHS.Windows.Server
{
    public class OneHourJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            LogHelper.Info("每小时执行一次");

            var requestUrl = ConfigurationManager.AppSettings["onehour"];
            if (!string.IsNullOrEmpty(requestUrl))
            {
                HttpNetRequest.HttpGet(requestUrl);
            }

            return Task.FromResult(true);
        }
    }
}
