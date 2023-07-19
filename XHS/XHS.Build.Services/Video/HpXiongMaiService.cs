using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Util;
using XHS.Build.Common;
using XHS.Build.Common.Util;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;
using UEncrypter = XHS.Build.Common.UEncrypter;

namespace XHS.Build.Service.Video
{
    /// <summary>
    /// 雄迈访问助手
    /// </summary>
    public partial class HpXiongMaiService : BaseServices<BaseEntity>, IHpXiongMaiService
    {
        private readonly IHpSystemSetting _systemSettingService;

        private static string _host ;
        private static string _username ;
        private static string _password;
        private static string g_realm = "Http-Realm";
        private static string g_cnonce = "757b505cfd34c64c85ca5b5690ee5293";
        private static string g_nc = "00000001";
        private static string g_auth = "auth";
        private static string g_uri ="client/login.jsp";
        private string g_nonce = "";
        public HpXiongMaiService(IHpSystemSetting systemSettingService)
        {
            _systemSettingService = systemSettingService;
            _host = _systemSettingService.getSettingValue(Const.Setting.S165);
            _username = _systemSettingService.getSettingValue(Const.Setting.S166);
            _password = _systemSettingService.getSettingValue(Const.Setting.S167);
        }


        public BnCameraResult<BnPlaybackURL> GetRealurl(BnCamera bc)
        {
            string streamtype = "0";
            string protocol = "rtsp";
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            if (!string.IsNullOrEmpty(bc.streamtype))
            {
                streamtype = bc.streamtype;
            }
            if (!string.IsNullOrEmpty(bc.protocol))
            {
                protocol = bc.protocol;
            }

            string digestAuth= creatDigestAuth();
            JObject jParam = new JObject();
            jParam.Add("deviceId", bc.cameracode);
            jParam.Add("channelNum", bc.channel);
            jParam.Add("mediaType", protocol);
            jParam.Add("recordFrom", "device");
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                   { "Authorization", "Digest " + digestAuth}
            };

