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
using XHS.Build.Common.Util;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadYiZhengJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadYiZhengJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly XHSRealnameToken _jwtToken;

        public SmartUpLoadYiZhengJob(ILogger<SmartUpLoadYiZhengJob> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService, IHpSystemSetting hpSystemSetting, XHSRealnameToken jwtToken)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _hpSystemSetting = hpSystemSetting;
            _jwtToken = jwtToken;
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
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int successcount = 0;
            int errcount = 0;
            var LogEntity = new CityUploadOperateLog();
            string uploaurl = string.Empty;
            string url = "http://49.4.11.116:8085/api/";//jso.GetValue("siteuploadurl").ToString();
            string urlj = "http://49.4.68.132:8094/api/";
            string bordurl = "http://124.70.9.139:8001/Board/insertBoard";
            #region
            //实名制url
            string realnameurl = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            if (!string.IsNullOrEmpty(realnameurl))
            {
                //对接数据获取
                DataTable siteajcodesdt = await _aqtUploadService.GetGroupSiteajcodes();
                if (siteajcodesdt.Rows.Count > 0)
                {
                    for (int j = 0; j < siteajcodesdt.Rows.Count; j++)
                    {
                        DataRow dr = siteajcodesdt.Rows[j];
                        JObject jso = new JObject();
                        foreach (DataColumn column in dr.Table.Columns)
                        {
                            if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                            }
                            else
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                            }
                        }
                        
                        string uploadurl = jso.GetValue("uploadurl").ToString();
                        string uploadaccount = jso.GetValue("uploadaccount").ToString();
                        string uploadpwd = jso.GetValue("uploadpwd").ToString();
                        string attenduserpsd = jso.GetValue("attenduserpsd").ToString();
                        string recordNumber = jso.GetValue("siteajcodes").ToString();
                        string belongto = jso.GetValue("belongto").ToString();
                        if (!uploadurl.Equals("http://49.4.11.116:8085/"))
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(attenduserpsd))
                        {
                            break;
                        }
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[2];
                        JObject jsoparam = new JObject();
                        jsoparam.Add("recordNumber", recordNumber);
                        //获取指定项目的人员基础信息
                        string uploadapi = "Personal/PeopleInOutProInfo";
                        string api = "construction-site-api/ordinary-employee-info";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl, currentDate);
                            }
                        }
                        string result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                JObject job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());
                                        datajob.Add("importOrExitFlag", datajob.GetValue("isResign").ToInt());
                                        datajob.Add("personName", datajob.GetValue("workerName"));
                                        datajob.Add("idcard", datajob.GetValue("idNumber"));
                                        datajob.Add("typeWork", datajob.GetValue("workType").ToInt());
                                        datajob.Add("importTime", datajob.GetValue("entryDate"));
                                        datajob.Add("exitTime", datajob.GetValue("exitDate"));
                                        datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode"));
                                        datajob.Add("userinfoid", datajob.GetValue("idNumber"));
                                        datajob.Add("job", datajob.GetValue("securityJob"));
                                        datajob.Remove("unifiedSocialCreditCode");
                                        datajob.Remove("workerName");
                                        datajob.Remove("idNumber");
                                        datajob.Remove("workType");
                                        datajob.Remove("entryDate");
                                        datajob.Remove("exitDate");
                                        datajob.Remove("isResign");
                                        datajob.Remove("securityJob");
                                        string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = uploadurl,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount += errcount;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                 _logger.LogError(uploadapi + ":" + ex.Message);
                                 return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }
                        //10.2、人员实时考勤数据上传
                        uploadapi = "Personal/ManagerInOutSite";
                        api = "construction-site-api/manager-in-out-site";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl, currentDate);
                            }
                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                JObject job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());
                                        datajob.Add("userinfoid", datajob.GetValue("idcard"));
                                        datajob.Add("belongedTo", belongto);
                                        string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = uploadurl,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount += errcount;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                 _logger.LogError(uploadapi + ":" + ex.Message);
                                 return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }
                        //10.3、教育信息上传
                        uploadapi = "Personal/PeopleSafeEduInfo";
                        api = "construction-site-api/employee-education-info";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl, currentDate);
                            }
                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                JObject job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());
                                        datajob.Add("idcard", datajob.GetValue("workerIdNumber"));
                                        datajob.Add("typeWork", datajob.GetValue("workType").ToInt());
                                        datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode"));
                                        datajob.Add("educatType", datajob.GetValue("educationType"));
                                        datajob.Add("educatContent", datajob.GetValue("educationContent"));
                                        datajob.Add("educatDate", datajob.GetValue("educationDate"));
                                        datajob.Add("educatAddress", "本工地" /*datajob.GetValue("educationLocation")*/);
                                        datajob.Add("educationTime", datajob.GetValue("educationDuration"));
                                        datajob.Add("userinfoid", datajob.GetValue("workerIdNumber"));
                                        string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = uploadurl,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount += errcount;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                 _logger.LogError(uploadapi + ":" + ex.Message);
                                 return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }
                        //10.4、奖惩信息上传
                        uploadapi = "Personal/PeopleRewardPunishInfo";
                        api = "construction-site-api/employee-reward-punishments-info";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl, currentDate);
                            }
                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                JObject job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());
                                        datajob.Add("idcard", datajob.GetValue("workerIdNumber"));
                                        datajob.Add("typeWork", datajob.GetValue("workType").ToInt());
                                        datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode"));
                                        datajob.Add("rewardType", datajob.GetValue("eventType"));
                                        datajob.Add("rewardContent", datajob.GetValue("eventContent"));
                                        datajob.Add("rewardDate", datajob.GetValue("eventDate"));
                                        datajob.Add("userinfoid", datajob.GetValue("workerIdNumber"));
                                        string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = uploadurl,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount += errcount;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                 _logger.LogError(uploadapi + ":" + ex.Message);
                                 return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }
                    }
                }
            }
            #endregion

            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForYiZheng();
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
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            string api = jso.GetValue("post").ToString();
                            string realapi = api;
                            if (api.Contains("UploadCraneHistory"))      //卸料平台实时数据上传
                            {
                                realapi = "Craneinterface/UploadCraneHistory";
                                JObject job = JObject.Parse(jso.GetValue("sedata").ToString());
                                jso.Add("load", job.GetValue("Weight").ToDouble());
                                jso.Add("range", job.GetValue("Margin").ToDouble());
                                jso.Add("moment", job.GetValue("MomentPercent").ToDouble());
                                jso.Add("rotation", job.GetValue("Rotation").ToDouble());
                                jso.Add("height", job.GetValue("Height").ToDouble());
                                jso.Add("windSpeed", job.GetValue("WindSpeed").ToDouble());
                                //  jso.Add("dip", job.GetValue("").ToDouble());
                                // jso.Add("multiplyingPower", job.GetValue("").ToDouble());
                                jso.Remove("sedata");
                                jso.Remove("paramjson");
                            }//塔机实时
                            else if (api.Contains("CraneAlarmOn"))
                            {
                                realapi = "Craneinterface/CraneAlarmInfo";
                            }//塔吊报警
                            else if (api.Contains("HoistHistory"))
                            {
                                realapi = "Hoistinterface/HoistHistory";
                                JObject job = JObject.Parse(jso.GetValue("sedata").ToString());
                                jso.Add("load", job.GetValue("Weight").ToString());
                                jso.Add("avgSpeed", job.GetValue("Speed").ToDouble());
                                jso.Add("numberOfPeopleLoaded", jso.GetValue("NumOfPeople").ToInt());
                                jso.Remove("sedata");

                            }//升降机
                            else if (api.Contains("UploadHistory"))
                            {
                                realapi = "Uploadinterface/UploadHistory";
                                JObject job = JObject.Parse(jso.GetValue("rtdjson").ToString());
                                jso.Add("powerPercent", job.GetValue("electric_quantity").ToDouble());
                                jso.Add("weight", job.GetValue("weight").ToDouble());
                                jso.Add("weightBias", job.GetValue("bias").ToDouble());
                                jso.Add("warningWeight", job.GetValue("early_warning_weight").ToDouble());
                                jso.Add("AlarmWeight", job.GetValue("alarm_weight").ToDouble());
                                jso.Add("dataType", job.GetValue("upstate").ToInt());
                                jso.Remove("rtdjson");
                            }//卸料平台
                            else if (api.Contains("UploadDustInfo"))
                            {
                                realapi = "DustInterface/UploadDustHistory";
                            }//扬尘实时
                            else if (api.Contains("FenceAlarmOn"))
                            {
                                realapi = "Fenceinterface/FenceAlarmInfo";
                                jso.Add("warnNumber", jso.GetValue("deviceId"));
                                jso.Add("defectPosition", jso.GetValue("description"));
                                jso.Add("defectDate", jso.GetValue("time"));
                            }//临边防护缺失记录
                            else if (api.Contains("FenceAlarmOff"))
                            {
                                realapi = "Fenceinterface/FenceAlarmInfo";
                                jso.Add("warnNumber", jso.GetValue("deviceId"));
                                jso.Add("defectPosition", jso.GetValue("description"));
                                jso.Add("recoveryDate", jso.GetValue("time"));
                            }//临边防护恢复记录
                            else if (api.Contains("InspectContentInfo"))//检查单数据上传
                            {
                                realapi = "Check/InspectContentInfo";
                                if (jso.ContainsKey("checkContent"))
                                {
                                    string rectifyPerson = "";
                                    JArray jar = new JArray();
                                    JObject job = new JObject();
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
                                        job.Add("urls", jarrayObj);
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
                                jso.Remove("idCard");
                                jso.Remove("checkContent");
                                jso.Remove("urls");
                            }//检查单数据上传
                            else if (api.Equals("Check/InspectionPoint"))
                            {
                                realapi = "Check/InspectionPoint";
                            }//巡检点
                            else if (api.Contains("RectifyContentInfo"))
                            {
                                realapi = "Check/RectifyContentInfo";

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
                            }//检查单整改完成数据上传
                            else if (api.Contains("InspectionPointContent"))
                            {
                                realapi = "Check/InspectionPointContent";
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
                            }//巡检内容数据上传
                            else if (api.Contains("FreeToShoot"))
                            {
                                realapi = "Check/FreeToShoot";
                                string urls = jso.GetValue("url").ToString();

                                JObject job = new JObject();
                                JArray jar = new JArray();
                                foreach (var item in urls.Split(';'))
                                {
                                    job = new JObject();
                                    job.Add("type", item.Split(',')[1]);
                                    job.Add("url", item.Split(',')[0]);
                                    jar.Add(job);
                                }
                                jso.Add("urls", jar);
                            }//随手拍数据上传
                            else if (api.Contains("HighFormworkDeviceInfo"))
                            {
                                realapi = "HighFormwork/HighFormworkHistory";
                            }//高支模数据上传
                            else if (api.Contains("UploadVideo"))
                            {
                                realapi = "Video/UploadVideos";
                            }//视频地址上传接口
                            else if (api.Contains("SmartSupervisionBoard"))
                            {
                                realapi = "Board/insertBoard";
                                jso.Add("url", jso.GetValue("uploadBoardUrl"));
                                jso.Remove("uploadBoardUrl");
                                var data = jso;
                                jso = new JObject();
                                jso.Add("boardVo", data);
                                jso.Add("appKey", account);
                                jso.Add("secret", pwd);
                            }//数据看板上传
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
                            try
                            {
                                if (api.Contains("InspectContentInfo") || api.Contains("InspectionPoint") || api.Contains("RectifyContentInfo") || api.Contains("InspectionPointContent") || api.Contains("FreeToShoot") || api.Contains("UploadVideo"))
                                {
                                    uploaurl = urlj + realapi + "?appkey=" + account + "&secret=" + pwd;
                                }
                                else if (api.Contains("SmartSupervisionBoard"))
                                {
                                    uploaurl = bordurl;
                                }
                                else
                                {
                                    uploaurl = url + realapi + "?appkey=" + account + "&secret=" + pwd;
                                }
                                string result = UHttp.Post(uploaurl, JsonConvert.SerializeObject(jso), UHttp.CONTENT_TYPE_JSON);
                                LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = uploaurl,
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
                                    int code = (int)mJObj.GetValue("code");
                                    if (code == 0)
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate("http://49.4.11.116:8085/", api, now);
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




            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
