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
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Video;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    /// <summary>
    /// 省对接接口数据获取（2号文档）
    /// </summary>
    /// <returns></returns>
    public class CitySZUpLoadHourJob : JobBase, IJob
    {
        private readonly ILogger<CitySZUpLoadHourJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly SiteCityWuzhongToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly XHSRealnameToken _jwtToken;
        private readonly IVideoService _videoService;
        public CitySZUpLoadHourJob(ILogger<CitySZUpLoadHourJob> logger, IVideoService videoService, XHSRealnameToken jwtToken, IOperateLogService operateLogService, IHpSystemSetting hpSystemSetting, SiteCityWuzhongToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
            _hpSystemSetting = hpSystemSetting;
            _jwtToken = jwtToken;
            _videoService = videoService;
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
            string result;
            JObject job = new JObject();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
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

                        //获取指定项目的施工单位基础信息

                        uploadapi = "ZhgdJgDataUpload/Companies";
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

                        api = "construction-site-api/construction-company";
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
                                        keyValues = new Dictionary<string, object>();
                                        keyValues.Add("params", mJArray[k]);
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
                            try
                            {
                                //获取指定项目的人员基础信息
                                uploadapi = "ZhgdJgDataUpload/Workers";
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
                                            keyValues = new Dictionary<string, object>();
                                            keyValues.Add("params", mJArray[k]);
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
                                //获取项目每小时场内人员进出数 
                                uploadapi = "ZhgdJgDataUpload/Attendances";
                                api = "construction-site-api/each-hour-employee-count";
                                if (jsoparam.ContainsKey("currentDate"))
                                {
                                    jsoparam.Remove("currentDate");
                                }
                                jsoparam.Add("recordDate", now.ToString("yyyy-MM-dd"));
                                jsoparam.Add("hour", now.Hour - 1);
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
                                            keyValues = new Dictionary<string, object>();
                                            keyValues.Add("params", mJArray[k]);
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
                                //获取项目当前场内人员不同工种数量
                                uploadapi = "ZhgdJgDataUpload/PeopleTypeCount";
                                api = "construction-site-api/employee-type-count";
                                if (jsoparam.ContainsKey("currentDate"))
                                {
                                    jsoparam.Remove("currentDate");
                                }
                                if (jsoparam.ContainsKey("recordDate"))
                                {
                                    jsoparam.Remove("recordDate");
                                    jsoparam.Remove("hour");
                                }
                                result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                                if (!string.IsNullOrEmpty(result))
                                {
                                    job = JObject.Parse(result);
                                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                    {
                                        JObject mJObject = new JObject();
                                        mJObject = (JObject)job.GetValue("data");

                                        keyValues = new Dictionary<string, object>();
                                        keyValues.Add("params", mJObject);
                                        string uploadresult = _cityToken.FormRequest(uploadurl, uploadaccount, uploadpwd, uploadapi, keyValues);
                                        var LogEntity = new CityUploadOperateLog
                                        {
                                            url = uploadurl,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJObject.ToString(),
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

                        //上传项目劳务人员安全教育信息
                        uploadapi = "ZhgdJgDataUpload/EducationInfos";
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
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        keyValues = new Dictionary<string, object>();
                                        keyValues.Add("params", mJArray[k]);
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
                            catch (Exception ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }

                        //上传项目劳务人员奖惩信息
                        uploadapi = "ZhgdJgDataUpload/RewardPunishments";
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
                                        keyValues = new Dictionary<string, object>();
                                        keyValues.Add("params", mJArray[k]);
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


            //省平台对接数据获取
            DataSet ds = await _aqtUploadService.GetListForSuzhou();

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
                            keyValues = new Dictionary<string, object>();
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
                            string uploadurl = jso.GetValue("uploadurl").ToString();
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            api = jso.GetValue("post").ToString();
                            string realapi = "ZhgdJgDataUpload/" + api.Split('/')[1];
                            jso.Remove("post");
                            jso.Remove("uploadurl");
                            jso.Remove("uploadaccount");
                            jso.Remove("uploadpwd");

                            if (api.Contains("DustInfoBoard"))
                            {
                                //扬尘信息看板参数替换
                                string boardurl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso.Remove("uploadBoardUrl");
                                jso.Add("dustBoardUrl", boardurl);
                            }
                            else if (api.Contains("UploadMachineryInfos"))//上传塔吊基本信息/上传升降机基本信息
                            {
                                var machineryType = jso.GetValue("machineryType").ToString();
                                jso["machineryType"] = machineryType;
                            }
                            else if (api.Contains("UploadSpecialOperationPersonnel"))//上传塔吊司机信息/上传升降机司机信息
                            {
                                jso["deviceType"] = (int)jso.GetValue("deviceType");
                            }
                            else if (api.Contains("StereotacticBoard"))//上传浏览项目当前立体定位看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("CraneBoard"))//上传塔吊信息看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("DeppPitBoard"))//上传深基坑设备信息看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("HighFormworkBoard"))//上传高支模设备信息看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("HoistBoard"))//上传升降机信息看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("PeopleBoard"))//上传浏览指定人员基本信息看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("PeoplesBoard"))//上传浏览人员信息看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("SmartSupervisionBoard"))//上传智慧监管整体看板
                            {
                                //智慧监管整体看板
                                string cityurl = _hpSystemSetting.getSettingValue(Const.Setting.S187);
                                string username = _hpSystemSetting.getSettingValue(Const.Setting.S188);
                                if (!string.IsNullOrEmpty(cityurl))
                                {
                                    string nowtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    string data = HttpUtility.UrlEncode(UEncrypter.EncryptByRSA(username + ";" + nowtime, Const.Encryp.PUBLIC_KEY_OTHER));
                                    string boardurl = cityurl + "/#/SingleSignOn?type=0&data=" + data;
                                    boardurl = boardurl.Replace("&", "%26");
                                    jso.Remove("uploadBoardUrl");
                                    jso.Add("uploadBoardUrl", boardurl);
                                }
                                else
                                {
                                    var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                    jso["uploadBoardUrl"] = uploadBoardUrl;
                                }
                            }
                            else if (api.Contains("UploadBoard"))//上传卸料平台信息看板
                            {
                                var uploadBoardUrl = jso.GetValue("uploadBoardUrl").ToString().Replace("&", "%26");
                                jso["uploadBoardUrl"] = uploadBoardUrl;
                            }
                            else if (api.Contains("UploadVideo")) //上传视频监控点信息
                            {
                                BnCamera bn = new BnCamera();
                                bn.cameracode = jso.GetValue("videoId").ToString();
                                bn.cameratype = jso.GetValue("cameratype").ToInt();
                                bn.channel = jso.GetValue("channel").ToInt();
                                BnCameraResult<BnPlaybackURL> a = _videoService.GetRealurl(bn);
                                if (string.IsNullOrEmpty(a.url))
                                {
                                    continue;
                                }
                                var url = a.url.Replace("&", "%26");
                                jso.Add("url", url);
                            }


                            keyValues.Add("params", jso);
                            try
                            {
                                result = _cityToken.FormRequest(uploadurl, account, pwd, realapi, keyValues);
                                if (api.Contains("Board"))
                                {
                                    Thread.Sleep(2000);
                                }
                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = uploadurl,
                                    api = realapi,
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
                                    int code = (int)mJObj["status"]["code"];
                                    if (code == 0)
                                    {
                                        if (!list.Contains(uploadurl + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(uploadurl, api, now);
                                        }
                                        successcount += successcount;
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
                                _logger.LogError(api + ":" + ex.Message);
                                break;
                            }


                        }
                    }
                }
            }

            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
