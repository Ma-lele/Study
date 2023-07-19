using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Util;
using XHS.Build.Common;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Service.Video
{
    /// <summary>
    /// 海康8200OpenApi连接助手
    /// </summary>
    public class DiaoGouApiService : BaseServices<BaseEntity>, IDiaoGouApiService
    {

        /// <summary>
        /// 平台端口
        /// </summary>
        private static string _url;

        /// <summary>
        /// 用户
        /// </summary>
        private static string _user;

        /// <summary>
        /// 密码
        /// </summary>
        private static string _pass;

        /// <summary>
        /// secret
        /// </summary>
        private static string _secret;

        public static string videoSession = "";

        private readonly IHpSystemSetting _systemSettingService;
        private readonly ICache _cache;


        public DiaoGouApiService(ICache cache, IHpSystemSetting systemSettingService)
        {
            _systemSettingService = systemSettingService;
            _cache = cache;
            _url = _systemSettingService.getSettingValue(Const.Setting.S107);
            _user = _systemSettingService.getSettingValue(Const.Setting.S108);
            _pass = _systemSettingService.getSettingValue(Const.Setting.S109);
            _secret = _systemSettingService.getSettingValue(Const.Setting.S110);
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
            // 
            if (string.IsNullOrEmpty(videoSession))
            {
                if (string.IsNullOrEmpty(videoSession))
                {
                    initVideoSession();
                }
            }
            var mid = new Random().Next(1, 1000000);
            
            var postStr = "{\"mid\":" + mid + ",\"method\":\"playvideo\",\"param\":{\"devId\":\"" + bc.cameracode +
                 "\",\"channelid\":\"" + bc.channel + "\",\"streamid\":\"0\",\"session\":\"" + videoSession +
                 "\",\"secretKey\":\"" + _secret + "\"}}";

            string result = UhttpClient.PostJson("http://" + _url + "/netalarm-rs/videoplay/playvideo", postStr);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "未获取到直播流，请确认摄像头编号是否正确";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
            
            if (resjb.Property("rt") != null && (resjb["rt"].ToInt() == -500020 || resjb["errmsg"].ToString().Contains("session不存在")))
            {
                initVideoSession();
                postStr = "{\"mid\":" + mid + ",\"method\":\"playvideo\",\"param\":{\"devId\":\"" + bc.cameracode +
                "\",\"channelid\":\"" + bc.channel + "\",\"streamid\":\"0\",\"session\":\"" + videoSession +
                "\",\"secretKey\":\"" + _secret + "\"}}";
                result = UhttpClient.PostJson("http://" + _url + "/netalarm-rs/videoplay/playvideo", postStr);
                if (string.IsNullOrEmpty(result))
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "未获取到直播流，请确认摄像头编号是否正确";
                    return bnCameraResult;
                }
                resjb = JObject.Parse(result);
            }

             if (resjb.Property("rt") != null && resjb["rt"].ToInt() == 0)
            {
                string param = resjb["param"].ToString();
                JObject resresult = JObject.Parse(param);
                bnCameraResult.code = "0";
                bnCameraResult.url = (string)resresult.GetValue("url");
                bnCameraResult.hasplayback = "0";
                bnCameraResult.hasptz = "0";                    
            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("errmsg") != null)
                    bnCameraResult.msg = (string)resjb.GetValue("errmsg");

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
        /// <param name="speed">云台速度(取值范围为1-255，默认50)</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> ptz(BnCamera bc)
        {  
            string actioncommand = "ptz_move_start";
           
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            int leftspeed = 50;
            int upspeed = 50;
            if (!string.IsNullOrEmpty(bc.speed))
            {
                upspeed=leftspeed = int.Parse(bc.speed);
            }
            
            if(bc.action == "0")
            {
                actioncommand = "ptz_move_start";
            }

            if (bc.command.ToUpper() == "LEFT")
            {
               
                upspeed = 0;
            }
            else if (bc.command.ToUpper() == "RIGHT")
            {
                leftspeed = -leftspeed;
                upspeed = 0;
            }
            else if (bc.command.ToUpper() == "UP")
            {
                leftspeed = 0;
            }
            else if (bc.command.ToUpper() == "DOWN")
            {
                upspeed = -upspeed;
                leftspeed = 0;
            }
            var mid = new Random().Next(1, 1000000);

            var postStr = "{\"mid\":" + mid + ",\"session\":\"" + videoSession +
                "\",\"secretKey\":\"" + _secret + "\",\"method\":\"push_to_device\",\"req\":{\"method\":\""+ actioncommand+"\",\"param\":{\"channelid\":" + bc.channel + ",\"panLeft\":"+ leftspeed  + ",\"tiltUp\":" + upspeed + ",\"zoomIn\":0}}}";
            string result = UhttpClient.PostJson("http://" + _url + "/netalarm-rs/push/push_to_device", postStr);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "操作失败";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
            if (resjb.Property("rt") != null && (resjb["rt"].ToInt() == -500020 || resjb["errmsg"].ToString().Contains("session不存在")))
            {
                initVideoSession();
                postStr = "{\"mid\":" + mid + ",\"session\":\"" + videoSession +
                "\",\"secretKey\":\"" + _secret + "\",\"method\":\"push_to_device\",\"req\":{\"method\":\"" + actioncommand + "\",\"param\":{\"channelid\":" + bc.channel + ",\"panLeft\":" + leftspeed + ",\"tiltUp\":" + upspeed + ",\"zoomIn\":0}}}";
                result = UhttpClient.PostJson("http://" + _url + "/netalarm-rs/push/push_to_device", postStr);
                if (string.IsNullOrEmpty(result))
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "操作失败";
                    return bnCameraResult;
                }
                resjb = JObject.Parse(result);
            }

            if (resjb.Property("rt") != null && resjb["rt"].ToInt() == 0)
            {
                string param = resjb["param"].ToString();
                JObject resresult = JObject.Parse(param);
                bnCameraResult.code = "0";
                bnCameraResult.msg = "操作成功";
            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("errmsg") != null)
                    bnCameraResult.msg = (string)resjb.GetValue("errmsg");

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

          
            //DateTime starttime = DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture); ;
            // DateTime endtime = DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            DateTime starttime = (DateTime)bc.begintime;// DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);;
            DateTime endtime = (DateTime)bc.endtime;// DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            string uri = "/apiserver/v1/device/video/replay";
            Dictionary<string, object> dictparam = new Dictionary<string, object>()
            {
                
                  { "camera_id", bc.cameracode },
                   { "start_time", starttime.ToString("yyyy-MM-dd HH:mm:ss")},
                    { "end_time", endtime.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "source_type", recordLocation },
                    { "stream_mode", protocol },
                { "is_local", "1" }
            };
            string result = HttpNetRequest.SendRequest(_url + uri, dictparam, "GET", new Dictionary<string, string>() { });
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
            return bnCameraResult;
        }

        private void initVideoSession()
        {
            videoSession = "";
             var md5 = UEncrypter.EncryptByMD5("r4QD4vG9" + _pass);
            var mid = new Random().Next(1, 1000000);
            var postStr = "{\"mid\":" + mid + ",\"method\":\"login\",\"param\":{\"user\":\"" + _user +
                          "\",\"pwd\":\"" + md5 + "\",\"phone_id\":\"wx:trd:" + _user + "\"}}";
            try
            {

                var result = UhttpClient.PostJson("http://"+_url + "/netalarm-rs/userauth/login", postStr);
                if (!string.IsNullOrEmpty(result))
                {
                    JObject resjb = JObject.Parse(result);
                    if (resjb.Property("rt") != null && resjb["rt"].ToInt() == 0)
                    {
                        string param = resjb["param"].ToString();
                        JObject resresult = JObject.Parse(param);
                        videoSession = resresult["session"].ToString();
                    }

                    
                }
                
            }
            catch (Exception ex)
            {

            }
        }
    

}


}
