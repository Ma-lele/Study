using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class SmartUpLoadHourfuningJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourfuningJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityFuningToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly XHSRealnameToken _jwtToken;

        public SmartUpLoadHourfuningJob(ILogger<SmartUpLoadHourfuningJob> logger, IOperateLogService operateLogService, SiteCityFuningToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService, XHSRealnameToken jwtToken)
        {
            _logger = logger;  
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
            _hpSystemSetting = systemSettingService;
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
            int successcount = 0;
            int errcount = 0;
            JArray jar = new JArray();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
            string api = string.Empty;
            string result = "";
            string Token = "IBGcPGQieY3m8518D1XIeVn65Zpm11GffuPR7oy/YDlIw5Xtx0SeMw==";
            string url = "http://221.231.127.53:84/interface";


            JObject job = new JObject();



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
                        string uploadurl = jso.GetValue("siteuploadurl").ToString();
                        string uploadaccount = jso.GetValue("uploadaccount").ToString();
                        string uploadpwd = jso.GetValue("uploadpwd").ToString();
                        string attenduserpsd = jso.GetValue("attenduserpsd").ToString();
                        string recordNumber = jso.GetValue("siteajcodes").ToString();
                        //string token = _cityToken.getSiteCityToken(uploadurl, uploadaccount, uploadpwd);
                        //if (string.IsNullOrEmpty(token))
                        //{
                        //    _logger.LogInformation("数据上传结束（对方鉴权获取失败）。", true);
                        //    return;
                        //}
                        if (string.IsNullOrEmpty(attenduserpsd) || string.IsNullOrEmpty(uploadurl))
                        {
                            continue;
                        }
                        if (!uploadurl.Equals("http://49.4.68.132:8094/api/"))
                        {
                            continue;
                        }
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[2];
                        JObject jsoparam = new JObject();
                        jsoparam.Add("recordNumber", "AJ320923120210172,AJ320923120200050,AJ320923120210213,AJ320925120220005,AJ320981120210351,AJ320906120220001");
                     
                        //获取指定项目的人员基础信息
                        string uploadapi = "/yancheng/personal/peopleInOutProInfo";
                        string realapi = "/yancheng/personal/peopleInOutProInfo";
                        api = "construction-site-api/ordinary-employee-info";
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
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }

                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {

                            job = JObject.Parse(result);
                            if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                            {
                                JArray mJArray = new JArray();
                                JObject jobject = new JObject();
                                mJArray = (JArray)job.GetValue("data");
                                for (int k = 0; k < mJArray.Count; k++)
                                {
                                    try
                                    {
                                        jobject = (JObject)mJArray[k];
                                        jobject.Add("importOrExitFlag", jobject.GetValue("isResign"));
                                        jobject.Add("personName", jobject.GetValue("workerName"));
                                        jobject.Add("idcard", jobject.GetValue("idNumber"));
                                        jobject.Add("typeWork", jobject.GetValue("workType"));
                                        jobject.Add("importTime", jobject.GetValue("entryDate"));
                                        jobject.Add("exitTime", jobject.GetValue("exitDate"));
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode"));
                                        jobject.Add("userinfoid", jobject.GetValue("idNumber"));
                                        string areastr = "{\"11\":\"北京\",\"12\":\"天津\",\"13\":\"河北\",\"14\":\"山西\",\"15\":\"内蒙古\",\"21\":\"辽宁\",\"22\":\"吉林\",\"23\":\"黑龙江\",\"31\":\"上海\",\"32\":\"江苏\",\"33\":\"浙江\",\"34\":\"安徽\",\"35\":\"福建\",\"36\":\"江西\",\"37\":\"山东\",\"41\":\"河南\",\"42\":\"湖北\",\"43\":\"湖南\",\"44\":\"广东\",\"45\":\"广西\",\"46\":\"海南\",\"50\":\"重庆\",\"51\":\"四川\",\"52\":\"贵州\",\"53\":\"云南\",\"54\":\"西藏\",\"61\":\"陕西\",\"62\":\"甘肃\",\"63\":\"青海\",\"64\":\"宁夏\",\"65\":\"新疆\",\"71\":\"台湾\",\"81\":\"香港\",\"82\":\"澳门\",\"91\":\"国外\"}";
                                        JObject areajob = JObject.Parse(areastr);
                                        string id = jobject.GetValue("idNumber").ToString().Substring(0, 2);
                                        jobject["nativeplace"] = areajob.GetValue(id).ToString();
                                        jobject["importTime"] = Convert.ToDateTime(jobject.GetValue("importTime")).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                                        jobject["exitTime"] = Convert.ToDateTime(jobject.GetValue("exitTime")).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                                        //jobject.Remove("unifiedSocialCreditCode");
                                        //jobject.Remove("belongedTo");
                                        //jobject.Remove("workerName");
                                        //jobject.Remove("idNumber");
                                        //jobject.Remove("workType");
                                        //jobject.Remove("securityJob");
                                        //jobject.Remove("entryDate");
                                        //jobject.Remove("exitDate");
                                        //jobject.Remove("isResign");
                                        //jobject.Remove("updateDate");
                                        //jobject.Remove("userinfoid");
                                        //jobject.Remove("cerNo");
                                        //jobject.Remove("cerUrl");
                                        var data = jobject;
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                               // await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                         _logger.LogError(uploadapi + ":" + ex.Message);
                                    }
                                }

                            }


                        }

                        //教育信息上传
                        uploadapi = "/yancheng/personal/peopleSafeEduInfo";
                        realapi = "/yancheng/personal/peopleSafeEduInfo";
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
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject jobject = (JObject)mJArray[k];
                                        jobject.Add("idcard", jobject.GetValue("workerIdNumber").ToString());
                                        jobject.Add("typeWork", jobject.GetValue("workType"));
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode"));
                                        jobject.Add("educatType", jobject.GetValue("educationContent"));
                                        jobject.Add("educatContent", jobject.GetValue("educatType"));
                                        jobject.Add("educatDate", jobject.GetValue("educationDate"));
                                        jobject.Add("educationTime", jobject.GetValue("educationDuration"));
                                        jobject.Add("educatAddress", jobject.GetValue("educationLocation"));
                                        jobject["educatAddress"] = "本工地";
                                        var data = jobject;
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
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
                            catch (Exception ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }

                        //获取人员实时考勤数据上传
                        uploadapi = "/yancheng/personal/managerInOutSite";
                        realapi = "/yancheng/personal/managerInOutSite";
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
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject jobject = (JObject)mJArray[k];
                                        jobject.Add("userinfoid", jobject.GetValue("idcard").ToString());

                                        var data = jobject;
                                        if(data.GetValue("type").ToString()== "管理班组-1")
                                        {
                                            data["type"] = "安全人员";
                                        }
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                               // await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                 _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }



                        //奖惩信息上传
                        uploadapi = "/yancheng/personal/peopleRewardPunishInfo";
                        realapi = "/yancheng/personal/peopleRewardPunishInfo";
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
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject jobject = (JObject)mJArray[k];
                                        jobject.Add("idcard", jobject.GetValue("workerIdNumber"));
                                        jobject.Add("typeWork", jobject.GetValue("workType"));
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode"));
                                        jobject.Add("rewardType", jobject.GetValue("eventType"));
                                        jobject.Add("rewardContent", jobject.GetValue("eventContent"));
                                        jobject.Add("rewardDate", jobject.GetValue("eventDate"));
                                        var data = jobject;
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
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
                            catch (Exception ex)
                            {
                                 _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }
                    }
                }
            }





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
                                job = new JObject();

                                //看板地址上传
                                if (api.Contains("yancheng/boardbaseurl/upload"))
                                {
                                    realapi = "/yancheng/boardbaseurl/upload";
                                }
                                else if (api.Equals("Check/InspectContentInfo"))         //4.6.1 检查单数据上传
                                {
                                    realapi = "yancheng/check/inspectContentInfo";
                                    if (jso.ContainsKey("checkContent"))
                                    {
                                        string rectifyPerson = "";
                                        jar = new JArray();
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
                                }
                                else if (api.Contains("RectifyContentInfo"))         //4.6.2 检查单整改完成数据上传
                                {
                                    realapi = "yancheng/check/rectifyContentInfo";
                                    JArray jarrayObj = new JArray();
                                    jar = new JArray();
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
                                }
                                else if (api.Contains("InspectionPointContent"))            //巡检内容数据上传
                                {
                                    realapi = "yancheng/check/inspectionPointContent";
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
                                }
                                else if (api.Equals("Check/InspectionPoint"))            //巡检点数据上传
                                {
                                    jso["building"] = "5";
                                    realapi = "yancheng/check/inspectionPoint";
                                }
                                else if (api.Contains("FenceAlarmInfo1"))       //缺失记录上传
                                {
                                    realapi = "yancheng/fenceinterface/fenceAlarmInfo";
                                }
                                else if (api.Contains("FenceAlarmInfo2"))            //4.4.2 上传恢复记录
                                {
                                    realapi = "yancheng/fenceinterface/fenceAlarmInfohuifu";
                                }
                                else if (api.Contains("CraneBindPeopleInfo"))           //4.2.3 上传操作设备人员识别数据
                                {
                                    if (jso.GetValue("setype").ToString() == "1")  //塔吊
                                    {
                                        realapi = "/yancheng/towerCraneMonitor/craneBindPeopleInfo";
                                        var path = jso.GetValue("path").ToString();
                                        string base64Photo = ImgHelper.ImageToBase64(path);
                                        jso.Add("base64Photo", base64Photo);
                                    }
                                    else       //升降机
                                    {
                                        realapi = "/yancheng/hoistinterface/hoistBindPeopleInfo";
                                        var path = jso.GetValue("path").ToString();
                                        string base64Photo = ImgHelper.ImageToBase64(path);
                                        jso.Add("base64Photo", base64Photo);
                                    }

                                }
                                else if (api.Contains("CraneReleasePeopleInfo"))            //人机解绑信息上传
                                {
                                    if (jso.GetValue("setype").ToString() == "1")  //塔吊
                                    {
                                        realapi = "/yancheng/towerCraneMonitor/craneReleasePeopleInfo";
                                    }
                                    else       //升降机
                                    {
                                        realapi = "/yancheng/hoistinterface/hoistReleasePeopleInfo";
                                    }
                                    realapi = "/yancheng/towerCraneMonitor/craneReleasePeopleInfo";
                                }
                                else if (api.Contains("AlarmInfo/CraneAlarmOn"))            //塔机预警数据
                                {
                                    realapi = "/yancheng/towerCraneMonitor/craneAlarmInfo";
                                    jso["warnLevel"] = jso.GetValue("warnLevel").ToString() == "一般预警" ? "01" : jso.GetValue("warnLevel").ToString() == "严重警告" ? "02" : "03";
                                    jso.Add("eventId", jso.GetValue("WARNID").ToString());
                                }
                                else if (api.Contains("/yancheng/alarmdeal/upload"))        //预警处理上传接口
                                {
                                    realapi = "/yancheng/alarmdeal/upload";
                                    string remark = jso.GetValue("remark").ToString();
                                    jso["dealphoto"] = jso.GetValue("dealphoto").ToString().Split(',')[0];
                                    jar = JArray.Parse(System.Web.HttpUtility.HtmlDecode("[" + remark + "]"));

                                    JObject jardata = JObject.Parse(jar.FirstOrDefault(x => x.Value<string>("warnstatus") == "1").ToString());

                                    jso["remark"] = jardata.GetValue("remark").ToString();
                                    jso.Add("examineStatus", "1");
                                }
                                else if (api.Contains("Device/AddDeviceFacture"))           //设备信息上传
                                {
                                    realapi = "/yancheng/Device/AddDeviceFacture";
                                }
                                else if (api.Contains("Craneinterface/UploadCraneHistory"))          //塔机实时数据上传
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

                                result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                LogEntity = new CityUploadOperateLog
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
                                         //   await _aqtUploadService.UpdateCityUploadDate(url, api, now);
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





            url = "http://49.4.68.132:8051/api/";
            //实名制url
            realnameurl = _hpSystemSetting.getSettingValue(Const.Setting.S172);
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
                        string uploadurl = jso.GetValue("siteuploadurl").ToString();
                        string uploadaccount = jso.GetValue("uploadaccount").ToString();
                        string uploadpwd = jso.GetValue("uploadpwd").ToString();
                        string attenduserpsd = jso.GetValue("attenduserpsd").ToString();
                        string recordNumber = "AJ320923120210172";

                        if (string.IsNullOrEmpty(attenduserpsd) || string.IsNullOrEmpty(uploadurl))
                        {
                            continue;
                        }
                        if (!uploadurl.Equals("http://49.4.68.132:8094/api/"))
                        {
                            continue;
                        }
                        string belongto = jso.GetValue("belongto").ToString();
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[2];
                        JObject jsoparam = new JObject();
                        jsoparam.Add("recordNumber", recordNumber);

                        //获取指定项目的人员基础信息
                        string uploadapi = "Personal/PeopleInOutProInfo";
                        string realapi = "Personal/PeopleInOutProInfo";
                         api = "construction-site-api/ordinary-employee-info";
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
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                         result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {

                            job = JObject.Parse(result);
                            if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                            {
                                JArray mJArray = new JArray();
                                JObject jobject = new JObject();
                                mJArray = (JArray)job.GetValue("data");
                                for (int k = 0; k < mJArray.Count; k++)
                                {
                                    try
                                    {
                                        jobject = (JObject)mJArray[k];
                                        jobject.Add("importOrExitFlag", jobject.GetValue("isResign"));
                                        jobject.Add("personName", jobject.GetValue("workerName"));
                                        jobject.Add("idcard", jobject.GetValue("idNumber"));
                                        jobject.Add("typeWork", jobject.GetValue("workType"));
                                        jobject.Add("importTime", jobject.GetValue("entryDate"));
                                        jobject.Add("exitTime", jobject.GetValue("exitDate"));
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode"));
                                        jobject.Add("userinfoid", jobject.GetValue("idNumber"));
                                        string uploadresult = _cityToken.JsonRequest(url, uploadaccount, uploadpwd, realapi, JsonConvert.SerializeObject(jobject));
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
                                            errcount++;
                                            break;
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
                                    catch (Exception ex)
                                    {
                                          _logger.LogError(uploadapi + ":" + ex.Message);
                                    }
                                }

                            }


                        }

                        //获取人员实时考勤数据上传
                        uploadapi = "Personal/ManagerInOutSite";
                        realapi = "Personal/ManagerInOutSite";
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
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject jobject = (JObject)mJArray[k];
                                        jobject.Add("userinfoid", jobject.GetValue("idcard").ToString());
                                        jobject.Add("belongedTo", belongto);

                                        string uploadresult = _cityToken.JsonRequest(url, uploadaccount, uploadpwd, realapi, JsonConvert.SerializeObject(jobject));
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
                                            errcount++;
                                            break;
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
                            catch (Exception ex)
                            {
                                     _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }
                    }
                }
            }


            



            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForFuning();
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
                            if (api.Contains("UploadVideo"))
                            {
                                //视频信息数据上传
                                realapi = "Video/UploadVideos?appkey=" + account + "&secret=" + pwd;
                            }
                            else if (api.Contains("Check/FreeToShoot"))
                            {
                                //随手拍数据上传
                                if (jso.ContainsKey("url"))
                                {
                                    string urls = jso.GetValue("url").ToString();
                                    JArray jarrayObj = new JArray();
                                    JObject imagejson = new JObject();
                                    if (string.IsNullOrEmpty(urls) || urls == "[]")
                                    {
                                        imagejson.Add("url", "无");
                                        jarrayObj.Add(imagejson);
                                    }
                                    else
                                    {
                                        string[] urlarray = urls.Split(";".ToCharArray());
                                        for (int k = 0; k < urlarray.Length; k++)
                                        {
                                            imagejson = new JObject();
                                            string[] imagestr = urlarray[k].Split(",".ToCharArray());
                                            imagejson.Add("url", imagestr[0]);
                                            imagejson.Add("type", imagestr[1]);
                                            jarrayObj.Add(imagejson);
                                        }
                                    }
                                    jso.Add("urls", jarrayObj);
                                }

                            }
                            else if (api.Contains("Check/InspectContentInfo"))
                            {
                                //检查单数据上传
                                JArray jarrayObj = new JArray();
                                JObject ja = new JObject();
                                JArray fad = new JArray();
                                if (jso.ContainsKey("urls"))
                                {
                                    string urls = jso.GetValue("urls").ToString();
                                    if (string.IsNullOrEmpty(urls) || urls == "[]")
                                    {
                                        jarrayObj.Add("无");
                                        //ja.Add("urls", jarrayObj);
                                    }
                                    else
                                    {
                                        string[] urlarray = urls.Split(",".ToCharArray());
                                        for (int k = 0; k < urlarray.Length; k++)
                                        {
                                            jarrayObj.Add(urlarray[k]);
                                        }
                                    }
                                    if (jso.ContainsKey("rectifyPerson"))
                                    {
                                        if (jso.ContainsKey("checkContent"))
                                        {
                                            string checkContent = jso.GetValue("checkContent").ToString();
                                            string checkContents = string.Empty;
                                            var res1 = string.Empty;
                                            JArray arr = JsonConvert.DeserializeObject<JArray>(checkContent);
                                            if (string.IsNullOrEmpty(checkContent) || checkContent == "[]")
                                            {
                                                ja.Add("itemId", 1);
                                                ja.Add("checkContent", "无");
                                                string rectifyPerson = jso.GetValue("rectifyPerson").ToString();
                                                if (string.IsNullOrEmpty(rectifyPerson) || rectifyPerson == "[]")
                                                {
                                                    ja.Add("rectifyPerson", "无");
                                                }
                                                else
                                                {
                                                    ja.Add("rectifyPerson", rectifyPerson);
                                                }
                                                ja.Add("urls", jarrayObj);
                                                fad.Add(ja);
                                            }
                                            else
                                            {
                                                for (int b = 0; b < arr.Count; b++)
                                                {
                                                    ja = new JObject();
                                                    ja.Add("itemId", b + 1);
                                                    string rectifyPerson = jso.GetValue("rectifyPerson").ToString();
                                                    if (string.IsNullOrEmpty(rectifyPerson) || rectifyPerson == "[]")
                                                    {
                                                        ja.Add("rectifyPerson", "无");
                                                    }
                                                    else
                                                    {
                                                        ja.Add("rectifyPerson", rectifyPerson);
                                                    }
                                                    var res = arr[b];
                                                    res1 = res.Last.ToString();
                                                    ja.Add("checkContent", res1);
                                                    ja.Add("urls", jarrayObj);
                                                    fad.Add(ja);
                                                }
                                            }
                                        }
                                    }
                                }
                                jso.Add("checkLists", fad);
                            }
                            else if (api.Contains("Check/RectifyContentInfo"))
                            {
                                //检查单整改完成数据上传
                                JArray jarrayObj = new JArray();
                                JObject imagejson = new JObject();
                                if (jso.ContainsKey("urls"))
                                {
                                    string urls = jso.GetValue("urls").ToString();
                                    if (string.IsNullOrEmpty(urls) || urls == "[]")
                                    {
                                        jarrayObj.Add("无");
                                        //imagejson.Add("urls", jarrayObj);
                                    }
                                    else
                                    {
                                        string[] urlarray = urls.Split(",".ToCharArray());
                                        for (int k = 0; k < urlarray.Length; k++)
                                        {
                                            jarrayObj.Add(urlarray[k]);
                                        }
                                    }

                                    if (jso.ContainsKey("rectifyContents"))
                                    {
                                        string rectifyContents = jso.GetValue("rectifyContents").ToString();
                                        if (string.IsNullOrEmpty(rectifyContents) || rectifyContents == "[]")
                                        {
                                            imagejson.Add("itemId", 1);
                                            imagejson.Add("rectifyRemark", "无");
                                            imagejson.Add("urls", jarrayObj);
                                            job.Add(imagejson);
                                        }
                                        else
                                        {
                                            JArray rectifyContentsJarr = JsonConvert.DeserializeObject<JArray>(rectifyContents);
                                            for (int b = 0; b < rectifyContentsJarr.Count; b++)
                                            {
                                                imagejson = new JObject();
                                                imagejson.Add("itemId", b + 1);
                                                imagejson.Add("rectifyRemark", rectifyContentsJarr[b].Last.ToString());
                                                imagejson.Add("urls", jarrayObj);
                                                job.Add(imagejson);
                                            }
                                        }
                                    }
                                    jso.Remove("rectifyContents");
                                    jso.Add("rectifyContents", job);

                                }

                            }
                            else if (api.Contains("Craneinterface/CraneBindPeopleInfo"))
                            {
                                //塔机操作设备人员识别数据
                                realapi = "Craneinterface/CraneBindPeopleInfo?appkey=" + account + "&secret=" + pwd;
                            }
                            else if (api.Contains("AlarmInfo/CraneAlarmOn"))
                            {

                                //1.5塔机预警数据
                                realapi = "Craneinterface/CraneAlarmInfo?appkey=" + account + "&secret=" + pwd;
                            }
                            else if (api.Contains("Fenceinterface/FenceAlarmInfo1"))
                            {
                                realapi = "Fenceinterface/FenceAlarmInfo";
                            }
                            else if (api.Contains("Fenceinterface/FenceAlarmInfo2"))
                            {
                                realapi = "Fenceinterface/FenceAlarmInfo";
                            }
                            else if (api.Contains("Check/InspectionPointContent"))
                            {
                                if (jso.ContainsKey("urls"))
                                {
                                    string urls = jso.GetValue("urls").ToString();
                                    JArray jarrayObj = new JArray();
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

                            }
                            else if (api.Contains("Craneinterface/CraneAlarmInfo"))
                            {
                                //塔机预警数据
                                realapi = "Craneinterface/CraneAlarmInfo?appkey=" + account + "&secret=" + pwd;
                            }
                            else if (api.Contains("Hoistinterface/HoistBindPeopleInfo"))
                            {
                                //升降机操作设备人员识别数据
                                realapi = "Hoistinterface/HoistBindPeopleInfo?appkey=" + account + "&secret=" + pwd;
                            }
                            jso.Remove("post");
                            jso.Remove("siteuploadurl");
                            jso.Remove("uploadaccount");
                            jso.Remove("uploadpwd");
                            try
                            {
                                 result = _cityToken.JsonRequest(funingurl, account, pwd, realapi, JsonConvert.SerializeObject(jso));
                                 LogEntity = new CityUploadOperateLog
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


            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
