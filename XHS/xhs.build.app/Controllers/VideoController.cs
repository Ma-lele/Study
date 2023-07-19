using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Video;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 摄像头实时，云台，回看操作
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        /// <summary>
        /// 摄像头实时，云台，回看操作
        /// </summary>
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        /// <summary>
        /// 获取摄像头直播流地址
        /// </summary>
        /// <param name="bc">摄像头信息</param>
        /// <returns></returns>
        [HttpPost]
        [Route("getRealUrl")]
        public async Task<IResponseOutput> GetRealUrl(BnCamera bc)

        {
           
            BnCameraResult<BnPlaybackURL> result = new BnCameraResult<BnPlaybackURL>();
            result = _videoService.GetRealurl(bc);
            if(result.code == "0")
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
        [Route("ptz")]
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
        [Route("getPlayBackUrl")]
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