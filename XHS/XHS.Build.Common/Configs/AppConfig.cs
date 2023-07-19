using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Configs
{
    public class AppConfig
    {
        /// <summary>
        /// 跨域地址
        /// </summary>
        public string[] CorUrls { get; set; }

        /// <summary>
        /// 日志配置
        /// </summary>
        public LogConfig Log { get; set; } = new LogConfig();
        /// <summary>
        /// 限流
        /// </summary>
        public bool RateLimit { get; set; } = false;

    }
}
