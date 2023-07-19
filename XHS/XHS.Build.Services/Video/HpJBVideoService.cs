using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Util;
using System.Xml;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Util;
using XHS.Build.Services.Base;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;

namespace XHS.Build.Service.Video

{
    /// <summary>
    /// 江北视频连接助手
    /// </summary>
    public class HpJBVideoService : BaseServices<BaseEntity>, IHpJBVideoService
    {
        private static string Url = "http://61.160.52.241:40006/videoCloud/cxf/StorageGatewayServices?wsdl";
        private static string JBVideoServerUrl = "http://121.224.59.218:21101";
        private static string AppName = "HpJBVideoService";
        private static string Account = null;//账户
        private const string CHANNEL = "34020000001320000001";//账户
        private static string LoginSession = null;//session
        private static DateTime LastLoginTime = DateTime.Now;
        private const string XML_LOGIN = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Header/>" +
                    "<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                    "<userLoginReq xmlns=\"http://www.sttri.com.cn/ns1MobileServices/\">" +
                    "<Account>{0}</Account>" +
                    "<LoginPassword>{1}</LoginPassword>" +
                    "</userLoginReq></s:Body></s:Envelope>";
        private const string XML_PLAY_URL = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Heade />" +
                    "<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                    "<getPlayUrlByClientReq xmlns=\"http://www.sttri.com.cn/ns1MobileServices/\">" +
                    "<Account>{0}</Account>" +
                    "<LoginSession>{1}</LoginSession>" +
                    "<DevId>{2}</DevId>" +
                    "<ChannelNo>{3}</ChannelNo>" +
                    "</getPlayUrlByClientReq></s:Body></s:Envelope>";

        private const string XML_HISTORY_URL = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Heade />" +
                    "<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                    "<getHistoryVideoReq xmlns=\"http://www.sttri.com.cn/ns1MobileServices/\">" +
                    "<Account>{0}</Account>" +
                    "<LoginSession>{1}</LoginSession>" +
                    "<DevId>{2}</DevId>" +
                    "<Type>1</Type>" +
                    "<ChannelNo>{3}</ChannelNo>" +
                    "<StartTime>{4}</StartTime>" +
                    "<EndTime>{5}</EndTime>" +
                    "</getHistoryVideoReq></s:Body></s:Envelope>";

        private const string XML_DEVICE_LIST = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Header />" +
                    "<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                    "<TerminalListReq xmlns=\"http://www.sttri.com.cn/ns1MobileServices/\">" +
                    "<Account>{0}</Account>" +
                    "<LoginSession>{1}</LoginSession>" +
                    "<CurrPage>{2}</CurrPage>" +
                    "<PageSize>{3}</PageSize>" +
                    "</TerminalListReq></s:Body></s:Envelope>";
        private static string domain = "jb.hb.xhs-sz.com";
        private static Dictionary<string, string> rtmpDic = new Dictionary<string, string> {
            {"10.4.164.163:1935",domain+":1933"},
            {"10.4.164.164:1935",domain+":1934"},
            {"10.4.164.165:1935",domain+":1935"},
            {"10.4.164.166:1935",domain+":1936"},
            {"10.4.164.167:1935",domain+":1937"},
            {"10.4.164.168:1935",domain+":1938"},
            {"10.4.164.169:1935",domain+":1939"},
        };
        private static Dictionary<string, string> rtspDic = new Dictionary<string, string> {
            {"10.4.164.163:554",domain+":553"},
            {"10.4.164.164:554",domain+":554"},
            {"10.4.164.165:554",domain+":555"},
            {"10.4.164.166:554",domain+":556"},
            {"10.4.164.167:554",domain+":557"},
            {"10.4.164.168:554",domain+":558"},
            {"10.4.164.169:554",domain+":559"},
        };


