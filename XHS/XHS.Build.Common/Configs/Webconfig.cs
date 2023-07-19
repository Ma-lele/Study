using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Configs
{
    /// <summary>
    /// Webconfig静态类
    /// </summary>
    public class Webconfig
    {
        /// <summary>
        /// 网站菜单前缀
        /// </summary>
        public string MPrefix { get; set; }
        /// <summary>
        /// 应用网站地址
        /// </summary>
        public string AppUrl { get; set; }
        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 是否为开发Debug模式
        /// </summary>
        public bool IsDebug { get; set; }
        /// <summary>
        /// 是否为后台管理模式
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 允许无验证码尝试登录次数(0为必须输入验证码)
        /// </summary>
        public int LoginTriedTime { get; set; }
        /// <summary>
        /// 手机应用apk名
        /// </summary>
        public string ApkName { get; set; }
        /// <summary>
        /// 公众号ID
        /// </summary>
        public string WUsername { get; set; }
        /// <summary>
        /// 公众号二维码图片地址
        /// </summary>
        public string WQrCodeUrl { get; set; }
        /// <summary>
        /// 接收服务器IP
        /// </summary>
        public string HJT212Ip { get; set; }
        /// <summary>
        /// 接收服务器端口
        /// </summary>
        public int HJT212Port { get; set; }
        /// <summary>
        /// 雾泡服务器IP
        /// </summary>
        public string EquipServerIp { get; set; }
        /// <summary>
        /// 雾泡服务器端口
        /// </summary>
        public int EquipServerPort { get; set; }
        /// <summary>
        /// 雾泡公钥
        /// </summary>
        public string EquipPublicKey { get; set; }
        /// <summary>
        /// 单点登录服务器
        /// </summary>
        public string CasServer { get; set; }
        /// <summary>
        /// 单点登录代理服务器
        /// </summary>
        public string CasServerAgent { get; set; }
        /// <summary>
        /// 单点登录页面
        /// </summary>
        public string LoginUri { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string EnvConnectionString { get; set; } = "";


        public string PrivateKey { get; set; }
    }
}
