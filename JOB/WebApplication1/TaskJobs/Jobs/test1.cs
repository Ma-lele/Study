using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskJobs.Jobs
{
    public class test1 : JobBase, IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }
        public async Task Run(IJobExecutionContext context, int jobid)
        {
            //创建数据库对象
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=1.117.195.24,1433;Initial Catalog=WebSocket;User ID=malele;PWD=mll.123",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });

            db.Insertable(new Student() { id = DateTime.Now.ToString() + "test", sex = 1, Datenow = DateTime.Now }).ExecuteCommand();


        }
    }
}
