using Microsoft.AspNetCore.Builder;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskJobs
{
    public static class QuartzJobMildd
    {
        public static void UseQuartzJobMildd(this IApplicationBuilder app, ISchedulerCenter schedulerCenter)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            //创建数据库对象
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=1.117.195.24,1433;Initial Catalog=Test;User ID=malele;Pwd=mll.123",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
            try
            {
                var allQzServices = db.Queryable<TasksQz>().ToList();
                // var allQzServices = tasksQzServices.Query(a => a.IsStart == true).Result;
                foreach (var item in allQzServices)
                {
                    if (item.IsStart)
                    {
                        var ResuleModel = schedulerCenter.AddScheduleJobAsync(item).Result;
                        if (ResuleModel.Contains("成功"))
                        {
                            Console.WriteLine($"QuartzNetJob{item.Name}启动成功！");
                        }
                        else
                        {
                            Console.WriteLine($"QuartzNetJob{item.Name}启动失败！错误信息：{"cuole"}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
