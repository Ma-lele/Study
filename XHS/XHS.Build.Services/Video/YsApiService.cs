using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    /// 萤石云UIKit连接助手
    /// </summary>
    public class YsApiService : BaseServices<BaseEntity>, IYsApiService
    {

        private readonly IHpSystemSetting _systemSettingService;
        private readonly ICache _cache;


        public YsApiService(ICache cache, IHpSystemSetting systemSettingService)
        {
            _systemSettingService = systemSettingService;
            _cache = cache;
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
            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            bool falg = true;
            //萤石云
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                result.code = "1";
                result.msg = "摄像头编号未设定";
                return result;
            }
            var key = Const.Setting.YSToken;
            var appKey = _systemSettingService.getSettingValue(Const.Setting.S027);
            var appSecret = _systemSettingService.getSettingValue(Const.Setting.S028);
            var YSToken = "";
            if (!string.IsNullOrEmpty(bc.cameraparam) && bc.cameraparam.Contains("|"))
            {
                falg = false;
                appKey = bc.cameraparam.Split("|".ToCharArray())[0];
                appSecret = bc.cameraparam.Split("|".ToCharArray())[1];
            }
            else
            {
                if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
                {
                    YSToken = _cache.Get(key);
                }
                if (!string.IsNullOrEmpty(YSToken))
                {
                    JObject mJObj = new JObject();
                    mJObj = JObject.Parse(YSToken);
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                    long now = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                    long expireTime = long.Parse(mJObj["expireTime"].ToString());
                    if (expireTime > now)
                    {
                        result.appKey = appKey;
                        result.accessToken = mJObj["accessToken"].ToString();
                    }
                    else
                    {
                        YSToken = "";
                    }
                }
            }
            if (string.IsNullOrEmpty(YSToken))
            {
                var retString = HttpNetRequest.PostSendRequestUrl("https://open.ys7.com/api/lapp/token/get", new Dictionary<string, object>() { { "appKey", appKey }, { "appSecret", appSecret } });
                if (string.IsNullOrEmpty(retString))
                {
                    result.code = "1";
                    result.msg = "返回内容为空";
                    return result;
                }
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(retString);
                if (mJObj["code"].ToString() == "200")//成功
                {
                    JObject mJdata = (JObject)mJObj["data"];
                    result.appKey = appKey;
                    result.accessToken = mJdata["accessToken"].ToString();
                    if (falg)
                    {
                        _cache.Set(key, mJdata.ToString());
                    }
                }
                else
                {
                    result.code = "1";
                    result.msg = mJObj["msg"].ToString();
                    return result;
                }
            }

            result.hasptz = "1";
            result.hasplayback = "1";
            string streamtype = ".hd";
            if (bc.streamtype == "1")
            {
                streamtype = "";
            }
            result.url = "ezopen://open.ys7.com/" + bc.cameracode + "/" + bc.channel + streamtype + ".live";
            result.code = "0";
            return result;
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
            bool falg = true;
            JObject mJObj;
            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                result.msg = "摄像头编号必须传值";
                result.code = "1";
                return result;
            }
            if (string.IsNullOrEmpty(bc.action) || string.IsNullOrEmpty(bc.command))
            {
                result.msg = "云台指令参数错误";
                result.code = "1";
                return result;
            }
            var key = Const.Setting.YSToken;
            var appKey = _systemSettingService.getSettingValue(Const.Setting.S027);
            var appSecret = _systemSettingService.getSettingValue(Const.Setting.S028);
            var YSToken = "";
            if (!string.IsNullOrEmpty(bc.cameraparam) && bc.cameraparam.Contains("|"))
            {
                falg = false;
                appKey = bc.cameraparam.Split("|".ToCharArray())[0];
                appSecret = bc.cameraparam.Split("|".ToCharArray())[1];
            }
            else
            {
                if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
                {
                    YSToken = _cache.Get(key);
                }

                if (!string.IsNullOrEmpty(YSToken))
                {
                    mJObj = new JObject();
                    mJObj = JObject.Parse(YSToken);
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                    long now = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                    long expireTime = long.Parse(mJObj["expireTime"].ToString());
                    if (expireTime > now)
                    {
                        result.appKey = appKey;
                        result.accessToken = mJObj["accessToken"].ToString();
                    }
                    else
                    {
                        YSToken = "";
                    }
                }
            }
            var retString = "";
            if (string.IsNullOrEmpty(YSToken))
            {
                retString = HttpNetRequest.PostSendRequestUrl("https://open.ys7.com/api/lapp/token/get", new Dictionary<string, object>() { { "appKey", appKey }, { "appSecret", appSecret } });
                if (string.IsNullOrEmpty(retString))
                {
                    result.msg = "返回内容为空";
                    result.code = "1";
                    return result;
                }
                mJObj = new JObject();
                mJObj = JObject.Parse(retString);
                if (mJObj["code"].ToString() == "200")//成功
                {
                    JObject mJdata = (JObject)mJObj["data"];
                    result.appKey = appKey;
                    result.accessToken = mJdata["accessToken"].ToString();
                    if (falg)
                    {
                        _cache.Set(key, mJdata.ToString());
                    }

                }
                else
                {
                    result.msg = mJObj["msg"].ToString();
                    result.code = "1";
                    return result;
                }
            }
            mJObj = new JObject();
            int direction = 3;
            if (bc.command.ToUpper() == "LEFT")
            {
                direction = 2;
            }
            else if (bc.command.ToUpper() == "UP")
            {
                direction = 0;
            }
            else if (bc.command.ToUpper() == "DOWN")
            {
                direction = 1;
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("accessToken", result.accessToken);
            param.Add("deviceSerial", bc.cameracode);
            param.Add("channelNo", bc.channel);
            param.Add("direction", direction);
            param.Add("speed", 1);
            //param.Add("endTime", form["tm"] + " 23:59:59");
            string url = "https://open.ys7.com/api/lapp/device/ptz/stop";
            if (bc.action.ToUpper() == "1")
            {
                url = "https://open.ys7.com/api/lapp/device/ptz/start";
            }
            retString = HttpNetRequest.PostSendRequestUrl(url, param);
            if (string.IsNullOrEmpty(retString))
            {
                result.msg = "返回内容为空";
                result.code = "1";
                return result;
            }
            mJObj = new JObject();
            mJObj = JObject.Parse(retString);
            if (mJObj["code"].ToString() == "200")//成功
            {
                result.code = "0";
            }
            else
            {
                result.msg = mJObj["msg"].ToString();
                result.code = "1";
            }

            return result;
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
            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            bool flag = true;
            //萤石云
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                result.msg = "摄像头编号必须传值";
                result.code = "1";
                return result;
            }

            if (bc.begintime == null)
            {
                result.msg = "开始时间必须传值";
                result.code = "1";
                return result;
            }
            if (bc.endtime == null)
            {
                result.msg = "结束时间必须传值";
                result.code = "1";
                return result;
            }
            DateTime starttime = (DateTime)bc.begintime;// DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture); ;
            DateTime endtime = (DateTime)bc.endtime;// DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

            var key = Const.Setting.YSToken;
            var appKey = _systemSettingService.getSettingValue(Const.Setting.S027);
            var appSecret = _systemSettingService.getSettingValue(Const.Setting.S028);
            var YSToken = "";
            if (!string.IsNullOrEmpty(bc.cameraparam) && bc.cameraparam.Contains("|"))
            {
                flag = false;
                appKey = bc.cameraparam.Split("|".ToCharArray())[0];
                appSecret = bc.cameraparam.Split("|".ToCharArray())[1];
            }
            else
            {
                if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
                {
                    YSToken = _cache.Get(key);
                }
                if (!string.IsNullOrEmpty(YSToken))
                {
                    JObject mJObj = new JObject();
                    mJObj = JObject.Parse(YSToken);
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                    long now = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                    long expireTime = long.Parse(mJObj["expireTime"].ToString());
                    if (expireTime > now)
                    {
                        result.appKey = appKey;
                        result.accessToken = mJObj["accessToken"].ToString();
                    }
                    else
                    {
                        YSToken = "";
                    }
                }
            }
            if (string.IsNullOrEmpty(YSToken))
            {
                var retString = HttpNetRequest.PostSendRequestUrl("https://open.ys7.com/api/lapp/token/get", new Dictionary<string, object>() { { "appKey", appKey }, { "appSecret", appSecret } });
                if (string.IsNullOrEmpty(retString))
                {
                    result.msg = "返回内容为空";
                    result.code = "1";
                    return result;
                }
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(retString);
                if (mJObj["code"].ToString() == "200")//成功
                {
                    JObject mJdata = (JObject)mJObj["data"];
                    result.appKey = appKey;
                    result.accessToken = mJdata["accessToken"].ToString();
                    if (flag)
                    {
                        _cache.Set(key, mJdata.ToString());
                    }

                }
                else
                {
                    result.msg = mJObj["msg"].ToString();
                    result.code = "1";
                    return result;
                }
            }
            string recordLocation = "";
            if (bc.recordLocation == "0")
            {
                recordLocation = ".cloud";
            }
            result.url = "ezopen://open.ys7.com/" + bc.cameracode + "/" + bc.channel + recordLocation + ".rec?begin=" + string.Format("{0:yyyyMMddHHmmss}", starttime) + "&end=" + string.Format("{0:yyyyMMddHHmmss}", endtime) + "&recType=1";
            result.code = "0";
            return result;
        }

        /// <summary>
        /// 获取萤石云直播流地址
        /// </summary>
        /// <param name="deviceSerial">摄像头编号</param>
        /// <param name="channelNo">通道号</param>
        /// <param name="expireTime">过期时长</param>
        /// <param name="protocol">播放协议</param>
        /// <returns></returns>
        public string Getysyurl(BnCamera bc)
        {
            bool falg = true;
            var key = Const.Setting.YSToken;
            var appKey = _systemSettingService.getSettingValue(Const.Setting.S027);
            var appSecret = _systemSettingService.getSettingValue(Const.Setting.S028);
            var YSToken = "";
            //取萤石云直播流地址
            string url = "https://open.ys7.com/api/lapp/v2/live/address/get";

            //if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
            //{
            //    YSToken = _cache.Get(key);
            //}
            if (!string.IsNullOrEmpty(YSToken))
            {
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(YSToken);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long now = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
                long expireTime = long.Parse(mJObj["expireTime"].ToString());
                if (expireTime < now)
                {
                    YSToken = "";
                }
            }

            if (string.IsNullOrEmpty(YSToken))
            {
                var retString = HttpNetRequest.PostSendRequestUrl("https://open.ys7.com/api/lapp/token/get", new Dictionary<string, object>() { { "appKey", appKey }, { "appSecret", appSecret } });
                //if (string.IsNullOrEmpty(retString))
                //{
                //    return "Token返回内容为空";
                //}
                JObject mJObj = new JObject();
                mJObj = JObject.Parse(retString);
                if (mJObj["code"].ToString() == "200")//成功
                {
                    JObject mJdata = (JObject)mJObj["data"];
                    YSToken = mJdata["accessToken"].ToString();
                    if (falg)
                    {
                        _cache.Set(key, mJdata["accessToken"].ToString());
                    }
                }

            }

            Dictionary<string, object> postData = new Dictionary<string, object>();
            postData.Add("deviceSerial", bc.cameracode);
            postData.Add("channelNo", bc.channel);
            postData.Add("expireTime", 7200);
            postData.Add("protocol", 2);
            postData.Add("quality", 1);
            postData.Add("accessToken", YSToken);
            var result = HttpNetRequest.PostForm(url, postData, new Dictionary<string, string>() { });
            if (!string.IsNullOrEmpty(result))
            {
                JObject resobj = JObject.Parse(result);
                if (resobj.GetValue("code").ToString() == "200")
                {
                    result = resobj["data"]["url"].ToString();
                }
                else
                {
                    return "";
                }
            }
            return result;
        }

    }


}
