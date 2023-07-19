using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApplication1.Models
{
    public class IM_Content
    {
        public int id { get; set; }
        /// <summary>
        /// 发消息人编号
        /// </summary>
        public int To_empid { get; set; }
        /// <summary>
        /// 接收消息人编号
        /// </summary>
        public int Form_empid { get; set; }
        public string Mes_content { get; set; }
        public DateTime Addtime { get; set; }


        public string To_ename { get; set; }
        public string Form_ename { get; set; }



       
        /// <summary>
        /// 保存聊天消息
        /// </summary>
        /// <param name="To_empid">发送人ID</param>
        /// <param name="form_empid">接收人ID</param>
        /// <param name="contentmes">内容消息</param>
        /// <returns></returns>
        public bool ToSendMes(IM_Content entity)
        {
            var db = GetInstance();
            /// "id", "To_ename", "Form_ename" 这三列不进行添加
            int count =  db.Insertable(entity).IgnoreColumns("id", "To_ename", "Form_ename").ExecuteReturnIdentity();
            return count > 0;
        }



            /// <summary>
            /// 查询当前用户与别一用户的聊天记录
            /// </summary>
            /// <param name="To_empid">发消息用户编号【当前用户编号】</param>
            /// <param name="form_empid">收消息用户编号</param>
            /// <returns></returns>
            public List<IM_Content> QueryList(int To_empid, int form_empid)
        {
            string sql = string.Empty;

            sql += "            select                                                        ";
            sql += "*                                                                         ";
            sql += ",                                                                         ";
            sql += "(select e.ename from[IM_Emp] as e where e.id = c.To_empid ) to_ename,     ";
            sql += "(select e.ename from[IM_Emp] as e where e.id = c.form_empid ) form_ename  ";
            sql += "                                                                          ";
            sql += " from[IM_Content] as c                                                    ";
            sql += " where                                                                    ";
            sql += " (c.To_empid = @To_empid and c.form_empid = @form_empid)                                    ";
            sql += " or                                                                       ";
            sql += " (c.To_empid = @form_empid and c.form_empid = @To_empid)                                    ";
            var db = GetInstance();

            ///不区分大小写的比较查询

            //参数2
            var dt = db.Ado.SqlQuery<IM_Content>(sql,new { To_empid, form_empid });

            return dt;
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