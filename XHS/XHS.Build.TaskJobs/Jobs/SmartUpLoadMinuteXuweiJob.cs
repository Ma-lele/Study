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
    public class SmartUpLoadMinuteXuweiJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadMinuteXuweiJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        private readonly SiteCityXuweiToken _xuweiToken;
        public SmartUpLoadMinuteXuweiJob(ILogger<SmartUpLoadMinuteXuweiJob> logger, IOperateLogService operateLogService, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService, SiteCityXuweiToken xuweiToken)
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
            _logger.LogInformation("分钟数据上传开始。", true);

            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
            int successcount = 0;
            int errcount = 0;
            string projectId = string.Empty;
            string realapi = string.Empty;
            string result = string.Empty;
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForXuweiMinute();
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
                                if (api.Contains("GuangliandaCraneOn"))           //6.3.2	上传塔吊运行数据（实时）
                                {
                                    realapi = "/api/provide/service/towerCrane/supervision";
                                    jso.Add("deviceName", jso.GetValue("name").ToString());
                                    jso.Add("projectId", projectId);
                                    if (jso.GetValue("sestatus").ToInt() == 1)
                                    {
                                        jso.Add("deviceStatus", 1);
                                    }
                                    else if (jso.GetValue("sestatus").ToInt() == 0)
                                    {
                                        jso.Add("deviceStatus", 0);
                                    }
                                    jso.Add("driverAuthStatus", "通过");
                                    jso.Add("driverAuthTime", jso.GetValue("updatedate").ToDateTime().ToString("yyyy-MM-dd HH:mm:ss"));
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        JObject jobdata = JObject.Parse(System.Web.HttpUtility.HtmlDecode(jso.GetValue("sedata").ToString()));
                                        jso.Add("driverName", jobdata.GetValue("DriverName").ToString());
                                        jso.Add("driverIdcard", jobdata.GetValue("DriverId").ToString());
                                        jso.Add("torqueRatio", jobdata.GetValue("MomentPercent").ToString());
                                        jso.Add("amplitude", jobdata.GetValue("Margin"));
                                        jso.Add("height", jobdata.GetValue("Height").ToString());
                                        jso.Add("rotation", jobdata.GetValue("Rotation").ToString());
                                        jso.Add("Weight", jobdata.GetValue("SafeLoad").ToString());
                                        jso.Add("windspeed", jobdata.GetValue("Wind").ToString());
                                        jso.Add("weight", jobdata.GetValue("Weight"));
                                        jso.Add("dataTimeStamp", DateTimeExtensions.ToTimestamp(jso.GetValue("UpdateTime").ToDateTime(), true));
                                        jso.Add("windLevel", jobdata.GetValue("Wind"));
                                        jso.Add("moment", jso.GetValue("MomentPercent").ToDouble());
                                    }
                                    if (jso.ContainsKey("paramjson"))
                                    {
                                        JObject paramjob = JObject.Parse(System.Web.HttpUtility.HtmlDecode(jso.GetValue("paramjson").ToString()));
                                        jso.Add("ratedWeight", paramjob.GetValue("MaxWeight").ToDouble());
                                    }
                                    jso.Add("angle", 0);
                                    jso.Add("loadRatio", "");
                                    jso.Remove("paramjson");
                                    jso.Remove("sedata");
                                }
                                else if (api.Contains("HoistHistory"))           //6.5.2	上传升降机运行数据（实时）
                                {
                                    realapi = "/api/provide/service/elevator/supervision";
                                    jso.Add("projectId", projectId);
                                    jso.Add("deviceName", jso.GetValue("name"));
                                    if (jso.GetValue("sestatus").ToInt() == 1)
                                    {
                                        jso.Add("deviceStatus", "active");
                                    }
                                    else if (jso.GetValue("sestatus").ToInt() == 0)
                                    {
                                        jso.Add("deviceStatus", "disconnected");
                                    }
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        if (!string.IsNullOrEmpty("sedata"))
                                        {
                                            JObject sedatajob = JObject.Parse(jso.GetValue("sedata").ToString());
                                            jso.Add("driverName", sedatajob.GetValue("DriverName"));
                                            jso.Add("driverIdcard", sedatajob.GetValue("DriverCardNo"));
                                            jso.Add("driverAuthStatus", "通过");
                                            jso.Add("driverAuthTime", sedatajob.GetValue("DeviceTime"));
                                            if (!string.IsNullOrEmpty(sedatajob.GetValue("DriverCardNo").ToString()))
                                            {
                                                if (sedatajob.GetValue("DriverCardNo").ToString().Substring(16, 1).ToInt() % 2 == 0)
                                                {
                                                    jso.Add("sex", 1);
                                                }
                                                else
                                                {
                                                    jso.Add("sex", 0);
                                                }
                                                jso.Add("age", DateTime.Now.Year - sedatajob.GetValue("DriverCardNo").ToString().Substring(6, 4).ToInt());
                                            }


                                            jso.Add("height", sedatajob.GetValue("Height").ToDouble());
                                            jso.Add("speed", sedatajob.GetValue("Speed"));
                                            jso.Add("weight", sedatajob.GetValue("Weight"));
                                            jso.Add("avgSpeed", sedatajob.GetValue("Speed").ToDouble());



                                            var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(sedatajob.GetValue("AlarmState")));

                                            if (intList.Contains(512))//重量
                                            {
                                                jso.Add("warningState", 2);
                                            }
                                            else
                                            {
                                                jso.Add("warningState", 0);
                                            }
                                        }
                                    }
                                    jso.Remove("sedata");
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
                            catch (Exception ex)
                            {

                                 _logger.LogError(realapi + ":" + ex.Message);
                            }
                        }


                    }
                }
            }

            _logger.LogInformation("分钟数据上传结束。", true);
        }
    }
}
