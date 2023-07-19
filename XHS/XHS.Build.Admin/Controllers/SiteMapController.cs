using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.File;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SiteMapController : ControllerBase
    {
        private readonly ISiteMapService _siteMapService;
        private readonly IFileService _fileService;
        private readonly IUser _user;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IHpSystemSetting _hpSystemSetting;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteMapService"></param>
        /// <param name="fileService"></param>
        /// <param name="hpSystemSetting"></param>
        /// <param name="user"></param>
        /// <param name="hpFileDoc"></param>
        public SiteMapController(ISiteMapService siteMapService, IFileService fileService, IHpSystemSetting hpSystemSetting, IUser user, IHpFileDoc hpFileDoc)
        {
            _siteMapService = siteMapService;
            _fileService = fileService;
            _hpSystemSetting = hpSystemSetting;
            _hpFileDoc = hpFileDoc;
            _user = user;
        }

        /// <summary>
        /// 增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCSiteMapEntity entity)
        {
            if (entity == null)
            {
                return ResponseOutput.NotOk("请输入信息");
            }
            entity.@operator = _user.Name;
            return ResponseOutput.Ok(await _siteMapService.Add(entity));
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Update(GCSiteMapEntity entity)
        {
            if (entity == null || entity.SMID <= 0)
            {
                return ResponseOutput.NotOk("请输入信息");
            }
            entity.@operator = _user.Name;
            return ResponseOutput.Ok(await _siteMapService.Update(entity));
        }

        /// <summary>
        /// 详细
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请选择需要查看的数据");
            }
            var Entitys = await _siteMapService.Query(a => a.SITEID == SITEID);
            var files = await _fileService.Query(a => a.SITEID == SITEID && a.filetype == "SiteMap" && a.bdel == 0);

            if (Entitys.Any())
            {
                if (files.Any() && files[0].filename.IndexOf(".") > 0)
                {
                    string suffix = "";
                    suffix = files[0].filename.Substring(files[0].filename.IndexOf("."), files[0].filename.Length - files[0].filename.IndexOf("."));
                    //平面图
                    Entitys[0].planeurl = $"http://{_hpSystemSetting.getSettingValue("S034")}/resourse/{Entitys[0].GROUPID}/SiteMap/{SITEID}/{files[0].FILEID}{suffix}";//await _hpFileDoc.GetSiteMapPlane(Entitys[0].GROUPID, Entitys[0].SITEID, true);

                }
                return ResponseOutput.Ok(Entitys[0]);
            }
            return ResponseOutput.Ok();
        }

        /// <summary>
        /// 上传覆盖在地图上的工地平面图
        /// </summary>
        /// <param name="formFile">上传的文件</param>
        /// <param name="GROUPID"></param>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SavePlane([FromForm] List<IFormFile> formFile, [FromForm] int GROUPID, [FromForm] int SITEID)
        {
            if (formFile == null || formFile.Count != 1 || formFile[0].Length == 0)
            {
                return ResponseOutput.NotOk("请选择需要上传的图片");
            }

            string FILEID = await _hpFileDoc.doRegist(formFile[0], new FileEntity.FileEntityParam(), _user.Name);

            var suc = await _hpFileDoc.doUpdate(FILEID,
                     new FileEntity.FileEntityParam
                     {
                         filetype = FileEntity.FileType.SiteMap,
                         linkid = "",
                         GROUPID = GROUPID,
                         SITEID = SITEID
                     },
                     _user.Name
                     );
            return ResponseOutput.Ok(suc);
        }
        /// <summary>
        /// 删除工地平面图
        /// </summary>
        /// <param name="FILEID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> DeletePlane(string FILEID)
        {
            if (String.IsNullOrEmpty(FILEID))
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }
            //return (await _hpFileDoc.DeleteSiteMapPlane(GROUPID, SITEID)) ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
            return (await _fileService.doDelete(FILEID)) > 0 ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }
    }
}
