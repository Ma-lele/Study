using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace XHS.Build.Common.Helps
{
    /// <summary>
    /// 雾泡命令助手
    /// </summary>
    public partial class HpFog
    {
        public const string OK = "ok";
        public const string NG = "ng";
        public const string UP = "up";
        public const string DOWN = "down";
        public const string PUBLICKEY = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCs9+/BDsS0YGvBigaL9wP7xAo0uw9SXMTQKEvSNEdPrZ30N2A9VFZnLH6c8AExqJ+Y28AMtaembWdifMu3/39yNtlYVzhLABEdPfezjXvkRga57qKttEKzGvQq0lDbaZtJTOffFbdIcx2NoKhakTy2XqMsp4puRA5n+Pgzq5TmXwIDAQAB";
        public static Encoding ENCODING = Encoding.UTF8;
        private static Config CONFIG = new Config();

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="bc">指令Bean</param>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        /// <param name="publickey">公钥</param>
        public static void SendCommand(BnCmd bc, string ip = "127.0.0.1", int port = 9021, string publickey = PUBLICKEY)
        {
            try
            {
                CONFIG.ServerIP = ip;
                CONFIG.Port = port;
                CONFIG.PublicKey = publickey;

                string command = GetCommand(bc, false);
                command = UEncrypter.EncryptByRSA16(command, publickey);

                SendAsClient(command);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, APPNAME);
            }
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="buffer">字节数组</param>
        public static void Send(byte[] buffer)
        {
            string strBuffer = ENCODING.GetString(buffer);
            SendAsClient(strBuffer);
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="strBuffer">字符串</param>
        public static void SendAsClient(string strBuffer)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPAddress ip;
                if (!IPAddress.TryParse(CONFIG.ServerIP, out ip))
                {
                    IPHostEntry host = Dns.GetHostByName(CONFIG.ServerIP);
                    ip = host.AddressList[0];
                    CONFIG.ServerIP = ip.ToString();
                }

                IPEndPoint ipe = new IPEndPoint(ip, CONFIG.Port);
                clientSocket.Connect(ipe);
                byte[] buffer = ENCODING.GetBytes(strBuffer);
                clientSocket.Send(buffer);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, APPNAME);
            }
            finally
            {
                clientSocket.Close();
            }
        }

        /// <summary>
        /// 获取指令字符串
        /// </summary>
        /// <param name="bc">指令Bean</param>
        /// <returns></returns>
        public static string GetCommand(BnCmd bc)
        {
            return GetCommand(bc, true);
        }
        /// <summary>
        /// 获取指令字符串
        /// </summary>
        /// <param name="bc">指令Bean</param>
        /// <param name="benter">是否需要回车.默认:true</param>
        /// <returns></returns>
        public static string GetCommand(BnCmd bc, bool benter)
        {
            StringBuilder sb = new StringBuilder(Const.Symbol.BRACES_START);
            Type t = bc.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                string value = Convert.ToString(p.GetValue(bc, null));
                if (!string.IsNullOrEmpty(value))
                {
                    sb.AppendFormat("{0}{1}{2}{3}", p.Name, Const.Symbol.EQUAL, value, Const.Symbol.SEMICOLON);
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(Const.Symbol.BRACES_END);
            if (benter)
                sb.Append(Environment.NewLine);

            string result = sb.ToString();
            return result;
        }

        /// <summary>
        /// 配置类
        /// </summary>
        public class Config
        {
            public string ServerIP { get; set; }
            public int Port { get; set; }
            public string PublicKey { get; set; }

        }
        /// <summary>
        /// 指令结合
        /// </summary>
        public class CMD
        {
            public const string ONLINE = "online";//下位机上传登录信息
            public const string ON = "on";//开启
            public const string OFF = "off";//关闭
            public const string HEARTBEAT = "hb";//心跳
        }

        /// <summary>
        /// 指令Bean
        /// </summary>
        public class BnCmd
        {
            /// <summary>
            /// 用户ID
            /// </summary>
            public string USERID { get; set; }
            /// <summary>
            /// 设备编号
            /// </summary>
            public string fc { get; set; }
            /// <summary>
            /// 命令
            /// </summary>
            public string cmd { get; set; }
            /// <summary>
            /// 延时
            /// </summary>
            public string delay { get; set; }
            /// <summary>
            /// 方向
            /// </summary>
            public string direct { get; set; }
            /// <summary>
            /// 结果
            /// </summary>
            public string msg { get; set; }
            /// <summary>
            /// 开关
            /// </summary>
            public string sw { get; set; }
            /// <summary>
            /// 指令全文
            /// </summary>
            public string command;

        }
    }
}
