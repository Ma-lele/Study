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
    public class SmartUpLoadHourjianyeJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourjianyeJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityNanjingToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        public SmartUpLoadHourjianyeJob(ILogger<SmartUpLoadHourjianyeJob> logger, XHSRealnameToken jwtToken, IOperateLogService operateLogService, SiteCityNanjingToken cityToken, IHpSystemSetting hpSystemSetting, IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _jwtToken = jwtToken;
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
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            int successcount = 0;
            int errcount = 0;
            string result = "";
            string uploadapi = "";
            string api = "";

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
                        JObject job = new JObject();
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
                        if (string.IsNullOrEmpty(attenduserpsd))
                        {
                            break;
                        }
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[2];
                        JObject jsoparam = new JObject();
                        jsoparam.Add("recordNumber", recordNumber);

                        try
                        {
                            //指定项目的人员教育信息
                            uploadapi = "peoinfodynamicmanagement/PeopleSafeEduInfo";
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
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());
                                        datajob.Add("typeWork", ((int)datajob.GetValue("workType")));
                                        datajob.Add("educatType", datajob.GetValue("educationType").ToString());
                                        datajob.Add("educatDate", datajob.GetValue("educationDate").ToString());
                                        datajob.Add("educationTime", datajob.GetValue("educationDuration").ToDouble());

                                        //教育地点
                                        datajob.Add("educatAddress","本工地");

                                        datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode").ToString());
                                        datajob.Add("educatContent", datajob.GetValue("educationContent").ToString());
                                        datajob.Add("idcard", datajob.GetValue("workerIdNumber").ToString());
                                        keyValues = new Dictionary<string, object>(); ;
                                        keyValues.Add("params", datajob);
                                        string uploadresult = _cityToken.FormRequest(uploadurl, uploadaccount, uploadpwd, uploadapi, keyValues);
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
                                            successcount += successcount;
                                        }
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

                        try
                        {

                            //人员奖惩信息上传
                            uploadapi = "peoinfodynamicmanagement/PeopleRewardPunishInfo";
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
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());
                                        datajob.Add("idcard", datajob.GetValue("workerIdNumber").ToString());
                                        datajob.Add("typeWork", datajob.GetValue("typeWork").ToInt());
                                        datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode").ToString());
                                        datajob.Add("rewardType", datajob.GetValue("eventType").ToString());
                                        datajob.Add("rewardContent", datajob.GetValue("eventContent").ToString());
                                        datajob.Add("rewardDate", datajob.GetValue("eventDate").ToString());

                                        keyValues = new Dictionary<string, object>();
                                        keyValues.Add("params", datajob);
                                        string uploadresult = _cityToken.FormRequest(uploadurl, uploadaccount, uploadpwd, uploadapi, keyValues);
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
                                            successcount += successcount;
                                        }
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
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForNanjing();            //小时

            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            string realapi = "";
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
                                api = jso.GetValue("post").ToString();
                                realapi = api;
                                //小时



                                if (api.Equals("Check/InspectionPoint"))          //巡检点数据上传
                                {
                                    realapi = "inspectcontentinfomanagement/InspectionPoint";
                                }
                                else if (api.Contains("StereotacticBoard"))            //3.3.5上传立体定位看板
                                {
                                    realapi = "peoinfodynamicmanagement/StereotacticBoard";
                                    jso["stereotacticBoardUrl"] = jso.GetValue("stereotacticBoardUrl").ToString().Replace("&", "%26");
                                }
                                else if (api.Contains("UploadVideo"))       //视频信息数据上传
                                {
                                    realapi = "dustvideomanagement/UploadVideo";
                                    jso["url"] = jso.GetValue("url").ToString().Replace("&", "%26");
                                }
                                else if (api.Contains("Fenceinterface/FenceAlarmInfo1"))    //缺失记录上传（可选）
                                {
                                    realapi = "fencealarminfomanagement/FenceAlarmInfo";
                                }
                                else if (api.Contains("Fenceinterface/FenceAlarmInfo2"))            //恢复记录上传
                                {
                                    realapi = "fencealarminfomanagement/FenceRecoveryInfo";
                                }
                                else if (api.Contains("Device/AddDeviceFacture"))           //设备信息上传
                                {
                                    realapi = "Device/AddDeviceFacture";
                                }
                                else if (api.Contains("DeleteDeviceFacture"))           ////删除设备信息
                                {
                                    realapi = "Device/DeleteDeviceFacture";
                                }
                                else if (api.Contains("bigboard/AddBoardURL"))          //看板地址上传
                                {
                                    realapi = "bigboard/AddBoardURL";
                                    JObject rtdjson = new JObject();
                                    if (jso.ContainsKey("params"))
                                    {
                                        rtdjson = JObject.Parse(jso.GetValue("params").ToString());
                                        rtdjson["currentsiteboard"] = rtdjson.GetValue("currentsiteboard").ToString().Replace("&", "%26");
                                        rtdjson["workerinfoboard"] = rtdjson.GetValue("workerinfoboard").ToString().Replace("&", "%26");
                                        rtdjson["positionboard"] = rtdjson.GetValue("positionboard").ToString().Replace("&", "%26");
                                        rtdjson["dustboard"] = rtdjson.GetValue("dustboard").ToString().Replace("&", "%26");
                                        rtdjson["craneboard"] = rtdjson.GetValue("craneboard").ToString().Replace("&", "%26");
                                        rtdjson["hoistboard"] = rtdjson.GetValue("hoistboard").ToString().Replace("&", "%26");
                                        rtdjson["uploadboard"] = rtdjson.GetValue("uploadboard").ToString().Replace("&", "%26");
                                        rtdjson["deeppitboard"] = rtdjson.GetValue("deeppitboard").ToString().Replace("&", "%26");
                                        rtdjson["highformworkboard"] = rtdjson.GetValue("highformworkboard").ToString().Replace("&", "%26");
                                        rtdjson["checkorderboard"] = rtdjson.GetValue("checkorderboard").ToString().Replace("&", "%26");
                                        rtdjson["takepictureboard"] = rtdjson.GetValue("takepictureboard").ToString().Replace("&", "%26");
                                        rtdjson["mobilecheckboard"] = rtdjson.GetValue("mobilecheckboard").ToString().Replace("&", "%26");
                                        rtdjson["fenceinterfaceboard"] = rtdjson.GetValue("fenceinterfaceboard").ToString().Replace("&", "%26");
                                        rtdjson["attendanceboard"] = rtdjson.GetValue("attendanceboard").ToString().Replace("&", "%26");
                                        rtdjson["siteboard"] = rtdjson.GetValue("siteboard").ToString().Replace("&", "%26");
                                        rtdjson["videoboard"] = rtdjson.GetValue("videoboard").ToString().Replace("&", "%26");
                                    }
                                    jso = new JObject();
                                    jso = rtdjson;
                                }
                                else if (api.Contains("InspectContentInfo"))         //4.6.1 检查单数据上传
                                {
                                    realapi = "inspectcontentinfomanagement/InspectContentInfo";
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
                                                job.Add("checkContent", res.First.ToString());
                                                job.Add("urls", jarrayObj);
                                                jar.Add(job);
                                            }
                                        }
                                        jso.Remove("checkContent");
                                        jso.Add("checkLists", jar);
                                        jso.Add("idcard", jso.GetValue("idCard").ToString());
                                    }
                                    jso.Remove("rectifyDate");
                                }
                                else if (api.Contains("Check/InspectBoard"))            //检查单数据看板
                                {
                                    realapi = "inspectcontentinfomanagement/InspectBoard";
                                    jso["stereotacticBoardUrl"] = jso.GetValue("stereotacticBoardUrl").ToString().Replace("&", "%26");
                                }

















                                else if (api.Contains("Check/RectifyContentInfo"))      //检查单整改完成数据上传
                                {
                                    realapi = "inspectcontentinfomanagement/RectifyContentInfo";
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

                                }
                                else if (api.Equals("Check/InspectionPointContent"))          //巡检内容数据上传
                                {
                                    realapi = "inspectcontentinfomanagement/InspectionPointContent";
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
                                else if (api.Contains("CraneBindPeopleInfo"))           //设备操作人员识别数据上传
                                {
                                    realapi = "Craneinterface/CraneBindPeopleInfo";
                                    if (jso.ContainsKey("path"))
                                    {
                                        var path = jso.GetValue("path").ToString();
                                        string base64Photo = ImgHelper.ImageToBase64(path);
                                        jso.Add("base64Photo", base64Photo);
                                    }
                                }
                                else if (api.Contains("CraneReleasePeopleInfo"))            //人机解绑信息上传
                                {
                                    realapi = "Craneinterface/CraneReleasePeopleInfo";
                                }



                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");

                                keyValues = new Dictionary<string, object>();
                                keyValues.Add("params", jso);

                                result = _cityToken.FormRequest(url, account, pwd, realapi, keyValues);

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
                                    int code = (int)mJObj["status"]["code"];
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
