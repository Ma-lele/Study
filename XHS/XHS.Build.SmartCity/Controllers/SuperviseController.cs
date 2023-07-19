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
using XHS.Build.Services.Supervise;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 监督业务
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SuperviseController : ControllerBase
    {
        private readonly ISupervise _SuperviseService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="superDangerService"></param>
        public SuperviseController(ISupervise superDangerService)
        {
            _SuperviseService = superDangerService;
        }

        /// <summary>
        /// --监督业务 整改效率统计
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="datayear">年份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetCloseCount(int datayear, int GROUPID = 0)
        {
            DataTable data = await _SuperviseService.GetCloseCountAsync(GROUPID, datayear);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// --监督业务 整改情况
        /// </summary>
        /// <param name="yearmonth">--年月</param>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetCloseCountList(string yearmonth, int GROUPID = 0)
        {
            var data = await _SuperviseService.GetCloseCountListAsync(GROUPID, yearmonth);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// --监督业务 总数统计
        /// </summary>
        /// <param name="datamonth">年月</param>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetGetCount(string datamonth, int GROUPID = 0)
        {
            var data = await _SuperviseService.GetCountAsync(GROUPID, datamonth);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// --监督业务 检查单类型分析
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="datayear">年份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTypeCount(int GROUPID, int datayear)
        {
            var data = await _SuperviseService.GetTypeCountAsync(GROUPID, datayear);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// --监督业务 检查用语排名
        /// </summary>
        /// <param name="GROUPID">0:市；非0:区</param>
        /// <param name="yearmonth">年月</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTypeRank(int GROUPID, string yearmonth)
        {
            var data = await _SuperviseService.GetTypeRankAsync(GROUPID, yearmonth);
            return ResponseOutput.Ok(data);
        }
    }
}
