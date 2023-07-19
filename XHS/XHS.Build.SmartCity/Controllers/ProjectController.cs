using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.Project;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 数据统计项目管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectService"></param>
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// 1.今日统计
        /// projstatus：1待审、2在建、3停工、4终止安监、5竣工
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAnalyseCount(int GROUPID = 0)
        {
            DataRow data = await _projectService.GetAnalyseCountAsync(GROUPID);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 超90天未竣工
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAnalyseOver90(int GROUPID = 0)
        {
            var data = await _projectService.GetAnalyseOver90Async(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 项目类型统计
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAnalyseTypeCount(int GROUPID = 0)
        {
            var data = await _projectService.GetAnalyseTypeCountAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 每月项目新增数统计
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="datayear">年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAnalyseYearCount(int GROUPID = 0, int datayear = 0)
        {
            var data = await _projectService.GetAnalyseYearCountAsync(GROUPID, datayear);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 【项目管理 列表】
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页几条</param>
        /// <param name="keyword">关键字</param>
        /// <param name="company">公司名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetGetList(int GROUPID = 0, int pageindex = 1, int pagesize = 20, string keyword = "", string company = "")
        {
            var data = await _projectService.GetListAsync(GROUPID, pageindex, pagesize, keyword, company);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 【项目管理 监测设备列表】
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页几条</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetDevList(int GROUPID = 0, int pageindex = 1, int pagesize = 20, string keyword = "")
        {
            var data = await _projectService.GetDevListAsync(GROUPID, pageindex, pagesize, keyword);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分析网站列表
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GettAnalyseSiteList(int GROUPID = 0)
        {
            var data = await _projectService.GettAnalyseSiteListAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 超90天未竣工
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">0:全部，1: 待审, 2: 在建, 3: 停工, 4: 终止安监, 5: 竣工 6:本月标化考评项目 7:超90天未竣工项目</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAnalyseSiteListByType(int GROUPID = 0, int type = 0)
        {
            var data = await _projectService.GetAnalyseSiteListByTypeAsync(GROUPID, type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取集成商在线率列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyword"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetIntegratorList(int GROUPID, DateTime startdate, DateTime enddate, int pageindex = 1, int pagesize = 10, string keyword = "")
        {
            var data = await _projectService.GetIntegratorListAsync(GROUPID, startdate, enddate, pageindex, pagesize, keyword);
            return ResponseOutput.Ok(data);
        }

    }
}
