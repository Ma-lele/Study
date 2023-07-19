using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.Services.SpecialEqpDoc;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 特种设备
    /// </summary>
    [ApiController]
    [Route("api/[controller]/")]
    [Authorize]
    public class SpecialController : ControllerBase
    {
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        private readonly ISpecialEqpService _specialEqpService;
        private readonly ISpecialEqpAuthHisService _specialEqpAuthHisService;
        private readonly ISpecialEqpRecordService _specialEqpRecordService;
        private readonly ISpecialEqpDocService _specialEqpDocService;
        private readonly IMongoDBRepository<SpecialEqpData> _specialMongoService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="specialEqpService"></param>
        /// <param name="specialEqpDocService"></param>
        /// <param name="specialEqpRecordService"></param>
        /// <param name="specialEqpAuthHisService"></param>
        public SpecialController(IUser user, ISpecialEqpService specialEqpService, 
            ISpecialEqpDocService specialEqpDocService, 
            ISpecialEqpRecordService specialEqpRecordService,
            ISpecialEqpAuthHisService specialEqpAuthHisService,
            IMongoDBRepository<SpecialEqpData> specialMongoService)
        {
            _specialEqpService = specialEqpService;
            _specialEqpAuthHisService = specialEqpAuthHisService;
            _specialEqpRecordService = specialEqpRecordService;
            _specialEqpDocService = specialEqpDocService;
            _specialMongoService = specialMongoService;
            _user = user;
        }

        /// <summary>
        /// 智慧工地2.0项目端-特种设备-人脸识别数据
        /// </summary>
        /// <param name="secode">设备号</param>
        /// <param name="billdate">月份筛选</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetAuthHis(string secode, DateTime billdate)
        {
            var data = await _specialEqpAuthHisService.GetListAsync(Convert.ToInt32(_user.SiteId), secode, billdate);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 智慧工地2.0项目端-特种设备-历史数据
        /// </summary>
        /// <param name="secode">设备号</param>
        /// <param name="billdate">日期</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetDataHis(string secode, DateTime billdate, int pageIndex = 1, int pageSize = 20)
        {
            DateTime startTime = billdate.Date;
            DateTime endTime = billdate.AddDays(1).Date;
            FilterDefinition<SpecialEqpData> filter = Builders<SpecialEqpData>.Filter.Empty;
            filter = filter & Builders<SpecialEqpData>.Filter.Eq(a => a.SeCode, secode);
            filter = filter & Builders<SpecialEqpData>.Filter.Gte(a => a.CreateTime, startTime);
            filter = filter & Builders<SpecialEqpData>.Filter.Lt(a => a.CreateTime, endTime);
            SortDefinition<SpecialEqpData> sort = Builders<SpecialEqpData>.Sort.Descending(a => a.CreateTime);
            List<SpecialEqpData> retString = (List<SpecialEqpData>)_specialMongoService.FindByFilterWithPage(filter, pageIndex, pageSize, sort);

            long total = _specialMongoService.FindByFilterTotalcount(filter, sort);
            JObject jso = new JObject();
            jso.Add("total", total);
            jso.Add("rows", JArray.FromObject(retString));
            return ResponseOutput.Ok(jso);
        }


        /// <summary>
        /// 智慧工地2.0项目端-特种设备-特种设备台数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetCount(int setype)
        {
            DataRow data = await _specialEqpService.GetCountAsync(Convert.ToInt32(_user.SiteId), setype);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }

        /// <summary>
        /// 智慧工地2.0项目端-特种设备-操作员列表
        /// </summary>
        /// <param name="secode">设备号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetEmpList(string secode)
        {
            var data = await _specialEqpService.GetEmpListAsync(Convert.ToInt32(_user.SiteId), secode);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 智慧工地2.0项目端-特种设备-特种设备列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetList(int setype)
        {
            var data = await _specialEqpService.GetListAsync(Convert.ToInt32(_user.SiteId), setype);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 智慧工地2.0项目端-特种设备-特种设备备案数据
        /// </summary>
        /// <param name="secode">设备号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetRecord(string secode)
        {
            var data = await _specialEqpRecordService.GetOneAsync(Convert.ToInt32(_user.SiteId), secode);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 智慧工地2.0项目端-特种设备-特种设备最近一条实时数据
        /// </summary>
        /// <param name="secode">设备号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetRtd(string secode)
        {
            DataRow data = await _specialEqpService.GetRtdAsync(Convert.ToInt32(_user.SiteId), secode);
            JObject job = JsonTransfer.dataRow2JObject(data);
            return ResponseOutput.Ok(job);
        }


        /// <summary>
        /// 智慧工地2.0项目端-特种设备-特种设备最近60条实时数据用于回放
        /// </summary>
        /// <param name="secode">设备号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        public async Task<IResponseOutput> GetRtdList(string secode)
        {
            var data = await _specialEqpService.GetRtdListAsync(Convert.ToInt32(_user.SiteId), secode);
            return ResponseOutput.Ok(data);
        }

    }
}
