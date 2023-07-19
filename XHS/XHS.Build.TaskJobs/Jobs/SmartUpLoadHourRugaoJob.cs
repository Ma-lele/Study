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
    public class SmartUpLoadHourRugaoJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourRugaoJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly CityToken _cityToken;
        private readonly XHSRealnameToken _jwtToken;
        private readonly IOperateLogService _operateLogService;
        private readonly IHpSystemSetting _hpSystemSetting;
        public SmartUpLoadHourRugaoJob(ILogger<SmartUpLoadHourRugaoJob> logger, IHpSystemSetting hpSystemSetting, XHSRealnameToken jwtToken, IOperateLogService operateLogService, CityToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
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
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();
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
                        try{ 
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
                            string token = _cityToken.getCityToken(uploadurl, uploadaccount, uploadpwd);
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
                                    jsoparam.Add("currentDate", currentDate);
                                    dic.Add(uploadurl + uploadapi, currentDate);
                                }

                            }
                            api = "construction-site-api/construction-company";
                            string result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                            if (!string.IsNullOrEmpty(result))
                            {
                                JObject job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {

                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, uploadapi, JsonConvert.SerializeObject(mJArray[k]));
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
                                    job = JObject.Parse(result);
                                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                    {
                                        JArray mJArray = new JArray();
                                        mJArray = (JArray)job.GetValue("data");
                                        for (int k = 0; k < mJArray.Count; k++)
                                        {
                                            string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, uploadapi, JsonConvert.SerializeObject(mJArray[k]));
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
                                    job = JObject.Parse(result);
                                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                    {
                                        JArray mJArray = new JArray();
                                        mJArray = (JArray)job.GetValue("data");
                                        for (int k = 0; k < mJArray.Count; k++)
                                        {
                                            string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, uploadapi, JsonConvert.SerializeObject(mJArray[k]));
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
                                    job = JObject.Parse(result);
                                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                    {
                                        JObject mJObject = new JObject();
                                        mJObject = (JObject)job.GetValue("data");

                                        string uploadresult = _cityToken.JsonRequest(uploadurl, uploadaccount, uploadpwd, uploadapi, JsonConvert.SerializeObject(mJObject));
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
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForRugao();
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
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                           api = jso.GetValue("post").ToString();
                            if (api.Contains("DustInfoBoard"))
                            {
                                //扬尘信息看板参数替换
                                string boardurl = jso.GetValue("uploadBoardUrl").ToString();
                                jso.Remove("uploadBoardUrl");
                                jso.Add("dustBoardUrl", boardurl);
                            }
                            if (jso.ContainsKey("funingurl"))
                            {
                                jso.Remove("funingurl");
                            }
                            jso.Remove("post");
                            jso.Remove("siteuploadurl");
                            jso.Remove("uploadaccount");
                            jso.Remove("uploadpwd");
                            int warnId = 0;
                            if (jso.ContainsKey("WARNID"))
                            {
                                warnId = int.Parse(jso.GetValue("WARNID").ToString());
                            }
                            
                                string result = _cityToken.JsonRequest(url, account, pwd, api, JsonConvert.SerializeObject(jso));
                                if (api.Contains("DustInfo/DayDustInfo") || api.Contains("Check/Checklist"))
                                {
                                    Thread.Sleep(2000);
                                }
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
                                    //如皋报警接口
                                    if (api.Contains("AlarmOn"))
                                    {
                                        JObject mJObj = new JObject();
                                        mJObj = JObject.Parse(result);
                                        if (mJObj.ContainsKey("data"))
                                        {
                                            JObject data = (JObject)mJObj.GetValue("data");
                                            string alarmid = data.GetValue("id").ToString();
                                            //更新如皋报警上传接口返回的报警记录id 
                                            await _aqtUploadService.UpdateWarnAlarmId(warnId, alarmid);
                                        }
                                    }
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
