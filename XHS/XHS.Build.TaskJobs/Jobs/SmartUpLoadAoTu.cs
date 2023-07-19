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
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadAoTu : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadAoTu> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        private readonly SiteCityFuningToken _cityToken;
        public SmartUpLoadAoTu(ILogger<SmartUpLoadAoTu> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService, SiteCityFuningToken cityToken)
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

            _logger.LogInformation("数据上传开始。", true);

            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string realapi = string.Empty;
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListForAuto();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count <= 0)
                    {
                        continue;
                    }
                    var dtdata = dt.Select("GONGCHENG_CODE='D7540911965C45A79B51A158E903A52E'");
                    if (dtdata.Length > 0)
                    {
                        for (int j = 0; j < dtdata.Length; j++)
                        {
                            JArray evejar = new JArray();
                            JObject evejob = new JObject();
                            JObject evejob1 = new JObject();
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
                                string url = "http://49.4.11.116:8085";
                                string account = "N7WGY1637718271npq33147219cwpRs";
                                string pwd = "Up1637718271g8sWLhL13928221Sn4egKGJ4M";
                                string api = jso.GetValue("post").ToString();
                                realapi = api;
                                if (api.Contains("UploadCraneHistory"))//塔吊实时
                                {
                                    realapi = "/api/Craneinterface/UploadCraneHistory";
                                    jso["projectInfoId"] = jso.GetValue("GONGCHENG_CODE");
                                    JObject sedatajob = JObject.Parse(jso.GetValue("sedata").ToString());
                                    jso.Add("load", sedatajob.GetValue("Weight").ToDouble());
                                    jso.Add("range", sedatajob.GetValue("Margin").ToDouble());
                                    jso.Add("moment", sedatajob.GetValue("MomentPercent").ToDouble());
                                    jso.Add("rotation", sedatajob.GetValue("Rotation").ToDouble());
                                    jso.Add("height", sedatajob.GetValue("Height").ToDouble());
                                    jso.Add("windSpeed", sedatajob.GetValue("WindSpeed").ToDouble());
                                    jso.Add("dip", 0);
                                    jso.Add("multiplyingPower", 0);
                                    jso.Remove("sedata");
                                    jso.Remove("GONGCHENG_CODE");
                                    jso.Remove("paramjson");
                                    jso.Remove("sedata");
                                }
                                else if (api.Contains("CraneBindPeopleInfo"))//上机
                                {
                                    if (jso.GetValue("setype").ToInt() == 1) //塔吊
                                    {
                                        realapi = "/api/Craneinterface/CraneBindPeopleInfo";
                                    }
                                    else if (jso.GetValue("setype").ToInt() == 2)//升降机
                                    {
                                        realapi = "/api/Hoistinterface/HoistBindPeopleInfo";
                                    }
                                    jso["projectInfoId"] = jso.GetValue("GONGCHENG_CODE");
                                    jso.Add("base64Photo", ImgHelper.ImageToBase64(jso.GetValue("path").ToString()));

                                }
                                else if (api.Contains("CraneReleasePeopleInfo"))//塔吊下机
                                {
                                    if (jso.GetValue("setype").ToInt() == 1)//塔吊
                                    {
                                        realapi = "/api/Craneinterface/CraneReleasePeopleInfo";

                                    }
                                    else if (jso.GetValue("setype").ToInt() == 2)//升降机
                                    {
                                        realapi = "/api/Hoistinterface/HoistReleasePeopleInfo";
                                    }
                                    jso["projectInfoId"] = jso.GetValue("GONGCHENG_CODE");
                                }
                                else if (api.Contains("CraneAlarmOn"))//塔吊报警
                                {
                                    realapi = "/api/Craneinterface/CraneAlarmInfo";
                                    jso["projectInfoId"] = jso.GetValue("GONGCHENG_CODE");
                                    jso.Remove("paramjson");
                                }
                                else if (api.Contains("UploadDustInfo"))//扬尘实时
                                {
                                    realapi = "/api/DustInterface/UploadDustHistory";
                                    jso.Add("projectInfoId", jso.GetValue("GONGCHENG_CODE"));
                                }
                                else if (api.Contains("HoistHistory"))//升降机实时
                                {
                                    realapi = "/api/Hoistinterface/HoistHistory";
                                    jso["projectInfoId"] = jso.GetValue("GONGCHENG_CODE");
                                    JObject sedatajob = JObject.Parse(jso.GetValue("sedata").ToString());
                                    jso.Add("load", sedatajob.GetValue("Weight"));
                                    jso.Add("avgSpeed", sedatajob.GetValue("Speed"));
                                    jso.Add("numberOfPeopleLoaded", sedatajob.GetValue("NumOfPeople"));
                                    jso.Add("isLoadAlarm", 0);
                                    jso.Add("isIdCardAlarm", 0);
                                    jso.Add("isSpeedAlarm", 0);
                                    jso.Add("isHeightAlarm", 0);
                                    jso.Add("isNumbersAlarm", 0);
                                    jso.Add("isInclinationAlarm", 0);
                                    int AlarmState = jso.GetValue("AlarmState").ToInt();
                                    if (AlarmState == 512)
                                    {
                                        jso["isLoadAlarm"] = 1;
                                    }
                                    else if (AlarmState == 1024)
                                    {
                                        jso["isSpeedAlarm"] = 1;
                                    }
                                    else if (AlarmState == 8 || AlarmState == 16)
                                    {
                                        jso["isHeightAlarm"] = 1;
                                    }
                                    else if (AlarmState == 256)
                                    {
                                        jso["isNumbersAlarm"] = 1;
                                    }
                                    else if (AlarmState == 1)
                                    {
                                        jso["isInclinationAlarm"] = 1;
                                    }
                                    jso.Remove("sedata");
                                }
                                else if (api.Contains("UploadHistory"))  //卸料平台
                                {
                                    realapi = "/api/Uploadinterface/UploadHistory";
                                    jso["projectInfoId"] = jso.GetValue("GONGCHENG_CODE");

                                    JObject rtdjob = JObject.Parse(jso.GetValue("rtdjson").ToString());
                                    jso.Add("powerPercent", rtdjob.GetValue("electric_quantity"));
                                    jso.Add("weight", rtdjob.GetValue("weight"));
                                    jso.Add("weightBias", rtdjob.GetValue("bias"));
                                    jso.Add("warningWeight", rtdjob.GetValue("early_warning_weight"));
                                    jso.Add("AlarmWeight", rtdjob.GetValue("alarm_weight"));
                                    jso.Add("dataType", rtdjob.GetValue("upstate"));
                                    jso.Remove("rtdjson");
                                }
                                if (jso.ContainsKey("funingurl"))
                                {
                                    jso.Remove("funingurl");
                                }
                                realapi = realapi + "?appkey=" + account + "&secret=" + pwd;
                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");

                                string result = _cityToken.JsonRequest(url, "6gjlb1620809112lfQ16245821pGGEl", "rr1635387682j1K0pCr40643940qAXamR1KnQ", realapi, JsonConvert.SerializeObject(jso));

                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = url,
                                    api = api,
                                    account = account,
                                    param = jso.ToString(),
                                    result = result,
                                    createtime = DateTime.Now
                                };
                                await _operateLogService.AddCityUploadApiLog(LogEntity);
                                if (string.IsNullOrEmpty(result))
                                {
                                    errcount += errcount;
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
            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
