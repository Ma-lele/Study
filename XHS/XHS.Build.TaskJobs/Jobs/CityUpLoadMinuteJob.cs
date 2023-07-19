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
    public class CityUpLoadMinuteJob : JobBase, IJob
    {
        private readonly ILogger<CityUpLoadMinuteJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly AqtToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly IVideoService _videoService;
        public CityUpLoadMinuteJob(ILogger<CityUpLoadMinuteJob> logger, IVideoService videoService, IOperateLogService operateLogService, IHpSystemSetting hpSystemSetting, AqtToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;  
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
            _hpSystemSetting = hpSystemSetting;
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
            Dictionary<string,string> dic = new Dictionary<string, string>();
            //账号
            string aqtaccount = _hpSystemSetting.getSettingValue(Const.Setting.S178);
            //密码
            string aqtpassword = _hpSystemSetting.getSettingValue(Const.Setting.S179);
            if (string.IsNullOrEmpty(aqtaccount))
            {
                _logger.LogInformation("数据上传结束。未设置上传账号。", true);
                return;
            }
           
            DataSet ds = await _aqtUploadService.GetListMinute();
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
                            string token = _cityToken.getAqtToken();
                            if (string.IsNullOrEmpty(token))
                            {
                                _logger.LogInformation("分钟数据上传结束（对方鉴权获取失败）。", true);
                                return;
                            }
                            try
                            {
                                if (api.Contains("UploadVideo")) //上传视频监控点信息
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
                                    jso.Add("url", a.url);
                                }
                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                           
                                string result = _cityToken.JsonRequest(api, JsonConvert.SerializeObject(jso), aqtaccount, aqtpassword);
                                    
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
                                    errcount += errcount;
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
