using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;
using Microsoft.Extensions.Configuration;
using XHS.Build.Common;
using XHS.Build.Common.Util;
using System.Text;
using System.Linq;
using XHS.Build.Repository.Base;
using XHS.Build.Model.Base;
using XHS.Build.Services.Event;
using System.Net.Http;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadWuxiRiskAnalyse : JobBase, IJob
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<SmartUpLoadWuxiRiskAnalyse> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        private readonly IBaseRepository<BaseEntity> _baseServices;
        private readonly IEventService _eventService;

        public SmartUpLoadWuxiRiskAnalyse(ILogger<SmartUpLoadWuxiRiskAnalyse> logger, IOperateLogService operateLogService, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService, IBaseRepository<BaseEntity> baseServices, IConfiguration configuration, IEventService eventService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _baseServices = baseServices;
            _configuration = configuration;
            _eventService = eventService;

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
            string APPID = "sysfdas2fvdasf33dag";
            string url = _configuration.GetSection("WXDY").GetValue<string>("PushUrl");
            JObject tokenjob = new JObject();
            string ACCESS_TOKEN = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken");
            if (!string.IsNullOrEmpty(ACCESS_TOKEN))
            {
                tokenjob = JObject.Parse(ACCESS_TOKEN);
            }
            string token = tokenjob.GetValue("data").ToString();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("数据上传结束。未获取到Token。", true);
                return;
            }
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            string realapi = string.Empty;
            int successcount = 0;
            int errcount = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SortedDictionary<string, object> jparam = new SortedDictionary<string, object>();
            StringBuilder sb = new StringBuilder(string.Empty);
            Dictionary<string, object> asciiDic = new Dictionary<string, object>();
            JObject jo;
            var uploadtime = await _baseServices.Db.Queryable<GCCityUploadDateEntity>().Where(it => it.uploadurl == url && it.post == "rest/AsEvent").Select(it => it.uploadtime).FirstAsync();
            //推送数据获取
            DataTable ds = await _eventService.UpEvent(uploadtime.ToString());
            DataTable dt = ds;
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
                        string api = jso.GetValue("post").ToString();
                        realapi = api;

                        if (api.Contains("AsEvent"))    //10.2	提交风险分析事件
                        {
                            realapi = "rest/AsEvent/save";
                            jso.Remove("SPID");
                        }
                        jso.Remove("post");
                        jso.Remove("uploadurl");
                        jso.Remove("uploadaccount");
                        jso.Remove("uploadpwd");
                        jso.Remove("funingurl");
                        jso.Remove("siteuploadurl");
                        jparam = new SortedDictionary<string, object>();
                        sb = new StringBuilder(string.Empty);
                        foreach (JProperty jProperty in jso.Properties())
                        {
                            jparam.Add(jProperty.Name, jProperty.Value);
                        }
                        asciiDic = new Dictionary<string, object>();
                        string[] arrKeys = jparam.Keys.ToArray();
                        Array.Sort(arrKeys, string.CompareOrdinal);
                        foreach (var key in arrKeys)
                        {
                            var value = jparam[key];
                            asciiDic.Add(key, value);
                        }
                        foreach (var item in asciiDic)
                        {
                            if (item.Key == "checkLists")
                            {
                                continue;
                            }
                            sb.Append(item.Value);
                        }
                        string sbdata = sb.ToString();
                        sb.Append(token);
                        string sign = UEncrypter.SHA256(sb.ToString());
                        string rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);
                        string result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                        if (!string.IsNullOrEmpty(result))
                        {
                            jo = JObject.Parse(result);

                            if (Convert.ToString(jo["flag"]) == "0006" || Convert.ToString(jo["flag"]) == "0007" || Convert.ToString(jo["flag"]) == "0010")
                            {
                                ACCESS_TOKEN = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken?isForce=true");
                                if (!string.IsNullOrEmpty(ACCESS_TOKEN))
                                {
                                    tokenjob = JObject.Parse(ACCESS_TOKEN);
                                }
                                token = tokenjob.GetValue("data").ToString();
                                sbdata = sbdata + token;
                                sign = UEncrypter.SHA256(sbdata.ToString());
                                rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", url, realapi, APPID, sign);

                                result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                            }
                        }


                        var LogEntity = new CityUploadOperateLog
                        {
                            //Id=Guid.NewGuid().ToString(),
                            url = url,
                            api = realapi,
                            account = APPID,
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
                            string flag = mJObj.GetValue("flag").ToString();
                            if (flag == "0000")
                            {
                                if (!list.Contains(url + api))
                                {
                                    await _aqtUploadService.UpdateCityUploadDate(url, api, jso.GetValue("createdate").ToDateTime());
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

            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
