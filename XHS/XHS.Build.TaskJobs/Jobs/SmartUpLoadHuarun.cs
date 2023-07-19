using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadHuarun : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHuarun> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadHuarun(ILogger<SmartUpLoadHuarun> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService)
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

            _logger.LogInformation("数据上传开始。", true);
            string url = "https://scs.crland.com.cn/smart-api";
            string account = "hr1637146548748";
            string pwd = "7E437F55BEE72604592C4AAB0BB0F66D";
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string realapi = string.Empty;
            string method = string.Empty;
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListForHuarun();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count <= 0)
                    {
                        continue;
                    }
                    var dtdata = dt.Select("GONGCHENG_CODE='Huarun'");
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

                                string api = jso.GetValue("post").ToString();
                                realapi = api;
                                if (api.Contains("UploadCraneHistory"))
                                {
                                    realapi = "/v1/tower-crane/saveRealtimeRecord";
                                    method = "tower-crane";
                                    jso.Add("hardwareId", jso.GetValue("deviceId"));
                                    jso.Add("datetime", jso.GetValue("moniterTime"));
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("sedata").ToString()))
                                        {
                                            JObject sedatajob = JObject.Parse(jso.GetValue("sedata").ToString());
                                            jso.Add("load", sedatajob.GetValue("Weight"));
                                            jso.Add("amplitude", sedatajob.GetValue("Margin"));
                                            jso.Add("torque", sedatajob.GetValue("Moment"));
                                            jso.Add("rotation", sedatajob.GetValue("Rotation"));
                                            jso.Add("height", sedatajob.GetValue("Height"));
                                            jso.Add("windScale", sedatajob.GetValue("Wind"));
                                            jso.Add("windSpeed", sedatajob.GetValue("WindSpeed"));
                                            jso.Add("dipAngle", 0);

                                            var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(sedatajob.GetValue("Alarm")));

                                            if (intList.Contains(512))
                                            {
                                                jso.Add("collisionAlarmStatus", true);
                                            }
                                            else
                                            {
                                                jso.Add("collisionAlarmStatus", false);
                                            }
                                        }
                                    }

                                    if (jso.ContainsKey("paramjson"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("paramjson").ToString()))
                                        {
                                            JObject paramjob = JObject.Parse(jso.GetValue("paramjson").ToString());
                                            jso.Add("ratedLoad", paramjob.GetValue("MaxWeight"));
                                            jso.Add("currentRatio", paramjob.GetValue("BeiLv"));
                                        }
                                    }
                                    jso.Remove("sedata");
                                    jso.Remove("paramjson");
                                }//塔吊实时
                                else if (api.Contains("CraneWorkData"))
                                {
                                    realapi = "/v1/tower-crane/saveCycleRecord";
                                    method = "tower-crane";
                                    jso.Add("datetime", jso.GetValue("updatedate").ToDateTime().ToString("yyyy-MM-dd"));
                                    if (jso.ContainsKey("workdata"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("workdata").ToString()))
                                        {
                                            JObject workjob = JObject.Parse(jso.GetValue("workdata").ToString());
                                            jso.Add("hardwareId", workjob.GetValue("SeCode"));
                                            jso.Add("startLoad", "");
                                            jso.Add("startRatio", "0");
                                            jso.Add("startAmplitude", workjob.GetValue("StartMargin"));
                                            jso.Add("startTorque", "0");
                                            jso.Add("startRotation", workjob.GetValue("StartRotation"));
                                            jso.Add("startHeight", workjob.GetValue("StartHeight"));
                                            jso.Add("startWindSpeed", "0");
                                            jso.Add("startDipAngle", "0");
                                            jso.Add("startWarningStatus", "");
                                            jso.Add("startAlarmStatus", "");
                                            jso.Add("startSensorStatus", "");
                                            jso.Add("startDatetime", Convert.ToDateTime(workjob.GetValue("StartTime")).ToString("yyyy-MM-dd HH:mm:ss"));
                                            jso.Add("endLoad", "0");
                                            jso.Add("endRatio", "0");
                                            jso.Add("endAmplitude", workjob.GetValue("EndMargin"));
                                            jso.Add("endTorque", "0");
                                            jso.Add("endRotation", workjob.GetValue("EndRotation"));
                                            jso.Add("endHeight", workjob.GetValue("EndHeight"));
                                            jso.Add("endWindSpeed", "0");
                                            jso.Add("endDipAngle", "0");
                                            jso.Add("endWarningStatus", "");
                                            jso.Add("endAlarmStatus", "");
                                            jso.Add("endSensorStatus", "");
                                            jso.Add("endDatetime", workjob.GetValue("EndTime").ToDateTime().ToString("yyyy-MM-dd HH:mm:ss"));
                                            jso.Add("maxTorque", workjob.GetValue("MaxMargin"));
                                        }
                                    }
                                    if (jso.ContainsKey("paramjson"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("paramjson").ToString()))
                                        {
                                            JObject paramjob = JObject.Parse(jso.GetValue("paramjson").ToString());
                                            jso.Add("ratio", paramjob.GetValue("BeiLv").ToString());
                                            jso.Add("maxTorqueLoad", "0");
                                            jso.Add("maxTorqueRatedLoad", "0");
                                            jso.Add("maxTorqueAmplitude", paramjob.GetValue("LiJvMaxMargin").ToString());
                                            jso.Add("maxWindSpeed", "0");

                                            var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(paramjob.GetValue("Alarm")));
                                            if (intList.Contains(256))
                                            {
                                                jso.Add("windSpeedAlarm", "1");
                                            }
                                            else
                                            {
                                                jso.Add("windSpeedAlarm", "0");
                                            }
                                        }
                                    }
                                    jso.Remove("recordNumber");
                                    jso.Remove("secode");
                                    jso.Remove("updatedate");
                                    jso.Remove("workdata");
                                }//塔吊循环记录数据
                                else if (api.Contains("HoistHistory"))
                                {
                                    realapi = "/v1/lift/saveRealtimeRecord";
                                    method = "lift";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("sedata").ToString()))
                                        {
                                            JObject sedatajob = JObject.Parse(jso.GetValue("sedata").ToString());
                                            jso.Add("hardwareId", sedatajob.GetValue("SeCode"));
                                            jso.Add("datetime", sedatajob.GetValue("DeviceTime").ToDateTime().ToString("yyyy-MM-dd HH:mm:dd"));
                                            jso.Add("load", (sedatajob.GetValue("Weight").ToDouble() * 1000).ToString());
                                            jso.Add("dipAngle", "");
                                            jso.Add("height", sedatajob.GetValue("Height").ToString());
                                            jso.Add("windScale", "");
                                            jso.Add("windSpeed", "");
                                        }
                                    }
                                    jso.Remove("sedata");
                                    jso.Remove("deviceId");
                                    jso.Remove("model");
                                    jso.Remove("name");
                                    jso.Remove("moniterTime");
                                    jso.Remove("projectInfoId");
                                    jso.Remove("recordNumber");
                                }//升降机实时
                                else if (api.Contains("UploadDustInfo"))
                                {
                                    realapi = "/v1/environment/saveRealtimeRecord";
                                    method = "environment";
                                    jso.Add("airHumidity", jso.GetValue("humidity"));
                                    jso.Add("airTemperature", jso.GetValue("temperature"));
                                    jso.Add("pm25", jso.GetValue("pm2dot5"));
                                }//扬尘实时
                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                if (jso.ContainsKey("funingurl"))
                                {
                                    jso.Remove("funingurl");
                                }
                                if (jso.ContainsKey("GONGCHENG_CODE"))
                                {
                                    jso.Remove("GONGCHENG_CODE");
                                }
                                if (jso.ContainsKey("rtdjson"))
                                {
                                    jso.Remove("rtdjson");
                                }
                                if (jso.ContainsKey("paramjson"))
                                {
                                    jso.Remove("paramjson");
                                }

                                long timestamp = DateTimeExtensions.ToTimestamp(DateTime.Now, true);
                                string signStr = pwd + "appId=" + account + "&version=" + "v1" + "&method=" + method + "&timestamp=" + timestamp + "&data=" + JsonConvert.SerializeObject(jso) + pwd;
                                string sign = UEncrypter.EncryptBySHA1(signStr).ToString().Replace("-", "").ToLower();
                                string URL = url + realapi + "?appId=" + account + "&version=" + "v1" + "&method=" + method + "&timestamp=" + timestamp + "&sign=" + sign;

                                string result = HttpNetRequest.POSTSendJsonRequest(URL, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });


                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = URL,
                                    api = api,
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
                                    string msg = mJObj.GetValue("msg").ToString();
                                    if (msg == "上报成功")
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate("120.195.199.66:5678", api, now);
                                            list.Add(url + api);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                _logger.LogError(realapi + ":" + ex.Message);
                                return;
                            }
                            catch (Exception ex)
                            {

                                _logger.LogError(realapi + ":" + ex.Message, true);
                            }
                        }
                    }
                }
            }
            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
