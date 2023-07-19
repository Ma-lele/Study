using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Services.Warning;
using XHS.Build.TaskJobs.Jobs;
using static XHS.Build.Common.Helps.HpFog;

namespace XHS.Build.TaskJobs
{
    public class FogAutoStartJob : JobBase, IJob
    {
        private readonly ILogger<FogAutoStartJob> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWarningService _warningService;
        public FogAutoStartJob(ILogger<FogAutoStartJob> logger, IConfiguration configuration, IWarningService warningService)
        {
            _logger = logger;
            _configuration = configuration;
            _warningService = warningService;
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
                DataTable dt = await _warningService.getSendCmdList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        BnCmd bnCmd = new BnCmd() { USERID = "0", fc = Convert.ToString(dt.Rows[i]["fogcode"]), sw = Convert.ToString(dt.Rows[i]["switchno"]), cmd = HpFog.CMD.ON, delay = Convert.ToString(dt.Rows[i]["delay"]), };
                        HpFog.SendCommand(bnCmd, _configuration.GetSection("BatchConsole").GetValue<string>("EquipServerIp"), _configuration.GetSection("BatchConsole").GetValue<int>("EquipServerPort"), _configuration.GetSection("BatchConsole").GetValue<string>("EquipPublicKey"));
                        await _warningService.doUpdateCmd(Convert.ToInt32(dt.Rows[i]["WARNID"]));
                        _logger.LogInformation(string.Format("根据报警ID {0} ,发送开启雾泡指令.{1}:{2}", dt.Rows[i]["WARNID"], dt.Rows[i]["fogcode"], dt.Rows[i]["switchno"]));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
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
