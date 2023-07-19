using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Unload;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 卸料平台
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UnLoadController : ControllerBase
    {
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IUnloadService _unloadService;
        public UnLoadController(IHpSystemSetting hpSystemSetting, IUnloadService unloadService)
        {
            _hpSystemSetting = hpSystemSetting;
            _unloadService = unloadService;
        }


        /// <summary>
        /// 获取卸料平台列表
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns>实时值数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListForSite(int SITEID)
        {
            return ResponseOutput.Ok(await _unloadService.getListForSite(SITEID));

        }


        /// <summary>
        /// 获取获取实时值
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <returns>实时值数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListRealUnloadData(string unloadid)
        { 
           return ResponseOutput.Ok(await _unloadService.GetListRealUnloadData(unloadid));
           
        }

        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="date">时间yyyy-MM-dd</param>
        /// <returns>数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListUnloadDataScheduleCurrent(string unloadid, DateTime date)
        {
            string startTime = string.Format("{0:yyyyMMdd}", date) + "000000";
            string endTime = string.Format("{0:yyyyMMdd}", date) + "235959";
            return ResponseOutput.Ok(await _unloadService.GetListUnloadDataScheduleCurrent(unloadid, startTime, endTime));

            //DateTime startTime = date.Date;
            //DateTime endTime = date.AddDays(1).Date;
            //List<UnloadInput> data = await _unloadService.GetListUnloadDataCurrent(unloadid, startTime, endTime);
            //JObject jso = new JObject();
            //JArray ja = new JArray();
            ////数据处理
            //foreach(UnloadInput ui in data)
            //{
            //    jso = new JObject();
            //    jso.Add("time", DateTimeExtensions.ToTimestamp(ui.createtime));
            //    jso.Add("bias", ui.bias);
            //    jso.Add("electric_quantity", ui.electric_quantity);
            //    jso.Add("weight", ui.weight);
            //    ja.Add(jso);
            //}
            //return ResponseOutput.Ok(ResponseOutput.Ok(ja));
        }

    }
}
