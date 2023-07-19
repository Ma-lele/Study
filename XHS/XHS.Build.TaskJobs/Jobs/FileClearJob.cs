using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Services.File;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class FileClearJob:JobBase,IJob
    {
        private readonly ILogger<FileClearJob> _logger;
        private readonly IFileService _fileService;
        public FileClearJob(ILogger<FileClearJob> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            await _fileService.DeleteFile();
        }
    }
}
