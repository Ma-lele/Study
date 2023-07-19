using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Cameras;
using XHS.Build.Services.Video;
using XHS.Build.SiteFront.Attributes;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 视频
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    [Permission]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ICameraService _cameraService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoService"></param>
        public VideoController(IVideoService videoService, ICameraService cameraService, IUser user)
        {
            _videoService = videoService;
            _cameraService = cameraService;
            _user = user;
        }

        /// <summary>
        /// 获取监测点下的摄像头列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T041/[action]")]
        public async Task<IResponseOutput> GetCameraList()
        {
            List<BnCamera> result = new List<BnCamera>();
            try
            {
                DataTable dt = await _cameraService.getCameraList(int.Parse(_user.SiteId));
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnCamera bc = new BnCamera();
                    bc.CAMERAID = Convert.ToInt32(dt.Rows[i]["CAMERAID"]);
                    bc.cameraname = Convert.ToString(dt.Rows[i]["cameraname"]);
                    bc.cameracode = Convert.ToString(dt.Rows[i]["cameracode"]);
                    bc.channel = Convert.ToInt32(dt.Rows[i]["channel"]);
                    bc.cameraparam = Convert.ToString(dt.Rows[i]["cameraparam"]);
                    bc.cameratype = Convert.ToInt32(dt.Rows[i]["cameratype"]);
                    bc.bonline = Convert.ToInt32(dt.Rows[i]["bonline"]);
                    result.Add(bc);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取监测点下的摄像头列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        public async Task<IResponseOutput> GetCameraListByDevCode(string devcode)
        {
            List<BnCamera> result = new List<BnCamera>();
            try
            {
                DataTable dt = await _cameraService.getCameraListByDevCode(devcode);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnCamera bc = new BnCamera();
                    bc.CAMERAID = Convert.ToInt32(dt.Rows[i]["CAMERAID"]);
                    bc.cameraname = Convert.ToString(dt.Rows[i]["cameraname"]);
                    bc.cameracode = Convert.ToString(dt.Rows[i]["cameracode"]);
                    bc.channel = Convert.ToInt32(dt.Rows[i]["channel"]);
                    bc.cameraparam = Convert.ToString(dt.Rows[i]["cameraparam"]);
                    bc.cameratype = Convert.ToInt32(dt.Rows[i]["cameratype"]);
                    bc.bonline = Convert.ToInt32(dt.Rows[i]["bonline"]);
                    result.Add(bc);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取摄像头直播流地址
        /// </summary>
        /// <param name="bc">摄像头信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("T031/[action]")]
        [Route("T041/[action]")]
        public async Task<IResponseOutput> GetRealUrl(BnCamera bc)

        {

            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            result = _videoService.GetRealurl(bc);
            if (result.code == "0")
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk(result.msg);
            }


        }

        /// <summary>
        /// 云台操作，对监控点云台方向，转动速度进行操作。
        /// </summary>
        /// <param name="bc">摄像头信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("T041/[action]")]
        public async Task<IResponseOutput> Ptz(BnCamera bc)

        {
            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            result = _videoService.ptz(bc);
            if (result.code == "0")
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk(result.msg);
            }

        }


        /// <summary>
        /// 获取摄像头回看流地址
        /// </summary>
        /// <param name="bc">摄像头信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("T041/[action]")]
        public async Task<IResponseOutput> GetPlayBackUrl(BnCamera bc)

        {
            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            result = _videoService.GetPlayBackurl(bc);
            if (result.code == "0")
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk(result.msg);
            }

        }
    }
}
