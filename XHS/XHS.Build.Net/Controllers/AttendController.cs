using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.EmployeeSite;
using XHS.Build.Services.RealName;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 实名考勤
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class AttendController : ControllerBase
    {
        private readonly IRAttendService _rAttendService;
        private readonly IEmployeeSiteService _employeeSiteService;
        private readonly IUserKey _userKey;
        public AttendController(IRAttendService rAttendService, IEmployeeSiteService employeeSiteService, IUserKey userKey)
        {
            _rAttendService = rAttendService;
            _employeeSiteService = employeeSiteService;
            _userKey = userKey;
        }

        /// <summary>
        /// 同步人员基本信息
        /// </summary>
        [HttpPost]
        public async Task<IResponseOutput> Employee(EmployeeInput entity)
        {
            return await _rAttendService.EmployeeRegist(entity);
        }

        /// <summary>
        /// 同步进出场实时记录
        /// </summary>
        [HttpPost]
        public async Task<IResponseOutput> EmployeePass(EmployeePassHisInsertInput entity)
        {
            return await _rAttendService.EmployeePassHisInsert(entity);
        }

        /// <summary>
        /// 用户站点信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> EmployeeSite(GCEmployeeSiteEntity entity)
        {
            if(entity==null || string.IsNullOrEmpty(entity.ID) || string.IsNullOrEmpty(entity.attendprojid))
            {
                return ResponseOutput.NotOk("请填写必填信息");
            }
            entity.GROUPID = _userKey.GroupId;
            entity.updatedate = DateTime.Now;
            var suc = _employeeSiteService.doSiteRegist(entity);

            //var suc = _employeeSiteService.doSiteRegist(new SugarParameter("@ID", entity.ID),
            //                new SugarParameter("@attendprojid",entity.attendprojid),
            //                new SugarParameter("@socialcreditcode",entity.socialcreditcode),
            //                new SugarParameter("@shiftname", entity.shiftname),
            //                new SugarParameter("@workertype",entity.workertype),
            //                new SugarParameter("@position",entity.position),
            //                new SugarParameter("@jobtype", entity.jobtype),
            //                new SugarParameter("@jobname",entity.jobname),
            //                new SugarParameter("@startdate",entity.startdate),
            //                new SugarParameter("@enddate", entity.enddate)); //await _employeeSiteService.Add(entity);

            if (suc == -27)
            {
                ResponseOutput.NotOk("未找到相关项目",suc);
            }
            return suc > 0 ? ResponseOutput.Ok(1):ResponseOutput.NotOk("请求发生错误");
        }
    }
}
