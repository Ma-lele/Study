using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadHourXuweiJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourXuweiJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        private readonly SiteCityXuweiToken _xuweiToken;

        public SmartUpLoadHourXuweiJob(ILogger<SmartUpLoadHourXuweiJob> logger, IOperateLogService operateLogService, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService, SiteCityXuweiToken xuweiToken)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _xuweiToken = xuweiToken;
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
            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
            int successcount = 0;
            int errcount = 0;
            string projectId = string.Empty;
            string result = string.Empty;
            string realapi = string.Empty;
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListForXuwei();
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
                                string url = jso.GetValue("siteuploadurl").ToString();
                                //string account = jso.GetValue("uploadaccount").ToString();
                                //string pwd = jso.GetValue("uploadpwd").ToString();
                                string api = jso.GetValue("post").ToString();
                                projectId = jso.GetValue("GONGCHENG_CODE").ToString();
                                int type = 1;

                                //url里带上appkey和appsecret
                                realapi = api;
                                if (api.Contains("UploadVideo"))       //6.4.1	推送视频数据
                                {
                                    realapi = "/api/provide/service/supervisor/videos";
                                    var data = jso;
                                    jso.Add("projectId", projectId);
                                    jso.Add("deviceName", jso.GetValue("site"));
                                    jso["deviceNumber"] = jso.GetValue("deviceNumber").ToString();
                                    jso.Add("token", "token");
                                    jso["deviceStatus"] = jso.GetValue("deviceStatus").ToInt();
                                    jso.Remove("site");
                                    jso.Remove("recordNumber");
                                    jso.Remove("GONGCHENG_CODE");
                                    jso.Remove("belongedTo");
                                    jso.Remove("videoId");
                                    jso.Remove("type");
                                }
                                else if (api.Contains("washCar/info"))        //6.6.1	上传车辆清洗设备信息
                                {
                                    jso.Add("projectId", projectId);
                                    if (jso.GetValue("washStatus").ToString() == "0")
                                    {
                                        jso.Add("alarmType", 0);
                                    }
                                    else
                                    {
                                        jso.Add("alarmType", 1);
                                    }
                                    jso.Remove("recordNumber");
                                    realapi = "/api/provide/service/washCar/info";
                                }
                                //塔吊
                                else if (api.Contains("UploadMachineryInfos"))//6.2.6	上传塔吊基本信息
                                {
                                    jso.Add("projectId", projectId);
                                    if (jso.GetValue("setype").ToInt() == 1)
                                    {
                                        realapi = "/api/provide/service/towerCrane/baseInfo";
                                    }
                                    else if (jso.GetValue("setype").ToInt() == 2)
                                    {
                                        realapi = "/api/provide/service/elevator/baseInfo";
                                    }
                                    jso["deviceId"] = jso.GetValue("propertyRightsRecordNo");
                                    if (jso.GetValue("sestatus").ToInt() == 1)
                                    {
                                        jso.Add("deviceStatus", 1);
                                    }
                                    else if (jso.GetValue("sestatus").ToInt() == 0)
                                    {
                                        jso.Add("deviceStatus", 0);
                                    }
                                    jso.Add("manufacturingLicense", jso.GetValue("propertyno"));
                                    jso.Add("model", jso.GetValue("machineryModel"));
                                    jso["recordNumber"] = jso.GetValue("recordno");
                                    jso.Add("manufacturer", jso.GetValue("oem"));
                                    jso.Add("propertyUnit", jso.GetValue("rightcompany"));
                                    jso.Add("manufacturingNo", jso.GetValue("leaveTheFactoryNo"));
                                    jso.Add("verifyDate", jso.GetValue("reCheckReviewDate").ToDateTime().ToString("yyyy-MM-dd HH:mm:ss"));
                                    jso.Add("deviceName", jso.GetValue("devicename"));
                                }
                                else if (api.Contains("CraneAlarmOn"))           //塔机报警
                                {
                                    realapi = "/api/provide/service/towerCrane/data";
                                    jso.Add("projectId", projectId);
                                    JObject paramjob = JObject.Parse(jso.GetValue("paramjson").ToString());

                                    jso.Add("startAlarmTime", DateTimeExtensions.ToTimestamp(paramjob.GetValue("StartTime").ToDateTime()));
                                    jso.Add("endAlarmTime", DateTimeExtensions.ToTimestamp(paramjob.GetValue("EndTime").ToDateTime()));
                                    jso["alarmLevel"] = 2;


                                    var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(paramjob.GetValue("Alarm")));

                                    if (intList.Contains(4) || intList.Contains(8))//幅度
                                    {
                                        jso.Add("alarmType", "倾斜告警");
                                    }
                                    else if (intList.Contains(64))//重量
                                    {
                                        jso.Add("alarmType", "超重告警");
                                    }
                                    else if (intList.Contains(128))
                                    {
                                        jso.Add("alarmType", "力矩告警");
                                    }
                                    else if (intList.Contains(256))
                                    {
                                        jso.Add("alarmType", "风速告警");
                                    }
                                   

                                    jso.Remove("WARNID");
                                    jso.Remove("recordNumber");
                                    jso.Remove("GONGCHENG_CODE");
                                    jso.Remove("belongedTo");
                                    jso.Remove("description");
                                    jso.Remove("time");
                                    jso.Remove("projectInfoId");
                                    jso.Remove("warnExplain");
                                    jso.Remove("warnLevel");
                                    jso.Remove("warnContent");
                                    jso.Remove("happenTime");
                                    jso.Remove("idcard");
                                    jso.Remove("paramjson");


                                }
                                else if (api.Contains("ElevatorAlarmOn"))     //	--升降机报警
                                {
                                    realapi = "/api/provide/service/elevator/data";
                                    jso.Add("projectId", projectId);
                                    jso.Add("alarmTime", jso.GetValue("time"));
                                    jso["alarmLevel"] = 2;
                                    if (jso.GetValue("alarmType").ToString() == "重量限位")
                                    {
                                        jso["alarmType"] = "超重告警";
                                    }
                                    else if (jso.GetValue("alarmType").ToString() == "倾角限位")
                                    {
                                        jso["alarmType"] = "倾斜告警";
                                    }
                                    
                                }

                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                if (jso.ContainsKey("funingurl"))
                                {
                                    jso.Remove("funingurl");
                                }
                                result = _xuweiToken.JsonRequest(type, projectId, realapi, JsonConvert.SerializeObject(jso));
                                LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = url,
                                    api = api,
                                    account = type.ToString(),
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
