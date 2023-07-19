using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Quartz;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Center.Jobs;
using XHS.Build.Model.Models;
using XHS.Build.Services.TaskQz;
using XHS.Build.Services.TestMongoDBService;
/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace XHS.Build.Center
{
    /// <summary>
    /// 
    /// </summary>
    public class JobQuartzMongoLogDelete : JobBase, IJob
    {
        private readonly ITasksQzServices _tasksQzServices;
        private readonly ILogService _logService;
        private readonly ILogger<JobQuartzMongoLogDelete> _logger;
        public JobQuartzMongoLogDelete(ITasksQzServices tasksQzServices, ILogService logService, ILogger<JobQuartzMongoLogDelete> logger)
        {
            _tasksQzServices = tasksQzServices;
            _logService = logService;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {

            //var param = context.MergedJobDataMap;
            // 可以直接获取 JobDetail 的值
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;

            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));

        }
        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("JobQuartzMongoLogDelete定时任务在跑的：" + DateTime.Now.ToString());
            DateTime now = DateTime.Now;
            //保留3个月数据
            DateTime date = now.AddMonths(-3);

      
            Expression<Func<OpenApiOperateLog, bool>> whereExpression = e => e.CreateTime <= date;
            var filter = Builders<OpenApiOperateLog>.Filter.Where(whereExpression); //声明过滤条件
            long result = await _logService.DeleteObjectsAsync(whereExpression);
            _logger.LogInformation("OpenApiOperateLog删除结束：共删除 " + result + " 条记录。");
            if (jobid > 0)
            {
                var model = await _tasksQzServices.QueryById(jobid);
                if (model != null)
                {
                    //model.RunTimes += 1;
                    var separator = "<br>";
                    model.Remark =
                        $"【{DateTime.Now}】执行任务【Id：{context.JobDetail.Key.Name}，组别：{context.JobDetail.Key.Group}】【执行成功】{separator}";

                    await _tasksQzServices.Update(model);
                }
            }
        }

    }
}
