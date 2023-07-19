using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.Device;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    /// <summary>
    /// 新合盛申请平台设备在线率统计数据上传
    /// </summary>
    /// <returns></returns>
    public class XHSMotionDataUpLoadDayJob : JobBase, IJob
    {
        private readonly ILogger<XHSMotionDataUpLoadDayJob> _logger;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IOperateLogService _operateLogService;
        private readonly IDeviceService _deviceService;
        private readonly IAqtUploadService _aqtUploadService;
        public XHSMotionDataUpLoadDayJob(ILogger<XHSMotionDataUpLoadDayJob> logger, IAqtUploadService aqtUploadService, IDeviceService deviceService, IOperateLogService operateLogService, IHpSystemSetting hpSystemSetting)
        {
            _logger = logger;
            _operateLogService = operateLogService;
            _hpSystemSetting = hpSystemSetting;
            _deviceService = deviceService;
            _aqtUploadService = aqtUploadService;
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
            string url = _hpSystemSetting.getSettingValue("S184");
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogInformation("上传地址未配置，数据上传结束。", false);
                return;
            }
            string PlatformID = _hpSystemSetting.getSettingValue("S185");
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogInformation("PlatformID未配置，数据上传结束。", false);
                return;
            }
            string now = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, "project/receiveonlinedata");
            if (citydt.Rows.Count > 0)
            {
                DataRow citydr = citydt.Rows[0];
                now = citydr["uploadtime"].ToString();
            }
            
            DateTime enddate = now.ToDateTime();
            DateTime startdate = now.ToDateTime().AddDays(-1);
           

            DataTable dt = await _deviceService.GetDevOnline(startdate, enddate);
            JArray jarr = new JArray();
            JObject resultjso = new JObject();
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
                    jarr.Add(jso);
                }
            }
            resultjso.Add("PlatformID", PlatformID);
            resultjso.Add("data", jarr);
            JObject jsonObject = new JObject();
            String accessToken = "";
            jsonObject.Add("loginName", "dataseeker");
            jsonObject.Add("password", "wiaXNhZG1pbiI6InRydWUiLCJyZWZ");
            string retString = HttpNetRequest.POSTSendJsonRequest(url + "login/login", jsonObject.ToString(), null);
            if (!string.IsNullOrEmpty(retString))
            {
                var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                if (resObj != null && resObj.Code == 0)
                {
                    accessToken = resObj.data["token"].ToString();
                }
            }
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogInformation("上传Token获取失败，数据上传结束。", false);
                return;
            }
            Dictionary<string, object> postData = new Dictionary<string, object>();
            postData.Add("json", resultjso);
            retString = HttpNetRequest.PostForm(url + "project/receiveonlinedata", postData, new Dictionary<string, string>() { { "Authorization", "Bearer " + accessToken } });
            JObject resultjob = new JObject(retString);
            if (resultjob.GetValue("success").ToBool())
            {
                enddate = enddate.AddDays(1);
                await _aqtUploadService.UpdateCityUploadDate(url, "project/receiveonlinedata", enddate);
            }
            else
            {
                _logger.LogInformation("数据上传失败。"+ resultjob.GetValue("msg").ToString(), true);
                return;
            }            

            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
