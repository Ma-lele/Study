using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Repository.Base;
using XHS.Build.Service.Video;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.Video
{
    public class VideoService:BaseServices<BaseEntity>,IVideoService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _videoRepository;
        private readonly IHkOpenApiService _hkOpenApiService;
        private readonly IHk8200ApiService _hk8200ApiService;
        private readonly IDiaoGouApiService _diaoGouApiService;
        private readonly IAQSApiService _aqsApiService;
        private readonly IYsApiService _ysApiService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IHpXiongMaiService _hpXiongMaiService;
        private readonly IHwOpenApiService _hwOpenApiService;
        private readonly IHpJBVideoService _hpJBVideoService;
        private readonly DayunToken _dayunToken;
        private readonly ICache _cache;
        

        public VideoService(ICache cache, DayunToken dayunToken,IHpSystemSetting hpSystemSetting, IHpXiongMaiService hpXiongMaiService, IYsApiService ysApiService, IAQSApiService aqsApiService, IUser user, IBaseRepository<BaseEntity> videoRepository, IHkOpenApiService hkOpenApiService, IHk8200ApiService hk8200ApiService, IDiaoGouApiService diaoGouApiService, IHwOpenApiService hwOpenApiService, IHpJBVideoService hpJBVideoService)
        {
            _user = user;
            _videoRepository = videoRepository;
            BaseDal = videoRepository;
            _hkOpenApiService = hkOpenApiService;
            _hk8200ApiService = hk8200ApiService;
            _diaoGouApiService = diaoGouApiService;
            _aqsApiService = aqsApiService;
            _hpXiongMaiService = hpXiongMaiService;
            _ysApiService = ysApiService;
            _cache = cache;
            _hpSystemSetting = hpSystemSetting;
            _hwOpenApiService = hwOpenApiService;
            _hpJBVideoService = hpJBVideoService;
            _dayunToken = dayunToken;
        }

        public async Task<int> doDelete(int VIDEOID)
        {
            return await _videoRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSiteVideoDelete", new { VIDEOID = VIDEOID });
        }

        public async Task<int> doInsert(object param)
        {
            return await _videoRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSiteVideoInsert", param);
        }

        public async Task<DataSet> getList()
        {
            return await _videoRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteVideoList", new { USERID = _user.Id });
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
            if (bc.cameratype == 1)
            {
                //萤石云
                if (bc.action == "hls")
                {
                    string url = _ysApiService.Getysyurl(bc);
                    result.code = "0";
                    result.url = url;
                }
                else
                {
                    result = _ysApiService.GetRealurl(bc);
                }


            }
            else if (bc.cameratype == 22)
            {
                //萤石云
                string url = _ysApiService.Getysyurl(bc);
                result.code = "0";
                result.url = url;
            }
            else if (bc.cameratype == 4)
            {
                //直播流方式
                if (string.IsNullOrEmpty(bc.cameracode))
                {
                    result.msg = "直播流地址未设定";
                    result.code = "1";
                }
                result.code = "0";
                result.hasptz = "0";
                result.hasplayback = "0";
                result.url = bc.cameracode;
            }
            else if (bc.cameratype == 5)
            {
                //华为千里眼
                result = _hwOpenApiService.GetRealurl(bc);

            }
            else if (bc.cameratype == 6)
            {
                //阿启视
                result = _aqsApiService.GetRealurl(bc);

            }
            else if (bc.cameratype == 12 || bc.cameratype == 13 || bc.cameratype == 14)
            {
                //海康8200api
                result = _hk8200ApiService.GetRealurl(bc);

            }
            else if (bc.cameratype == 15)
            {
                //吊钩视频
                result = _diaoGouApiService.GetRealurl(bc);

            }
            else if (bc.cameratype == 17)
            {
                //江北视频
                result = _hpJBVideoService.GetRealurl(bc);

            }
            else if (bc.cameratype == 16 || bc.cameratype == 18 || bc.cameratype == 23 || bc.cameratype == 24)
            {
                //海康api
                result = _hkOpenApiService.GetRealurl(bc);

            }
            else if (bc.cameratype == 19)
            {
                //雄迈视频
                result = _hpXiongMaiService.GetRealurl(bc);

            }
            else if (bc.cameratype == 20)
            {
                //大运视频
                BnCameraResult<BnPlaybackURL> bnCameraResult = new BnCameraResult<BnPlaybackURL>();
                string url = "http://zh1.wxzjj.com:5678";
                string uploadapi = "rest/ProjectInfo/getVideoUrl";
                JObject jObject = new JObject();
                jObject.Add("id", bc.cameracode);
                string data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                JObject jresult = JObject.Parse(data);
                if (jresult.ContainsKey("flag") && jresult.GetValue("flag").ToString() == "0000")
                {
                    JObject jurl = JObject.Parse(jresult.GetValue("result").ToString());
                    result.code = "0";
                    result.url = (string)jurl.GetValue("url");
                    result.hasplayback = "0";
                    result.hasptz = "0";
                }
                else
                {
                    result.code = "1";
                    result.msg = jresult.GetValue("msg").ToString();
                }

            }
            else
            {
                result.code = "1";
                result.msg = "暂不支持的摄像头类型";
            }
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
            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            if (bc.cameratype == 1)
            {
                result = _ysApiService.ptz(bc);

            }
            else if (bc.cameratype == 5)
            {
                //华为千里眼
                result = _hwOpenApiService.ptz(bc);

            }
            else if (bc.cameratype == 6)
            {
                //阿启视
                result = _aqsApiService.ptz(bc);


            }
            else if (bc.cameratype == 12 || bc.cameratype == 13 || bc.cameratype == 14)
            {
                //海康8200api
                result = _hk8200ApiService.ptz(bc);
            }
            else if (bc.cameratype == 16 || bc.cameratype == 18)
            {
                //海康api
                result = _hkOpenApiService.ptz(bc);
            }
            else
            {
                result.code = "1";
                result.msg = "暂不支持的摄像头类型";
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
            if (bc.cameratype == 1)
            {
                //萤石云
                result = _ysApiService.GetPlayBackurl(bc);
            }
            else if (bc.cameratype == 5)
            {
                //华为千里眼
                result = _hwOpenApiService.GetPlayBackurl(bc);

            }
            else if (bc.cameratype == 6)
            {
                //阿启视
                result = _aqsApiService.GetPlayBackurl(bc);
            }
            else if (bc.cameratype == 17)
            {
                //江北视频
                result = _hpJBVideoService.GetPlayBackurl(bc);

            }
            else if (bc.cameratype == 16 || bc.cameratype == 18)
            {
                //海康api
                result = _hkOpenApiService.GetPlayBackurl(bc);
            }
            else if (bc.cameratype == 19)
            {
                //雄迈视频
                result = _hpXiongMaiService.GetPlayBackurl(bc);

            }
            else
            {
                result.code = "1";
                result.msg = "暂不支持的摄像头类型";
            }
            return result;
        }

        public Task<DataTable> GetListAsync(int GROUPID)
        {
            return _videoRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtVideoList", new { GROUPID = GROUPID});

        }
    }
}
