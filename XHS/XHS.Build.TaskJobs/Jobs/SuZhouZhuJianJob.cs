using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Util;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SuZhouZhuJianJob : JobBase, IJob
    {
        private readonly ILogger<SuZhouZhuJianJob> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBaseRepository<BaseEntity> _baseRepository;
        public SuZhouZhuJianJob(ILogger<SuZhouZhuJianJob> logger, IConfiguration configuration, IBaseRepository<BaseEntity> baseRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _baseRepository = baseRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            int SUCCESS = 200;                                       //成功状态
            string APPKEY = "d9390eac-0488-4517-b37c-a1d5e072c19a";             //appkey
            string APPSECRET = "17104541-f4b2-4172-b9b9-a73ce851627f";          //秘钥
            string SQL = "SELECT TOP 1000 devicecode,dr.SITEID,tsp,pm2_5,pm10,atmos," +
                  "direction,noise,dampness,temperature,speed,dbo.fnGetWindSpeedStr(speed) windlevel,updatetime " +
                  "FROM T_GC_DeviceRtd dr  INNER JOIN T_GC_Site s ON dr.SITEID = s.SITEID AND s.bpush = 1 " +
                  " WHERE updatetime > DATEADD(MINUTE,-{0},GETDATE()) AND s.GROUPID IN ({1})";
            int EXPTIME = 300000;
            string URL = "http://221.224.132.158:8081/smart-site/rest/hj/uploadhj";
            string ERROR_MESSAGE = "设备 {0} 的数据推送失败: {1}";

            try
            {
                long _Timestamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
                string sql = string.Format(SQL, _configuration.GetSection("SuZhouZhuJian").GetValue<int>("Interval"), _configuration.GetSection("SuZhouZhuJian").GetValue<string>("GroupIds"));
                DataTable dt = _baseRepository.Db.Ado.GetDataTable(sql);
                int count = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    try
                    {
                        long timestamp = _Timestamp;
                        string signature = string.Format("{0}&{1}&{2}&{3}", APPKEY, APPSECRET, timestamp, EXPTIME);
                        string token = UEncrypter.HmacSHA1(signature, APPSECRET);

                        WebHeaderCollection wheader = new WebHeaderCollection();
                        wheader["app_Key"] = APPKEY;
                        wheader["access_token"] = token;
                        wheader["time_stamp"] = timestamp.ToString();
                        string data = "\"MN\":\"{0}\",\"DateTime\":\"{1}\",\"TSP\":\"{2}\",\"PM25\":\"{3}\",\"PM10\":\"{4}\",\"temperature\":\"{5}\"," +
                            "\"humidity\":\"{6}\",\"atmos\":\"{7}\",\"windspeed\":\"{8}\",\"winddirection\":\"{9}\",\"leq\":\"{10}\",\"windLevel\":\"{11}\"," +
                            "\"atmospheric\":\"\",\"date\":\"\",\"time\":\"\"";
                        data = "params={" + string.Format(data, dr["devicecode"], Convert.ToDateTime(dr["updatetime"]).ToString("yyyMMddHHmmss"),
                            dr["tsp"], dr["pm2_5"], dr["pm10"], dr["temperature"], dr["dampness"], dr["atmos"], dr["speed"],
                            dr["direction"], dr["noise"], dr["windlevel"]) + "}";
                        _logger.LogInformation(data);
                        string response = UHttp.Post(URL, data, UHttp.CONTENT_TYPE_FORM, wheader);
                        JObject jo = JObject.Parse(response);
                        if (Convert.ToInt32(jo["code"]) == SUCCESS)
                            count++;
                        else
                            _logger.LogInformation(string.Format(ERROR_MESSAGE, dr["devicecode"], jo["msg"]), true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(string.Format(ERROR_MESSAGE, dr["devicecode"], ex.Message), true);
                    }
                }

                _logger.LogInformation(string.Format("数据推送结束. {0} / {1}", count, dt.Rows.Count));

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, true);
            }
            finally
            {
            }
        }
    }
}
