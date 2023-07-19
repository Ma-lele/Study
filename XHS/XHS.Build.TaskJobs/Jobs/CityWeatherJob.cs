using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Services.CityAqi;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class CityWeatherJob : JobBase, IJob
    {
        private readonly ILogger<CityWeatherJob> _logger;
        private readonly ISystemSettingService _systemSettingService;
        private readonly ICityAqiService _cityAqiService;
        public CityWeatherJob(ILogger<CityWeatherJob> logger, ISystemSettingService systemSettingService, ICityAqiService cityAqiService)
        {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            string citys = "";
            string key = "";
            string domain = "";

            //获取设定值
            DataTable dt = _systemSettingService.getValue("S035,S036,S077");
            if (dt.Rows.Count <= 0)
            {
                return;
            }
            else
            {
                citys = dt.Rows[0]["S036"].ToString();
                key = dt.Rows[0]["S035"].ToString();
                domain = dt.Rows[0]["S077"].ToString();

                if (String.IsNullOrEmpty(citys) || String.IsNullOrEmpty(key))
                {
                    return;
                }
            }

            string[] citylist = citys.Split(',');
            string result = "";
            JObject jobj = new JObject();
            JObject jresult = new JObject();

            //只在每个小时的0分~15分之间获取实况天气，保证每个城市每小时只调用一次接口
            if (DateTime.Now.Minute > 15)
            {
                return;
            }

            _logger.LogInformation("城市是实况天气取得开始（" + citys + "）", false);



            //循环多个城市
            for (int k = 0; k < citylist.Length; k++)
            {
                result = HpCityAqi.GetCityWeather(domain, citylist[k], key);
                jobj = new JObject();
                jobj = JObject.Parse(result);
                jresult = JObject.Parse(jobj["HeWeather6"][0].ToString());

                if (jresult["status"].ToString().ToUpper() != "OK")
                {
                    return;
                }

                string weather = jresult["now"].ToString();
                string updatedate = jresult["update"]["loc"].ToString();

                _cityAqiService.doWeatherUpdate(new DBParams("@area", citylist[k]), new DBParams("@weather", weather), new DBParams("@updatedate", updatedate));

            }
        }
    }
}
