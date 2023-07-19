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
using XHS.Build.Services.Board;

namespace XHS.Build.SmartCity.Controllers
{
    /// <summary>
    /// 数据看板
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardService"></param>
        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        /// <summary>
        /// 区分组列表，以及每个区的工地数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetGroupList(int GROUPID =-1)
        {
            var data = await _boardService.GetGroupListAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 左上角项目概况
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetGeneral(int GROUPID=0)
        {
            var data = await _boardService.GetGeneralAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 右上角工地数统计
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetDevSiteCount(int GROUPID=0)
        {
            DataRow data = await _boardService.GetDevSiteCountAsync(GROUPID);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 项目概况点击以后的项目列表弹框
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="pageindex">当前第几页</param>
        /// <param name="pagesize">每页几条</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetGeneralList(int GROUPID=0,int pageindex=1,int pagesize=20,string keyword="")
        {
            var data = await _boardService.GetGeneralListAsync(GROUPID,pageindex,pagesize,keyword);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 中间地图 
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetMap(int GROUPID=0)
        {
            var data = await _boardService.GetMapAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 右上角监测项目覆盖率点击后的弹框
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1：扬尘 2：视频 3：冲洗 4：特设 5：临边 6：考勤</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetDevSiteList(int type=0,int GROUPID= 0)
        {
            var data = await _boardService.GetDevSiteListAsync(GROUPID, type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 一周工地监测动态
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWeekWarn(int GROUPID=0)
        {
            var data = await _boardService.GetWeekWarnAsync(GROUPID);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 项目告警情况
        /// </summary>
        /// <param name="GROUPID">0:市 非0:区编号</param>
        /// <param name="type">1:今日，2：一周，3：一个月，4：一年</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWarn(int GROUPID=0,int type=1)
        {
            var data = await _boardService.GetWarnAsync(GROUPID, type);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 头部天气预报
        /// </summary>
        /// <param name="city">市</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTopWeather(string city)
        {
            DataRow data = await _boardService.GetTopWeatherAsync(city);
            JObject job = JObject.Parse(data.ItemArray[0].ToString());
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 当前视频在线率
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOnlineRank(int GROUPID = 0)
        {
            var data = await _boardService.GetOnlineRank(GROUPID);
            return ResponseOutput.Ok(data);
        }
    }
}
