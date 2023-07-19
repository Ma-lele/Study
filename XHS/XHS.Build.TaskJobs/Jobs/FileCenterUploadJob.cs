using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.File;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class FileCenterUploadJob : JobBase,IJob
    {
        private readonly ILogger<FileCenterUploadJob> _logger;
        private readonly IFileService _fileService;
        private readonly CenterToken _centerToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        public FileCenterUploadJob(ILogger<FileCenterUploadJob> logger, IHpSystemSetting hpSystemSetting,CenterToken centerToken, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
            _centerToken = centerToken;
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
            var Files = await _fileService.Query(f => f.bdel == 0 && f.centerup == 0 && f.filetype == "CaRound");
            try { 
                if (Files.Count > 0)
                {
                    string STR_RESOURSE = "/resourse/";
                    var S034 = _hpSystemSetting.getSettingValue(Const.Setting.S034);
                    foreach (var file in Files)
                    {
                        //删文件
                        string fileExt = Path.GetExtension(file.filename).ToLower();
                        string fname = "http://" + S034 + STR_RESOURSE + file.GROUPID + "/" + file.filetype + "/" + file.SITEID + "/" + file.linkid + "/" + file.FILEID + fileExt;

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("fileUrl", fname);
                        string result = _centerToken.FormRequest("http://test.center.xhs-sz.com:9029", "/api/File/HttpPost", dic);
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(result);
                        if (resObj.success)
                        {
                            JObject datajso = (JObject)resObj.data;
                            string url = datajso.GetValue("tmburl").ToString();
                            file.centerfileurl = url;
                            file.centerup = 1;
                            await _fileService.Update(file);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogError(message, true);
            }
        }
    }
}
