using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Common;
using Newtonsoft.Json.Linq;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Cameras;
using System.Text.RegularExpressions;
using MySqlX.XDevAPI.Common;
using XHS.Build.Model.Base;
using Util;

namespace XHS.Build.Service.Video
{
    /// <summary>
    /// 海康8200OpenApi连接助手
    /// </summary>
    public class Hk8200ApiService : BaseServices<BaseEntity>, IHk8200ApiService
    {

        /// <summary>
        /// 平台APPKey
        /// </summary>
        private static string _appkey;

        /// <summary>
        /// 平台host
        /// </summary>
        private static string _host;

        /// <summary>
        /// 平台APPSecret
        /// </summary>
        private static string _secret;

        /// <summary>
        /// 是否使用HTTPS协议
        /// </summary>
        private static bool _isHttps = true;

        public const string URL_HLS = "https://{0}/artemis/api/mss/v1/hls/{1}?indexCode={1}";
        public const string URL_RTSP = "https://{0}/artemis/api/vms/v1/rtsp/basic/{1}?indexCode={1}";
        public const string PTZ = "https://{0}/artemis/api/video/v1/ptz?cameraIndexCode={1}&action={2}&command={3}";
        private const char QUESTION = '?';
        private const string GET = "GET";
        private const string POST = "POST";
        private readonly IHpSystemSetting _systemSettingService;
     
        public Hk8200ApiService(IHpSystemSetting systemSettingService)
        {
            _systemSettingService = systemSettingService;
            _appkey = _systemSettingService.getSettingValue(Const.Setting.S158);
            _secret = _systemSettingService.getSettingValue(Const.Setting.S159);
            _host = _systemSettingService.getSettingValue(Const.Setting.S157);
        }


        /// <summary>
        /// 获取摄像头直播流地址
        /// </summary>
        /// <param name="cameracode">摄像头编号</param>
        /// <param name="channel">通道号</param>
        /// <param name="cameraparam">参数</param>
        /// <param name="streamType">码流类型(0-主码流,1-子码流),未填默认为主码流</param>
        /// <param name="protocol">协议类型（rtsp-rtsp协议,rtmp-rtmp协议,hls-hLS协议），未填写为rtsp协议</param>
        /// <param name="transmode">协议类型( 0-udp，1-tcp),默认为tcp</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> GetRealurl(BnCamera bc)
        {
          
            string protocol = "rtsp";
            string url_protocol = URL_RTSP;
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
           
            if (!string.IsNullOrEmpty(bc.protocol))
            {
                protocol = bc.protocol;
            }
            if(protocol.ToLower() == "hls")
            {
                url_protocol = URL_HLS;
            }
           
            string url = string.Format(url_protocol, _host, bc.cameracode);
            try
            {
                string result = GetRealUrl(url, bc.cameracode);
                if (string.IsNullOrEmpty(result))
                {
                    bnCameraResult.code = "1001";
                    bnCameraResult.msg = "返回内容为空";
                    return bnCameraResult;
                }
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(result);
                if (mJObj["code"].ToString() == "0")//成功
                {
                    bnCameraResult.code = mJObj["code"].ToString();
                    bnCameraResult.msg = mJObj["msg"].ToString();
                    bnCameraResult.url = mJObj["data"].ToString();
                    bnCameraResult.hasplayback = "0";
                    bnCameraResult.hasptz = "1";
                }
                else
                {
                    bnCameraResult.code = mJObj["code"].ToString();
                    bnCameraResult.msg = mJObj["msg"].ToString();
                    return bnCameraResult;
                }

            }
            catch (Exception ex)
            {
                bnCameraResult.code = "1002";
                bnCameraResult.msg = ex.Message;
                return bnCameraResult;
            }

            return bnCameraResult;


            //return bnCameraResult;
        }

        public static string GetRealUrl(string url, string indexCode)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 5000;
                SortedDictionary<string, string> param = new SortedDictionary<string, string>();
                param.Add("indexCode", indexCode);
                SortedDictionary<string, string> dd = signHeader(url.Split(QUESTION)[0], GET, param, null);
                if (_isHttps)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(remoteCertificateValidate);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }         
                WebHeaderCollection wheader = new WebHeaderCollection();
                string[] kk = dd.Keys.ToArray();
                for (int i = 0; i < kk.Length; i++)
                {
                    wheader.Add(kk[i], dd[kk[i]]);
                }
                request.Headers = wheader;

