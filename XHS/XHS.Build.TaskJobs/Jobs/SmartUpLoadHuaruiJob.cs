using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadHuaruiJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHuaruiJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadHuaruiJob(ILogger<SmartUpLoadHuaruiJob> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService)
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
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            int successcount = 0;
            int errcount = 0;
            string result = "";
            string realapi = "";

            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListForHuarui();

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
                                string account = jso.GetValue("uploadaccount").ToString();
                                string pwd = jso.GetValue("uploadpwd").ToString();
                                string api = jso.GetValue("post").ToString();
                                realapi = api;
                                //小时



                                if (api.Contains("UploadCraneHistory"))          //塔吊实时监测数据上传
                                {
                                    realapi = "/plat/lcb/equipMachine/interfaceCraneRecord";
                                    string sedatestr = System.Web.HttpUtility.HtmlDecode(jso.GetValue("sedata").ToString());
                                    if (!string.IsNullOrEmpty(sedatestr))
                                    {
                                        JObject sedatejob = JObject.Parse(sedatestr);
                                        jso.Add("heightValue", sedatejob.GetValue("Height").ToInt());
                                        jso.Add("machineId", jso.GetValue("deviceId").ToString());
                                        jso.Add("marginValue", sedatejob.GetValue("Margin").ToInt());
                                        jso.Add("momentPercent", sedatejob.GetValue("MomentPercent").ToString());
                                        jso.Add("momentValue", sedatejob.GetValue("Moment").ToInt());
                                        jso.Add("recordTime", jso.GetValue("moniterTime").ToString());
                                        jso.Add("rotationValue", sedatejob.GetValue("Rotation").ToInt());
                                        jso.Add("weightValue", sedatejob.GetValue("Weight").ToInt());
                                        jso.Add("windLevel", sedatejob.GetValue("Wind").ToInt());
                                        //jso.Add("rotationPercent", 0.0);      //实时回转百分比
                                        //jso.Add("windValue", 0.0);            //实时风速值
                                    }
                                }
                                else if (api.Contains("HoistHistory"))      //升降机实时监测数据上传
                                {
                                    realapi = "plat/lcb/equipMachine/interfaceElevatorRecord";
                                    string sedatestr = jso.GetValue("sedata").ToString();
                                    if (!string.IsNullOrEmpty(sedatestr))
                                    {
                                        JObject sedatejob = JObject.Parse(sedatestr);
                                        jso.Add("heightValue", ((int)sedatejob.GetValue("Height")));
                                        jso.Add("machineId", jso.GetValue("deviceId").ToString());
                                        jso.Add("personCount", ((int)sedatejob.GetValue("NumOfPeople")));
                                        jso.Add("recordTime", jso.GetValue("moniterTime").ToString());
                                        jso.Add("speedValue", ((int)sedatejob.GetValue("Speed")));
                                        jso.Add("weightValue", ((int)sedatejob.GetValue("Weight")));
                                        //jso.Add("xvalue", sedatejob.GetValue("").ToString());
                                        //jso.Add("yvalue", sedatejob.GetValue("").ToString());

                                    }
                                }
                                else if (api.Contains("UploadDustHistory"))           //扬尘数据上传
                                {
                                    realapi = "plat/lcb/equipMachine/interfaceDustRecord";
                                    jso.Add("dustId", jso.GetValue("deviceId").ToString());
                                    jso.Add("monitorTime", jso.GetValue("upload").ToString());
                                    jso.Add("rtHum", ((int)jso.GetValue("humidity")));
                                    jso.Add("rtNoise", ((int)jso.GetValue("noise")));
                                    jso.Add("rtPm10", ((int)jso.GetValue("pm10")));
                                    jso.Add("rtPm25", ((int)jso.GetValue("pm2dot5")));
                                    jso.Add("rtTemp", ((int)jso.GetValue("temperature")));
                                    jso.Add("rtWd", jso.GetValue("windDirection").ToString());
                                    jso.Add("rtWs", ((int)jso.GetValue("windSpeed")));
                                    //jso.Add("pollute", jso.GetValue("").ToString());          //首要污染物
                                    //jso.Add("rtQuality", jso.GetValue("").ToString());        //空气质量
                                    //jso.Add("warning", jso.GetValue("").ToString());          //报警

                                }
                                else if (api.Contains("FenceAlarmOn"))          //临边防护预警数据上传
                                {
                                    realapi = "/plat/lcb/protection/interfaceProtectionRecord";
                                    jso.Add("eventType", "1");
                                    //jso.Add("latitude", jso.GetValue("").ToString());         //纬度
                                    //jso.Add("longitude", jso.GetValue("").ToString());        //经度
                                    jso.Add("warningCode", jso.GetValue("WARNID").ToString());
                                    jso.Add("warningTime", jso.GetValue("time").ToString());
                                }
                                else if (api.Contains("FenceAlarmOff"))         //临边防护预警数据上传
                                {
                                    realapi = "/plat/lcb/protection/interfaceProtectionRecord";
                                    jso.Add("eventType", "2");
                                    //jso.Add("latitude", jso.GetValue("").ToString());     //纬度
                                    //jso.Add("longitude", jso.GetValue("").ToString());    //经度
                                    jso.Add("warningCode", jso.GetValue("WARNID").ToString());
                                    jso.Add("warningTime", jso.GetValue("time").ToString());
                                }
                                else if (api.Contains("UploadHistory"))      //卸料平台实时监测数据上传
                                {
                                    realapi = "/plat/lcb/equipMachine/interfaceUnloadRecord";
                                    jso.Add("machineId", jso.GetValue("deviceId").ToString());

                                    string rtdjsonstr = jso.GetValue("rtdjson").ToString();
                                    if (!string.IsNullOrEmpty(rtdjsonstr))
                                    {
                                        JObject rtdjsonjob = JObject.Parse(rtdjsonstr);
                                        jso.Add("recordTime", Convert.ToDateTime(rtdjsonjob.GetValue("updatetime")).ToString("yyyy-MM-dd HH:mm:ss"));
                                        jso.Add("weightValue", ((int)rtdjsonjob.GetValue("weight")));
                                    }
                                }
                                else if (api.Contains("UploadSpecialOperationPersonnel"))       //司机绑定数据上传
                                {
                                    realapi = "/plat/lcb/equipMachine/interfaceUsageRecord";
                                    jso.Add("equipType", jso.GetValue("setype").ToString());
                                    jso.Add("identifyResult", 1);
                                    jso.Add("machineId", jso.GetValue("propertyRightsRecordNo").ToString());
                                    jso.Add("registerId", jso.GetValue("idCard").ToString());
                                }
                                else if (api.Contains("CraneAlarmOn"))        //上传塔吊报警信息，
                                {
                                    realapi = "/plat/lcb/equipMachine/interfaceWarning";
                                    jso.Add("eventType", "crane_alarm");
                                    jso.Add("machineId", jso.GetValue("deviceId").ToString());
                                    if (jso.GetValue("alarmType").ToString() == "风速报警")
                                    {
                                        jso.Add("warningAlarmCategory", 120);
                                    }
                                    else if (jso.GetValue("alarmType").ToString() == "超力矩报警")
                                    {
                                        jso.Add("warningAlarmCategory", 130);

                                    }
                                    else if (jso.GetValue("alarmType").ToString() == "左回转报警" || jso.GetValue("alarmType").ToString() == "右回转报警")
                                    {
                                        jso.Add("warningAlarmCategory", 160);
                                    }
                                    else if (jso.GetValue("alarmType").ToString() == "小车内变幅报警" || jso.GetValue("alarmType").ToString() == "小车外变幅报警")
                                    {
                                        jso.Add("warningAlarmCategory", 150);
                                    }
                                    else if (jso.GetValue("alarmType").ToString() == "吊钩上升报警" || jso.GetValue("alarmType").ToString() == "吊钩下降报警")
                                    {
                                        jso.Add("warningAlarmCategory", 140);
                                    }
                                    else if (jso.GetValue("alarmType").ToString() == "超重报警")
                                    {
                                        jso.Add("warningAlarmCategory", 110);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    jso.Add("registerId", jso.GetValue("idcard").ToString());
                                    jso.Add("warningAlarmCode", jso.GetValue("WARNID").ToString());
                                    jso.Add("warningAlarmContent", jso.GetValue("description").ToString());
                                    jso.Add("warningAlarmLevel", ((int)jso.GetValue("alarmLevel")));
                                    jso.Add("warningAlarmTime", jso.GetValue("time").ToString());
                                    jso.Add("warningAlarmType", 2);
                                    //jso.Add("realTimeValue", jso.GetValue("").ToString());        //瞬时值
                                    //jso.Add("warningValue", jso.GetValue("").ToString());         //预警值
                                    //jso.Add("alarmValue", ((int)jso.GetValue("description")));    //报警值

                                }
                                else if (api.Contains("ElevatorAlarmOn"))       //升降机报警上传
                                {
                                    realapi = "/plat/lcb/equipMachine/interfaceWarning";

                                    jso.Add("eventType", "elevator_alarm");
                                    jso.Add("machineId", jso.GetValue("deviceId").ToString());
                                    jso.Add("registerId", jso.GetValue("driverId").ToString());
                                    if (jso.GetValue("alarmType").ToString() == "重量限位")
                                    {
                                        jso.Add("warningAlarmCategory", 210);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    jso.Add("warningAlarmCode", jso.GetValue("WARNID").ToString());
                                    jso.Add("warningAlarmContent", jso.GetValue("description").ToString());
                                    jso.Add("warningAlarmLevel", ((int)jso.GetValue("alarmLevel")));
                                    jso.Add("warningAlarmTime", jso.GetValue("time").ToString());
                                    jso.Add("warningAlarmType", 2);
                                    //jso.Add("alarmValue", jso.GetValue("").ToString());       //报警值
                                    //jso.Add("realTimeValue", jso.GetValue("").ToString());    //瞬时值
                                    //jso.Add("warningValue", jso.GetValue("").ToString());     //预警值
                                }
                                else if (api.Contains("DischargeAlarmOn"))      //卸料平台报警
                                {
                                    realapi = "/plat/lcb/equipMachine/interfaceWarning";

                                    string paramjsonstr = jso.GetValue("paramjson").ToString();
                                    if (!string.IsNullOrEmpty(paramjsonstr))
                                    {
                                        JObject jsonjob = JObject.Parse(paramjsonstr);
                                        if (jsonjob.GetValue("alerttype").ToString() == "重量报警")
                                        {
                                            jso.Add("realTimeValue", jsonjob.GetValue("weight").ToString());
                                            jso.Add("warningValue", jsonjob.GetValue("early_warning_weight").ToString());
                                            jso.Add("alarmValue", jsonjob.GetValue("alarm_weight").ToString());
                                        }
                                    }

                                    jso.Add("eventType", "unload_alarm");
                                    jso.Add("machineId", jso.GetValue("deviceId").ToString());
                                    jso.Add("registerId", jso.GetValue("idcard").ToString());
                                    if (jso.GetValue("alarmType").ToString() == "重量报警")
                                    {
                                        jso.Add("warningAlarmCategory", 310);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    jso.Add("warningAlarmCode", jso.GetValue("WARNID").ToString());
                                    jso.Add("warningAlarmContent", jso.GetValue("description").ToString());
                                    jso.Add("warningAlarmLevel", ((int)jso.GetValue("alarmLevel")));
                                    jso.Add("warningAlarmTime", jso.GetValue("time").ToString());
                                    jso.Add("warningAlarmType", 2);
                                    //jso.Add("alarmValue", jso.GetValue("").ToString());           //报警值
                                    //jso.Add("realTimeValue", jso.GetValue("").ToString());        //瞬时值
                                    //jso.Add("warningValue", jso.GetValue("").ToString());         //预警值
                                }



                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                if (jso.ContainsKey("funingurl"))
                                {
                                    jso.Remove();
                                }

                                result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });

                                var LogEntity = new CityUploadOperateLog
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
                                    if (code == 200)
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
                            catch (HttpRequestException ex)
                            {
                                _logger.LogError(realapi + ":" + ex.Message);
                                return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(realapi + ":" + ex.Message);
                            }


                        }
                    }
                }
            }

            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
