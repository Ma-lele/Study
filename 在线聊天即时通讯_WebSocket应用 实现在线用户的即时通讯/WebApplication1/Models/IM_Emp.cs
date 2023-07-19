using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class IM_Emp
    {
        public int ID { get; set; }
        public string EName { get; set; }



        public IM_Emp FirstEnity(string ename,string pwd)
        {
            var db = GetInstance();

            ///不区分大小写的比较查询
            var getFirst = db.Queryable<IM_Emp>().First(it => it.EName.ToLower() == ename.ToLower());//查询单条
            return getFirst;
        }



        /// <summary>
        /// 读取所有员工
        /// </summary>
        /// <returns></returns>
        public List<IM_Emp> EmpList()
        {
            var db = GetInstance();

            ///不区分大小写的比较查询
            var getFirst = db.Queryable<IM_Emp>().ToList();
            return getFirst;
        }

        SqlSugarClient GetInstance()
        {
            //创建数据库对象
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "server=1.117.195.24;database=WebSocket;uid=malele;pwd=mll.123;",//连接符字串
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });

            //添加Sql打印事件，开发中可以删掉这个代码
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql);
            };
            return db;
        }
    }
}