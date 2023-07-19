using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XHS.Build.Center.Attributes;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Center;
using XHS.Build.Services.File;


namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class FileController : ControllerBase
    {
        private readonly ICenterFile _centerFile;
        private readonly IConfiguration _configuration;
        private readonly IBaseRepository<BaseEntity> _baseServices;
        public FileController(ICenterFile centerFile, IConfiguration configuration, IBaseRepository<BaseEntity> baseServices)
        {
            _centerFile = centerFile;
            _configuration = configuration;
            _baseServices = baseServices;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="formFile">文件对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(IFormFile formFile)
        {
            int fileSize = _configuration.GetSection("Filesupload:filesize").Get<int>();
            if (formFile == null)
            {
                return ResponseOutput.NotOk("请选择文件。");
            }
            if (formFile.Length / (1024 * 1024.0) > fileSize)
            {
                return ResponseOutput.NotOk("文件限制为20M");
            }
            Object result = await _centerFile.upoadFile(formFile);
            if (result == null)
            {
                return ResponseOutput.NotOk("文件上传失败");
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileUrl">文件地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> HttpPost(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return ResponseOutput.NotOk("请填入下载地址。");
            }
            string fileType = _configuration.GetSection("Filesupload:filetype").Get<string>();
            Regex r = new Regex($@"^((https|http):\/\/)+[^\s]+(?i)('{fileType}')$");
            if (!r.Match(fileUrl).Success)
            {
                return ResponseOutput.NotOk("地址格式不正确");
            }
            object result = await _centerFile.downloadFile(fileUrl);
            if (result == null)
            {
                return ResponseOutput.NotOk("文件上传失败");
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId">文件UUID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> deleteFile(string fileId)
        {
            if (await _baseServices.Db.Queryable<CenterFileEntity>().Where(it => it.UUID.ToString() == fileId).AnyAsync())
            {
                return ResponseOutput.Result(await _centerFile.deleteFile(fileId));
            }
            else
                return ResponseOutput.NotOk("文件不存在");
        }
    }
}
