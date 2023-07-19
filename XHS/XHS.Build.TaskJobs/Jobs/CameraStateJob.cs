using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Services.AqtUpload;
using XHS.Build.TaskJobs.Jobs;
using Newtonsoft.Json;
using XHS.Build.Service.Video;
using XHS.Build.Services.Cameras;

namespace XHS.Build.TaskJobs
{
    /// <summary>
    /// 省对接接口数据获取（3号文档）
    /// </summary>
    /// <returns></returns>
    public class CameraStateJob : JobBase, IJob
    {
        private readonly ILogger<CameraStateJob> _logger;
        private readonly IHkOpenApiService _hkOpenApiService;
        private readonly ICameraService _cameraService;

        public CameraStateJob(ILogger<CameraStateJob> logger, IHkOpenApiService hkOpenApiService, ICameraService cameraService)
        {
            _logger = logger;
            _hkOpenApiService = hkOpenApiService;
            _cameraService = cameraService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("数据处理开始。", true);
            //
            int minute = DateTime.Now.Minute;
            int upstatehis = 0;
            // 取每小时中第一次插状态历史
            if(minute < 5)
            {
                upstatehis = 1;
            }

            var result = _hkOpenApiService.UpdateCameraState(await _cameraService.CameraCodeSpliceAsync("18,24"), 18, upstatehis);
            _logger.LogInformation(result.ToString(), true);


            result = _hkOpenApiService.UpdateCameraState(await _cameraService.CameraCodeSpliceAsync("16,23"), 16, upstatehis);
            _logger.LogInformation(result.ToString(), true);


            _logger.LogInformation("数据处理结束。", true);
        }
    }
}
