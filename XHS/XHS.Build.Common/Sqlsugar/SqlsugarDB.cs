using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Common.Configs;

namespace XHS.Build.Common.Sqlsugar
{
    public static class SqlsugarDB
    {
        public static void AddSqlsugar(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var dbConfig = configuration.GetSection("DBConfig").Get<List<DbConfig>>();

            services.AddScoped<ISqlSugarClient>(o =>
            {

                var configs = new List<ConnectionConfig>();

                dbConfig.ForEach(ii =>
                {
                    configs.Add(new ConnectionConfig()
                    {
                        ConfigId = ii.ConnId,
                        ConnectionString = ii.ConnectionString,
                        DbType = (DbType)ii.DBType,
                        IsAutoCloseConnection = true,
                        IsShardSameThread = false,
                    });
                });


                var db = new SqlSugarClient(configs);
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //我可以在这里面写逻辑
                };
                return db;
                //// 连接字符串
                //var Config = new ConnectionConfig()
                //{
                //    ConnectionString = dbConfig.ConnectionString,
                //    DbType = (DbType)dbConfig.DBType,
                //    IsAutoCloseConnection = true,
                //    IsShardSameThread = false,
                //};
                //var db= new SqlSugarClient(Config);
                ////每次Sql执行前事件
                //db.Aop.OnLogExecuting = (sql, pars) =>
                //{
                //    //我可以在这里面写逻辑
                //};
                //return db;
            });
        }
    }
}
