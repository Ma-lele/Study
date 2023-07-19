using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.File;
using XHS.Build.Services.Security;
using static XHS.Build.Model.Models.FileEntity;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 巡检
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly ISecurityHisService _securityHisService;
        private readonly IFileService _fileService;
        private readonly IUser _user;
        private readonly IHpFileDoc _hpFileDoc;
        public SecurityController(ISecurityService securityService, IUser user, ISecurityHisService securityHisService, IFileService fileService, IHpFileDoc hpFileDoc)
        {
            _securityService = securityService;
            _securityHisService = securityHisService;
            _fileService = fileService;
            _user = user;
            _hpFileDoc = hpFileDoc;
        }

        /// <summary>
        /// 获取巡检站点列表
        /// </summary>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="page">当前页 1开始</param>
        /// <param name="size">默认条数 20</param>
        /// <param name="order">排序字段</param>
        /// <param name="ordertype">排序方式(asc  desc)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> SitePageList(string keyword = "", int page = 1, int size = 20, string order = "", string ordertype = "")
        {
            if (page < 1)
            {
                page = 1;
            }
            if (size < 1)
            {
                size = 20;
            }
            return ResponseOutput.Ok(await _securityService.GetSecuritySitePageList(keyword, page, size, order, ordertype));
        }

        /// <summary>
        /// 今日 站点下巡检点
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="date">时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> TodaySecurityListCount(int siteid, DateTime date)
        {
            return ResponseOutput.Ok(await _securityService.GetTodaySecurityListCount(siteid, date));
        }

        /// <summary>
        /// 巡检记录(带图片,分页) 
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="securityid">巡检点（全部传0）</param>
        /// <param name="date">时间</param>
        /// <param name="page">当前页 1开始</param>
        /// <param name="size">默认条数 10</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> HisPageList(int siteid, int securityid, DateTime date, int page = 1, int size = 10)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (size < 1)
            {
                size = 10;
            }
            return ResponseOutput.Ok(await _securityHisService.GetHisPageList(siteid, securityid, date, page, size));
        }


        /// <summary>
        /// 巡检记录(不带图片，不分页) 
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="securityid">巡检点（全部传0）</param>
        /// <param name="date">时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> HisList(int siteid, int securityid, DateTime date)
        {
            return ResponseOutput.Ok(await _securityHisService.GetHisALlList(siteid, securityid, date));
        }

        /// <summary>
        /// 获取巡检点详细
        /// </summary>
        /// <param name="securityid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Security(int securityid)
        {
            var entity = await _securityService.GetSiteSecurityOutput(securityid);
            return entity == null ? ResponseOutput.NotOk("没有权限查看信息") : ResponseOutput.Ok(entity);
        }

        /// <summary>
        /// 获取站点下巡检点列表
        /// </summary>
        /// <param name="siteid">站点id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> SecurityList(int siteid)
        {
            return ResponseOutput.Ok(await _securityService.Query(s => s.SITEID == siteid && s.bdel == 0));
        }

        /// <summary>
        /// 新增巡检记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> PostHis(GCSecureHisEntity entity)
        {
            if (entity == null || entity.SITEID < 1 || entity.SECURITYID < 0)
            {
                return ResponseOutput.NotOk("请选择站点或巡检点");
            }
            var dbSecure = await _securityService.Query(s => s.SECURITYID == entity.SECURITYID && s.bdel == 0);
            if (!dbSecure.Any())
            {
                return ResponseOutput.NotOk("请选择正确的巡检点");
            }

            entity.createddate = DateTime.Now;
            entity.GROUPID = _user.GroupId;
            entity.USERID = _user.Id;
            entity.UserName = _user.Name;
            var db = await _securityHisService.AddEntity(entity);
            if (db != null && db.SCHISID > 0)
            {
                if (entity.Files != null && entity.Files.Count > 0)
                {
                    foreach (var file in entity.Files)
                    {
                        var param2 = new FileEntityParam();
                        param2.GROUPID = _user.GroupId;
                        param2.SITEID = entity.SITEID;
                        param2.linkid = db.SCHISID.ToString();
                        param2.filetype = FileEntity.FileType.Security;
                        await _hpFileDoc.doUpdate(file.FILEID, param2, _user.Name);
                    }
                }
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("新增巡检记录失败");
            }
        }
        
        /// <summary>
        /// 更新巡检点记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Update(GCSecurityEntity entity)
        {
            if (entity == null || entity.SECURITYID < 0)
            {
                return ResponseOutput.NotOk("请选择巡检点");
            }
            var dbSecure = await _securityService.Query(s => s.SECURITYID == entity.SECURITYID && s.bdel == 0);
            if (!dbSecure.Any())
            {
                return ResponseOutput.NotOk("巡检点已删除");
            }

            entity.GROUPID = _user.GroupId;
            entity.@operator = _user.Name;
            int result = await _securityService.Update(entity);
            if (result > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("更新巡检点失败");
            }
        }

        /// <summary>
        /// 新增巡检点记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Insert(GCSecurityEntity entity)
        {
            if (entity == null || entity.SITEID < 1)
            {
                return ResponseOutput.NotOk("请选择站点");
            }

            entity.GROUPID = _user.GroupId;
            entity.@operator = _user.Name;
            int result = await _securityService.Insert(entity);
            if (result > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("新增巡检点失败");
            }
        }

        /// <summary>
        /// 删除巡检点记录
        /// </summary>
        /// <param name="securityid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Delete(int securityid)
        {
            if (securityid < 1)
            {
                return ResponseOutput.NotOk("请选择巡检点");
            }

            int result = await _securityService.Delete(securityid, _user.Name);
            if (result > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("新增巡检点失败");
            }
        }

        /// <summary>
        /// 时间 每个月的数量统计
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="dt">例如：2020-01-01</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> MonthCount(int siteid, DateTime dt)
        {
            return ResponseOutput.Ok(await _securityHisService.GetMonthCount(siteid, dt));
        }
    }
}
