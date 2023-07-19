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
    public class SmartUpLoadHourGuannanJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourGuannanJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityGuannanToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        public SmartUpLoadHourGuannanJob(ILogger<SmartUpLoadHourGuannanJob> logger, IHpSystemSetting hpSystemSetting, XHSRealnameToken jwtToken,IOperateLogService operateLogService, SiteCityGuannanToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;  
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _hpSystemSetting = hpSystemSetting;
            _cityToken = cityToken;
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
            Dictionary<string, string> dic = new Dictionary<string, string>();
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
                        string token = _cityToken.getSiteCityToken(uploadurl, uploadaccount, uploadpwd);
                        if (string.IsNullOrEmpty(token))
                        {
                            _logger.LogInformation("数据上传结束（对方鉴权获取失败）。", true);
                            return;
                        }
                        if (string.IsNullOrEmpty(attenduserpsd) || string.IsNullOrEmpty(uploadurl))
                        {
                            continue;
                        }
                        if (!uploadurl.Equals("http://120.195.150.173:9007/"))
                        {
                            continue;
                        }
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[2];
                        JObject jsoparam = new JObject();
                        jsoparam.Add("recordNumber", recordNumber);
                        JObject job;

                        //获取指定项目的人员基础信息
                        string uploadapi = "Personal/PeopleInOutProInfo";
                        string realapi = uploadapi + "?appkey=" + uploadaccount + "&appsecret=" + uploadpwd;
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
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        string result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    JObject jobject = new JObject();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        jobject = (JObject)mJArray[k];
                                        jobject.Add("idcard", jobject.GetValue("idNumber").ToString());
                                        jobject.Add("personName", jobject.GetValue("workerName").ToString());
                                        jobject.Add("typeWork", jobject.GetValue("workType").ToInt() > 192 ? 193 : jobject.GetValue("workType").ToInt());
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode").ToString());
                                        jobject.Add("importTime", jobject.GetValue("entryDate").ToString().Substring(0, 10));
                                        jobject.Add("importOrExitFlag", 0);
                                        jobject["unitType"] = jobject.GetValue("unitType").ToString().Replace("总包单位", "总承包单位");
                                        jobject["teamName"] = string.IsNullOrEmpty(jobject.GetValue("teamName").ToString()) ? jobject.GetValue("unitType").ToString() : jobject.GetValue("teamName").ToString();
                                        jobject["photo"] = realnameurl.Substring(0, realnameurl.LastIndexOf("api")) + jobject.GetValue("photo").ToString();
                                        string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, realapi, JsonConvert.SerializeObject(jobject));
                                        var LogEntity = new CityUploadOperateLog
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
                                            successcount ++;
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
                                break;
                                //return ResponseOutput.NotOk(uploadapi + ":" + ex.Message);
                            }

                        }

                        //获取人员实时考勤数据上传
                        uploadapi = "Personal/ManagerInOutSite";
                        realapi = uploadapi + "?appkey=" + uploadaccount + "&appsecret=" + uploadpwd;
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
                                        string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, realapi, JsonConvert.SerializeObject(mJArray[k]));
                                        var LogEntity = new CityUploadOperateLog
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
                                            successcount ++;
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
                                //return ResponseOutput.NotOk(uploadapi + ":" + ex.Message);
                            }

                        }

                        //上传项目劳务人员安全教育信息
                        uploadapi = "Personal/PeopleSafeEduInfo";
                        realapi = uploadapi + "?appkey=" + uploadaccount + "&appsecret=" + uploadpwd;
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
                        }
                        api = "construction-site-api/employee-education-info";
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    JObject jobject = new JObject();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        jobject = (JObject)mJArray[k];
                                        jobject.Add("idcard", jobject.GetValue("workerIdNumber").ToString());
                                        jobject.Add("typeWork", jobject.GetValue("workType").ToInt() > 192 ? 193 : jobject.GetValue("workType").ToInt());
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode").ToString());
                                        jobject.Add("educatDate", jobject.GetValue("educationDate").ToString());
                                        jobject.Add("educatType", jobject.GetValue("educationType").ToString());
                                        jobject.Add("educatAddress", string.IsNullOrEmpty(jobject.GetValue("educationLocation").ToString()) ? "工地现场" : jobject.GetValue("educationLocation").ToString());
                                        jobject.Add("educatContent", jobject.GetValue("educationContent").ToString());
                                        jobject.Add("educationTime", jobject.GetValue("educationDuration").ToString());
                                        jobject.Remove("workerIdNumber");
                                        jobject.Remove("workType");
                                        jobject.Remove("unifiedSocialCreditCode");
                                        jobject.Remove("educationDate");
                                        jobject.Remove("educationType");
                                        jobject.Remove("educationLocation");
                                        jobject.Remove("educationContent");
                                        jobject.Remove("educationDuration");
                                        string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, realapi, JsonConvert.SerializeObject(mJArray[k]));
                                        var LogEntity = new CityUploadOperateLog
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
                                            successcount ++;
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
                                //return ResponseOutput.NotOk(uploadapi + ":" + ex.Message);
                            }
                        }

                        //上传项目劳务人员奖惩信息
                        uploadapi = "Personal/PeopleRewardPunishInfo";
                        realapi = uploadapi + "?appkey=" + uploadaccount + "&appsecret=" + uploadpwd;
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
                        }

                        api = "construction-site-api/employee-reward-punishments-info";
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
                                        string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, realapi, JsonConvert.SerializeObject(mJArray[k]));
                                        var LogEntity = new CityUploadOperateLog
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
                                            successcount ++;
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
                                //return ResponseOutput.NotOk(uploadapi + ":" + ex.Message);
                            }
                        }

                    }

                }


            }
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForGuannan();
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
                            string url = jso.GetValue("siteuploadurl").ToString();
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            string api = jso.GetValue("post").ToString();
                            if (api.Contains("DustInfoBoard"))
                            {
                                //扬尘信息看板参数替换
                                string boardurl = jso.GetValue("uploadBoardUrl").ToString();
                                jso.Remove("uploadBoardUrl");
                                jso.Add("dustBoardUrl", boardurl);
                            }else if (api.Contains("Craneinterface/CraneBindPeopleInfo"))
                            {
                                //设备操作人员识别数据上传
                                if (jso.ContainsKey("path"))
                                {
                                   // string path = jso.GetValue("path").ToString();
                                    //string base64Photo = ImgHelper.ImageToBase64(path);
                                    jso.Add("base64Photo", jso.GetValue("path").ToString());
                                }
                            }
                            else if (api.Contains("DeviceInfo/UploadVideo"))
                            {
                                //设备操作人员识别数据上传
                                api = "DustVideo/UploadVideo";
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
                                jso["checkNumType"] = jso.GetValue("checkNumType").ToInt() + 30000;
                                jso.Add("checkLists", fad);
                            }
                            else if (api.Contains("Check/RectifyContentInfo"))
                            {
                                //检查单整改完成数据上传
                                JArray jarrayObj = new JArray();
                                JObject imagejson = new JObject();
                                JArray job = new JArray();
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

                            //url里带上appkey和appsecret
                            string realapi = api + "?appkey=" + account + "&appsecret=" + pwd;

                            jso.Remove("post");
                            jso.Remove("siteuploadurl");
                            jso.Remove("uploadaccount");
                            jso.Remove("uploadpwd");
                            if (jso.ContainsKey("funingurl"))
                            {
                                jso.Remove("funingurl");
                            }
                            try
                            {
                                string result = _cityToken.JsonRequest(url, account, pwd, realapi, JsonConvert.SerializeObject(jso));
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
                                    errcount++;
                                    break;
                                }
                                else
                                {
                                    JObject mJObj = JObject.Parse(result);
                                    int code = (int)mJObj.GetValue("code");
                                    if (code == 200)
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                            list.Add(url + api);
                                        }
                                        successcount ++;
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
