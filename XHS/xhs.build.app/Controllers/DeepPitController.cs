using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.DeepPit;
using XHS.Build.Common.Helps;
using XHS.Build.Model.ModelDtos;
using Newtonsoft.Json.Linq;

namespace XHS.Build.app.Controllers
{
    /// <summary>
    /// 深基坑
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DeepPitController : ControllerBase
    {
        private readonly IDeepPitService _deepPitService;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="deepPitService"></param>
        public DeepPitController(IUser user, IDeepPitService deepPitService)
        {
            _deepPitService = deepPitService;
        }


        /// <summary>
        /// 深基坑统计
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Total(int SITEID)
        {
            var result = await _deepPitService.spV2DpStats(SITEID);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 深基坑-下拉框数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Select(int SITEID)
        {
            var result = await _deepPitService.spV2DpSelect(SITEID);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 深基坑-监控信息
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="DPID">深基坑ID</param>
        /// <param name="date">开始时间</param>
        /// <param name="monitorType">监控类型</param>
        /// <param name="deviceid">设备ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Monitor(int SITEID,int DPID, DateTime date, string monitorType, string deviceid)
        {
            DateTime startTime = date.Date;
            DateTime endTime = date.AddDays(1).Date;
            var result = await _deepPitService.spV2DpMonitor(SITEID, DPID, startTime, endTime, monitorType, deviceid);

            JArray arr = new JArray();
            var list = result.ToDataList<DpMonitorDto>();
            list.GroupBy(ii => ii.collectionTime).ToList().ForEach(ii =>
            {
                JObject obj = new JObject();
                obj["date"] = ii.Key;
                obj["value"] = Newtonsoft.Json.JsonConvert.SerializeObject( list.Where(ww => ww.collectionTime == ii.Key));
                arr.Add(obj);
            });

            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 深基坑-实时数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="DPID">深基坑ID</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> LiveData(int SITEID,int DPID,int pageindex,int pagesize)
        {
            var result = await _deepPitService.spV2DpLive(SITEID, DPID, pageindex, pagesize);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 深基坑-历史数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="DPID">深基坑ID</param>
        /// <param name="date">日期</param>
        /// <param name="monitorType">监控类型</param>
        /// <param name="deviceid">设备ID</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> HisData(int SITEID, int DPID, DateTime date,  string monitorType, string deviceid, int pageindex=1, int pagesize=10)
        {
            DateTime startTime = date.Date;
            DateTime endTime = date.AddDays(1).Date;
            var result = await _deepPitService.spV2DpHis(SITEID, DPID, startTime, endTime, monitorType, deviceid, pageindex, pagesize);

            return ResponseOutput.Ok(result);
        }
    }
}
