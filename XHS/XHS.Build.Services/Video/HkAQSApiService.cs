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
using Org.BouncyCastle.Utilities.Encoders;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Cache;
using Newtonsoft.Json;
using Util;

namespace XHS.Build.Service.Video
{
    /// <summary>
    /// 海康8200OpenApi连接助手
    /// </summary>
    public class HkAQSApiService : BaseServices<BaseEntity>, IAQSApiService
    {
        /// <summary>
        /// 平台ip
        /// </summary>
        private static string _ip;

        /// <summary>
        /// 平台端口
        /// </summary>
        private static string _port ;

        /// <summary>
        /// 用户
        /// </summary>
        private static string _user;

        /// <summary>
        /// 用户base64
        /// </summary>
        private static string _userbase64;

        private readonly IHpSystemSetting _systemSettingService;
        private readonly ICache _cache;

        public HkAQSApiService(ICache cache, IHpSystemSetting systemSettingService)
        {
            _systemSettingService = systemSettingService;
            _cache = cache;
            _ip = _systemSettingService.getSettingValue(Const.Setting.S061);
            _port =_systemSettingService.getSettingValue(Const.Setting.S062);
            _user = _systemSettingService.getSettingValue(Const.Setting.S063);
           
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
            _userbase64 = "Basic " + Encoding.Default.GetString(Base64.Encode(Encoding.Default.GetBytes(_user)));
            string streamtype = "0";
            string protocol = "1";
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
                if(bc.protocol.ToUpper() == "RTMP")
                {
                    protocol = "2";
                }else if(bc.protocol.ToUpper() == "HLS"){
                    protocol = "3";
                }
            }
           
