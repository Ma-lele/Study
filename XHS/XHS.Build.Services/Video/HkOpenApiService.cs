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
using XHS.Build.Model.Base;
using XHS.Build.Services.Cameras;

namespace XHS.Build.Service.Video
{
    /// <summary>
    /// 海康8200OpenApi连接助手
    /// </summary>
    public class HkOpenApiService : BaseServices<BaseEntity>, IHkOpenApiService
    {
        /// <summary>
        /// 平台host
        /// </summary>
        private static string _host;

        /// <summary>
        /// 平台APPKey
        /// </summary>
        private static string _appkey;

        /// <summary>
        /// 平台APPSecret
        /// </summary>
        private static string _secret;

        /// <summary>
        /// 是否使用HTTPS协议
        /// </summary>
        private static bool _isHttps = true;

        private readonly IHpSystemSetting _systemSettingService;
        private readonly ICameraService _cameraService;

        public HkOpenApiService(IHpSystemSetting systemSettingService, ICameraService cameraService)
        {
            _systemSettingService = systemSettingService;
            _cameraService = cameraService;
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
            string streamtype = "1";
            string protocol = "rtsp";
            string transmode = "1";
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            if (bc.cameratype == 18 || bc.cameratype == 24)
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S161);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S162);
                _host = _systemSettingService.getSettingValue(Const.Setting.S160);
            }
            else
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S098);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S099);
                _host = _systemSettingService.getSettingValue(Const.Setting.S097);
            }
            if (!string.IsNullOrEmpty(bc.streamtype))
            {
                streamtype = bc.streamtype;
            }
            if (!string.IsNullOrEmpty(bc.protocol))
            {
                protocol = bc.protocol;
            }
            if (!string.IsNullOrEmpty(bc.transmode))
            {
                transmode = bc.transmode;
            }
            //   HttpUtillib.SetPlatformInfo("21857458", "ThSirz29xa6BcJojMSCT", "video.xhs-sz.com", 443, true);

            string uri = "/artemis/api/video/v1/cameras/previewURLs";
            JObject jb = new JObject();
            jb.Add("cameraIndexCode", bc.cameracode);
            jb.Add("streamType", streamtype);
            jb.Add("protocol", protocol);
            jb.Add("transmode", transmode);
            if (protocol == "rtsp")
            {
                jb.Add("expand", "streamform=rtp");
            }
            byte[] response = HttpPost(uri, jb.ToString(), 15);
            string result = System.Text.Encoding.UTF8.GetString(response);
            JObject resjb = JObject.Parse(result);

            bnCameraResult.code = (string)resjb.GetValue("code");
            if (bnCameraResult.code == "0")
            {
                JObject urljb = (JObject)resjb.GetValue("data");
                //JObject urljb = JObject.Parse(data);
                bnCameraResult.url = (string)urljb.GetValue("url");
                bnCameraResult.hasplayback = "1";
                bnCameraResult.hasptz = "0";
                uri = "/artemis/api/resource/v1/cameras/indexCode";
                jb = new JObject();
                jb.Add("cameraIndexCode", bc.cameracode);
                response = HttpPost(uri, jb.ToString(), 15);
                result = System.Text.Encoding.UTF8.GetString(response);
                resjb = JObject.Parse(result);
                string code = (string)resjb.GetValue("code");
                if (code == "0")
                {
                    urljb = (JObject)resjb.GetValue("data");
                    int cameraType = (int)urljb.GetValue("cameraType");
                    if (cameraType > 0)
                    {
                        bnCameraResult.hasptz = "1";
                    }
                }

            }
            else
            {
                bnCameraResult.msg = (string)resjb.GetValue("msg");
            }


            return bnCameraResult;
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
        /// <param name="speed">云台速度(取值范围为1-100，默认40)</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> ptz(BnCamera bc)
        {
            string speed = "40";
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            if (bc.cameratype == 18 || bc.cameratype == 24)
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S161);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S162);
                _host = _systemSettingService.getSettingValue(Const.Setting.S160);
            }
            else
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S098);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S099);
                _host = _systemSettingService.getSettingValue(Const.Setting.S097);
            }
            if (!string.IsNullOrEmpty(bc.speed))
            {
                speed = bc.speed;
            }
            //   HttpUtillib.SetPlatformInfo("21857458", "ThSirz29xa6BcJojMSCT", "video.xhs-sz.com", 443, true);
            string action = "1";
            if (bc.action == "1")
            {
                action = "0";
            }
            string uri = "/artemis/api/video/v1/ptzs/controlling";
            JObject jb = new JObject();
            jb.Add("cameraIndexCode", bc.cameracode);
            jb.Add("action", action);
            jb.Add("command", bc.command);
            jb.Add("speed", speed);
            byte[] response = HttpPost(uri, jb.ToString(), 15);
            string result = System.Text.Encoding.UTF8.GetString(response);
            JObject resjb = JObject.Parse(result);


            bnCameraResult.code = (string)resjb.GetValue("code");
            if (bnCameraResult.code == "0")
            {
                bnCameraResult.msg = (string)resjb.GetValue("msg");
            }
            else
            {
                bnCameraResult.msg = (string)resjb.GetValue("msg");
            }


            return bnCameraResult;
        }


        /// <summary>
        /// 更新摄像头在线状态
        /// </summary>
        /// <param name="cameracode">逗号拼接摄像头编号</param>
        /// <returns></returns>
        public int UpdateCameraState(string cameracode, int type, int upstatehis = 0)
        {
            if (type == 18 || type == 24)
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S161);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S162);
                _host = _systemSettingService.getSettingValue(Const.Setting.S160);
            }
            else
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S098);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S099);
                _host = _systemSettingService.getSettingValue(Const.Setting.S097);
            }
            string uri = "/artemis/api/nms/v1/online/camera/get";
            JObject jb = new JObject();
            JArray jr = new JArray();
            byte[] response = new byte[] { };
            var i = 0;
            foreach (var code in cameracode.Split(','))
            {
                jr.Add(code);
                ++i;
                if (i % 500 == 0)
                {

                    jb.Add("indexCodes", jr);
                    jb.Add("pageSize", 500);
                    response = HttpPost(uri, jb.ToString(), 15);
                    if (response != null)
                    {
                        string result = System.Text.Encoding.UTF8.GetString(response);
                        JObject resjb = JObject.Parse(result);
                        var data = resjb["data"]["list"];
                        string codestr = string.Empty;
                        foreach (var item in data)
                        {
                            codestr += item["indexCode"].ToString() + ",";
                            JObject camerajob = JObject.Parse(item.ToString());
                            if (string.IsNullOrEmpty(item["online"].ToString()))
                            {
                                _cameraService.CamerabonlineUpdateAsync(camerajob.GetValue("indexCode").ToString(), 0, upstatehis);
                            }
                            else
                            {
                                _cameraService.CamerabonlineUpdateAsync(camerajob.GetValue("indexCode").ToString(), (int)camerajob.GetValue("online"), upstatehis);
                            }
                        }
                        codestr = codestr.TrimEnd(',');
                        List<string> listjrcode = jr.ToObject<List<string>>();
                        List<string> listcode = codestr.Split(',').ToList();
                        List<string> list3 = listjrcode.Except(listcode).ToList();//未获取信息编号
                        foreach (var item in list3)
                        {
                            _cameraService.CamerabonlineUpdateAsync(item.ToString(), 0, upstatehis);
                        }
                    }
                    else
                    {
                        return -1;
                    }
                    jr = new JArray();
                    jb = new JObject();
                }
            }
            if (jr != null)
            {
                jb.Add("indexCodes", jr);
                jb.Add("pageSize", 500);
                response = HttpPost(uri, jb.ToString(), 15);
                if (response != null)
                {
                    string result = System.Text.Encoding.UTF8.GetString(response);
                    JObject resjb = JObject.Parse(result);
                    var data = resjb["data"]["list"];
                    string codestr = string.Empty;
                    foreach (var item in data)
                    {
                        codestr += item["indexCode"].ToString() + ",";

                        JObject camerajob = JObject.Parse(item.ToString());
                        if (string.IsNullOrEmpty(item["online"].ToString()))
                        {
                            _cameraService.CamerabonlineUpdateAsync(camerajob.GetValue("indexCode").ToString(), 0, upstatehis);
                        }
                        else
                        {
                            _cameraService.CamerabonlineUpdateAsync(camerajob.GetValue("indexCode").ToString(), (int)camerajob.GetValue("online"), upstatehis);
                        }
                    }

                    codestr = codestr.TrimEnd(',');
                    List<string> listjrcode = jr.ToObject<List<string>>();
                    List<string> listcode = codestr.Split(',').ToList();
                    List<string> list3 = listjrcode.Except(listcode).ToList();//未获取信息编号
                    foreach (var item in list3)
                    {
                        _cameraService.CamerabonlineUpdateAsync(item, 0, upstatehis);
                    }
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            return 0;

        }

        /// <summary>
        /// 获取摄像头回看流地址
        /// </summary>
        /// <param name="cameracode">摄像头编号</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="recordLocation">录像存储位置（0-中心存储，1-设备存储）,默认设备存储</param>
        /// <param name="protocol">协议类型（rtsp-rtsp协议,rtmp-rtmp协议,hLS-hLS协议，默认为rtsp）</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> GetPlayBackurl(BnCamera bc)
        {
            string recordLocation = "1";
            string transmode = "1";
            string protocol = "rtsp";
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            if (bc.begintime == null)
            {
                bnCameraResult.code = "1001";
                bnCameraResult.msg = "开始时间必须传值";
                return bnCameraResult;
            }
            if (bc.endtime == null)
            {
                bnCameraResult.code = "1002";
                bnCameraResult.msg = "结束时间必须传值";
                return bnCameraResult;
            }
            if (bc.cameratype == 18 || bc.cameratype == 24)
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S161);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S162);
                _host = _systemSettingService.getSettingValue(Const.Setting.S160);
            }
            else
            {
                _appkey = _systemSettingService.getSettingValue(Const.Setting.S098);
                _secret = _systemSettingService.getSettingValue(Const.Setting.S099);
                _host = _systemSettingService.getSettingValue(Const.Setting.S097);
            }
            if (!string.IsNullOrEmpty(bc.recordLocation))
            {
                recordLocation = bc.recordLocation;
            }
            if (!string.IsNullOrEmpty(bc.protocol))
            {
                protocol = bc.protocol;
            }
            if (!string.IsNullOrEmpty(bc.transmode))
            {
                transmode = bc.transmode;
            }
            DateTime starttime = (DateTime)bc.begintime;// DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);;
            DateTime endtime = (DateTime)bc.endtime;// DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            string uri = "/artemis/api/video/v1/cameras/playbackURLs";
            JObject jb = new JObject();
            jb.Add("cameraIndexCode", bc.cameracode);
            jb.Add("beginTime", starttime.GetDateTimeFormats('s')[0].ToString() + ".000+08:00");
            jb.Add("endTime", endtime.GetDateTimeFormats('s')[0].ToString() + ".000+08:00");
            jb.Add("recordLocation", recordLocation);
            jb.Add("protocol", protocol);
            jb.Add("transmode", transmode);
            jb.Add("needReturnClipInfo", false);
            jb.Add("expand", "streamform=rtp");
            byte[] response = HttpPost(uri, jb.ToString(), 15);

            string result = System.Text.Encoding.UTF8.GetString(response);
            JObject resjb = JObject.Parse(result);

            bnCameraResult.code = (string)resjb.GetValue("code");
            if (bnCameraResult.code == "0")
            {
                JObject urljb = (JObject)resjb.GetValue("data");
                bnCameraResult.msg = (string)resjb.GetValue("msg");
                if (!string.IsNullOrEmpty((string)urljb.GetValue("url")))
                {
                    bnCameraResult.url = (string)urljb.GetValue("url") + "?beginTime=" + string.Format("{0:yyyyMMddTHHmmss}", starttime) + "&endTime=" + string.Format("{0:yyyyMMddTHHmmss}", endtime);
                    JArray myJsonArray;
                    myJsonArray = (JArray)urljb.GetValue("list");
                    List<BnPlaybackURL> list = new List<BnPlaybackURL>();
                    foreach (var item in myJsonArray)
                    {
                        BnPlaybackURL bnPlaybackURL = new BnPlaybackURL();
                        DateTime beginTime = (DateTime)((JObject)item)["beginTime"];
                        DateTime endTime = (DateTime)((JObject)item)["endTime"];
                        bnPlaybackURL.begintime = string.Format("{0:HH:mm:ss}", beginTime);
                        bnPlaybackURL.endtime = string.Format("{0:HH:mm:ss}", endTime);
                        TimeSpan TS = endTime - beginTime;
                        if (TS.Hours > 0)
                        {
                            bnPlaybackURL.timespan = TS.Hours + "小时";
                        }
                        bnPlaybackURL.timespan = bnPlaybackURL.timespan + TS.Minutes + "分" + TS.Seconds + "秒";

                        bnPlaybackURL.size = (string)((JObject)item)["size"];
                        bnPlaybackURL.url = (string)urljb.GetValue("url") + "?beginTime=" + string.Format("{0:yyyyMMddTHHmmss}", beginTime) + "&endTime=" + string.Format("{0:yyyyMMddTHHmmss}", endTime);
                        list.Add(bnPlaybackURL);
                    }
                    bnCameraResult.data = list;
                }

            }
            else
            {
                bnCameraResult.msg = (string)resjb.GetValue("msg");
            }


            return bnCameraResult;
        }

        /// <summary>
        /// HTTP GET请求
        /// </summary>
        /// <param name="uri">HTTP接口Url，不带协议和端口，如/artemis/api/resource/v1/cameras/indexCode?cameraIndexCode=a10cafaa777c49a5af92c165c95970e0</param>
        /// <param name="timeout">请求超时时间，单位：秒</param>
        /// <returns></returns>
        static byte[] HttpGet(string uri, int timeout)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();

            // 初始化请求：组装请求头，设置远程证书自动验证通过
            initRequest(header, uri, "", false);

            // build web request object
            StringBuilder sb = new StringBuilder();
            sb.Append(_isHttps ? "https://" : "http://").Append(_host.ToString()).Append(uri);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(sb.ToString());
            req.KeepAlive = false;
            req.ProtocolVersion = HttpVersion.Version11;
            req.AllowAutoRedirect = false;   // 不允许自动重定向
            req.Method = "GET";
            req.Timeout = timeout * 1000;    // 传入是秒，需要转换成毫秒
            req.Accept = header["Accept"];
            req.ContentType = header["Content-Type"];

            foreach (string headerKey in header.Keys)
            {
                if (headerKey.Contains("x-ca-"))
                {
                    req.Headers.Add(headerKey + ":" + header[headerKey]);
                }
            }

            HttpWebResponse rsp = null;
            try
            {
                rsp = (HttpWebResponse)req.GetResponse();
                if (HttpStatusCode.OK == rsp.StatusCode)
                {
                    Stream rspStream = rsp.GetResponseStream();     // 响应内容字节流
                    StreamReader sr = new StreamReader(rspStream);
                    string strStream = sr.ReadToEnd();
                    long streamLength = strStream.Length;
                    byte[] response = System.Text.Encoding.UTF8.GetBytes(strStream);
                    rsp.Close();
                    return response;
                }
                else if (HttpStatusCode.Found == rsp.StatusCode || HttpStatusCode.Moved == rsp.StatusCode)  // 302/301 redirect
                {
                    string reqUrl = rsp.Headers["Location"].ToString();   // 获取重定向URL
                    WebRequest wreq = WebRequest.Create(reqUrl);          // 重定向请求对象
                    WebResponse wrsp = wreq.GetResponse();                // 重定向响应
                    long streamLength = wrsp.ContentLength;               // 重定向响应内容长度
                    Stream rspStream = wrsp.GetResponseStream();          // 响应内容字节流
                    byte[] response = new byte[streamLength];
                    rspStream.Read(response, 0, (int)streamLength);       // 读取响应内容至byte数组
                    rspStream.Close();
                    rsp.Close();
                    return response;
                }

                rsp.Close();
            }
            catch (WebException e)
            {
                if (rsp != null)
                {
                    rsp.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// HTTP Post请求
        /// </summary>
        /// <param name="uri">HTTP接口Url，不带协议和端口，如/artemis/api/resource/v1/org/advance/orgList</param>
        /// <param name="body">请求参数</param>
        /// <param name="timeout">请求超时时间，单位：秒</param>
        /// <return>请求结果</return>
        byte[] HttpPost(string uri, string body, int timeout)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();

            // 初始化请求：组装请求头，设置远程证书自动验证通过
            initRequest(header, uri, body, true);

            // build web request object
            StringBuilder sb = new StringBuilder();
            sb.Append(_isHttps ? "https://" : "http://").Append(_host.ToString()).Append(uri);

            // 创建POST请求
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(sb.ToString());
            req.KeepAlive = false;
            req.ProtocolVersion = HttpVersion.Version11;
            req.AllowAutoRedirect = false;   // 不允许自动重定向
            req.Method = "POST";
            req.Timeout = timeout * 1000;    // 传入是秒，需要转换成毫秒
            req.Accept = header["Accept"];
            req.ContentType = header["Content-Type"];

            foreach (string headerKey in header.Keys)
            {
                if (headerKey.Contains("x-ca-"))
                {
                    req.Headers.Add(headerKey + ":" + header[headerKey]);
                }
            }

            if (!string.IsNullOrWhiteSpace(body))
            {
                byte[] postBytes = Encoding.UTF8.GetBytes(body);
                req.ContentLength = postBytes.Length;
                Stream reqStream = null;

                try
                {
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postBytes, 0, postBytes.Length);
                    reqStream.Close();
                }
                catch (WebException e)
                {
                    if (reqStream != null)
                    {
                        reqStream.Close();
                    }

                    return null;
                }
            }

            HttpWebResponse rsp = null;
            try
            {
                rsp = (HttpWebResponse)req.GetResponse();
                if (HttpStatusCode.OK == rsp.StatusCode)
                {
                    Stream rspStream = rsp.GetResponseStream();
                    StreamReader sr = new StreamReader(rspStream);
                    string strStream = sr.ReadToEnd();
                    long streamLength = strStream.Length;
                    byte[] response = System.Text.Encoding.UTF8.GetBytes(strStream);
                    rsp.Close();
                    return response;
                }
                else if (HttpStatusCode.Found == rsp.StatusCode || HttpStatusCode.Moved == rsp.StatusCode)  // 302/301 redirect
                {
                    try
                    {
                        string reqUrl = rsp.Headers["Location"].ToString();    // 如需要重定向URL，请自行修改接口返回此参数
                        WebRequest wreq = WebRequest.Create(reqUrl);
                        rsp = (HttpWebResponse)wreq.GetResponse();
                        Stream rspStream = rsp.GetResponseStream();
                        long streamLength = rsp.ContentLength;
                        int offset = 0;
                        byte[] response = new byte[streamLength];
                        while (streamLength > 0)
                        {
                            int n = rspStream.Read(response, offset, (int)streamLength);
                            if (0 == n)
                            {
                                break;
                            }

                            offset += n;
                            streamLength -= n;
                        }

                        return response;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }

                rsp.Close();
            }
            catch (WebException e)
            {
                if (rsp != null)
                {
                    rsp.Close();
                }
            }

            return null;
        }

        private static void initRequest(Dictionary<string, string> header, string url, string body, bool isPost)
        {
            // Accept                
            string accept = "application/json";// "*/*";
            header.Add("Accept", accept);

            // ContentType  
            string contentType = "application/json";
            header.Add("Content-Type", contentType);

            if (isPost)
            {
                // content-md5，be careful it must be lower case.
                string contentMd5 = computeContentMd5(body);
                header.Add("content-md5", contentMd5);
            }

            // x-ca-timestamp
            string timestamp = ((DateTime.Now.Ticks - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks) / 1000).ToString();
            header.Add("x-ca-timestamp", timestamp);

            // x-ca-nonce
            string nonce = System.Guid.NewGuid().ToString();
            header.Add("x-ca-nonce", nonce);

            // x-ca-key
            header.Add("x-ca-key", _appkey);

            // build string to sign
            string strToSign = buildSignString(isPost ? "POST" : "GET", url, header);
            string signedStr = computeForHMACSHA256(strToSign, _secret);

            // x-ca-signature
            header.Add("x-ca-signature", signedStr);

            if (_isHttps)
            {
                // set remote certificate Validation auto pass
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(remoteCertificateValidate);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
        }

        /// <summary>
        /// 计算content-md5
        /// </summary>
        /// <param name="body"></param>
        /// <returns>base64后的content-md5</returns>
        private static string computeContentMd5(string body)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(body));
            return Convert.ToBase64String(result);
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
        /// 计算HMACSHA265
        /// </summary>
        /// <param name="str">待计算字符串</param>
        /// <param name="secret">平台APPSecet</param>
        /// <returns>HMAXH265计算结果字符串</returns>
        private static string computeForHMACSHA256(string str, string secret)
        {
            var encoder = new System.Text.UTF8Encoding();
            byte[] secretBytes = encoder.GetBytes(secret);
            byte[] strBytes = encoder.GetBytes(str);
            var opertor = new HMACSHA256(secretBytes);
            byte[] hashbytes = opertor.ComputeHash(strBytes);
            return Convert.ToBase64String(hashbytes);
        }

        /// <summary>
        /// 计算签名字符串
        /// </summary>
        /// <param name="method">HTTP请求方法，如“POST”</param>
        /// <param name="url">接口Url，如/artemis/api/resource/v1/org/advance/orgList</param>
        /// <param name="header">请求头</param>
        /// <returns>签名字符串</returns>
        private static string buildSignString(string method, string url, Dictionary<string, string> header)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(method.ToUpper()).Append("\n");
            if (null != header)
            {
                if (null != header["Accept"])
                {
                    sb.Append((string)header["Accept"]);
                    sb.Append("\n");
                }

                if (header.Keys.Contains("Content-MD5") && null != header["Content-MD5"])
                {
                    sb.Append((string)header["Content-MD5"]);
                    sb.Append("\n");
                }

                if (null != header["Content-Type"])
                {
                    sb.Append((string)header["Content-Type"]);
                    sb.Append("\n");
                }

                if (header.Keys.Contains("Date") && null != header["Date"])
                {
                    sb.Append((string)header["Date"]);
                    sb.Append("\n");
                }
            }

            // build and add header to sign
            string signHeader = buildSignHeader(header);
            sb.Append(signHeader);
            sb.Append(url);
            return sb.ToString();
        }

        /// <summary>
        /// 计算签名头
        /// </summary>
        /// <param name="header">请求头</param>
        /// <returns>签名头</returns>
        private static string buildSignHeader(Dictionary<string, string> header)
        {
            Dictionary<string, string> sortedDicHeader = new Dictionary<string, string>();
            sortedDicHeader = header;
            var dic = from objDic in sortedDicHeader orderby objDic.Key ascending select objDic;

            StringBuilder sbSignHeader = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in dic)
            {
                if (kvp.Key.Replace(" ", "").Contains("x-ca-"))
                {
                    sb.Append(kvp.Key + ":");
                    if (!string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        sb.Append(kvp.Value);
                    }
                    sb.Append("\n");
                    if (sbSignHeader.Length > 0)
                    {
                        sbSignHeader.Append(",");
                    }
                    sbSignHeader.Append(kvp.Key);
                }
            }

            header.Add("x-ca-signature-headers", sbSignHeader.ToString());

            return sb.ToString();
        }


    }


}
