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
    /// 苏州对接接口数据获取（扬尘，视频流）
    /// </summary>
    /// <returns></returns>
    public class CitySZUpLoadMinuteJob : JobBase, IJob
    {
        private readonly ILogger<CitySZUpLoadMinuteJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly SiteCityWuzhongToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly IVideoService _videoService;
        public CitySZUpLoadMinuteJob(ILogger<CitySZUpLoadMinuteJob> logger, IVideoService videoService, IOperateLogService operateLogService, SiteCityWuzhongToken cityToken, IAqtUploadService aqtUploadService)
        {
            _logger = logger;  
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
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
            _logger.LogInformation("分钟数据上传开始。", true);
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string result;
            JObject job = new JObject();
            Dictionary<string,string> dic = new Dictionary<string, string>();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            string uploadapi = "";
            string api = "";

            //省平台对接数据获取
            DataSet ds = await _aqtUploadService.GetListForSuzhouMinute();

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
                            string uploadurl = jso.GetValue("siteuploadurl").ToString();
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                             api = jso.GetValue("post").ToString();
                            string realapi = "ZhgdJgDataUpload/" + api.Split('/')[1];
                            jso.Remove("post");
                            jso.Remove("siteuploadurl");
                            jso.Remove("uploadaccount");
                            jso.Remove("uploadpwd");

                            string token = _cityToken.getSiteCityToken(uploadurl, account, pwd);
                            if (string.IsNullOrEmpty(token))
                            {
                                _logger.LogInformation("分钟数据上传结束（对方鉴权获取失败）。", true);
                                return;
                            }
                            if (api.Contains("DustInterface/UploadDustHistory"))
                            {
                                //扬尘实时数据
                                realapi = "ZhgdJgDataUpload/DayDustInfo";
                                if (jso.ContainsKey("GONGCHENG_CODE"))
                                {
                                    jso.Remove("GONGCHENG_CODE");
                                }
                                if (jso.ContainsKey("funingurl"))
                                {
                                    jso.Remove("funingurl");
                                }
                                if (jso.ContainsKey("projectInfoId"))
                                {
                                    jso.Remove("projectInfoId");
                                }
                                if (jso.ContainsKey("upload"))
                                {
                                    jso.Add("moniterTime", jso.GetValue("upload"));
                                    jso.Remove("upload");
                                }

                            }
                            else if (api.Contains("UploadVideo")) //上传视频监控点信息
                            {
                                BnCamera bn = new BnCamera();
                                bn.cameracode = jso.GetValue("videoId").ToString();
                                bn.cameratype = jso.GetValue("cameratype").ToInt();
                                bn.channel = jso.GetValue("channel").ToInt();
                                bn.protocol = "rtmp";
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




            _logger.LogInformation("分钟数据上传结束。", true);
        }
    }
}
