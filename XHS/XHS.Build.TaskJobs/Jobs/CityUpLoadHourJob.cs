using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
    /// <summary>
    /// 省对接接口数据获取（2号文档）
    /// </summary>
    /// <returns></returns>
    public class CityUpLoadHourJob : JobBase, IJob
    {
        private readonly ILogger<CityUpLoadHourJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly AqtToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly XHSRealnameToken _jwtToken;
        public CityUpLoadHourJob(ILogger<CityUpLoadHourJob> logger, XHSRealnameToken jwtToken, IOperateLogService operateLogService, IHpSystemSetting hpSystemSetting, AqtToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
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
            var LogEntity = new CityUploadOperateLog();
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //账号
            string aqtaccount = _hpSystemSetting.getSettingValue(Const.Setting.S178);
            //密码
            string aqtpassword = _hpSystemSetting.getSettingValue(Const.Setting.S179);
            if (string.IsNullOrEmpty(aqtaccount))
            {
                _logger.LogInformation("数据上传结束。未设置上传账号。", true);
                return;
            }
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
                        string token = _cityToken.getAqtToken();
                        if (string.IsNullOrEmpty(token))
                        {
                            _logger.LogInformation("上传结束（对方鉴权获取失败）。", true);
                            return;
                        }
                        if (string.IsNullOrEmpty(attenduserpsd))
                        {
                            break;
                        }
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[1];
                        JObject jsoparam = new JObject();
                        jso.Add("recordNumber", recordNumber);

                        //获取指定项目的施工单位基础信息
                        string uploadapi = "Companies";
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
                                jso.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }

                        }
                        string api = "construction-site-api/construction-company";
                        string result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {

                            JObject job = JObject.Parse(result);
                            try
                            {
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {

                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        string uploadresult = _cityToken.JsonRequest(uploadapi, JsonConvert.SerializeObject(mJArray[k]), aqtaccount, aqtpassword);
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
                            catch (HttpRequestException ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                                return;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(uploadapi + ":" + ex.Message);
                            }

                            //获取指定项目的人员基础信息
                            uploadapi = "Workers";
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
                                try
                                {
                                    job = JObject.Parse(result);
                                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                    {
                                        JArray mJArray = new JArray();
                                        mJArray = (JArray)job.GetValue("data");
                                        for (int k = 0; k < mJArray.Count; k++)
                                        {
                                            string uploadresult = _cityToken.JsonRequest(uploadapi, JsonConvert.SerializeObject(mJArray[k]), aqtaccount, aqtpassword);
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

                            //获取项目每小时场内人员进出数 
                            uploadapi = "Attendances";
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
                                try
                                {
                                    job = JObject.Parse(result);
                                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                    {
                                        JArray mJArray = new JArray();
                                        mJArray = (JArray)job.GetValue("data");
                                        for (int k = 0; k < mJArray.Count; k++)
                                        {
                                            string uploadresult = _cityToken.JsonRequest(uploadapi, JsonConvert.SerializeObject(mJArray[k]), aqtaccount, aqtpassword);
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

                            //获取项目当前场内人员不同工种数量
                            uploadapi = "Attendances/PeopleTypeCount";
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
                                try
                                {

                                    job = JObject.Parse(result);
                                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                    {
                                        JObject mJObject = new JObject();
                                        mJObject = (JObject)job.GetValue("data");

                                        string uploadresult = _cityToken.JsonRequest(uploadapi, JsonConvert.SerializeObject(mJObject), aqtaccount, aqtpassword);
                                        LogEntity = new CityUploadOperateLog
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
                                            string uploadresult = _cityToken.JsonRequest(uploadapi, JsonConvert.SerializeObject(mJArray[k]), aqtaccount, aqtpassword);
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
                                            string uploadresult = _cityToken.JsonRequest(uploadapi, JsonConvert.SerializeObject(mJArray[k]), aqtaccount, aqtpassword);
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


            }
            DataSet ds = await _aqtUploadService.GetListById();
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
                            string url = jso.GetValue("uploadurl").ToString();
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            string api = jso.GetValue("post").ToString();
                            try
                            {
                                if (api.Contains("DustInfoBoard"))
                                {
                                    //扬尘信息看板参数替换
                                    string boardurl = jso.GetValue("uploadBoardUrl").ToString();
                                    jso.Remove("uploadBoardUrl");
                                    jso.Add("dustBoardUrl", boardurl);
                                }
                                else if (api.Contains("SmartSupervisionBoard"))
                                {
                                    //智慧监管整体看板
                                    string cityurl = _hpSystemSetting.getSettingValue(Const.Setting.S187);
                                    string username = _hpSystemSetting.getSettingValue(Const.Setting.S188);
                                    //_logger.LogError(api + "智慧监管整体看板url:" + cityurl);
                                    if (!string.IsNullOrEmpty(cityurl))
                                    {
                                        string nowtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        string data = HttpUtility.UrlEncode(UEncrypter.EncryptByRSA(username + ";" + nowtime, Const.Encryp.PUBLIC_KEY_OTHER));
                                        string boardurl = cityurl + "/#/SingleSignOn?type=0&data=" + data;
                                        jso.Remove("uploadBoardUrl");
                                        jso.Add("uploadBoardUrl", boardurl);
                                    }
                                }
                                else if (api.Contains("UploadSpecialOperationPersonnel"))
                                {
                                    if (jso.GetValue("workTypeCode").ToInt() >= 193)
                                    {
                                        jso["workTypeCode"] = 193;
                                    }
                                }
                                else if (api.Contains("/Board/insertBoard"))    //项目看板
                                {
                                    JObject datajob = new JObject();
                                    datajob.Add("appKey", "muj1F1568714076oby3342714HlZSe");
                                    datajob.Add("secret", "hd1568714076NVeejUu26775018gXudvbDzWF");
                                    jso.Remove("uploadurl");
                                    jso.Remove("uploadaccount");
                                    jso.Remove("uploadpwd");
                                    jso.Remove("siteuploadurl");
                                    jso.Remove("appKey");
                                    jso.Remove("secret");
                                    jso.Remove("post");
                                    datajob.Add("boardVo", jso);
                                    string retString = HttpNetRequest.POSTSendJsonRequest("http://124.70.9.139:8001/Board/insertBoard", JsonConvert.SerializeObject(datajob), new Dictionary<string, string>() { });
                                    LogEntity = new CityUploadOperateLog
                                    {
                                        //Id=Guid.NewGuid().ToString(),
                                        url = "http://124.70.9.139:8001/",
                                        api = api,
                                        account = account,
                                        param = datajob.ToString(),
                                        result = retString,
                                        createtime = DateTime.Now
                                    };
                                    await _operateLogService.AddCityUploadApiLog(LogEntity);
                                    if (string.IsNullOrEmpty(retString))
                                    {
                                        errcount++;
                                        break;
                                    }
                                    else
                                    {
                                        JObject Obj = JObject.Parse(retString);
                                        int code = (int)Obj.GetValue("code");
                                        if (code == 20000)
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
                                jso.Remove("post");
                                jso.Remove("uploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");

                                if(api.Contains("/Board/insertBoard"))
                                {
                                    continue; 
                                }
                                string result = _cityToken.JsonRequest(api, JsonConvert.SerializeObject(jso), aqtaccount, aqtpassword);

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
                                    JObject Obj = JObject.Parse(result);
                                    int code = (int)Obj.GetValue("code");
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
