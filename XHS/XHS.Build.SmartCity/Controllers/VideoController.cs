using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Services.Video;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 视频
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoService"></param>
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }


        /// <summary>
        /// 已安装监控的项目/视频
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int GROUPID=0)
        {
            
            var data = await _videoService.GetListAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }
       


    }
}
