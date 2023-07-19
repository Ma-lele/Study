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
using XHS.Build.Services.Inspection;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IFileService _fileService;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IInspectionService _inspectionService;
        public FileController(IFileService FileService, IUser user, IHpFileDoc hpFileDoc, IInspectionService inspectionService)
        {
            _user = user;
            _fileService = FileService;
            _hpFileDoc = hpFileDoc;
            _inspectionService = inspectionService;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="formFiles">文件对象。一次只允许上传1个文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post([FromForm] List<IFormFile> formFiles)
        {
            if (formFiles.Count != 1)
                return ResponseOutput.NotOk("请每次上传一个文件");
            var formFile = formFiles[0];

            string FILEID = await _hpFileDoc.doRegist(formFile, new FileEntity.FileEntityParam(), _user.Name);
            return ResponseOutput.Ok(FILEID);
        }

        /// <summary>
        /// 上传图片并压缩
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Compress([FromForm] IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return ResponseOutput.NotOk("请选择需要上传的图片");
            }
            return ResponseOutput.Ok(await _hpFileDoc.UploadImgWithTmp(formFile, _user.Name));
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FILEID">文件ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Delete(string FILEID)
        {
            var files = await _fileService.Query(x => x.FILEID == FILEID);
            if (files.Count > 0)
            {
                var file = files.First();
                file.bdel = 1;
                return ResponseOutput.Result(await _fileService.Update(file, new List<string> { "bdel" }, null, string.Format(" FILEID='{0}' ", FILEID)));
            }
            else
                return ResponseOutput.NotOk("文件不存在");
        }
    }
}
