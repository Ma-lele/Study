using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Util;
using XHS.Build.Common;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Service.Video
{
    /// <summary>
    /// 海康8200OpenApi连接助手
    /// </summary>
    public class HwOpenApiService : BaseServices<BaseEntity>, IHwOpenApiService
    {

        /// <summary>
        /// 平台端口
        /// </summary>
        private static string _host;

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
        private static string _port;

        public static string videoSession = "";

        private readonly IHpSystemSetting _systemSettingService;
        private readonly ICache _cache;
        private static string _hwurl = "https://139.159.181.177:8843/devHelp/getTryItResult";

        public HwOpenApiService(ICache cache, IHpSystemSetting systemSettingService)
        {
            _systemSettingService = systemSettingService;
            _cache = cache;
            _host = _systemSettingService.getSettingValue(Const.Setting.S085);
            _user = _systemSettingService.getSettingValue(Const.Setting.S083);
            _pass = _systemSettingService.getSettingValue(Const.Setting.S084);
            _port = _systemSettingService.getSettingValue(Const.Setting.S086);
            if (string.IsNullOrEmpty(Const.Setting.S199))
            {
                _hwurl = _systemSettingService.getSettingValue(Const.Setting.S199);
            }
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
            if (string.IsNullOrEmpty(bc.cameraparam))
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
                    protocol = "3";
                }else if(bc.protocol.ToUpper() == "HLS"){
                    protocol = "2";
                }
            }
            NameValueCollection postData = new NameValueCollection();
            postData.Set("host", _host);
            postData.Set("port", _port);
            postData.Set("username", _user);
            postData.Set("password", _pass);
            postData.Set("httpTimeout", "30");
            postData.Set("uri", "/api/media/live");
            postData.Set("requestJson", "{\"cameraId\":\"" + bc.cameraparam +"\",\"streamType\":" + streamtype + ",\"agentType\":1,\"urlType\":" + protocol + "}");
            
            string result = UHttp.PostForm(_hwurl, postData);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "未获取到直播流，请确认摄像头编号是否正确";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
             if (resjb.Property("status") != null && resjb["status"].ToInt() ==200)
            {
                string param = resjb["data"].ToString();
                JObject resresult = JObject.Parse(param);
                string code = (string)resresult.GetValue("resultCode");
                if(code != "0")
                {
                    bnCameraResult.code = code;
                    bnCameraResult.msg = (string)resresult.GetValue("resultDesc");
                    return bnCameraResult;
                }
                bnCameraResult.code = "0";
                bnCameraResult.url = (string)resresult.GetValue("url");
                bnCameraResult.hasplayback = "1";
                bnCameraResult.hasptz = "0";
                postData = new NameValueCollection();
                postData.Set("host", _host);
                postData.Set("port", _port);
                postData.Set("username", _user);
                postData.Set("password", _pass);
                postData.Set("httpTimeout", "30");
                postData.Set("uri", "/api/dev/detail");
                postData.Set("requestJson", "{\"cameraId\":\"" + bc.cameraparam + "\"}");
                result = UHttp.PostForm(_hwurl, postData);
                if (!string.IsNullOrEmpty(result))
                {
                    resjb = JObject.Parse(result);
                    if (resjb.Property("status") != null && resjb["status"].ToInt() == 200)
                    {
                        string param1 = resjb["data"].ToString();
                        resresult = JObject.Parse(param1);
                        JObject cameraInfo = JObject.Parse(resresult["cameraInfo"].ToString());
                        string ptzType = cameraInfo["ptzType"].ToString();
                        if (!string.IsNullOrEmpty(ptzType))
                        {
                            if (ptzType == "2" || ptzType == "3")
                            {
                                bnCameraResult.hasptz = "1";
                            }
                        }
                    }
                }

            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("msg") != null)
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
        /// <param name="speed">云台速度(取值范围为1-255，默认50)</param>
        /// <returns></returns>
        public BnCameraResult<BnPlaybackURL> ptz(BnCamera bc)
        {  
            string actioncommand = "PTZ_STOP";
           
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameracode))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            int speed = 2;
            if (!string.IsNullOrEmpty(bc.speed))
            {
                speed = int.Parse(bc.speed)/10;
            }
            
            if(bc.action == "1")
            {
                if (bc.command.ToUpper() == "LEFT")
                {
                    actioncommand = "PTZ_LEFT";
                }
                else if (bc.command.ToUpper() == "RIGHT")
                {
                    actioncommand = "PTZ_RIGHT";
                }
                else if (bc.command.ToUpper() == "UP")
                {
                    actioncommand = "PTZ_UP";
                }
                else if (bc.command.ToUpper() == "DOWN")
                {
                    actioncommand = "PTZ_DOWN";
                }

            }
            NameValueCollection postData = new NameValueCollection();
            postData.Set("host", _host);
            postData.Set("port", _port);
            postData.Set("username", _user);
            postData.Set("password", _pass);
            postData.Set("httpTimeout", "30");
            postData.Set("uri", "/api/ptz/control");
            postData.Set("requestJson", "{\"cameraId\":\"" + bc.cameraparam + "\",\"opCode\":\"" + actioncommand + "\",\"param1\":\"2\",\"param2\":\"" + speed + "\"}");

            string result = UHttp.PostForm(_hwurl, postData);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "操作失败。";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
            if (resjb.Property("status") != null && resjb["status"].ToInt() == 200)
            {
                string param = resjb["data"].ToString();
                JObject resresult = JObject.Parse(param);
                string code = (string)resresult.GetValue("resultCode");
                bnCameraResult.code = code;
                bnCameraResult.msg = (string)resresult.GetValue("resultDesc");
            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("msg") != null)
                    bnCameraResult.msg = (string)resjb.GetValue("msg");
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
            string streamtype = "1";
            string recordLocation = "0";
            string protocol = "1";
            BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
            if (string.IsNullOrEmpty(bc.cameraparam))
            {
                bnCameraResult.code = "1000";
                bnCameraResult.msg = "摄像头编号必须传值";
                return bnCameraResult;
            }
            if (!string.IsNullOrEmpty(bc.streamtype))
            {
                streamtype = bc.streamtype;
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
                    protocol = "3";
                }
                else if (bc.protocol.ToUpper() == "HLS")
                {
                    protocol = "2";
                }
            }
            DateTime starttime = (DateTime)bc.begintime;// DateTime.ParseExact(bc.begintime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);;
            DateTime endtime = (DateTime)bc.endtime;// DateTime.ParseExact(bc.endtime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            
            string requestJson = "{\"pageInfo\":{\"pageNum\":1,\"pageSize\":1},\"cameraList\":[{\"cameraId\":\"" + bc.cameraparam + "\"}],\"searchInfo\":{\"from\":\"PLATFORM\",\"beginTime\":\"" + starttime.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"endTime\":\"" + endtime.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"eventList\":[{\"event\":\"TIMING\"}]}}";
            NameValueCollection postData = new NameValueCollection();
            postData.Set("host", _host);
            postData.Set("port", _port);
            postData.Set("username", _user);
            postData.Set("password", _pass);
            postData.Set("httpTimeout", "30");
            postData.Set("uri", "/api/record/list");
            postData.Set("requestJson", requestJson);
            string nvrCode = "";
            string result = UHttp.PostForm(_hwurl, postData);
            if (string.IsNullOrEmpty(result))
            {
                bnCameraResult.code = "1";
                bnCameraResult.msg = "未查询到录像。";
                return bnCameraResult;
            }
            JObject resjb = JObject.Parse(result);
            if (resjb.Property("status") != null && resjb["status"].ToInt() == 200)
            {
                string param = resjb["data"].ToString();
                JObject resresult = JObject.Parse(param);
                string code = (string)resresult.GetValue("resultCode");
                if (code != "0")
                {
                    bnCameraResult.code = code;
                    bnCameraResult.msg = (string)resresult.GetValue("resultDesc");
                    return bnCameraResult;
                }
                JObject pageInfo = (JObject)resresult.GetValue("pageInfo");
                int totalNum = (int)pageInfo.GetValue("totalNum");
                JArray recordarray = null;
                if (totalNum > 0)
                {
                    recordarray = (JArray)resresult.GetValue("recordList");
                }
                else
                {
                    bnCameraResult.code = "1";
                    bnCameraResult.msg = "未查询到录像。";
                    return bnCameraResult;
                }
                if (recordarray != null)
                {
                    JObject record = (JObject)recordarray[0];
                    nvrCode = (string)record.GetValue("nvrCode");
                    recordLocation = (string)record.GetValue("location");
                    requestJson = "{\"cameraId\":\"" + bc.cameraparam + "\",\"streamType\":" + streamtype + ",\"agentType\":1,\"urlType\":" + protocol + ",\"vodType\":\"vod\",\"vodInfo\":{\"contentId\":\"\",\"cameraId\":\"" + bc.cameraparam + "\",\"beginTime\":\"" + starttime.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"endTime\":\"" + endtime.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"location\":" + recordLocation + ",\"nvrCode\":\""+ nvrCode+ "\"}}";
                    postData = new NameValueCollection();
                    postData.Set("host", _host);
                    postData.Set("port", _port);
                    postData.Set("username", _user);
                    postData.Set("password", _pass);
                    postData.Set("httpTimeout", "30");
                    postData.Set("uri", "/api/record/url");
                    postData.Set("requestJson", requestJson);
                    result = UHttp.PostForm(_hwurl, postData);
                    if (string.IsNullOrEmpty(result))
                    {
                        bnCameraResult.code = "1";
                        bnCameraResult.msg = "未查询到录像。";
                        return bnCameraResult;
                    }
                    resjb = JObject.Parse(result);
                    if (resjb.Property("status") != null && resjb["status"].ToInt() == 200)
                    {
                         param = resjb["data"].ToString();
                         resresult = JObject.Parse(param);
                         code = (string)resresult.GetValue("resultCode");
                        if (code != "0")
                        {
                            bnCameraResult.code = code;
                            bnCameraResult.msg = (string)resresult.GetValue("resultDesc");
                            return bnCameraResult;
                        }
                        bnCameraResult.code = "0";
                        bnCameraResult.url = (string)resresult.GetValue("url");
                        bnCameraResult.hasplayback = "1";
                    }
                    else
                    {
                        bnCameraResult.code = "3";
                        if (resjb.Property("msg") != null)
                            bnCameraResult.msg = (string)resjb.GetValue("msg");
                    }
                }
            }
            else
            {
                bnCameraResult.code = "3";
                if (resjb.Property("msg") != null)
                    bnCameraResult.msg = (string)resjb.GetValue("msg");
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


}
