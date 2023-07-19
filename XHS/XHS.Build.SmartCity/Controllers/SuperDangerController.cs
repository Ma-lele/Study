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
using XHS.Build.Services.SuperDanger;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 超危项目
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SuperDangerController : ControllerBase
    {
        private readonly ISuperDanger _superDangerService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="superDangerService"></param>
        public SuperDangerController(ISuperDanger superDangerService)
        {
            _superDangerService = superDangerService;
        }

        /// <summary>
        /// 超危项目统计详情
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="pageindex">支持分页抓取 页码(从1开始)</param>
        /// <param name="pagesize">每页抓取数据条数</param>
        /// <param name="keyword">内容</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int GROUPID = 0, int pageindex = 1, int pagesize = 20, string keyword = "")
        {
            var data = await _superDangerService.GetListAsync(GROUPID, pageindex, pagesize, keyword);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 文件内容
        /// </summary>
        /// <param name="SDID">SDID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetFile(string SDID)
        {
            var data = await _superDangerService.GetFileAsync(SDID);
            return ResponseOutput.Ok(JArray.Parse(data.Rows[0]["files"].ToString()));

        }

        /// <summary>
        /// 模块详情
        /// </summary>
        /// <param name="SDID">SDID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOne(string SDID)
        {
            DataRow data = await _superDangerService.GetOneAsync(SDID);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 超危项目统计
        /// </summary>
        /// <param name="REGIONID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetCount(int REGIONID)
        {
            var data = await _superDangerService.GetCount(REGIONID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 超危项目类型统计
        /// </summary>
        /// <param name="REGIONID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTypeCount(int REGIONID)
        {
            var data = await _superDangerService.GetTypeCount(REGIONID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 超危项目状态统计
        /// </summary>
        /// <param name="REGIONID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetStatusCount(int REGIONID)
        {
            var data = await _superDangerService.GetStatusCount(REGIONID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取有超危工程的项目列表
        /// </summary>
        /// <param name="REGIONID"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteList(int REGIONID, int pageindex = 1, int pagesize = 20, string keyword = "")
        {
            var data = await _superDangerService.GetSiteList(REGIONID, pageindex, pagesize, keyword);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取单个项目的超危工程
        /// </summary>
        /// <param name="siteajcode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetBySite(string siteajcode)
        {
            var data = await _superDangerService.GetBySite(siteajcode);
            return ResponseOutput.Ok(data);
        }
    }
}
