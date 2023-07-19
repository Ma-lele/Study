using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.HighFormwork;
using XHS.Build.Common.Helps;
using XHS.Build.Model.ModelDtos;
using Newtonsoft.Json.Linq;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 高支模
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HighFormworkController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IHighFormworkService _highFormworkService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="highFormworkService"></param>
        /// <param name="user"></param>
        public HighFormworkController(IHighFormworkService highFormworkService, IUser user)
        {
            _user = user;
            _highFormworkService = highFormworkService;
        }


        /// <summary>
        /// 高支模-左上角统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T037/[action]")]
        public async Task<IResponseOutput> Total()
        {
            var result = await _highFormworkService.spV2HfwStats(_user.SiteId.ToInt());
            JObject job = JsonTransfer.dataRow2JObject(result.Rows[0]);
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 高支模-下拉框数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T037/[action]")]
        public async Task<IResponseOutput> Select()
        {
            var result = await _highFormworkService.spV2HfwSelect(_user.SiteId.ToInt());
            //var data = result.ToDataList<HfwSelectData>();

            //List<HfwSelectDto> select = new List<HfwSelectDto>();
            //foreach (var i in data.GroupBy(ii => ii.HFWID).ToList())
            //{
            //    HfwSelectDto dto = new HfwSelectDto
            //    {
            //        HFWID = i.Key,
            //        hfwname = data.Find(ii => ii.HFWID == i.Key)?.hfwname,
            //        areas = new List<HfwSelectArea>()
            //    };

            //    foreach (var j in i.GroupBy(ii => ii.HFWAID))
            //    {
            //        HfwSelectArea area = new HfwSelectArea
            //        {
            //            HFWAID = j.Key,
            //            hfwaname = data.Find(ii => ii.HFWAID == j.Key)?.hfwaname,
            //            spotcode = j.Select(ii => ii.spotcode).ToList()
            //        };

            //        dto.areas.Add(area);
            //    }
            //    select.Add(dto);
            //}

            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 高支模-监控信息
        /// </summary>
        /// <param name="hfwcode">设备编号</param>
        /// <param name="HFWAID">区域ID</param>
        /// <param name="spotcode">点位编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T037/[action]")]
        public async Task<IResponseOutput> Monitor(string hfwcode, int HFWAID, string spotcode)
        {
            var result = await _highFormworkService.spV2HfwMonitor(_user.SiteId.ToInt(), hfwcode, HFWAID, spotcode);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 高支模-监控信息-历史数据
        /// </summary>
        /// <param name="hfwcode">设备编号</param>
        /// <param name="HFWAID">区域ID</param>
        /// <param name="spotcode">点位编号</param>
        /// <param name="startDate">查找开始日期</param>
        /// <param name="endDate">查找结束日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T037/[action]")]
        public async Task<IResponseOutput> MonitorHistory(string hfwcode, int HFWAID, string spotcode,
            DateTime startDate, DateTime endDate, int pageIndex = 0, int pageSize = 20)
        {
            var result = await _highFormworkService.spV2HfwMonitorHistory(_user.SiteId.ToInt(), hfwcode, HFWAID,
                spotcode, startDate, endDate, pageIndex, pageSize);
            return ResponseOutput.Ok(result);
        }
    }
}
