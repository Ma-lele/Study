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
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadHourYanchengJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourYanchengJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadHourYanchengJob(ILogger<SmartUpLoadHourYanchengJob> logger, IOperateLogService operateLogService, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
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
            JObject carwashjob = new JObject();
            JObject job = new JObject();
            JArray jar = new JArray();
            int successcount = 0;
            int errcount = 0;
            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
            string api = "";
            string result = "";
            string Token = "IBGcPGQieY3m8518D1XIeVn65Zpm11GffuPR7oy/YDlIw5Xtx0SeMw==";

            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForYancheng();
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
                                string funingurl = "";
                                if (jso.ContainsKey("funingurl"))
                                {
                                    funingurl = jso.GetValue("funingurl").ToString();
                                    jso.Remove("funingurl");
                                }
                                string url = jso.GetValue("siteuploadurl").ToString();
                                api = jso.GetValue("post").ToString();
                                string realapi = api;


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
                                else if (api.Equals("/yancheng/cheliang/clwcx"))          //车辆未冲洗接口
                                {
                                    realapi = "/yancheng/cheliang/clwcx";
                                    carwashjob.Add("xmbh", jso.GetValue("xmbh").ToString());
                                    carwashjob.Add("deviceNo", jso.GetValue("deviceNo").ToString());
                                }
                                else if (api.Contains("Device/AddDeviceFacture"))           //设备信息上传
                                {
                                    realapi = "/yancheng/Device/AddDeviceFacture";
                                }

                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");



                                if (api.Contains("/yancheng/cheliang/clwcx"))
                                {
                                    jar = new JArray();
                                    jar.Add(jso);
                                    jso = new JObject();
                                    jso.Add("Token", Token);
                                    jso.Add("JSON", jar);
                                }
                                else
                                {
                                    var data = jso;
                                    jso = new JObject();
                                    jso.Add("Token", Token);
                                    jso.Add("JSON", data);
                                }

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
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                        }
                                        successcount += successcount;
                                    }
                                }

                                if (carwashjob.ContainsKey("xmbh") && carwashjob.ContainsKey("deviceNo"))           //车辆未冲洗设备告警接口
                                {
                                    realapi = "/yancheng/cheliang/clwcxRegisterAlarm";
                                    jar = new JArray();
                                    jso = new JObject();
                                    jso.Add("Token", Token);
                                    carwashjob.Add("devstatus", "1");
                                    jar.Add(carwashjob);
                                    jso.Add("JSON", jar);
                                    result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                    carwashjob = new JObject();
                                    LogEntity = new CityUploadOperateLog
                                    {
                                        //Id=Guid.NewGuid().ToString(),
                                        url = url,
                                        api = realapi,
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
                                                await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                            }
                                            successcount += successcount;
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
