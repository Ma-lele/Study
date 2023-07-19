﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class SmartUpLoadMinuteGuannanJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadMinuteGuannanJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityGuannanToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadMinuteGuannanJob(ILogger<SmartUpLoadMinuteGuannanJob> logger, IOperateLogService operateLogService, SiteCityGuannanToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
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
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForGuannanMinute();
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
                            string token = _cityToken.getSiteCityToken(url, account, pwd);
                            if (string.IsNullOrEmpty(token))
                            {
                                _logger.LogInformation("分钟数据上传结束（对方鉴权获取失败）。", true);
                                return;
                            }
                            if (api.Contains("UploadHistory"))      //卸料平台实时数据上传
                            {
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
                                        int upstate = data1.GetValue("upstate").ToInt();
                                        jso.Add("dataType", upstate);
                                    }
                                }
                            }
                            else if (api.Contains("Craneinterface/UploadCraneHistory"))
                            {
                                //分钟
                                //塔机实时数据上传
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
                                    if (data1.ContainsKey("WindSpeed"))
                                    {
                                        double WindSpeed = data1.GetValue("WindSpeed").ToDouble();
                                        jso.Add("windSpeed", WindSpeed);
                                    }
                                    jso.Add("dip", 0);
                                    jso.Add("multiplyingPower", 0);
                                }
                            }
                            else if (api.Contains("Hoistinterface/HoistHistory"))
                            {
                                //分钟
                                //施工升降机实时数据上传
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
                                    if (data1.ContainsKey("Speed"))
                                    {
                                        double Speed = data1.GetValue("Speed").ToDouble();
                                        jso.Add("avgSpeed", Speed);
                                    }
                                    else
                                    {
                                        jso.Add("avgSpeed", 0);
                                    }
                                    jso.Add("WindSpeed", 0);
                                    jso.Add("dip", 0);
                                    jso.Add("multiplyingPower", 0);
                                    Random rd = new Random();
                                    if (rd.Next() % 2 == 0)
                                    {
                                        jso.Add("direction", 1);
                                    }
                                    else
                                    {
                                        jso.Add("direction", -1);
                                    }
                                    jso.Add("loadPercent", 0);
                                    jso.Add("beginHoistHeight", 0);
                                    jso.Add("endHoistHeight", 0);
                                    jso.Add("hoistDistance", 0);
                                    jso.Add("beginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jso.Add("endTime", DateTime.Now.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                            string realapi = api + "?appkey=" + account + "&appsecret=" + pwd;
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
                            if (jso.ContainsKey("rtdjson"))
                            {
                                jso.Remove("rtdjson");
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
