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
using XHS.Build.Services.Env;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 环境监测
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class EnvController : ControllerBase
    {
        private readonly IEnvService _envService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="envService"></param>
        public EnvController(IEnvService envService)
        {
            _envService = envService;
        }

        /// <summary>
        /// 安装各种监测设备的工地数量
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSiteCount(int GROUPID = 0)
        {
            DataRow data = await _envService.GetSiteCountAsync(GROUPID);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 扬尘PM10监测告警分布图
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1:今日，2：一周，3：一个月，4：一年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWarnCount(int GROUPID = 0, int type = 1)
        {
            var data = await _envService.GetWarnCountAsync(GROUPID, type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 环境告警统计
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWarnCountTotal(int GROUPID = 0)
        {
            var data = await _envService.GetWarnCountTotalAsync(GROUPID);
            List<dynamic> list = new List<dynamic>();
            foreach (DataTable table in data.Tables)
            {
                var obj = new
                {
                    index = table.Rows[0]["index"],
                    pmcount = table.Rows[0]["pmcount"],
                    washcount = table.Rows[0]["washcount"],
                    solicount = table.Rows[0]["solicount"],
                    airtightcount = table.Rows[0]["airtightcount"],
                    totalcount = table.Rows[0]["totalcount"],
                };
                list.Add(obj);
            }
            return ResponseOutput.Ok(list);
        }

        /// <summary>
        /// 车辆未冲洗统计[AI识别]
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1:今日，2：一周，3：一个月，4：一年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWashCount(int GROUPID = 0, int type = 1)
        {
            DataRow data = await _envService.GetWashCountAsync(GROUPID, type);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 车辆密闭统计[AI识别]
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1:今日，2：一周，3：一个月，4：一年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAirTightCount(int GROUPID = 0, int type = 1)
        {
            DataRow data = await _envService.GetAirTightCountAsync(GROUPID, type);
            JObject job = JsonTransfer.dataRow2JObject(data);
            var result = ResponseOutput.Ok(job);
            return result;
        }

        /// <summary>
        /// 裸土覆盖统计[AI识别]
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1:今日，2：一周，3：一个月，4：一年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSoilCount(int GROUPID = 0, int type = 1)
        {
            DataRow data = await _envService.GetSoilCountAsync(GROUPID, type);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 制造项目管理
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSitePm(int GROUPID)
        {
            var data = await _envService.GetSitePmAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 环境监测 扬尘实时数据排行
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPmRtdRank(int GROUPID)
        {
            var data = await _envService.GetPmRtdRankAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// --环境监测 扬尘小时均值排行
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPmHourRank(int GROUPID)
        {
            var data = await _envService.GetPmHourRankAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// --环境监测 扬尘日均值排行
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPmDailyRank(int GROUPID)
        {
            var data = await _envService.GetPmDailyRankAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 环境监测 扬尘每月日均值查询
        /// </summary>
        /// <param name="billdate">指定年月</param>
        /// <param name="keyword">关键字</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页多少条</param>
        /// <param name="GROUPID">0:市看区数据 1:区看工地数据</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPmDailyList(string billdate, string keyword="", int pageindex=1, int pagesize=20, int GROUPID = 0)
        {
            var data = await _envService.GetPmDailyListAsync(GROUPID, billdate, keyword, pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// --环境监测 扬尘每日小时均值查询
        /// </summary>
        /// <param name="billdate">指定日期</param>
        /// <param name="keyword">关键字</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页多少条</param>
        /// <param name="GROUPID">0:市看区数据 1:区看工地数据</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPmHourList(string billdate, string keyword="", int pageindex=1, int pagesize=20, int GROUPID=0)
        {
            var data = await _envService.GetPmHourListAsync(GROUPID, billdate, keyword, pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// --告警数据查询
        /// </summary>
        /// <param name="GROUPID">指定日期</param>
        /// <param name="SITEID">关键字</param>
        /// <param name="startdate">关键字</param>
        /// <param name="enddate">关键字</param>
        /// <param name="type"> 1:扬尘，2：车冲，3：密闭，4：裸土</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页多少条</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetEnvWarnList(int GROUPID, int SITEID ,DateTime startdate, DateTime enddate,int type=0, int pageindex = 1, int pagesize = 20)
        {
            var data = await _envService.GetCtEnvWarnListAsync(GROUPID, SITEID, startdate, enddate, type, pageindex, pagesize);
            return ResponseOutput.Ok(data);
        }
    }
}
