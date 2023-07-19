using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Services.Fog;
using XHS.Build.TaskJobs.Jobs;
using static XHS.Build.Common.Helps.HpFog;

namespace XHS.Build.TaskJobs
{
    public class FogKickerJob : JobBase, IJob
    {
        private readonly ILogger<FogKickerJob> _logger;
        private readonly IFogJobService _fogJobService;
        private readonly IConfiguration _configuration;
        public FogKickerJob(ILogger<FogKickerJob> logger, IFogJobService fogJobService,IConfiguration configuration)
        {
            _logger = logger;
            _fogJobService = fogJobService;
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
                DataTable dt = await _fogJobService.GetFogKickerDataTable();
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            BnCmd bnCmd = new BnCmd() { USERID = "0", fc = Convert.ToString(dt.Rows[i]["fogcode"]), sw = Convert.ToString(dt.Rows[i]["switchno"]), cmd = HpFog.CMD.ON, delay = Convert.ToString(dt.Rows[i]["delay"]), };
                            HpFog.SendCommand(bnCmd, _configuration.GetSection("BatchConsole").GetValue<string>("EquipServerIp"), _configuration.GetSection("BatchConsole").GetValue<int>("EquipServerPort"), _configuration.GetSection("BatchConsole").GetValue<string>("EquipPublicKey"));

                            _logger.LogInformation(string.Format("{0} 的PM10为 {1} 已超过雾炮连动阀值 {2},已发送开启指令给 {3}:{4}",
                                dt.Rows[i]["devicecode"], dt.Rows[i]["pm10"], dt.Rows[i]["fogkickline"], dt.Rows[i]["fogcode"], dt.Rows[i]["switchno"]));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
