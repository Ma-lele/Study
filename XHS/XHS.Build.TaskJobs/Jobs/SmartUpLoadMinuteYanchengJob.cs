using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadMinuteYanchengJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadMinuteYanchengJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadMinuteYanchengJob(ILogger<SmartUpLoadMinuteYanchengJob> logger, IOperateLogService operateLogService,IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
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
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string Token = "IBGcPGQieY3m8518D1XIeVn65Zpm11GffuPR7oy/YDlIw5Xtx0SeMw==";

            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForYanchengMinute();
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
                                string funingurl = "";
                                if (jso.ContainsKey("funingurl"))
                                {
                                    funingurl = jso.GetValue("funingurl").ToString();
                                    jso.Remove("funingurl");
                                }
                                string url = jso.GetValue("siteuploadurl").ToString();
                                string api = jso.GetValue("post").ToString();
                                string realapi = api;


                                if (api.Contains("Craneinterface/UploadCraneHistory"))          //塔机实时数据上传
                                {
                                    realapi = "/yancheng/towerCraneMonitor/uploadCraneHistory";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        var data1 = JObject.Parse(jso.GetValue("sedata").ToString());
                                        if (data1.ContainsKey("SafeLoad"))
                                        {
                                            double SafeLoad = data1.GetValue("SafeLoad").ToDouble();
                                            jso.Add("load", SafeLoad);
                                        }
                                        else
                                        {
                                            jso.Add("load", 0);
                                        }
                                        if (data1.ContainsKey("Margin"))
                                        {
                                            double Margin = data1.GetValue("Margin").ToDouble();
                                            jso.Add("range", Margin);
                                        }
                                        else
                                        {
                                            jso.Add("range", 0);
                                        }
                                        if (data1.ContainsKey("MomentPercent"))
                                        {
                                            double MomentPercent = data1.GetValue("MomentPercent").ToDouble();
                                            jso.Add("moment", MomentPercent);
                                        }
                                        else
                                        {
                                            jso.Add("moment", 0);

                                        }
                                        if (data1.ContainsKey("Rotation"))
                                        {
                                            double Rotation = data1.GetValue("Rotation").ToDouble();
                                            jso.Add("rotation", Rotation);
                                        }
                                        else
                                        {
                                            jso.Add("rotation", 0);
                                        }
                                        if (data1.ContainsKey("Height"))
                                        {
                                            double Height = data1.GetValue("Height").ToDouble();
                                            jso.Add("height", Height);
                                        }
                                        else
                                        {
                                            jso.Add("height", 0);
                                        }
                                    }
                                }
                                else if (api.Contains("UploadHistory"))             //卸料平台实时数据上传
                                {
                                    realapi = "/yancheng/uploadinterface/uploadHistory";
                                    if (jso.ContainsKey("rtdjson"))
                                    {
                                        var data1 = JObject.Parse(jso.GetValue("rtdjson").ToString());
                                        if (data1.ContainsKey("weight"))
                                        {
                                            double weight = data1.GetValue("weight").ToDouble();
                                            jso.Add("weight", weight);
                                        }
                                        if (data1.ContainsKey("bias"))
                                        {
                                            double bias = data1.GetValue("bias").ToDouble();
                                            jso.Add("weightBias", bias);
                                        }
                                        if (data1.ContainsKey("early_warning_weight"))
                                        {
                                            double early_warning_weight = data1.GetValue("early_warning_weight").ToDouble();
                                            jso.Add("warningWeight", early_warning_weight);
                                        }
                                        if (data1.ContainsKey("alarm_weight"))
                                        {
                                            double alarm_weight = data1.GetValue("alarm_weight").ToDouble();
                                            jso.Add("alarmWeight", alarm_weight);
                                        }
                                        if (data1.ContainsKey("electric_quantity"))
                                        {
                                            double electric_quantity = data1.GetValue("electric_quantity").ToDouble();
                                            jso.Add("powerPercent", electric_quantity);
                                        }
                                        if (data1.ContainsKey("upstate"))
                                        {
                                            double upstate = data1.GetValue("upstate").ToDouble();
                                            jso.Add("dataType", upstate);
                                        }
                                        if (data1.ContainsKey("Id"))
                                        {
                                            jso.Add("eventId", data1.GetValue("Id").ToString());
                                        }
                                    }
                                }
                                else if (api.Contains("Hoistinterface/HoistHistory"))           //施工升降机实时数据上传
                                {
                                    realapi = "/yancheng/hoistinterface/hoistHistory";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        var data1 = JObject.Parse(jso.GetValue("sedata").ToString());
                                        if (data1.ContainsKey("SafeLoad"))
                                        {
                                            double SafeLoad = data1.GetValue("SafeLoad").ToDouble();
                                            jso.Add("load", SafeLoad);
                                        }
                                        if (data1.ContainsKey("Margin"))
                                        {
                                            double Margin = data1.GetValue("Margin").ToDouble();
                                            jso.Add("range", Margin);
                                        }
                                        if (data1.ContainsKey("MomentPercent"))
                                        {
                                            double MomentPercent = data1.GetValue("MomentPercent").ToDouble();
                                            jso.Add("moment", MomentPercent);
                                        }
                                        if (data1.ContainsKey("Rotation"))
                                        {
                                            double Rotation = data1.GetValue("Rotation").ToDouble();
                                            jso.Add("rotation", Rotation);
                                        }
                                        if (data1.ContainsKey("Height"))
                                        {
                                            double Height = data1.GetValue("Height").ToDouble();
                                            jso.Add("height", Height);
                                        }
                                    }
                                }
                                else if (api.Contains("DeppPitHistory"))//卸料平台
                                {
                                    realapi = "/yancheng/deppPit/deppPitHistory";

                                    JArray jar = JArray.Parse(jso.GetValue("data").ToString());
                                    JObject job = JObject.Parse(jar[0].ToString());
                                    jso.Add("WatchPoint", job.GetValue("watchPoint"));
                                    jso.Add("WatchPointValue", job.GetValue("watchPointValue"));

                                }
                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                var data = jso;
                                jso = new JObject();
                                jso.Add("Token", Token);
                                jso.Add("JSON", data);
                                string result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = url,
                                    api = api,
                                    account = Token,
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
                                _logger.LogError(ex.Message);
                            }


                        }
                    }
                }
            }

            _logger.LogInformation("分钟数据上传结束。", true);
        }
    }
}