                response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ex.Message;
            }
        }



        /// <summary>
        /// 远程证书验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns>验证是否通过，始终通过</returns>
        private static bool remoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
        /// <summary>
        /// 云台操作，对监控点云台方向，转动速度进行操作。
        /// </summary>
        /// <param name="cameracode">摄像头编号</param>
        /// <param name="action">开始或停止操作(1 开始 0 停止)</param>
        /// <param name="command">控制命令(不区分大小写) 说明： LEFT 左转 RIGHT 右转 UP 上转 DOWN 下转 ZOOM_IN 焦距变大 ZOOM_OUT 焦距变小 
        ///                       LEFT_UP 左上 LEFT_DOWN 左下 RIGHT_UP 右上 RIGHT_DOWN 右下 FOCUS_NEAR 焦点前移 FOCUS_FAR 焦点后移 
        ///                       IRIS_ENLARGE 光圈扩大 IRIS_REDUCE 光圈缩小 以下命令presetIndex不可为空： GOTO_PRESET到预置点
        /// <param name="presetIndex"> 预置点编号(取值范围为1-128)</param>
        /// <param name="speed">云台速度(取值范围为1-10，默认4)</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> ptz(BnCamera bc)
        {  
            string speed = "4";
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            if (string.IsNullOrEmpty(bc.action)|| string.IsNullOrEmpty(bc.command))
            {
                bnCameraResult.code = "1004";
                bnCameraResult.msg = "云台指令参数错误";
                return bnCameraResult;
            }

            if (!string.IsNullOrEmpty(bc.speed))
            {
                speed = bc.speed;
            }
            string url = string.Format(PTZ, _host, bc.cameracode, bc.action, bc.command);
            try
            {
                string result = SetPtz(url, bc.cameracode, bc.action, bc.command);
                if (string.IsNullOrEmpty(result))
                {
                    bnCameraResult.code = "1001";
                    bnCameraResult.msg = "返回内容为空";
                    return bnCameraResult;
                }
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(result);
                bnCameraResult.code = mJObj["code"].ToString();
                bnCameraResult.msg = mJObj["msg"].ToString();
              
             }
            catch (Exception ex)
            {
                bnCameraResult.code = "1002";
                bnCameraResult.msg = ex.Message;
                return bnCameraResult;
            }


            return bnCameraResult;
        }

        public static string SetPtz(string url, string indexCode, string action, string command)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 5000;
                SortedDictionary<string, string> param = new SortedDictionary<string, string>();
                param.Add("cameraIndexCode", indexCode);
                param.Add("action", action);
                param.Add("command", command);
                SortedDictionary<string, string> dd = signHeader(url.Split(QUESTION)[0], GET, param, null);
                if (_isHttps)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(remoteCertificateValidate);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                WebHeaderCollection wheader = new WebHeaderCollection();
                string[] kk = dd.Keys.ToArray();
                for (int i = 0; i < kk.Length; i++)
                {
                    wheader.Add(kk[i], dd[kk[i]]);
                }
                request.Headers = wheader;

                response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ex.Message;
            }
        }



        private static SortedDictionary<string, string> signHeader(string url, string method, SortedDictionary<string, string> param, string data)
        {
            SortedDictionary<string, string> headers = new SortedDictionary<string, string>();

            Regex r = new Regex(@"https?:\/\/[\w.]+(:[0-9]*)?");
            url = r.Replace(url, string.Empty);

            headers["X-Ca-Timestamp"] = Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            headers["X-Ca-Key"] = _appkey;
            headers["Accept"] = "*/*";
            string _signString = signString(method, headers, url, param, data);
            headers["X-Ca-Signature"] = UEncrypter.HmacSHA256(_signString, _secret);
            return headers;
        }

        private static string signString(string method, SortedDictionary<string, string> headers, string url, SortedDictionary<string, string> param, string data)
        {
            List<string> list = new List<string>();

            method = method.ToUpper();
            list.Add(method);
            list.Add(headers["Accept"]);

            if (!headers.ContainsKey("Content-Type") || string.IsNullOrEmpty(headers["Content-Type"]))
            {
                if (string.IsNullOrEmpty(data))
                {
                    headers["Content-Type"] = "application/x-www-form-urlencoded;charset=UTF-8";
                }
                else
                {
                    headers["Content-Type"] = "text/plain;charset=UTF-8";
                }
            }

            list.Add(headers["Content-Type"]);

            if (headers.ContainsKey("Date") && !string.IsNullOrEmpty(headers["Date"]))
                list.Add(headers["Date"]);

            List<string> keys = new List<string>();
            foreach (string key in headers.Keys)
            {
                if (key.IndexOf("X-", 0) >= 0)
                {
                    list.Add(key.ToLower() + ":" + headers[key]);
                    keys.Add(key);
                }
            }

            headers["X-Ca-Signature-Headers"] = string.Join(",", keys).ToLower();

            list.Add(buildUrl(url, param));

            return string.Join("\n", list.ToArray());
        }

        private static string buildUrl(string url, SortedDictionary<string, string> param)
        {
            if (param != null && param.Count > 0)
            {
                StringBuilder sb = new StringBuilder(Const.Symbol.QUESTION);
                foreach (var item in param)
                {
                    sb.AppendFormat("{0}={1}&", item.Key, item.Value);
                }
                sb.Remove(sb.Length - 1, 1);
                return url + sb.ToString();
            }
            return url;
        }

        /// <summary>
        /// 云台控制指令
        /// </summary>
        public enum PtzOP
        {
            //控制命令(不区分大小写) 说明： 
            LEFT,           //左转
            RIGHT,          //右转
            UP,             //上转
            DOWN,           //下转
            ZOOM_IN,        //焦距变大
            ZOOM_OUT,       //焦距变小
            LEFT_UP,        //左上
            LEFT_DOWN,      //左下
            RIGHT_UP,       //右上
            RIGHT_DOWN,     //右下
            FOCUS_NEAR,     //焦点前移
            FOCUS_FAR,      //焦点后移
            IRIS_ENLARGE,   //光圈扩大
            IRIS_REDUCE,    //光圈缩小
                            //以下命令presetIndex不可为空： 
            SET_PRESET,     //设置预置点 
            GOTO_PRESET     //到预置点
        }


    }

    
}