            string result = UhttpClient.PostXM("http://" + _host + "/queryRealplayUri", jParam.ToString(),  header);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "未获取到直播流，请确认摄像头编号是否正确";
                return bnCameraResult;
            }
            else if (result.Contains("nonce"))
            {
                g_nonce = result.Substring(result.IndexOf("nonce") + 6).Replace("\"","");
                digestAuth = creatDigestAuth();
                header = new Dictionary<string, string>()
                {
                       { "Authorization", "Digest " + digestAuth}
                };
                result = UhttpClient.PostXM("http://" + _host + "/queryRealplayUri", jParam.ToString(), header);
                if (string.IsNullOrEmpty(result))
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "未获取到直播流，请确认摄像头编号是否正确";
                    return bnCameraResult;
                } else
                {
                    JObject mJObj = JObject.Parse(result);
                    if (mJObj.Property("result") != null && mJObj["result"].ToString() == "200")//成功
                    {
                        bnCameraResult.code = "0";
                        bnCameraResult.url = mJObj["uri"].ToString();
                        bnCameraResult.hasplayback = "1";
                        bnCameraResult.hasptz = "0";
                    }
                    else
                    {
                        bnCameraResult.code = "2";
                        bnCameraResult.msg = mJObj["msg"].ToString();
                        return bnCameraResult;
                    }
                    
                }
            }
            else
            {
                JObject mJObj = JObject.Parse(result);
                if (mJObj.Property("result") != null && mJObj["result"].ToString() == "200")//成功
                {
                    bnCameraResult.code = "0";
                    bnCameraResult.url = mJObj["uri"].ToString();
                    bnCameraResult.hasplayback = "1";
                    bnCameraResult.hasptz = "0";
                }
                else
                {
                    bnCameraResult.code = "2";
                    bnCameraResult.msg = mJObj["msg"].ToString();
                    return bnCameraResult;
                }
            }
           
            return bnCameraResult;
        }
        public string creatDigestAuth()
        {
            string A1 = _username + ":" + g_realm + ":" + _password;
            string HA1 = UEncrypter.EncryptByMD5(A1);
            string A2 = "POST:" + g_uri;
            string HA2 = UEncrypter.EncryptByMD5(A2);
            string response = UEncrypter.EncryptByMD5(HA1 + ":" + g_nonce + ":" + g_nc + ":" + g_cnonce + ":" + g_auth + ":" + HA2);
            StringBuilder digestAuth = new StringBuilder();
            digestAuth.Append("username").Append("=").Append('"').Append(_username).Append('"').Append(",");
            digestAuth.Append("response").Append("=").Append('"').Append(response).Append('"').Append(",");
            digestAuth.Append("realm").Append("=").Append('"').Append(g_realm).Append('"').Append(",");
            digestAuth.Append("nonce").Append("=").Append('"').Append(g_nonce).Append('"').Append(",");
            digestAuth.Append("uri").Append("=").Append('"').Append(g_uri).Append('"').Append(",");
            digestAuth.Append("qop").Append("=").Append('"').Append(g_auth).Append('"').Append(",");
            digestAuth.Append("nc").Append("=").Append(g_nc).Append(",");
            digestAuth.Append("cnonce").Append("=").Append('"').Append(g_cnonce).Append('"');
            return digestAuth.ToString();
        }
            public BnCameraResult<BnPlaybackURL> GetPlayBackurl(BnCamera bc)
        {
            string streamtype = "0";
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
            if (!string.IsNullOrEmpty(bc.streamtype))
            {
                streamtype = bc.streamtype;
            }
            if (!string.IsNullOrEmpty(bc.protocol))
            {
                protocol = bc.protocol;
            }
            DateTime starttime = (DateTime)bc.begintime;// DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);;
            DateTime endtime = (DateTime)bc.endtime;

            string digestAuth = creatDigestAuth();
            JObject jParam = new JObject();
            jParam.Add("deviceId", bc.cameracode);
            jParam.Add("channelNum", bc.channel);
            jParam.Add("mediaType", protocol);
            jParam.Add("source", "device");
            jParam.Add("recordTimeStart", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
            jParam.Add("recordTimeEnd", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                   { "Authorization", "Digest " + digestAuth}
            };

            string result = UhttpClient.PostXM("http://" + _host + "/queryPlaybackUri", jParam.ToString(), header);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "未获取到回放流，请确认摄像头编号是否正确";
                return bnCameraResult;
            }
            else if (result.Contains("nonce"))
            {
                g_nonce = result.Substring(result.IndexOf("nonce") + 6).Replace("\"", "");
                digestAuth = creatDigestAuth();
                header = new Dictionary<string, string>()
                {
                       { "Authorization", "Digest " + digestAuth}
                };
                result = UhttpClient.PostXM("http://" + _host + "/queryPlaybackUri", jParam.ToString(), header);
                if (string.IsNullOrEmpty(result))
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "未获取到回放流，请确认摄像头编号是否正确";
                    return bnCameraResult;
                }
                else
                {
                    JObject mJObj = JObject.Parse(result);
                    if (mJObj.Property("result") != null && mJObj["result"].ToString() == "200")//成功
                    {
                        bnCameraResult.code = "0";
                        bnCameraResult.url = mJObj["uri"].ToString();
                        List<BnPlaybackURL> list = new List<BnPlaybackURL>();
                        BnPlaybackURL bnPlaybackURL = new BnPlaybackURL();

                        bnPlaybackURL.begintime = string.Format("{0:HH:mm:ss}", bc.begintime);
                        bnPlaybackURL.endtime = string.Format("{0:HH:mm:ss}", bc.endtime);
                        TimeSpan TS = endtime - starttime;
                        if (TS.Hours > 0)
                        {
                            bnPlaybackURL.timespan = TS.Hours + "小时";
                        }
                        bnPlaybackURL.timespan = bnPlaybackURL.timespan + TS.Minutes + "分" + TS.Seconds + "秒";

                        bnPlaybackURL.url = bnCameraResult.url;
                        list.Add(bnPlaybackURL);
                        bnCameraResult.data = list;
                    }
                    else
                    {
                        bnCameraResult.code = "2";
                        bnCameraResult.msg = mJObj["msg"].ToString();
                        return bnCameraResult;
                    }

                }
            }
            else
            {
                JObject mJObj = JObject.Parse(result);
                if (mJObj.Property("result") != null && mJObj["result"].ToString() == "200")//成功
                {
                    bnCameraResult.code = "0";
                    bnCameraResult.url = mJObj["uri"].ToString();
                    List<BnPlaybackURL> list = new List<BnPlaybackURL>();
                    BnPlaybackURL bnPlaybackURL = new BnPlaybackURL();

                    bnPlaybackURL.begintime = string.Format("{0:HH:mm:ss}", bc.begintime);
                    bnPlaybackURL.endtime = string.Format("{0:HH:mm:ss}", bc.endtime);
                    TimeSpan TS = endtime - starttime;
                    if (TS.Hours > 0)
                    {
                        bnPlaybackURL.timespan = TS.Hours + "小时";
                    }
                    bnPlaybackURL.timespan = bnPlaybackURL.timespan + TS.Minutes + "分" + TS.Seconds + "秒";

                    bnPlaybackURL.url = bnCameraResult.url;
                    list.Add(bnPlaybackURL);
                    bnCameraResult.data = list;
                }
                else
                {
                    bnCameraResult.code = "2";
                    bnCameraResult.msg = mJObj["msg"].ToString();
                    return bnCameraResult;
                }
            }
            
            return bnCameraResult;
        }

        public BnCameraResult<BnPlaybackURL> ptz(BnCamera bc)
        {
            throw new NotImplementedException();
        }
    }

}