        /// <summary>
        /// 用户登录
        /// </summary>
        private static void _UserLoing()
        {
            //超过10分钟就重新获取
            if (LastLoginTime.AddMinutes(10) > DateTime.Now && Account != null)
                return;

            string requestXml = string.Format(XML_LOGIN,"pkdx", "pkdx@123");
            string response = UHttp.Post(Url, requestXml, UHttp.CONTENT_TYPE_TEXT_XML);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(response);
            string resultcode = xmldoc.GetElementsByTagName("Result").Item(0).InnerText;
            if (resultcode.Equals("0"))
            {//成功
                Account = xmldoc.GetElementsByTagName("Account").Item(0).InnerText;
                LoginSession = xmldoc.GetElementsByTagName("LoginSession").Item(0).InnerText;
                LastLoginTime = DateTime.Now;
            }
            else
                ULog.WriteError(resultcode, AppName);

        }

        /// <summary>
        /// 获取直播流地址
        /// </summary>
        /// <param name="DevId">设备编号</param>
        /// <param name="ChannelNo">通道号</param>
        /// <returns></returns>
        public static JObject GetPlayUrlByClient(string DevId, int ChannelNo = 0)
        {
            JObject result = null;
            try
            {
                _UserLoing();
                string requestXml = string.Format(XML_PLAY_URL, Account, LoginSession, DevId, ChannelNo);
                string response = UHttp.Post(Url, requestXml, UHttp.CONTENT_TYPE_TEXT_XML);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(response);
                string resultcode = xmldoc.GetElementsByTagName("Result").Item(0).InnerText;
                if (resultcode.Equals("0"))
                {
                    result = new JObject();
                    result.Add("rtmp", transferUrl(rtmpDic, xmldoc.GetElementsByTagName("PlayUrl").Item(0).InnerText));
                    result.Add("rtsp", transferUrl(rtspDic, xmldoc.GetElementsByTagName("MediaSource").Item(0).InnerText));
                }
                else
                    ULog.WriteError(resultcode, AppName);
            }
            catch (Exception ex)
            {
                ULog.WriteError(ex.Message, AppName);
            }

            return result;
        }

        /// <summary>
        /// 获取视频历史列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="DevId">设备编号</param>
        /// <param name="ChannelNo">通道号</param>
        /// <returns></returns>
        public static JArray GetHistoryVideo(DateTime StartTime, DateTime EndTime, string DevId, int ChannelNo = 0)
        {
            JArray result = null;
            try
            {
                _UserLoing();
                string requestXml = string.Format(XML_HISTORY_URL, Account, LoginSession, DevId, ChannelNo,
                    StartTime.ToString("yyyy-MM-dd HH:mm:ss"), EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                string response = UHttp.Post(Url, requestXml, UHttp.CONTENT_TYPE_TEXT_XML);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(response);

                string resultcode = xmldoc.GetElementsByTagName("Result").Item(0).InnerText;
                if (resultcode.Equals("0"))
                {
                    if (xmldoc.GetElementsByTagName("historyVideoInfo").Count > 0)
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(response)));
                        DataTable dt = ds.Tables["historyVideoInfo"];
                        result = JArray.FromObject(dt);
                    }
                    else
                        result = new JArray();
                }
                else
                    ULog.WriteError(resultcode, AppName);
            }
            catch (Exception ex)
            {
                ULog.WriteError(ex.Message, AppName);
            }

