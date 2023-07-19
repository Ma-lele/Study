using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    public class SmartUpLoadHourWuxiJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourWuxiJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        private readonly IConfiguration _configuration;
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        public SmartUpLoadHourWuxiJob(IConfiguration configuration, ILogger<SmartUpLoadHourWuxiJob> logger, IOperateLogService operateLogService,
             IAqtUploadService aqtUploadService, XHSRealnameToken jwtToken, IHpSystemSetting hpSystemSetting)
        {
            _configuration = configuration;
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _jwtToken = jwtToken;
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

            string APPID = "sysfdas2fvdasf33dag";
            JObject tokenjob = new JObject();
            string ACCESS_TOKEN = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken");
            if (!string.IsNullOrEmpty(ACCESS_TOKEN))
            {
                tokenjob = JObject.Parse(ACCESS_TOKEN);
            }
            string token = tokenjob.GetValue("data").ToString();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Token获取失败。", true);
                return;
            }
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string result = "";
            string sign = "";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SortedDictionary<string, object> jparam = new SortedDictionary<string, object>();
            StringBuilder sb = new StringBuilder(string.Empty);
            Dictionary<string, object> asciiDic = new Dictionary<string, object>();
            string api = "";
            string realapi = "";
            string rtdUrl = "";
            string[] arrKeys;
            JObject jo;
            string sbdata = "";
            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
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

                        if (string.IsNullOrEmpty(attenduserpsd) || string.IsNullOrEmpty(uploadurl))
                        {
                            continue;
                        }
                        if (!uploadurl.Equals("120.195.199.66:5678"))
                        {
                            continue;
                        }
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[2];
                        JObject jsoparam = new JObject();
                        jsoparam.Add("recordNumber", recordNumber);
                        JObject job;

                        //获取指定项目的人员奖励惩戒信息
                        string uploadapi = "Personnelbehavior";
                        realapi = uploadapi + "?appkey=" + uploadaccount + "&appsecret=" + uploadpwd;
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
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, api);
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
                                    JObject jobject = new JObject();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());
                                        if (datajob.GetValue("eventType").ToString() == "惩罚")
                                        {
                                            realapi = "rest/Esponsible/IrresponsibleDeeds";
                                            datajob.Add("irresponsibleDeedsId", datajob.GetValue("rewardPunishId").ToString());
                                            datajob.Add("name", datajob.GetValue("personName").ToString());
                                            datajob.Add("certNo", datajob.GetValue("workerIdNumber").ToString());
                                            if (datajob.GetValue("workType").ToString() == "10")
                                            {
                                                datajob.Add("typeWork", "砌筑工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "20")
                                            {
                                                datajob.Add("typeWork", "钢筋工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "30")
                                            {
                                                datajob.Add("typeWork", "架子工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "40")
                                            {
                                                datajob.Add("typeWork", "混凝土工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "50")
                                            {
                                                datajob.Add("typeWork", "混凝土模具工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "60")
                                            {
                                                datajob.Add("typeWork", "机械设备安装工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "70")
                                            {
                                                datajob.Add("typeWork", "通风工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "80")
                                            {
                                                datajob.Add("typeWork", "安装起重工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "90")
                                            {
                                                datajob.Add("typeWork", "安装钳工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "100")
                                            {
                                                datajob.Add("typeWork", "电气设备安装调试工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "110")
                                            {
                                                datajob.Add("typeWork", "管道工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "130")
                                            {
                                                datajob.Add("typeWork", "弱电工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "140")
                                            {
                                                datajob.Add("typeWork", "司泵工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "150")
                                            {
                                                datajob.Add("typeWork", "挖掘铲运和桩工机械司机");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "160")
                                            {
                                                datajob.Add("typeWork", "桩机操作工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "170")
                                            {
                                                datajob.Add("typeWork", "起重信号工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "190")
                                            {
                                                datajob.Add("typeWork", "装饰装修工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "220")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "230")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "240")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "370")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "380")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }

                                            datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode").ToString());
                                            datajob.Add("irresponsibleDeedsType", datajob.GetValue("eventContent").ToString());
                                            datajob.Add("irresponsibleDeedsContent", datajob.GetValue("eventContent").ToString());
                                            //datajob.Add("eventType", datajob.GetValue("").ToString());
                                            //datajob.Add("result", datajob.GetValue("").ToString());
                                            jso.Remove("personName");
                                            jso.Remove("workerIdNumber");
                                            jso.Remove("workType");
                                            jso.Remove("rewardPunishId");
                                            jso.Remove("eventContent");
                                            jso.Remove("rewardPunishId");
                                            jso.Remove("rewardPunishId");
                                        }
                                        else if (datajob.GetValue("eventType").ToString() == "奖励")
                                        {
                                            realapi = "rest/Esponsible/ResponsibleBehavior";
                                            datajob.Add("responsibleBehaviorId", datajob.GetValue("rewardPunishId").ToString());
                                            datajob.Add("name", datajob.GetValue("personName").ToString());
                                            datajob.Add("certNo", datajob.GetValue("workerIdNumber").ToString());
                                            if (datajob.GetValue("workType").ToString() == "10")
                                            {
                                                datajob.Add("typeWork", "砌筑工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "20")
                                            {
                                                datajob.Add("typeWork", "钢筋工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "30")
                                            {
                                                datajob.Add("typeWork", "架子工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "40")
                                            {
                                                datajob.Add("typeWork", "混凝土工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "50")
                                            {
                                                datajob.Add("typeWork", "混凝土模具工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "60")
                                            {
                                                datajob.Add("typeWork", "机械设备安装工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "70")
                                            {
                                                datajob.Add("typeWork", "通风工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "80")
                                            {
                                                datajob.Add("typeWork", "安装起重工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "90")
                                            {
                                                datajob.Add("typeWork", "安装钳工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "100")
                                            {
                                                datajob.Add("typeWork", "电气设备安装调试工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "110")
                                            {
                                                datajob.Add("typeWork", "管道工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "130")
                                            {
                                                datajob.Add("typeWork", "弱电工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "140")
                                            {
                                                datajob.Add("typeWork", "司泵工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "150")
                                            {
                                                datajob.Add("typeWork", "挖掘铲运和桩工机械司机");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "160")
                                            {
                                                datajob.Add("typeWork", "桩机操作工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "170")
                                            {
                                                datajob.Add("typeWork", "起重信号工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "190")
                                            {
                                                datajob.Add("typeWork", "装饰装修工");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "220")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "230")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "240")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "370")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            else if (datajob.GetValue("workType").ToString() == "380")
                                            {
                                                datajob.Add("typeWork", "其他");
                                            }
                                            datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode").ToString());
                                            datajob.Add("responsibleBehaviorContent", datajob.GetValue("eventContent").ToString());
                                            //datajob.Add("eventType", datajob.GetValue("").ToString());
                                            //datajob.Add("result", datajob.GetValue("").ToString());
                                            datajob.Remove("rewardPunishId");
                                            datajob.Remove("personName");
                                            datajob.Remove("workerIdNumber");
                                            datajob.Remove("workType");
                                            datajob.Remove("unifiedSocialCreditCode");
                                            datajob.Remove("eventContent");
                                        }
                                        jparam = new SortedDictionary<string, object>();
                                        sb = new StringBuilder(string.Empty);
                                        foreach (JProperty jProperty in datajob.Properties())
                                        {
                                            jparam.Add(jProperty.Name, jProperty.Value);
                                        }
                                        asciiDic = new Dictionary<string, object>();
                                        arrKeys = jparam.Keys.ToArray();
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
                                        sbdata = sb.ToString();
                                        sb.Append(token);
                                        sign = UEncrypter.SHA256(sb.ToString());
                                        rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);
                                        result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        if (string.IsNullOrEmpty(result))
                                        {
                                            continue;
                                        }
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
                                            rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);

                                            result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        }

                                        LogEntity = new CityUploadOperateLog
                                        {
                                            //Id=Guid.NewGuid().ToString(),
                                            url = uploadurl,
                                            api = realapi,
                                            account = account,
                                            param = datajob.ToString(),
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
                                                if (!list.Contains(uploadurl + api))
                                                {
                                                    await _aqtUploadService.UpdateCityUploadDate(uploadurl, api, now);
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
                                //return ResponseOutput.NotOk(uploadapi + ":" + ex.Message);
                            }
                        }



                        //上传项目劳务人员安全教育信息
                         uploadapi = "Education";
                        realapi = uploadapi + "?appkey=" + uploadaccount + "&appsecret=" + uploadpwd;
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
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, api);
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
                                    JObject jobject = new JObject();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        realapi = "rest/Companies/EducationInfos";
                                        JObject datajob = JObject.Parse(mJArray[k].ToString());

                                        datajob.Add("educationInfosId", datajob.GetValue("safeEduId").ToString());
                                        datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode").ToString());
                                        if (datajob.GetValue("workType").ToString() == "10")
                                        {
                                            datajob.Add("typeWork", "砌筑工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "20")
                                        {
                                            datajob.Add("typeWork", "钢筋工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "30")
                                        {
                                            datajob.Add("typeWork", "架子工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "40")
                                        {
                                            datajob.Add("typeWork", "混凝土工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "50")
                                        {
                                            datajob.Add("typeWork", "混凝土模具工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "60")
                                        {
                                            datajob.Add("typeWork", "机械设备安装工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "70")
                                        {
                                            datajob.Add("typeWork", "通风工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "80")
                                        {
                                            datajob.Add("typeWork", "安装起重工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "90")
                                        {
                                            datajob.Add("typeWork", "安装钳工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "100")
                                        {
                                            datajob.Add("typeWork", "电气设备安装调试工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "110")
                                        {
                                            datajob.Add("typeWork", "管道工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "130")
                                        {
                                            datajob.Add("typeWork", "弱电工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "140")
                                        {
                                            datajob.Add("typeWork", "司泵工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "150")
                                        {
                                            datajob.Add("typeWork", "挖掘铲运和桩工机械司机");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "160")
                                        {
                                            datajob.Add("typeWork", "桩机操作工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "170")
                                        {
                                            datajob.Add("typeWork", "起重信号工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "190")
                                        {
                                            datajob.Add("typeWork", "装饰装修工");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "220")
                                        {
                                            datajob.Add("typeWork", "其他");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "230")
                                        {
                                            datajob.Add("typeWork", "其他");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "240")
                                        {
                                            datajob.Add("typeWork", "其他");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "370")
                                        {
                                            datajob.Add("typeWork", "其他");
                                        }
                                        else if (datajob.GetValue("workType").ToString() == "380")
                                        {
                                            datajob.Add("typeWork", "其他");
                                        }
                                        datajob.Remove("workType");
                                        datajob.Remove("safeEduId");
                                        jparam = new SortedDictionary<string, object>();
                                        sb = new StringBuilder(string.Empty);
                                        foreach (JProperty jProperty in datajob.Properties())
                                        {
                                            jparam.Add(jProperty.Name, jProperty.Value);
                                        }
                                        asciiDic = new Dictionary<string, object>();
                                        arrKeys = jparam.Keys.ToArray();
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
                                        sbdata = sb.ToString();
                                        sb.Append(token);
                                        sign = UEncrypter.SHA256(sb.ToString());
                                        rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);
                                        result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        if (string.IsNullOrEmpty(result))
                                        {
                                            continue;
                                        }
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
                                            rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);

                                            result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
                                        }

                                        LogEntity = new CityUploadOperateLog
                                        {
                                            //Id=Guid.NewGuid().ToString(),
                                            url = uploadurl,
                                            api = realapi,
                                            account = account,
                                            param = datajob.ToString(),
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
                                                if (!list.Contains(uploadurl + api))
                                                {
                                                    await _aqtUploadService.UpdateCityUploadDate(uploadurl, api, now);
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
                                //return ResponseOutput.NotOk(uploadapi + ":" + ex.Message);
                            }
                        }
                    }
                }
            }
            //推送数据获取
            DataSet ds = await _aqtUploadService.GetListsForWuxi();
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

                                string uploadurl = jso.GetValue("siteuploadurl").ToString();
                                string account = "";/*jso.GetValue("uploadaccount").ToString();*/
                                string pwd = "";/* jso.GetValue("uploadpwd").ToString();*/
                                api = jso.GetValue("post").ToString();
                                jso.Remove("post");
                                jso.Remove("uploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                jso.Remove("funingurl");
                                jso.Remove("siteuploadurl");
                                if (api.Contains("rest/Board/Board"))           //10.39	提交数据看板
                                {
                                    realapi = "rest/Board/Board";
                                    jso["type"] = jso.GetValue("type").ToString();
                                }
                                if(api.Contains("SmartSupervisionBoard"))       //10.40	工地集成平台URL（新增）
                                {
                                    realapi = "rest/Board/URL";
                                    jso.Add("siteurl", jso.GetValue("uploadBoardUrl").ToString());
                                    jso.Remove("post");
                                    jso.Remove("siteuploadurl");
                                    jso.Remove("uploadaccount");
                                    jso.Remove("uploadpwd");
                                    jso.Remove("belongedTo");
                                    jso.Remove("uploadBoardUrl");
                                }
                                else if (api.Contains("Check/InspectContentInfo"))          //10.17	提交检查单信息/10.20	检查单数据上传
                                {
                                    string jsodata = jso.ToString();
                                    ////10.17	提交检查单信息
                                    try
                                    {
                                        realapi = "rest/Check/Checklist";
                                        jso.Add("isProvinStand", jso.GetValue("IsProvinStand").ToString());
                                        jso.Add("isNeedToRectify", jso.GetValue("isRectify"));
                                        jso.Add("recommendFinishDate", jso.GetValue("rectifyDate").ToString());
                                        jso.Add("checkComment", jso.GetValue("remark".ToString()));
                                        jso.Add("checkPeople", jso.GetValue("checkPerson").ToString());
                                        jso.Remove("IsProvinStand");
                                        jso.Remove("idCard");
                                        jso.Remove("checkPerson");
                                        jso.Remove("checkNumType");
                                        jso.Remove("checkContent");
                                        jso.Remove("rectifyPerson");
                                        jso.Remove("isRectify");
                                        jso.Remove("rectifyDate");
                                        jso.Remove("remark");
                                        jso.Remove("urls");

                                        jparam = new SortedDictionary<string, object>();
                                        sb = new StringBuilder(string.Empty);
                                        foreach (JProperty jProperty in jso.Properties())
                                        {
                                            jparam.Add(jProperty.Name, jProperty.Value);
                                        }
                                        asciiDic = new Dictionary<string, object>();
                                        arrKeys = jparam.Keys.ToArray();
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
                                        sbdata = sb.ToString();
                                        sb.Append(token);
                                        sign = UEncrypter.SHA256(sb.ToString());
                                        rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);

                                        result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
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
                                                rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);

                                                result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                                            }
                                        }
                                        LogEntity = new CityUploadOperateLog
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
                                            errcount++;
                                            break;
                                        }
                                        else
                                        {
                                            JObject mJObj = JObject.Parse(result);
                                            string flag = mJObj.GetValue("flag").ToString();
                                            if (flag == "0000")
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
                                        _logger.LogError(realapi + ":" + ex.Message);
                                        return;
                                    }
                                    catch (Exception ex)
                                    {

                                        _logger.LogError(realapi + ":" + ex.Message);
                                    }

                                    //10.20   检查单数据上传（新增）
                                    if (!string.IsNullOrEmpty(jsodata))
                                    {
                                        jso = JObject.Parse(jsodata);
                                        realapi = "rest/Check/InspectContentInfo";
                                        JObject jobdata = new JObject();
                                        string rectifyPerson = "";
                                        JArray jar = new JArray();
                                        string checkContent = jso.GetValue("checkContent").ToString();
                                        JArray arr = JsonConvert.DeserializeObject<JArray>(checkContent);

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
                                            jobdata.Add("checkContent", "无");
                                            jobdata.Add("isProvinStand", (int)jso.GetValue("IsProvinStand"));
                                            jobdata.Add("itemId", 1);
                                            jobdata.Add("rectifyPerson", rectifyPerson);
                                            jar.Add(jobdata);
                                        }
                                        else
                                        {
                                            for (int b = 0; b < 1; b++)
                                            {
                                                jobdata = new JObject();
                                                var res = arr[b];
                                                jobdata.Add("checkContent", res.Last.ToString());
                                                jobdata.Add("isProvinStand", (int)jso.GetValue("IsProvinStand"));
                                                jobdata.Add("itemId", Convert.ToString(b + 1));
                                                jobdata.Add("rectifyPerson", rectifyPerson);
                                                jar.Add(jobdata);
                                            }
                                        }
                                        jso["checkNumType"] = (int)jso.GetValue("checkNumType");
                                        jso["idcard"] = jso.GetValue("idCard");
                                        jso.Add("checkLists", jar);
                                        jso.Remove("idCard");
                                        jso.Remove("checkContent");
                                        jso.Remove("urls");
                                        jso.Remove("belongedTo");
                                        jso.Remove("rectifyPerson");
                                        jso.Remove("IsProvinStand");
                                        jso.Remove("rectifyDate");

                                    }
                                }
                                else if (api.Contains("Check/RectifyContentInfo"))           //检查单整改完成数据上传
                                {
                                    realapi = "rest/Check/RectifyContentInfo";
                                    jso.Remove("rectifyContents");
                                    jso.Remove("belongedTo");
                                    jso.Add("itemId", "1");
                                }
                                else if (api.Equals("Check/InspectionPoint"))      //10.18	提交/编辑/删除移动巡检点
                                {
                                    realapi = "rest/Check/CheckPoints";

                                    jso.Add("checkPointId", jso.GetValue("inspectionId"));
                                    jso.Add("summary", jso.GetValue("site"));
                                }
                                else if (api.Equals("Check/InspectionPointContent"))            //10.19 提交移动巡检信息
                                {
                                    realapi = "rest/Check/MobileCheck";
                                }
                                else if (api.Contains("AlarmInfo/CraneAlarmOn"))            //10.3	提交塔机预警数据（新增）
                                {
                                    realapi = "rest/Tower/CraneAlarmInfo";
                                    ///
                                    jso["happenTime"] = Convert.ToDateTime(jso.GetValue("happenTime")).ToString("yyyy-MM-dd HH:mm:ss");

                                    jso.Remove("WARNID");
                                    jso.Remove("belongedTo");
                                    jso.Remove("alarmType");
                                    jso.Remove("alarmLevel");
                                    jso.Remove("description");
                                    jso.Remove("time");
                                    jso.Remove("projectInfoId");
                                }
                                else if (api.Contains("DockingMachineryInfos/UploadSpecialOperationPersonnel"))     //10.27	提交/编辑/删除升降机司机信息/10.25	提交/编辑/删除塔吊司机信息
                                {
                                    if (jso.GetValue("setype").ToString() == "1")      //塔吊
                                    {
                                        //删除塔吊司机信息
                                        realapi = "rest/Tower/UploadSpecialOperationPersonnel";
                                        jso.Add("personName", jso.GetValue("PersonName").ToString());
                                        jso.Add("sex", (int)jso.GetValue("Sex"));
                                        jso.Add("type", "Tower");
                                        jso["workTypeCode"] = "高处作业吊篮操作工";
                                        jso.Remove("PersonName");
                                        jso.Remove("Sex");
                                        jso.Remove("Type");
                                        jso.Remove("setype");
                                    }
                                    else if (jso.GetValue("setype").ToString() == "2")
                                    {
                                        //删除升降机司机信息
                                        realapi = "rest/Elevator/UploadSpecialOperationPersonnel";
                                        jso.Add("personName", jso.GetValue("PersonName").ToString());
                                        jso.Add("sex", (int)jso.GetValue("Sex"));
                                        jso.Add("type", "Elevator");
                                        jso.Add("workType", "安装起重工");
                                        jso.Remove("PersonName");
                                        jso.Remove("Sex");
                                        jso.Remove("Type");
                                        jso.Remove("setype");
                                        jso.Remove("workTypeCode");
                                    }
                                }
                                else if (api.Contains("DockingMachineryInfos/UploadMachineryInfos"))            //10.28	提交/编辑/删除升降机基本信息/10.26	提交/编辑/删除塔吊基本信息
                                {
                                    if (jso.GetValue("setype").ToString() == "1")      //塔吊
                                    {
                                        realapi = "rest/Tower/UploadMachineryInfos";
                                        jso["reCheckReviewDate"] = Convert.ToDateTime(jso.GetValue("reCheckReviewDate")).ToString("yyyy-MM-dd HH:mm:ss");
                                        jso["machineryCheckState"] = (int)jso.GetValue("machineryCheckState");
                                        jso["checkState"] = (int)jso.GetValue("checkState");
                                        jso.Remove("setype");
                                        jso.Remove("checkUrl");
                                        jso.Remove("useRecordNo");
                                        jso.Remove("useRecordNoUrl");
                                    }
                                    else if (jso.GetValue("setype").ToString() == "2")     //升降机
                                    {
                                        realapi = "rest/Elevator/UploadMachineryInfos";
                                        jso["reCheckReviewDate"] = Convert.ToDateTime(jso.GetValue("reCheckReviewDate")).ToString("yyyy-MM-dd HH:mm:ss");
                                        jso["machineryCheckState"] = (int)jso.GetValue("machineryCheckState");
                                        jso["checkState"] = (int)jso.GetValue("checkState");
                                        jso.Remove("setype");
                                        jso.Remove("checkUrl");
                                        jso.Remove("useRecordNo");
                                        jso.Remove("useRecordNoUrl");
                                    }
                                }
                                else if (api.Contains("DeviceInfo/UploadDeviceInfo"))           //10.29	提交/编辑/删除卸料平台基本信息
                                {
                                    realapi = "rest/Material/UploadDeviceInfo";
                                }
                                else if (api.Contains("DeviceInfo/DeppPitDeviceInfo"))          //10.30 提交/编辑/删除深基坑设备基本信息
                                {
                                    realapi = "rest/DeppPit/DeppPitDeviceInfo";

                                }
                                else if (api.Contains("DeviceInfo/HighFormworkDeviceInfo"))         //10.33 提交/编辑/删除高支模设备基本信息
                                {
                                    realapi = "rest/HighFormwork/HighFormworkDeviceInfo";
                                }
                                else if (api.Contains("DustInfo/DustDeviceInfo"))           //10.23 提交/编辑/删除扬尘设备信息
                                {
                                    realapi = "rest/DustNoise/DustDeviceInfo";
                                }
                                else if (api.Contains("DeviceInfo/UploadVideo"))            //10.24	提交/编辑/删除视频监控点信息
                                {
                                    realapi = "rest/Video/UploadVideo";

                                }
                                else if (api.Equals("Device/AddDeviceFacture"))         //10.22	提交/删除设备信息（新增）
                                {
                                    realapi = "rest/Device/AddDeviceFacture";
                                }
                                else if (api.Contains("Fenceinterface/FenceAlarmInfo1"))    //缺失记录上传（可选）
                                {
                                    realapi = "rest/Fenceinterface/FenceAlarmInfo";
                                    jso.Add("act", "缺失");
                                    jso.Add("deviceId", jso.GetValue("warnNumber"));
                                    if (jso.ContainsKey("GONGCHENG_CODE"))
                                    {
                                        jso.Remove("GONGCHENG_CODE");
                                    }
                                    jso.Remove("belongedTo");
                                    jso.Remove("warnNumber");
                                    jso.Remove("ProjectInfoId");
                                }
                                else if (api.Contains("Fenceinterface/FenceAlarmInfo2"))            //恢复记录上传
                                {
                                    realapi = "rest/Fenceinterface/FenceAlarmInfo";
                                    jso.Add("act", "恢复");
                                    jso.Add("deviceId", jso.GetValue("warnNumber"));
                                    jso.Add("defectDate", jso.GetValue("DefectDate").ToString());
                                    jso.Add("defectPosition", jso.GetValue("DefectPosition").ToString());
                                    if (jso.ContainsKey("GONGCHENG_CODE"))
                                    {
                                        jso.Remove("GONGCHENG_CODE");
                                    }
                                    jso.Remove("belongedTo");
                                    jso.Remove("warnNumber");
                                    jso.Remove("ProjectInfoId");
                                    jso.Remove("recoveryDate");
                                    jso.Remove("DefectDate");
                                    jso.Remove("DefectPosition");
                                }
                                else if (api.Contains("CCraneinterface/CraneReleasePeopleInfo"))         //10.37 人机解绑信息上传
                                {
                                    realapi = "rest/Craneinterface/CraneReleasePeopleInfo";
                                    jso.Remove("projectInfoId");
                                    ///
                                    jso["relieveDate"] = Convert.ToDateTime(jso.GetValue("relieveDate")).ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                else if (api.Contains("Craneinterface/CraneBindPeopleInfo"))            //10.36 设备操作人员识别数据上传
                                {
                                    realapi = "rest/Craneinterface/CraneBindPeopleInfo";
                                    string base64Photo = ImgHelper.ImageToBase64(jso.GetValue("path").ToString());
                                    jso.Add("base64Photo", base64Photo);
                                    jso.Remove("path");
                                }

                                jparam = new SortedDictionary<string, object>();
                                sb = new StringBuilder(string.Empty);
                                foreach (JProperty jProperty in jso.Properties())
                                {
                                    jparam.Add(jProperty.Name, jProperty.Value);
                                }
                                asciiDic = new Dictionary<string, object>();
                                arrKeys = jparam.Keys.ToArray();
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
                                sbdata = sb.ToString();
                                sb.Append(token);
                                sign = UEncrypter.SHA256(sb.ToString());
                                rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);
                                result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
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
                                        rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);

                                        result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                                    }
                                }


                                LogEntity = new CityUploadOperateLog
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
                                    errcount++;
                                    break;
                                }
                                else
                                {
                                    JObject mJObj = JObject.Parse(result);
                                    string flag = mJObj.GetValue("flag").ToString();
                                    if (flag == "0000")
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
