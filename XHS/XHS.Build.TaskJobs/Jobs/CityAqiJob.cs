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
    public class CityAqiJob : JobBase, IJob
    {
        private readonly ILogger<CityAqiJob> _logger; 
        private readonly ISystemSettingService _systemSettingService;
        private readonly ICityAqiService _cityAqiService;
        public CityAqiJob(ILogger<CityAqiJob> logger,  ISystemSettingService systemSettingService, ICityAqiService cityAqiService)
        {
            _logger = logger;  
            _systemSettingService = systemSettingService;
            _cityAqiService = cityAqiService;
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
            string result = ""; string positionnames = "";
            string pm25s = "";
            string pm10s = "";
            string qualitys = "";
            string aqis = "";
            string areas = "";
            string pubtime = "";
            DateTime billdate = DateTime.Parse("1900-01-01 00:00:00");
            JObject jobj = new JObject();
            JObject jobjcityall = new JObject();
            JObject obj = new JObject();
            JArray jarysiteall = new JArray();

            _logger.LogInformation("城市AQI取得开始（" + citys + "）", false);

            //循环多个城市
            for (int k = 0; k < citylist.Length; k++)
            {
                result = HpCityAqi.GetCityAqi(domain, citylist[k], key);
                jobj = new JObject();
                jobjcityall = new JObject();
                jobj = JObject.Parse(result);
                jobjcityall = JObject.Parse(jobj["HeWeather6"][0].ToString());

                //城市均值
                obj = new JObject();

                pubtime = jobjcityall["air_now_city"]["pub_time"].ToString();
                DateTime point = DateTime.Parse(pubtime);

                //只使用上二小时或者上一小时或者当前小时的数据
                if (point.Date == DateTime.Now.AddHours(-2).Date && point.Hour == DateTime.Now.AddHours(-2).Hour
                    || point.Date == DateTime.Now.AddHours(-1).Date && point.Hour == DateTime.Now.AddHours(-1).Hour
                    || point.Date == DateTime.Now.Date && point.Hour == DateTime.Now.Hour)
                {
                    areas += jobjcityall["basic"]["location"] + ",";
                    positionnames += ",";
                    pm25s += jobjcityall["air_now_city"]["pm25"] + ",";
                    pm10s += jobjcityall["air_now_city"]["pm10"] + ",";
                    qualitys += jobjcityall["air_now_city"]["qlty"] + ",";
                    aqis += jobjcityall["air_now_city"]["aqi"] + ",";
                    billdate = point;
                }

                //循环城市各个监测点
                jarysiteall = new JArray();
                jarysiteall = JArray.Parse(jobjcityall["air_now_station"].ToString());

                for (int i = 0; i < jarysiteall.Count; i++)
                {

                    pubtime = jarysiteall[i]["pub_time"].ToString();
                    point = DateTime.Parse(pubtime);

                    //只使用上二小时或者上一小时当前小时的数据
                    if (point.Date == DateTime.Now.AddHours(-2).Date && point.Hour == DateTime.Now.AddHours(-2).Hour
                        || point.Date == DateTime.Now.AddHours(-1).Date && point.Hour == DateTime.Now.AddHours(-1).Hour
                        || point.Date == DateTime.Now.Date && point.Hour == DateTime.Now.Hour)
                    {
                        areas += jobjcityall["basic"]["location"] + ",";
                        positionnames += jarysiteall[i]["air_sta"] + ",";
                        pm25s += jarysiteall[i]["pm25"] + ",";
                        pm10s += jarysiteall[i]["pm10"] + ",";
                        qualitys += jarysiteall[i]["qlty"] + ",";
                        aqis += jarysiteall[i]["aqi"] + ",";
                        billdate = point;
                    }
                }
            }

            //如果存在有效数据，插入数据库
            if (areas.Length > 0)
            {
                areas = areas.Substring(0, areas.Length - 1);
                positionnames = positionnames.Substring(0, positionnames.Length - 1);
                pm25s = pm25s.Substring(0, pm25s.Length - 1);
                pm10s = pm10s.Substring(0, pm10s.Length - 1);
                qualitys = qualitys.Substring(0, qualitys.Length - 1);
                aqis = aqis.Substring(0, aqis.Length - 1);

                int ret = _cityAqiService.doInsert(new DBParams("@areas", areas), new DBParams("@positionnames", positionnames), new DBParams("@pm25s", pm25s), new DBParams("@pm10s", pm10s), new DBParams("@qualitys", qualitys), new DBParams("@aqis", aqis), new DBParams("@billdate", billdate));
                _logger.LogInformation("城市AQI插入数据库（" + ret + "）");
            }
        }
    }
}
