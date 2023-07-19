using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Video;

namespace xhs.build.Net.Controllers
{
    /// <summary>
    /// 摄像头实时操作
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        /// <summary>
        /// 摄像头实时操作
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
        public IResponseOutput GetRealUrl(BnCamera bc)

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

    }
}