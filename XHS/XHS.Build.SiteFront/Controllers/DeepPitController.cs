using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.DeepPit;
using XHS.Build.Common.Helps;
using XHS.Build.Model.ModelDtos;
using Newtonsoft.Json.Linq;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 深基坑
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeepPitController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IDeepPitService _deepPitService;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="deepPitService"></param>
        public DeepPitController(IUser user, IDeepPitService deepPitService)
        {
            _user = user;
            _deepPitService = deepPitService;
        }


        /// <summary>
        /// 深基坑-左上角统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T038/[action]")]
        public async Task<IResponseOutput> Total()
        {
            var result = await _deepPitService.spV2DpStats(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 深基坑-下拉框数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T038/[action]")]
        public async Task<IResponseOutput> Select()
        {
            var result = await _deepPitService.spV2DpSelect(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 深基坑-监控信息
        /// </summary>
        /// <param name="DPID">深基坑ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="monitorType">监控类型</param>
        /// <param name="deviceid">设备ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T038/[action]")]
        public async Task<IResponseOutput> Monitor(int DPID, DateTime startTime, DateTime endTime, string monitorType, string deviceid)
        {
            var result = await _deepPitService.spV2DpMonitor(_user.SiteId.ToInt(), DPID, startTime, endTime, monitorType, deviceid);

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
        /// <param name="DPID">深基坑ID</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T038/[action]")]
        public async Task<IResponseOutput> LiveData(int DPID,int pageindex,int pagesize)
        {
            var result = await _deepPitService.spV2DpLive(_user.SiteId.ToInt(), DPID, pageindex, pagesize);
            return ResponseOutput.Ok(result);
        }
    }
}
