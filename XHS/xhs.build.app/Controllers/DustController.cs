using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Services.Dust;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 扬尘
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DustController : ControllerBase
    {
        private readonly IDustService _dustService;
        public DustController(IDustService dustService)
        {
            _dustService = dustService;
        }

        /// <summary>
        /// 根据设备种类获取监测对象
        /// </summary>
        /// <param name="linktype"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("devicetypelist")]
        public async Task<IResponseOutput> GetDeviceTypeList(int linktype)
        {
            if (linktype <= 0)
            {
                return ResponseOutput.NotOk("请选择正确的类型");
            }
            return ResponseOutput.Ok(await _dustService.getListByDeviceType(linktype));
        }

        /// <summary>
        /// 施工扬尘 工程航拍
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("flyvideos")]
        public async Task<IResponseOutput> GetFlyVideoList()
        {
            return ResponseOutput.Ok(await _dustService.GetFlyVideoList());
        }

        /// <summary>
        /// 临边围挡 预警列表
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("warninglist")]
        public async Task<IResponseOutput> GetWarningList(int SITEID, int type, DateTime startdate, DateTime enddate)
        {
            return ResponseOutput.Ok(await _dustService.GetWarningList(SITEID, type, startdate, enddate));
        }

        /// <summary>
        /// 获取用户负责的所有雾泡
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("foglist")]
        public async Task<IResponseOutput> GetFogListByUser()
        {
            return ResponseOutput.Ok(await _dustService.GetFogListByUser());
        }
    }
}