using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadMinuteXuzhouJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadMinuteXuzhouJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityXuzhouToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadMinuteXuzhouJob(ILogger<SmartUpLoadMinuteXuzhouJob> logger, IOperateLogService operateLogService, SiteCityXuzhouToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("分钟数据上传开始。", true);

            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            JObject carwashjob = new JObject();
            JObject job = new JObject();
            JArray jar = new JArray();
            int successcount = 0;
            int errcount = 0;
            string api = "";
            string result = "";
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListForXuzhouMinute();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            try
                            {
                                DataRow dr = dt.Rows[j];
                                JObject jso = new JObject();
                                foreach (DataColumn column in dr.Table.Columns)
                                {
                                    if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.Decimal")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDouble());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.DateTime")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDateTime());
                                    }
                                    else
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                                    }
                                }

                                string account = jso.GetValue("uploadaccount").ToString();
                                string pwd = jso.GetValue("uploadpwd").ToString();
                                string url = jso.GetValue("siteuploadurl").ToString();
                                api = jso.GetValue("post").ToString();
                                string realapi = api;


                                if (api.Equals("DustInterface/UploadDustHistory"))      //15.	上传扬尘实时数据信息
                                {
                                    realapi = "/dustRecordData";
                                }
                                else if (api.Equals("Craneinterface/UploadCraneHistory")) //21.上传塔机实时数据信息
                                {
                                    realapi = "/towerDetection";

                                    string jobstr = System.Web.HttpUtility.HtmlDecode(jso.GetValue("sedata").ToString());
                                    if (!string.IsNullOrEmpty(jobstr))
                                    {
                                        JObject datajob = JObject.Parse(jobstr);
                                        jso.Add("load", datajob.GetValue("SafeLoad").ToString());
                                        jso.Add("range", datajob.GetValue("Margin").ToString());
                                        jso.Add("moment", datajob.GetValue("MomentPercent").ToString());
                                        jso.Add("rotation", datajob.GetValue("Rotation").ToString());
                                        jso.Add("height", datajob.GetValue("Height").ToString());
                                        jso.Add("windSpeed", datajob.GetValue("WindSpeed").ToString());
                                        jso.Add("driverName", string.IsNullOrEmpty(datajob.GetValue("DriverName").ToString()) ? "未知": datajob.GetValue("DriverName").ToString());
                                        jso.Add("cid", datajob.GetValue("DriverId").ToString());
                                        jso.Add("multiplyingPower", "0.0");
                                        jso.Add("dip", "0.0");
                                        jso.Remove("sedata");
                                    }
                                }
                                else if (api.Equals("Hoistinterface/HoistHistory"))         //26. 上传升降机实时数据信息
                                {
                                    realapi = "/liftDetection";
                                    string jobstr = jso.GetValue("sedata").ToString();
                                    if (!string.IsNullOrEmpty(jobstr))
                                    {
                                        JObject datajob = JObject.Parse(jobstr);
                                        jso.Add("load", "0.0");
                                        jso.Add("loadPercent", "0.0");
                                        jso.Add("endHoistHeight", "0.0");
                                        jso.Add("heightPercent", "0.0");
                                        jso.Add("height", datajob.GetValue("Height".ToString()));
                                        jso.Add("speed", datajob.GetValue("Speed").ToString());
                                        jso.Add("driverName", string.IsNullOrEmpty(datajob.GetValue("DriverName").ToString()) ? "未知" : datajob.GetValue("DriverName").ToString());
                                        jso.Add("cid", datajob.GetValue("DriverCardNo").ToString());
                                        jso.Remove("sedata");
                                    }
                                }
                                else if (api.Equals("Uploadinterface/UploadHistory"))       //29. 上传卸料平台实时数据信息
                                {
                                    realapi = "/unloadingDetection";
                                    string jobstr = jso.GetValue("rtdjson").ToString();
                                    if (!string.IsNullOrEmpty(jobstr))
                                    {
                                        JObject datajob = JObject.Parse(jobstr);
                                        jso.Add("weight", datajob.GetValue("weight").ToString());
                                        jso.Add("moniterTime", datajob.GetValue("updatetime").ToString());
                                    }
                                    jso.Remove("rtdjson");
                                }

                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                if (jso.ContainsKey("funingurl"))
                                {
                                    jso.Remove("funingurl");
                                }

                                result = _cityToken.JsonRequest(url, account, pwd, realapi, JsonConvert.SerializeObject(jso));

                                LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = url,
                                    api = realapi,
                                    account = account,
                                    param = jso.ToString(),
                                    result = result,
                                    createtime = DateTime.Now
                                };
                                await _operateLogService.AddCityUploadApiLog(LogEntity);
                                if (string.IsNullOrEmpty(result))
                                {
                                    errcount++;
                                    break;
                                }
                                else
                                {
                                    JObject mJObj = JObject.Parse(result);
                                    int code = (int)mJObj["code"];
                                    if (code == 0)
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(api + ":" + ex.Message);
                            }

                        }
                    }
                }
            }

            _logger.LogInformation("分钟数据上传结束。", true);
        }
    }
}
