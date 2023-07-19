using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;
using Microsoft.Extensions.Configuration;
using XHS.Build.Common;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadWuxiRisk : JobBase, IJob
    {
        private readonly IHpSystemSetting _hpSystemSetting;

        private readonly ILogger<SmartUpLoadWuxiRisk> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly XinheshengToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadWuxiRisk(ILogger<SmartUpLoadWuxiRisk> logger, IOperateLogService operateLogService, XinheshengToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService,IHpSystemSetting hpSystemSetting)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
            _hpSystemSetting = hpSystemSetting;
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
            DataSet ds = new DataSet();
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string recordNumberstr = "";
            string siteuploadurl = _hpSystemSetting.getSettingValue(Const.Setting.S190);
            string recordNumber = _hpSystemSetting.getSettingValue(Const.Setting.S191);

            for (int i = 0; i < recordNumber.Split(',').Length; i++)
            {
                if (!string.IsNullOrEmpty(recordNumber.Split(',')[i]))
                {
                    recordNumberstr += "recordNumber='" + recordNumber.Split(',')[i] + "' or ";
                }
            }
            if (recordNumberstr.Length >= 3)
            {
                recordNumberstr = recordNumberstr.Substring(0, recordNumberstr.Length - 3);
                //将数据上传时间修改为半年前
                await _aqtUploadService.UpdateMonthDate(-6, siteuploadurl);
                //对接数据获取
                ds = await _aqtUploadService.GetListForXinhesheng();
                await _aqtUploadService.UpdateMonthDate(6, siteuploadurl);
            }
            else
            {
                //对接数据获取
                ds = await _aqtUploadService.GetListForXinhesheng();
            }
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        var dtdata = dt.Select(recordNumberstr);
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
                                string realapi = "";
                                //api + "?appkey=" + account + "&appsecret=" + pwd;
                                string url = jso.GetValue("siteuploadurl").ToString();
                                string account = jso.GetValue("uploadaccount").ToString();
                                string pwd = jso.GetValue("uploadpwd").ToString();
                                string api = jso.GetValue("post").ToString();
                                CityUploadOperateLog LogEntity = new CityUploadOperateLog();
                                string result = "";

                                if (api.Contains("AddDeviceFacture"))
                                {
                                    realapi = "api/Device/AddDeviceFacture";
                                    if (jso.GetValue("deviceType").ToString() == "扬尘监测")
                                    {
                                        jso["deviceType"] = 6;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "塔吊监控")
                                    {
                                        jso["deviceType"] = 3;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "施工升降机")
                                    {
                                        jso["deviceType"] = 4;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "远程监控")
                                    {
                                        jso["deviceType"] = 2;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "卸料平台")
                                    {
                                        jso["deviceType"] = 5;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "人员定位")
                                    {
                                        jso["deviceType"] = 1;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "临边防护")
                                    {
                                        jso["deviceType"] = 7;
                                    }

                                }//5.2	设备信息上传接口
                                else if (api.Contains("AddSite"))
                                {
                                    realapi = "api/Site/AddSite";
                                    string wfjsstr = jso.GetValue("wfjs").ToString();
                                    foreach (string wfjs in wfjsstr.Split('|'))
                                    {
                                        string[] stringArr = wfjs.Split(',');

                                        if (stringArr[0].Equals("1"))
                                        {
                                            jso.Add("constructorcode", stringArr[1].ToString());
                                            jso.Add("constructorname", stringArr[2]);
                                            jso.Add("constructorcontact", stringArr[3]);
                                            jso.Add("constructortel", stringArr[4]);
                                        }
                                        else if (stringArr[0].Equals("2"))
                                        {
                                            jso.Add("buildercode", stringArr[1]);
                                            jso.Add("buildername", stringArr[2]);
                                            jso.Add("buildercontact", stringArr[3]);
                                            jso.Add("buildertel", stringArr[4]);
                                        }
                                        else if (stringArr[0].Equals("3"))
                                        {
                                            jso.Add("supervisorcode", stringArr[1]);
                                            jso.Add("supervisorname", stringArr[2]);
                                            jso.Add("supervisorcontact", stringArr[3]);
                                            jso.Add("supervisortel", stringArr[4]);
                                        }
                                        else if (stringArr[0].Equals("4"))
                                        {
                                            jso.Add("surveycode", stringArr[1]);
                                            jso.Add("surveyname", stringArr[2]);
                                            jso.Add("surveycontact", stringArr[3]);
                                            jso.Add("surveytel", stringArr[4]);
                                        }
                                        else if (stringArr[0].Equals("5"))
                                        {
                                            jso.Add("designcode", stringArr[1]);
                                            jso.Add("designname", stringArr[2]);
                                            jso.Add("designcontact", stringArr[3]);
                                            jso.Add("designtel", stringArr[4]);
                                        }
                                    }
                                }//5.1 项目信息上传接口
                                else if (api.Contains("DeleteDeviceFacture"))
                                {
                                    realapi = "api/Device/DeleteDeviceFacture";
                                    if (jso.GetValue("deviceType").ToString() == "扬尘监测")
                                    {
                                        jso["deviceType"] = 6;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "塔吊监控")
                                    {
                                        jso["deviceType"] = 3;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "实名制系统")
                                    {
                                        jso["deviceType"] = 1;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "远程监控")
                                    {
                                        jso["deviceType"] = 0;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "卸料平台")
                                    {
                                        jso["deviceType"] = 5;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "施工升降机")
                                    {

                                        jso["deviceType"] = 4;
                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "现场安全隐患")
                                    {
                                        jso["deviceType"] = 0;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "人员定位")
                                    {
                                        jso["deviceType"] = 0;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "高支模")
                                    {
                                        jso["deviceType"] = 9;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "深基坑监测")
                                    {
                                        jso["deviceType"] = 8;

                                    }
                                    else if (jso.GetValue("deviceType").ToString() == "临边防护")
                                    {
                                        jso["deviceType"] = 7;

                                    }

                                }//5.3	删除设备信息接口
                                else if (api.Contains("UploadDustInfo"))
                                {
                                    realapi = "api/Device/SetRtdData";
                                    jso.Add("lat", 0);
                                    jso.Add("lng", 0);
                                    jso["direction"] = jso.GetValue("direction").ToInt();
                                    jso["atmos"] = jso.GetValue("atmos").ToInt();
                                    jso.Add("devicecode", jso.GetValue("deviceId").ToString());
                                    jso.Add("tsp", jso.GetValue("rtTSP").ToDouble());
                                    jso.Add("dampness", jso.GetValue("humidity").ToDouble());
                                    jso.Add("speed", jso.GetValue("windSpeed").ToDouble());
                                    jso["temperature"] = jso.GetValue("temperature").ToDouble();
                                    jso.Add("pm2_5", jso.GetValue("pm2dot5").ToDouble());
                                    jso["noise"] = jso.GetValue("noise").ToDouble();
                                    jso["pm10"] = jso.GetValue("pm10").ToDouble();
                                    jso.Remove("deviceId");
                                    jso.Remove("recordNumber");
                                    jso.Remove("belongedTo");
                                    jso.Remove("humidity");
                                    jso.Remove("windSpeed");
                                    jso.Remove("windDirection");
                                    jso.Remove("pm2dot5");
                                    jso.Remove("upload");
                                    jso.Remove("rtTSP");
                                    jso.Remove("windDirectionvalue");
                                }//6.1 扬尘实时数据上传
                                else if (api.Contains("UploadCraneHistory"))
                                {
                                    try
                                    {
                                        //7.4 参数数据
                                        realapi = "api/TowerCrane/ParamsData";
                                        JObject parjob = JObject.Parse(jso.GetValue("paramjson").ToString());
                                        parjob.Add("SeCode", jso.GetValue("deviceId").ToString());
                                        result = _cityToken.JsonRequest(url, account, pwd, realapi, JsonConvert.SerializeObject(parjob));
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            //Id=Guid.NewGuid().ToString(),
                                            url = url,
                                            api = api,
                                            account = account,
                                            param = parjob.ToString(),
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
                                            bool code = (bool)mJObj.GetValue("success");
                                            if (code == true)
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

                                        //  _logger.LogError(api + ":" + ex.Message, true);
                                    }

                                    //7.3 实时数据
                                    realapi = "api/TowerCrane/RealData";
                                    jso.Add("SeCode", jso.GetValue("deviceId").ToString());
                                    JObject job = JObject.Parse(WebUtility.HtmlDecode(jso.GetValue("sedata").ToString()));
                                    jso.Add("Height", job.GetValue("Height").ToDouble());
                                    jso.Add("Margin", job.GetValue("Margin").ToDouble());
                                    jso.Add("Weight", job.GetValue("Weight").ToDouble());
                                    jso.Add("Rotation", job.GetValue("Rotation").ToDouble());
                                    jso.Add("Moment", job.GetValue("Moment").ToDouble());
                                    jso.Add("MomentPercent", job.GetValue("MomentPercent").ToDouble());
                                    jso.Add("Wind", job.GetValue("Wind").ToInt());
                                    jso.Add("UpdateTime", job.GetValue("UpdateTime").ToDateTime());
                                    jso.Add("SafeLoad", job.GetValue("SafeLoad").ToDouble());
                                    jso.Add("Alarm", job.GetValue("Alarm").ToInt());
                                    jso.Add("HasReport", job.GetValue("HasReport").ToInt());
                                    jso.Add("DriverId", job.GetValue("DriverId").ToString());
                                    jso.Add("DriverCardNo", job.GetValue("DriverCardNo").ToString());
                                    jso.Add("DriverName", job.GetValue("DriverName").ToString());
                                    jso.Add("WindSpeed", job.GetValue("WindSpeed").ToDouble());
                                    jso.Remove("sedata");
                                }//塔吊 7.3 实时数据  7.4 参数数据
                                else if (api.Contains("UploadVideo"))
                                {
                                    if (jso.GetValue("bonline").ToInt() == 1)
                                    {
                                        realapi = "api/Device/VideoOnline?deviceId=" + jso.GetValue("videoId").ToInt();
                                        result = _cityToken.JsonRequest(url, account, pwd, realapi, JsonConvert.SerializeObject(jso));
                                    }//在线
                                    else if (jso.GetValue("bonline").ToInt() == 0)
                                    {
                                        realapi = "api/Device/VideoOffline?deviceId=" + jso.GetValue("videoId").ToInt();
                                        jso.Add("deviceId", jso.GetValue("videoId").ToInt());

                                    }//下线
                                }//6.2	视频设备上线 //6.3	视频设备下线
                                else if (api.Contains("CraneAlarmOn"))
                                {
                                    realapi = "api/TowerCrane/TipOverData";
                                    string paramstr = jso.GetValue("paramjson").ToString();
                                    if (!string.IsNullOrEmpty(paramstr))
                                    {
                                        jso = JObject.Parse(paramstr);
                                    }
                                }//7.5 防倾翻报警诊断
                                else if (api.Contains("CraneBindPeopleInfo"))
                                {
                                    if (jso.GetValue("setype").ToInt() == 1)//塔吊
                                    {
                                        realapi = "api/TowerCrane/AuthData";
                                        jso.Add("SeCode", jso.GetValue("deviceId").ToString());
                                        jso.Add("IsOn", jso.GetValue("bindResult").ToInt());
                                        jso.Add("DriverCardNo", jso.GetValue("idcard").ToString());
                                        jso.Add("Image64", ImgHelper.ImageToBase64(jso.GetValue("path").ToString()));
                                        try
                                        {
                                            result = _cityToken.JsonRequest(url, account, pwd, realapi, JsonConvert.SerializeObject(jso));
                                            LogEntity = new CityUploadOperateLog
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
                                                errcount++;
                                                break;
                                            }
                                            else
                                            {
                                                JObject mJObj = JObject.Parse(result);
                                                bool code = (bool)mJObj.GetValue("success");
                                                if (code == true)
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

                                            //  _logger.LogError(api + ":" + ex.Message, true);
                                        }
                                    }
                                    else if (jso.GetValue("setype").ToInt() == 2)//升降机
                                    {
                                        realapi = "api/Elevator/AuthData";
                                        jso.Add("SeCode", jso.GetValue("deviceId").ToString());
                                        jso.Add("IsOn", jso.GetValue("bindResult").ToInt());
                                        jso.Add("DriverCardNo", jso.GetValue("idcard").ToString());
                                        jso.Add("Image64", ImgHelper.ImageToBase64(jso.GetValue("path").ToString()));
                                    }

                                }//7.7 人员上机下机刷脸认证
                                else if (api.Contains("CraneWorkData"))
                                {
                                    realapi = "api/TowerCrane/WorkData";
                                    JObject job = JObject.Parse(jso.GetValue("workdata").ToString());
                                    jso.Add("SeCode", jso.GetValue("secode").ToString());
                                    jso.Add("StartTime", job.GetValue("StartTime").ToDateTime());
                                    jso.Add("StartRotation", job.GetValue("StartRotation").ToDouble());
                                    jso.Add("StartMargin", job.GetValue("StartMargin").ToDouble());
                                    jso.Add("StartHeight", job.GetValue("StartHeight").ToDouble());
                                    jso.Add("EndTime", job.GetValue("EndTime").ToDateTime());
                                    jso.Add("EndRotation", job.GetValue("EndRotation").ToDouble());
                                    jso.Add("EndMargin", job.GetValue("EndMargin").ToDouble());
                                    jso.Add("EndHeight", job.GetValue("EndHeight").ToDouble());
                                    jso.Add("MaxWeight", job.GetValue("MaxWeight").ToDouble());
                                    jso.Add("MaxMargin", job.GetValue("MaxMargin").ToDouble());
                                    jso.Add("DriverCardNo", job.GetValue("DriverCardNo").ToString());
                                    jso.Add("DriverName", job.GetValue("DriverName").ToString());
                                    jso.Remove("workdata");
                                }//7.8	工作循环数据
                                else if (api.Contains("UploadMachineryInfos"))
                                {
                                    if (jso.GetValue("setype").ToInt() == 1)//塔吊
                                    {
                                        if (jso.GetValue("sestatus").ToInt() == 0)//离线
                                        {
                                            realapi = "api/TowerCrane/Offline?secode=" + jso.GetValue("propertyRightsRecordNo").ToInt();
                                        }
                                        else if (jso.GetValue("sestatus").ToInt() == 1)//在线
                                        {
                                            realapi = "api/TowerCrane/Online?secode=" + jso.GetValue("propertyRightsRecordNo").ToInt();
                                        }
                                    }
                                    else if (jso.GetValue("setype").ToInt() == 0)//升降机
                                    {
                                        if (jso.GetValue("sestatus").ToInt() == 0)//离线
                                        {
                                            realapi = "api/Elevator/Offline?secode=" + jso.GetValue("propertyRightsRecordNo").ToInt();
                                        }
                                        else if (jso.GetValue("sestatus").ToInt() == 1)//在线
                                        {
                                            realapi = "api/Elevator/Online?secode=" + jso.GetValue("propertyRightsRecordNo").ToInt();
                                        }
                                    }

                                }//特种设备上下线
                                else if (api.Contains("HoistHistory"))
                                {

                                    try
                                    {
                                        realapi = "api/Elevator/ParamsData";
                                        JObject jsonjob = JObject.Parse(jso.GetValue("paramjson").ToString());
                                        result = _cityToken.JsonRequest(url, account, pwd, realapi, JsonConvert.SerializeObject(jsonjob));
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            //Id=Guid.NewGuid().ToString(),
                                            url = url,
                                            api = api,
                                            account = account,
                                            param = jsonjob.ToString(),
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
                                            bool code = (bool)mJObj.GetValue("success");
                                            if (code == true)
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

                                        //  _logger.LogError(api + ":" + ex.Message, true);
                                    }
                                    realapi = "api/Elevator/RealData";
                                    string datastr = jso.GetValue("sedata").ToString();
                                    JObject job = new JObject();
                                    if (!string.IsNullOrEmpty(datastr))
                                    {
                                        job = JObject.Parse(HttpUtility.HtmlDecode(datastr));
                                    }
                                    jso.Add("SeCode", jso.GetValue("deviceId").ToString());
                                    jso.Add("Height", job.GetValue("Height").ToInt());
                                    jso.Add("Weight", job.GetValue("Weight").ToInt());
                                    jso.Add("AlarmState", job.GetValue("alarmState").ToInt());
                                    jso.Add("NumOfPeople", job.GetValue("NumOfPeople").ToInt());
                                    jso.Add("DeviceTime", job.GetValue("DeviceTime").ToString());
                                    jso.Add("HasReport", job.GetValue("HasReport").ToInt());
                                    jso.Add("Speed", job.GetValue("Speed").ToInt());
                                    jso.Add("Floor", job.GetValue("Floor").ToInt());
                                    jso.Add("DriverId", job.GetValue("DriverId").ToString());
                                    jso.Add("DriverCardNo", job.GetValue("DriverCardNo").ToString());
                                    jso.Add("DriverName", job.GetValue("DriverName").ToString());
                                    jso.Remove("sedata");
                                    jso.Remove("recordNumber");
                                    jso.Remove("deviceId");
                                    jso.Remove("model");
                                    jso.Remove("name");
                                    jso.Remove("moniterTime");
                                    jso.Remove("projectInfoId");
                                }//升降机实时数据
                                else if (api.Contains("UploadHistory"))
                                {
                                    realapi = "api/Unload/RealData";
                                    JObject jsonobj = JObject.Parse(jso.GetValue("rtdjson").ToString());
                                    jso = jsonobj;
                                    jso.Remove("Id");
                                }//卸料平台实时数据上传
                                else if (api.Contains("InspectContentInfo"))
                                {
                                    realapi = "api/Inspection/InspectContentInfo";
                                    if (jso.ContainsKey("checkContent"))
                                    {
                                        string rectifyPerson = "";
                                        JObject job = new JObject();
                                        JArray jar = new JArray();
                                        JArray jarrayObj = new JArray();
                                        string checkContent = jso.GetValue("checkContent").ToString();
                                        JArray arr = JsonConvert.DeserializeObject<JArray>(checkContent);
                                        if (jso.ContainsKey("urls"))
                                        {
                                            string urls = jso.GetValue("urls").ToString();
                                            if (string.IsNullOrEmpty(urls) || urls == "[]")
                                            {
                                                jarrayObj.Add("无");
                                            }
                                            else
                                            {
                                                string[] urlarray = urls.Split(",".ToCharArray());
                                                for (int k = 0; k < urlarray.Length; k++)
                                                {
                                                    jarrayObj.Add(urlarray[k]);
                                                }
                                            }
                                        }
                                        if (jso.ContainsKey("rectifyPerson"))
                                        {
                                            rectifyPerson = jso.GetValue("rectifyPerson").ToString();
                                            if (string.IsNullOrEmpty(rectifyPerson) || rectifyPerson == "[]")
                                            {
                                                rectifyPerson = "无";
                                            }
                                        }
                                        if (string.IsNullOrEmpty(checkContent) || checkContent == "[]")
                                        {
                                            job.Add("itemId", 1);
                                            job.Add("checkContent", "无");
                                            job.Add("rectifyPerson", rectifyPerson);
                                            jar.Add(job);
                                        }
                                        else
                                        {
                                            for (int b = 0; b < arr.Count; b++)
                                            {
                                                job = new JObject();
                                                job.Add("itemId", b + 1);
                                                job.Add("rectifyPerson", rectifyPerson);
                                                var res = arr[b];
                                                job.Add("checkContent", res.Last.ToString());
                                                job.Add("urls", jarrayObj);
                                                jar.Add(job);
                                            }
                                        }
                                        jso.Add("checkLists", jar);
                                        jso.Add("idcard", jso.GetValue("idCard").ToString());
                                    }
                                }//10.1	检查单数据上传接口
                                else if (api.Contains("clwcx"))
                                {
                                    realapi = "api/CarWash/Wash";
                                    if (jso.GetValue("alarmType").ToInt() == 5)
                                    {
                                        jso.Add("washresult", 1);
                                    }
                                    else
                                    {
                                        jso.Add("washresult", 2);
                                    }
                                    jso.Add("cwcode", jso.GetValue("deviceNo".ToString()));
                                    jso.Add("outtime", jso.GetValue("leaveTime").ToString());
                                    jso.Add("carno", jso.GetValue("ztcCph").ToString());
                                    jso.Add("platecolor", jso.GetValue("ztcColor"));
                                    jso["cwminute"] = jso.GetValue("cwminute").ToInt();
                                    jso.Add("imgpath", jso.GetValue("photoUrl").ToString());
                                    jso.Add("videopath", jso.GetValue("leaveVedioUrl").ToString());

                                }//11.2 车辆冲洗
                                else if (api.Contains("RectifyContentInfo"))
                                {
                                    realapi = "api/Inspection/RectifyContentInfo";
                                    JArray jarrayObj = new JArray();
                                    JObject job = new JObject();
                                    JArray jar = new JArray();
                                    if (jso.ContainsKey("urls"))
                                    {
                                        string urls = jso.GetValue("urls").ToString();
                                        if (string.IsNullOrEmpty(urls) || urls == "[]")
                                        {
                                            jarrayObj.Add("无");
                                        }
                                        else
                                        {
                                            string[] urlarray = urls.Split(",".ToCharArray());
                                            for (int k = 0; k < urlarray.Length; k++)
                                            {
                                                jarrayObj.Add(urlarray[k]);
                                            }
                                        }
                                        if (jso.ContainsKey("rectifyRemark"))
                                        {
                                            string rectifyRemark = jso.GetValue("rectifyRemark").ToString();
                                            job.Add("itemId", 1);
                                            job.Add("rectifyRemark", rectifyRemark);
                                            job.Add("urls", jarrayObj);
                                            jar.Add(job);
                                        }
                                    }
                                    jso.Remove("rectifyContents");
                                    jso.Add("rectifyContents", jar);
                                }//10.2 检查单整改完成数据上传
                                else if (api.Equals("Check/InspectionPoint"))
                                {
                                    realapi = "api/Inspection/InspectionPoint";
                                }//10.3 巡检点数据上传
                                else if (api.Contains("InspectionPointContent"))
                                {
                                    realapi = "api/Inspection/InspectionPointContent";
                                    if (jso.ContainsKey("urls"))
                                    {
                                        JArray jarrayObj = new JArray();
                                        string urls = jso.GetValue("urls").ToString();
                                        if (string.IsNullOrEmpty(urls) || urls == "[]")
                                        {
                                            jarrayObj.Add("无");
                                        }
                                        else
                                        {
                                            string[] urlarray = urls.Replace("[", "").Replace("]", "").Split(",".ToCharArray());
                                            for (int k = 0; k < urlarray.Length; k++)
                                            {
                                                jarrayObj.Add(urlarray[k]);
                                            }
                                        }
                                        jso.Remove("urls");
                                        jso.Add("urls", jarrayObj);
                                    }
                                }//10.4 巡检内容数据上传
                                else if (api.Equals("Check/FreeToShoot"))
                                {
                                    realapi = "api/Inspection/FreeToShoot";
                                    //随手拍数据上传
                                    if (jso.ContainsKey("url"))
                                    {
                                        string urls = jso.GetValue("url").ToString();
                                        JArray jarrayObj = new JArray();
                                        if (string.IsNullOrEmpty(urls) || urls == "[]")
                                        {
                                            jarrayObj.Add("无");
                                        }
                                        else
                                        {
                                            string[] urlarray = urls.Split(";".ToCharArray());
                                            for (int k = 0; k < urlarray.Length; k++)
                                            {
                                                string[] imagestr = urlarray[k].Split(",".ToCharArray());
                                                jarrayObj.Add(imagestr[0]);
                                            }
                                        }
                                        jso.Add("urls", jarrayObj);
                                    }
                                }//10.5 随手拍数据上传
                                else if (api.Contains("FreeToShootRectify"))
                                {
                                    realapi = "api/Inspection/FreeToShootRectify";
                                    if (jso.ContainsKey("url"))
                                    {
                                        string urls = jso.GetValue("url").ToString();
                                        JArray jarrayObj = new JArray();
                                        if (string.IsNullOrEmpty(urls) || urls == "[]")
                                        {
                                            jarrayObj.Add("无");
                                        }
                                        else
                                        {
                                            string[] urlarray = urls.Split(";".ToCharArray());
                                            for (int k = 0; k < urlarray.Length; k++)
                                            {
                                                string[] imagestr = urlarray[k].Split(",".ToCharArray());
                                                jarrayObj.Add(imagestr[0]);
                                            }
                                        }
                                        jso.Add("urls", jarrayObj);
                                    }
                                }//10.6 随手拍完成数据上传
                                else if (api.Contains("Helmet"))
                                {
                                    realapi = "api/Warning/Helmet";
                                    string parstr = jso.GetValue("jsonall").ToString();
                                    if (!string.IsNullOrEmpty(parstr))
                                    {
                                        jso = JObject.Parse(parstr);
                                    }
                                }//11.1	安全帽未佩戴识别数据上传
                                else if (api.Contains("soil"))
                                {
                                    realapi = "api/AI/soil";
                                }//裸土覆盖
                                else if (api.Contains("smoke"))
                                {
                                    realapi = "api/Warning/smoke";
                                    string parstr = jso.GetValue("jsonall").ToString();
                                    if (!string.IsNullOrEmpty(parstr))
                                    {
                                        jso = JObject.Parse(parstr);
                                    }
                                }//1.2	烟雾识别
                                else if (api.Contains("illegalcar"))
                                {
                                    realapi = "api/AI/illegalcar";

                                }//1.3	非法车辆进入
                                else if (api.Contains("airtight"))
                                {
                                    realapi = "api/AI/airtight";
                                }//1.4	密闭运输


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

                                result = _cityToken.JsonRequest(url, account, pwd, realapi, JsonConvert.SerializeObject(jso));
                                LogEntity = new CityUploadOperateLog
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
                                    errcount++;
                                    break;
                                }
                                else
                                {
                                    JObject mJObj = JObject.Parse(result);
                                    bool code = (bool)mJObj.GetValue("success");
                                    if (code == true)
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

                                //  _logger.LogError(api + ":" + ex.Message, true);
                            }

                        }
                    }
                }
            }

            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
