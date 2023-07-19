using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class SmartUpLoadMinutefuningJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadMinutefuningJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityFuningToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadMinutefuningJob(ILogger<SmartUpLoadMinutefuningJob> logger, IOperateLogService operateLogService, SiteCityFuningToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
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
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string api = string.Empty;
            string url = string.Empty;
            string Token = "IBGcPGQieY3m8518D1XIeVn65Zpm11GffuPR7oy/YDlIw5Xtx0SeMw==";






            //对接数据获取
            DataSet dss = await _aqtUploadService.GetListForYancheng();
            if (dss.Tables.Count > 0)
            {
                for (int i = 0; i < dss.Tables.Count; i++)
                {
                    DataTable dt = dss.Tables[i];
                    var dtdata = dt.Select("recordNumber='AJ320923120210172' or recordNumber='AJ320923120210213' or recordNumber='AJ320923120200050'");
                    if (dtdata.Length > 0)
                    {
                        for (int j = 0; j < dtdata.Length; j++)
                        {
                            try
                            {
                                DataRow dr = dtdata[j];
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
                                url = jso.GetValue("siteuploadurl").ToString();
                                api = jso.GetValue("post").ToString();
                                string realapi = api;
                                JObject job = new JObject();




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

                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");

                                var data = jso;
                                jso = new JObject();
                                jso.Add("Token", Token);
                                jso.Add("JSON", data);

                                string result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                CityUploadOperateLog LogEntity = new CityUploadOperateLog
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
                                           // await _aqtUploadService.UpdateCityUploadDate(url, api, now);
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




            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForFuningMinute();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
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
                            url = jso.GetValue("siteuploadurl").ToString();
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            api = jso.GetValue("post").ToString();
                            string realapi = api;
                            if (api.Contains("Craneinterface/UploadCraneHistory"))
                            {
                                //塔机实时数据
                                realapi = "Craneinterface/UploadCraneHistory?appkey=" + account + "&secret=" + pwd;
                                string sedata = jso.GetValue("sedata").ToString().Replace("\\\"", "\"");
                                if (string.IsNullOrEmpty(sedata))
                                {
                                    continue;
                                }
                                JObject sedatejso = (JObject)JsonConvert.DeserializeObject(sedata);
                                //JObject sedatejso = (JObject)jso.GetValue("sedata");
                                jso.Add("load", sedatejso.GetValue("Weight").ToDouble());
                                jso.Add("range", sedatejso.GetValue("Margin").ToDouble());
                                jso.Add("moment", sedatejso.GetValue("Moment").ToDouble());
                                jso.Add("rotation", sedatejso.GetValue("Rotation").ToDouble());
                                jso.Add("height", sedatejso.GetValue("Height").ToDouble());
                                jso.Add("windSpeed", sedatejso.GetValue("WindSpeed").ToDouble());
                                jso.Add("dip", 0);
                                jso.Add("multiplyingPower", 0);
                            }
                            else if (api.Contains("Hoistinterface/HoistHistory"))
                            {
                                //升降机实时数据
                                realapi = "Hoistinterface/HoistHistory?appkey=" + account + "&secret=" + pwd;
                            }
                            else if (api.Contains("DustInterface/UploadDustHistory"))
                            {
                                //扬尘实时数据
                                realapi = "DustInterface/UploadDustHistory?appkey=" + account + "&secret=" + pwd;
                            }
                            else if (api.Contains("Uploadinterface/UploadHistory"))
                            {
                                //卸料平台产生的数据实时上传
                                realapi = "Uploadinterface/UploadHistory?appkey=" + account + "&secret=" + pwd;
                                string rtdjson = jso.GetValue("rtdjson").ToString().Replace("\\\"", "\"");
                                if (string.IsNullOrEmpty(rtdjson))
                                {
                                    continue;
                                }
                                JObject rtdjso = (JObject)JsonConvert.DeserializeObject(rtdjson);
                                jso.Add("powerPercent", rtdjso.GetValue("electric_quantity").ToString());
                                jso.Add("weight", rtdjso.GetValue("weight").ToString());
                                jso.Add("weightBias", rtdjso.GetValue("bias").ToString());
                                jso.Add("warningWeight", rtdjso.GetValue("early_warning_weight").ToString());
                                jso.Add("AlarmWeight", rtdjso.GetValue("alarm_weight").ToString());
                                jso.Add("dataType", rtdjso.GetValue("upstate").ToString());
                            }
                            jso.Remove("post");
                            jso.Remove("siteuploadurl");
                            jso.Remove("uploadaccount");
                            jso.Remove("uploadpwd");
                            try
                            {
                                string result = _cityToken.JsonRequest(funingurl, account, pwd, realapi, JsonConvert.SerializeObject(jso));
                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = funingurl,
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
                                    int code = (int)mJObj.GetValue("code");
                                    if (code == 0)
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                            list.Add(url + api);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                _logger.LogError(api + ":" + ex.Message, true);
                            }

                        }
                    }
                }
            }










           

            _logger.LogInformation("分钟数据上传结束。", true);
        }
    }
}