            return result;

        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        public static JArray GetDeviceList(int pageIndex = 1, int pageSize = 1000)
        {
            JArray result = null;
            try
            {
                _UserLoing();
                string requestXml = string.Format(XML_DEVICE_LIST, Account, LoginSession, pageIndex, pageSize);
                string response = UHttp.Post(Url, requestXml, UHttp.CONTENT_TYPE_TEXT_XML);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(response);
                string resultcode = xmldoc.GetElementsByTagName("Result").Item(0).InnerText;
                if (resultcode.Equals("0"))
                {
                    if (xmldoc.GetElementsByTagName("terminalInfo").Count > 0)
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(response)));
                        DataTable dt = ds.Tables["terminalInfo"];
                        result = JArray.FromObject(dt);
                    }
                    else
                        result = new JArray();
                }
                else
                    ULog.WriteError(resultcode, AppName);
            }
            catch (Exception ex)
            {
                ULog.WriteError(ex.Message, AppName);
            }

            return result;
        }

        private static string transferUrl(Dictionary<string, string> dic, string orignalUrl)
        {
            if (string.IsNullOrEmpty(orignalUrl) || dic == null || dic.Count == 0)
                return null;
            string result = null;
            Uri uri = new Uri(orignalUrl);
            string domainPort = uri.Authority;
            result = orignalUrl.Replace(domainPort, dic[domainPort]);

            return result;
        }

        public BnCameraResult<BnPlaybackURL> GetRealurl(BnCamera bc)
        {
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            JObject result = null;
            try
            {

                if (string.IsNullOrEmpty(bc.cameraparam))
                {
                    bnCameraResult.code = "1000";
                    bnCameraResult.msg = "摄像头编号必须传值";
                    return bnCameraResult;
                }
                string protocol = "rtmp";
                if (!string.IsNullOrEmpty(bc.protocol))
                {
                    protocol = bc.protocol;
                }

                string url = string.Format("{0}/cmpt/vep/device/getPlayAddress", JBVideoServerUrl);
                string data = string.Format("deviceId={0}&channelid={1}", bc.cameraparam, CHANNEL);
                string response = UHttp.Post(url, data, UHttp.CONTENT_TYPE_FORM);
                JToken jt = JToken.Parse(response);
                bool success = Convert.ToBoolean(jt["success"]);
                if (!success)
                {
                    bnCameraResult.code = "1001";
                    bnCameraResult.msg = Convert.ToString(jt["message"]);
                    return bnCameraResult;
                }

                JArray ja = JArray.FromObject(jt["data"]["playAddressInfos"]);

                for (int i = 0; i < ja.Count; i++)
                {
                    if (protocol.Equals("rtmp"))
                    {
                        if (Convert.ToInt16(ja[i]["playType"]) == 1)
                            bnCameraResult.url = (string)ja[i]["playAddress"];
                    }
                    else
                    {
                        if (Convert.ToInt16(ja[i]["playType"]) == 3)
                            bnCameraResult.url = (string)ja[i]["playAddress"];
                    }
                }
               
                    bnCameraResult.code = "0";
                    bnCameraResult.hasptz = "0";
                    bnCameraResult.hasplayback = "1";
                   
            }
            catch (Exception ex)
            {
                ULog.WriteError(ex.Message, AppName);
            }
            return bnCameraResult;
        }

        public BnCameraResult<BnPlaybackURL> GetPlayBackurl(BnCamera bc)
        {
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
            DateTime starttime = (DateTime)bc.begintime;// DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);;
            DateTime endtime = (DateTime)bc.endtime;// DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

            JArray result = null;
            try
            {
                _UserLoing();
                string requestXml = string.Format(XML_HISTORY_URL, Account, LoginSession, bc.cameracode, bc.channel,
                    starttime.ToString("yyyy-MM-dd HH:mm:ss"), endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                string response = UHttp.Post(Url, requestXml, UHttp.CONTENT_TYPE_TEXT_XML);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(response);

                string resultcode = xmldoc.GetElementsByTagName("Result").Item(0).InnerText;
                if (resultcode.Equals("0"))
                {
                    if (xmldoc.GetElementsByTagName("historyVideoInfo").Count > 0)
                    {
                        DataSet ds = new DataSet();
                        ds.ReadXml(new XmlTextReader(new StringReader(response)));
                        DataTable dt = ds.Tables["historyVideoInfo"];
                        result = JArray.FromObject(dt);
                        if (result.Count > 0)
                        {
                            bnCameraResult.code = "0";
                            JObject job = (JObject)result[0];
                            bnCameraResult.url = (string)job.GetValue("PlayUrl");
                        }
                        else
                        {
                            result = new JArray();
                        }
                    }
                    else
                        result = new JArray();
                }
                else
                    ULog.WriteError(resultcode, AppName);
            }
            catch (Exception ex)
            {
                ULog.WriteError(ex.Message, AppName);
            }
            return bnCameraResult;
        }

        public BnCameraResult<BnPlaybackURL> ptz(BnCamera bc)
        {
            throw new NotImplementedException();
        }
    }
}
