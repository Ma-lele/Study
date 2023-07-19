using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Wechat;
using XHS.Build.Services.DeviceCN;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SNDJob : JobBase, IJob
    {
        private readonly ILogger<SNDJob> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDeviceCNService _deviceCNService;
        public SNDJob(ILogger<SNDJob> logger, IConfiguration configuration, IDeviceCNService deviceCNService)
        {
            _logger = logger;
            _configuration = configuration;
            _deviceCNService = deviceCNService;
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
                //WriteLog("智能公示牌节目单下发");
                DataTable dt = _deviceCNService.getRtdList();
                if (dt.Rows.Count <= 0)
                    return;
                JArray ja = new JArray();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    long time = (Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000;
                    JObject pm25 = new JObject()
                {
                   { "indexId", "4101001" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["pm2_5"]) },
                };
                    JObject pm10 = new JObject()
                {
                   { "indexId", "4101002" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["pm10"]) },
                };
                    JObject atmos = new JObject()
                {
                   { "indexId", "4101011" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["atmos"]) },
                };
                    JObject speed = new JObject()
                {
                   { "indexId", "4101012" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["speed"]) },
                };
                    JObject direction = new JObject()
                {
                   { "indexId", "4101012" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["direction"]) },
                };
                    JObject temperature = new JObject()
                {
                   { "indexId", "4101014" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["temperature"]) },
                };
                    JObject dampness = new JObject()
                {
                   { "indexId", "4101015" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["dampness"]) },
                };
                    JObject tsp = new JObject()
                {
                   { "indexId", "4101091" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["tsp"]) },
                };
                    JArray data = new JArray();
                    data.Add(pm25);
                    data.Add(pm10);
                    data.Add(atmos);
                    data.Add(speed);
                    data.Add(direction);
                    data.Add(temperature);
                    data.Add(dampness);
                    data.Add(tsp);
                    JObject device = new JObject();
                    device.Add("mn", Convert.ToString(dt.Rows[i]["devicecode"]));
                    device.Add("data", data);
                    ja.Add(device);
                }
                WClient wc = new WClient(_configuration.GetSection("SND").GetValue<string>("URL"), WClient.CONTENT_TYPE_FORM);
                string param = string.Format("key={0}&data=", _configuration.GetSection("SND").GetValue<string>("AppKey")) + ja.ToString();
                string response = wc.PostData(param);
                _logger.LogInformation(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, true);
            }
        }
    }
}
