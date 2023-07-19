using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Configs
{
    public class WsConfig
    {
        /// <summary>
        /// 服务端地址+端口
        /// </summary>
        public string wsAddress { get; set; }
        public string privateKey { get; set; }
        public string publicKey { get; set; }
    }
}
