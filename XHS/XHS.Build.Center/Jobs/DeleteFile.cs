using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Model.TaskJobs;
using XHS.Build.Repository.Base;

namespace XHS.Build.Center.Jobs
{
    public class DeleteFi : JobBase, IJob
    {
        private readonly ILogger<DeleteFi> _logger;
        private readonly IBaseRepository<BaseEntity> _baseServices;

        public DeleteFi(ILogger<DeleteFi> logger, IBaseRepository<BaseEntity> baseServices)
        {
            _logger = logger;
            _baseServices = baseServices;


        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("文件删除开始。", true);

            // 缩略图前缀
            string THUMB_PREFIX = "tmb_";
            // 压缩图前缀
            string COMPRESS_PREFIX = "comp_";
            var list = _baseServices.Db.Queryable<CenterFileEntity>().Where(it => it.bsuccess == 0).ToList();
            foreach (var item in list)
            {
                try
                {
                    //删除记录
                    await _baseServices.Db.Deleteable<CenterFileEntity>().Where(it => it.FILEID == item.FILEID).ExecuteCommandAsync();
                    //删除文件
                    string fileName = item.physicalpath.Substring(item.physicalpath.LastIndexOf('\\') + 1);
                    string root = item.physicalpath.Substring(0, item.physicalpath.LastIndexOf('\\'));
                    //删除缩略图
                    UFile.DeleteFile(Path.Combine(root, THUMB_PREFIX + fileName));
                    //删除压缩图
                    UFile.DeleteFile(Path.Combine(root, COMPRESS_PREFIX + fileName));
                    //删除原文件
                    UFile.DeleteFile(item.physicalpath);
                }
                catch (Exception ex)
                {
                    //删除失败恢复记录
                    int FileId = await _baseServices.Db.Insertable<CenterFileEntity>(item).ExecuteReturnIdentityAsync();

                    _logger.LogError("FileId:" + FileId + ":" + ex.Message);
                }
            }
            _logger.LogInformation("文件删除结束。", true);
        }
    }

}