            //   HttpUtillib.SetPlatformInfo("21857458", "ThSirz29xa6BcJojMSCT", "video.xhs-sz.com", 443, true);
            string token = "";
            var key = Const.Setting.AQSToken;
            var AQSToken = "";
            if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
            {
                AQSToken = _cache.Get(key);
            }
            if (!string.IsNullOrEmpty(AQSToken))
            {
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(AQSToken);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long now = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                long expireTime = long.Parse(mJObj["expireTime"].ToString());
                if (expireTime > now)
                {
                    token = mJObj["accessToken"].ToString();
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                //var retString = HttpNetRequest.SendRequest(_ip + ":"+_port + "/apiserver/v1/user/authentication-token", new Dictionary<string, object>() { }, "GET", new Dictionary<string, string>() { { "Authorization", _userbase64 } });
                var retString = UhttpClient.Get(_ip + ":" + _port + "/apiserver/v1/user/authentication-token",null, new Dictionary<string, string>() { { "Authorization", _userbase64 } });
                if (string.IsNullOrEmpty(retString))
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "Token获取失败";
                    return bnCameraResult;
                }
                JObject mJObj = JObject.Parse(retString);
                if (mJObj.Property("data") != null)//成功
                {
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                    long now = (DateTime.Now.AddMinutes(20).Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                    JObject mJdata = new JObject();
                    mJdata.Add("accessToken", mJObj["data"].ToString());
                    mJdata.Add("expireTime", now);
                    _cache.Set(key, mJdata.ToString());
                    token = mJObj["data"].ToString();
                }
                else
                {
                    bnCameraResult.code = "2";
                    bnCameraResult.msg = "Token获取失败";
                    return bnCameraResult;
                }
            }

            string uri = "/apiserver/v1/device/video/preview";
            Dictionary<string, object> dictparam = new Dictionary<string, object>()
            {
                 { "token", token },
                  { "camera_id", bc.cameracode },
                   { "stream_type", streamtype },
                    { "stream_mode", protocol },
                { "is_local", 1 }
            };
            string result = UhttpClient.Get(_ip + ":" + _port + uri, dictparam);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "未获取到直播流，请确认摄像头编号是否正确";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
            if(resjb.Property("data") != null)
            {
                bnCameraResult.code = "0";
                bnCameraResult.url = (string)resjb.GetValue("data");
                bnCameraResult.hasplayback = "1";
                bnCameraResult.hasptz = "1";                    
            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("message") != null)
                    bnCameraResult.msg = (string)resjb.GetValue("message");

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
            string speed = "3";
            bool stop = true;
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
          
            if (!string.IsNullOrEmpty(bc.speed))
            {
                int step = int.Parse(bc.speed) / 10;
                speed = step.ToString();
            }
            
            if(bc.action == "1")
            {
                stop = false;
            }

            int direction = 4;
            if (bc.command.ToUpper() == "LEFT")
            {
                direction = 3;
            }
            else if (bc.command.ToUpper() == "UP")
            {
                direction = 1;
            }
            else if (bc.command.ToUpper() == "DOWN")
            {
                direction = 2;
            }
            _userbase64 = "Basic " + Encoding.Default.GetString(Base64.Encode(Encoding.Default.GetBytes(_user)));
            string token = "";
            var key = Const.Setting.AQSToken;
            var AQSToken = "";
            if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
            {
                AQSToken = _cache.Get(key);
            }
            if (!string.IsNullOrEmpty(AQSToken))
            {
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(AQSToken);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long now = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                long expireTime = long.Parse(mJObj["expireTime"].ToString());
                if (expireTime > now)
                {
                    token = mJObj["accessToken"].ToString();
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                var retString = HttpNetRequest.SendRequest(_ip + ":" + _port + "/apiserver/v1/user/authentication-token", new Dictionary<string, object>() { }, "GET", new Dictionary<string, string>() { { "Authorization", _userbase64 } });
                if (string.IsNullOrEmpty(retString))
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "Token获取失败";
                    return bnCameraResult;
                }
                JObject mJObj = JObject.Parse(retString);
                if (mJObj.Property("data") != null)//成功
                {
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                    long now = (DateTime.Now.AddMinutes(20).Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                    JObject mJdata = new JObject();
                    mJdata.Add("accessToken", mJObj["data"].ToString());
                    mJdata.Add("expireTime", now);
                    _cache.Set(key, mJdata.ToString());
                    token = mJObj["data"].ToString();
                }
                else
                {
                    bnCameraResult.code = "2";
                    bnCameraResult.msg = "Token获取失败";
                    return bnCameraResult;
                }
            }
            string uri = "/apiserver/v1/device/ptz/pt-cmd";
           
            var postData = new { camera_id = bc.cameracode, direction = direction, step = speed, stop = stop };
            var result = HttpNetRequest.POSTSendJsonRequest(_ip + ":" + _port + uri + "?token="+ token, JsonConvert.SerializeObject(postData), new Dictionary<string, string>() { });
           // string result = HttpNetRequest.SendRequest(_ip + ":" + _port + uri, dictparam, "POST", new Dictionary<string, string>() { });
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "操作失败";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
            if (resjb.Property("data") != null)
            {
                bnCameraResult.code = "0";
            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("message") != null) {
                    bnCameraResult.msg = (string)resjb.GetValue("message");
                }
                else
                {
                    bnCameraResult.msg = "操作失败";
                }
            }
            return bnCameraResult;
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
            string protocol = "1";
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
            if (bc.endtime==null)
            {
                bnCameraResult.code = "1002";
                bnCameraResult.msg = "结束时间必须传值";
                return bnCameraResult;
            }
            if (!string.IsNullOrEmpty(bc.recordLocation))
            {
                recordLocation = bc.recordLocation;
            }
            if (!string.IsNullOrEmpty(bc.protocol))
            {
                if (bc.protocol.ToUpper() == "RTMP")
                {
                    protocol = "2";
                }
                else if (bc.protocol.ToUpper() == "HLS")
                {
                    protocol = "3";
                }
            }
            _userbase64 = "Basic " + Encoding.Default.GetString(Base64.Encode(Encoding.Default.GetBytes(_user)));
            string token = "";
            var key = Const.Setting.AQSToken;
            var AQSToken = "";
            if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
            {
                AQSToken = _cache.Get(key);
            }
            if (!string.IsNullOrEmpty(AQSToken))
            {
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(AQSToken);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long now = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                long expireTime = long.Parse(mJObj["expireTime"].ToString());
                if (expireTime > now)
                {
                    token = mJObj["accessToken"].ToString();
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                var retString = HttpNetRequest.SendRequest(_ip + ":" + _port + "/apiserver/v1/user/authentication-token", new Dictionary<string, object>() { }, "GET", new Dictionary<string, string>() { { "Authorization", _userbase64 } });
                if (string.IsNullOrEmpty(retString))
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "Token获取失败";
                    return bnCameraResult;
                }
                JObject mJObj = JObject.Parse(retString);
                if (mJObj.Property("data") != null)//成功
                {
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                    long now = (DateTime.Now.AddMinutes(20).Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                    JObject mJdata = new JObject();
                    mJdata.Add("accessToken", mJObj["data"].ToString());
                    mJdata.Add("expireTime", now);
                    _cache.Set(key, mJdata.ToString());
                    token = mJObj["data"].ToString();
                }
                else
                {
                    bnCameraResult.code = "2";
                    bnCameraResult.msg = "Token获取失败";
                    return bnCameraResult;
                }
            }
            //DateTime starttime = DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture); ;
            // DateTime endtime = DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            DateTime starttime = (DateTime)bc.begintime;// DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);;
            DateTime endtime = (DateTime)bc.endtime;// DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            string uri = "/apiserver/v1/device/video/replay";
            Dictionary<string, object> dictparam = new Dictionary<string, object>()
            {
                 { "token", token },
                  { "camera_id", bc.cameracode },
                   { "start_time", starttime.ToString("yyyy-MM-dd HH:mm:ss")},
                    { "end_time", endtime.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "source_type", recordLocation },
                    { "stream_mode", protocol },
                { "is_local", "1" }
            };
            string result = HttpNetRequest.SendRequest(_ip + ":" + _port + uri, dictparam, "GET", new Dictionary<string, string>() { });
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "未获取到回看流，请确认参数是否正确";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
            if (resjb.Property("data") != null)
            {
                bnCameraResult.code = "0";
                bnCameraResult.url = (string)resjb.GetValue("data");
                bnCameraResult.hasplayback = "1";
                bnCameraResult.hasptz = "1";
            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("message") != null)
                    bnCameraResult.msg = (string)resjb.GetValue("message");

            }
            //List<BnPlaybackURL> list = new List<BnPlaybackURL>();
            //BnPlaybackURL bnPlaybackURL = new BnPlaybackURL();
           
            //bnPlaybackURL.begintime = string.Format("{0:HH:mm:ss}", bc.begintime);
            //bnPlaybackURL.endtime = string.Format("{0:HH:mm:ss}", bc.endtime);
            //TimeSpan TS = endtime - starttime;
            //if (TS.Hours > 0)
            //{
            //    bnPlaybackURL.timespan = TS.Hours + "小时";
            //}
            //bnPlaybackURL.timespan = bnPlaybackURL.timespan + TS.Minutes + "分" + TS.Seconds + "秒";

            //bnPlaybackURL.url = bnCameraResult.url;
            //list.Add(bnPlaybackURL);
            //bnCameraResult.data = list;
            return bnCameraResult;
        }


      
    }

    /// <summary>
    /// 云台控制指令
    /// </summary>
    public enum PtzOP
    {
        //控制命令(不区分大小写) 说明：
        ZOOM_IN,        //焦距变大
        UP,             //上转
        DOWN,           //下转 
        LEFT,           //左转
        RIGHT,          //右转
        LEFT_UP,        //左上
        LEFT_DOWN,      //左下
        RIGHT_UP,       //右上
        RIGHT_DOWN      //右下
    }


}
