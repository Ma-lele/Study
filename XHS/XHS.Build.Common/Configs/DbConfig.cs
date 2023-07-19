using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Configs
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbConfig
    {
        /// <summary>
        /// 数据库id
        /// </summary>
        public string ConnId { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DBType { get; set; }

        /// <summary>
        /// 数据库字符串
        /// </summary>
        public string ConnectionString { get; set; }
        ///
        /// 监听所有操作
        /// </summary>
        public bool MonitorCommand { get; set; } = false;

        /// <summary>
        /// 监听Curd操作
        /// </summary>
        public bool Curd { get; set; } = false;
    }

    public enum DataBaseType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4
    }
}
