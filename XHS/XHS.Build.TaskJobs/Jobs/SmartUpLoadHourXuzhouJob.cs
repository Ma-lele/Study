using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading;
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
    public class SmartUpLoadHourXuzhouJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourXuzhouJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityXuzhouToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadHourXuzhouJob(ILogger<SmartUpLoadHourXuzhouJob> logger, IOperateLogService operateLogService, SiteCityXuzhouToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
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
            DataSet ds = await _aqtUploadService.GetListForXuzhou();
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

                                if (api.Contains("/projectInfo"))           //2.上传项目基本信息(首先上传)
                                {
                                    realapi = "/projectInfo";
                                }
                                else if (api.Contains("securityCheck"))          //4.上传安全检查信息
                                {
                                    realapi = "/securityCheck";
                                }
                                else if (api.Equals("inspectionRectifier"))         //5.上传安全检查整改记录信息
                                {
                                    realapi = "/securityCheckRectifier";
                                }
                                else if (api.Equals("securityCheckReview"))     //6. 上传安全检查复查记录信息
                                {
                                    realapi = "/securityCheckReview";
                                }
                                else if (api.Equals("Check/InspectionPoint"))           //7.	上传移动巡检点信息
                                {
                                    realapi = "/inspectionPoint";
                                    jso.Add("checkPointId", jso.GetValue("inspectionId").ToString());
                                    jso.Add("summary", jso.GetValue("site").ToString());
                                    jso.Remove("inspectionId");
                                    jso.Remove("site");
                                }
                                else if (api.Equals("Check/InspectionPointContent"))         //8.	上传移动巡检信息
                                {
                                    realapi = "/inspectionInfo";
                                    jso.Add("number", jso.GetValue("inspectionContentId").ToString());
                                    jso.Add("checkPointId", jso.GetValue("inspectionId").ToString());
                                    jso.Add("cid", jso.GetValue("checkPersonId").ToString());
                                    jso.Add("inspector", jso.GetValue("checkPerson").ToString());
                                    jso.Add("isRectification", "0");
                                    jso.Add("fileUrl", jso.GetValue("urls").ToString().Replace("|", ",").TrimStart('[').TrimEnd(']').ToString());
                                    jso.Remove("inspectionContentId");
                                    jso.Remove("inspectionId");
                                    jso.Remove("checkPersonId");
                                    jso.Remove("checkPerson");
                                    jso.Remove("urls");
                                }
                                else if (api.Equals("clapAtWillCheck"))            //11.	上传随手拍信息
                                {
                                    realapi = "/clapAtWillCheck";
                                }
                                else if (api.Equals("clapAtWillCheckRectifier"))     //	--12.	上传随手拍整改记录信息
                                {
                                    realapi = "/clapAtWillCheckRectifier";
                                }
                                else if (api.Equals("clapAtWillCheckReview"))        //--13.	上传随手拍复查记录信息
                                {
                                    realapi = "/clapAtWillCheckReview";
                                }
                                else if (api.Contains("DustInfo/DustDeviceInfo"))            //14.	上传扬尘设备信息
                                {
                                    realapi = "/dustDevice";
                                }
                                else if (api.Contains("dustDeviceWarning"))            //16.上传扬尘报警信息
                                {
                                    realapi = "/dustDeviceWarning";
                                }
                                else if (api.Equals("DeviceInfo/UploadVideo"))           //18.	上传视频监控点信息
                                {
                                    realapi = "/monitorInfo";
                                    jso.Add("deviceId", jso.GetValue("videoId").ToString());
                                    jso.Add("address", jso.GetValue("site").ToString());
                                    jso.Add("deviceType", jso.GetValue("type").ToString());
                                    jso.Add("monitorDeviceId", jso.GetValue("videoId").ToString());
                                }
                                else if (api.Contains("DockingMachineryInfos/UploadMachineryInfos"))     //20.	上传塔机基本信息    24.	上传升降机基本信息
                                {
                                    if (jso.GetValue("setype").ToString() == "1")
                                    {
                                        realapi = "/towerInfo";
                                    }
                                    else
                                    {
                                        realapi = "/liftInfo";
                                    }
                                }
                                else if (api.Contains("AlarmInfo/CraneAlarmOn"))         //22.上传塔吊预警信息
                                {
                                    realapi = "/towerDeviceWarning";
                                    jso.Add("driverId", jso.GetValue("idcard").ToString());
                                    ///
                                    jso.Add("warningTime", Convert.ToDateTime(jso.GetValue("time")).ToString("yyyy-MM-dd HH:mm:ss"));
                                    jso.Add("warningContent", jso.GetValue("description").ToString());
                                    jso.Add("currentNum", "0");
                                    jso.Add("threshold", "0");
                                    jso.Remove("WARNID");
                                    jso.Remove("recordNumber");
                                    jso.Remove("belongedTo");
                                    jso.Remove("alarmType");
                                    jso.Remove("alarmLevel");
                                    jso.Remove("time");
                                    jso.Remove("projectInfoId");
                                    jso.Remove("warnExplain");
                                    jso.Remove("warnLevel");
                                    jso.Remove("description");
                                    jso.Remove("warnContent");
                                    jso.Remove("idcard");
                                }
                                else if (api.Equals("DeviceInfo/UploadDeviceInfo"))          //28.上传卸料平台基本信息
                                {
                                    realapi = "/unloadingInfo";
                                }
                                else if (api.Equals("DeviceInfo/DeppPitDeviceInfo"))         //32.	上传深基坑基本信息
                                {
                                    realapi = "/deppPitDeviceInfo";
                                }
                                else if (api.Equals("DeviceInfo/HighFormworkDeviceInfo"))            //35.	上传高支模基本信息
                                {
                                    realapi = "/highFormworkDeviceInfo";
                                }
                                else if (api.Equals("DeviceInfo/UploadFenceInfo"))           //38.	上传临边防护基本信息
                                {
                                    realapi = "/edgeDeviceInfo";
                                }
                                else if (api.Equals("AlarmInfo/FenceAlarmOn"))           //39.	上传临边防护预警信息
                                {
                                    realapi = "/edgeDeviceWarning";
                                    jso.Add("currentNum", "缺失");
                                    jso.Add("threshold", "缺失");
                                    jso.Add("warningContent", jso.GetValue("description").ToString());
                                    ///
                                    jso.Add("warningTime", Convert.ToDateTime(jso.GetValue("time")).ToString("yyyy-MM-dd HH:mm:ss"));
                                    jso.Remove("recordNumber");
                                    jso.Remove("belongedTo");
                                    jso.Remove("alarmId");
                                    jso.Remove("alarmType");
                                    jso.Remove("alarmLevel");
                                    jso.Remove("description");
                                    jso.Remove("time");
                                }
                                else if (api.Equals("AlarmInfo/FenceAlarmOff"))      ////39.	上传临边防护预警信息
                                {
                                    realapi = "/edgeDeviceWarning";
                                    jso.Add("currentNum", "恢复");
                                    jso.Add("threshold", "恢复");
                                    jso.Add("warningContent", jso.GetValue("description").ToString());
                                    ///
                                    jso.Add("warningTime", Convert.ToDateTime(jso.GetValue("time")).ToString("yyyy-MM-dd HH:mm:ss"));
                                    jso.Remove("recordNumber");
                                    jso.Remove("belongedTo");
                                    jso.Remove("alarmId");
                                    jso.Remove("alarmType");
                                    jso.Remove("alarmLevel");
                                    jso.Remove("description");
                                    jso.Remove("time");
                                }
                                else if (api.Contains("boardbaseurl"))
                                {
                                    if (jso.GetValue("boardtype").ToString() == "dustinfo")       //17. 上传扬尘信息看板
                                    {
                                        realapi = "/dustDevicePanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "crane")     //23. 上传塔吊信息看板
                                    {
                                        realapi = "/towerDevicePanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString().Equals("stereotactic"))          //3.上传浏览项目当前立体定位信息看板
                                    {
                                        realapi = "/projectPanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "hoist")       //27. 上传升降机信息看板
                                    {
                                        realapi = "/liftDevicePanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "people")      //43. 上传浏览人员指定信息看板
                                    {
                                        realapi = "/personBasicPanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "site")        //45. 上传智慧监管整体信息看板
                                    {
                                        realapi = "/wisdomWatchPanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "peoples")     //42. 上传浏览人员信息看板
                                    {
                                        realapi = "/personPanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "depppit")     //34. 上传深基坑信息看板
                                    {
                                        realapi = "/deppPitDevicePanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "upload")        //31. 上传卸料平台信息看板
                                    {
                                        realapi = "/unloadingDevicePanel";
                                    }
                                    else if (jso.GetValue("boardtype").ToString() == "highformwork")      //37. 上传高支模信息看板
                                    {
                                        realapi = "/highFormworkDevicePanel";
                                    }
                                    jso.Remove("boardtype");
                                }
                                else if (api.Contains("AlarmInfo/ElevatorAlarmOn"))      //升降机预警
                                {
                                    realapi = "/liftDeviceWarning";
                                    jso.Add("currentNum", "0");
                                    jso.Add("threshold", "0");
                                    jso.Add("warningContent", jso.GetValue("description").ToString());
                                    ///
                                    jso.Add("warningTime", Convert.ToDateTime(jso.GetValue("time")).ToString("yyyy-MM-dd HH:mm:ss"));
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
                            catch (HttpRequestException ex)
                            {
                                _logger.LogError(api + ":" + ex.Message);
                                return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(api + ":" + ex.Message);
                            }

                        }
                    }
                }
            }
            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
