using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Services.DailyJob;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class DbBackClearJob : JobBase, IJob
    {
        private readonly ILogger<DbBackClearJob> _logger;
        private readonly IDailyJobService _dailyJobService;
        private readonly IConfiguration _configuration;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IOperateLogService _operateLogService;
        public DbBackClearJob(ILogger<DbBackClearJob> logger, IDailyJobService dailyJobService, IConfiguration configuration, IHpSystemSetting hpSystemSetting, IOperateLogService operateLogService)
        {
            _logger = logger;
            _dailyJobService = dailyJobService;
            _configuration = configuration;
            _hpSystemSetting = hpSystemSetting;
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
            //数据库备份
            try
            {
                string DB_NAME = "XJ_Env";
                _logger.LogInformation("数据库备份开始!", false);
                int dayDiff = 30;//时间间隔（单位：天）
                                 //清理数据库备份目录下过期文件
                string fullPath = Path.Combine(_configuration.GetSection("BatchConsole").GetValue<string>("DbBackupPath"), DB_NAME);
                UFile.ClearExpiredFile(fullPath, dayDiff, Const.FileEx.BAK_ALL, true, false);
                //执行备份
                _dailyJobService.Excute(fullPath, DB_NAME, _configuration.GetSection("BatchConsole").GetValue<int>("DbBackupTimeout"));
                _logger.LogInformation("数据库备份完成!", false);
            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message, true);
            }

            //清理日志
            try
            {
                string LOG_CLEAR = "执行日志清理.";
                UFile.ClearExpiredFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../xjlog"),
                    _configuration.GetSection("BatchConsole").GetValue<int>("LogSaveDay"), Const.FileEx.LOG_ALL, true, false);
                _logger.LogInformation(LOG_CLEAR, false);

            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message, true);
            }

            //清理临时文件夹内3日文件
            try
            {
                string TMP_CLEAR = "执行临时文件夹清理.";
                string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
                UFile.ClearExpiredFile(tmpPath, 3, Const.FileEx.ALL, true, false);
                _logger.LogInformation(TMP_CLEAR, false);
            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message, true);
            }

            //清理验证码
            try
            {
                char SEPARATOR = ';';
                string codePath = _configuration.GetSection("BatchConsole").GetValue<string>("TmpCodePath");
                string[] codePathArray = codePath.Split(SEPARATOR);

                foreach (string path in codePathArray)
                {
                    if (string.IsNullOrEmpty(path) || !UFile.IsExistDirectory(path))
                        continue;

                    try
                    {
                        string CODE_CLEAR = "执行验证码清理.";
                        //默认保留3天
                        UFile.ClearExpiredFile(path, 3, Const.FileEx.ALL, true, false);
                        _logger.LogInformation(CODE_CLEAR, false);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message + Environment.NewLine + ex.StackTrace;
                        _logger.LogInformation(message, true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, true);
            }

            try
            {
                await _operateLogService.ClearMongoDBLog();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
