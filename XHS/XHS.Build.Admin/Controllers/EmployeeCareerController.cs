using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Employee;
using XHS.Build.Services.File;
using static XHS.Build.Model.Models.FileEntity;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class EmployeeCareerController : ControllerBase
    {
        private readonly IEmployeeCareerService _employeeCareerService;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IFileService _fileService;
        private readonly IUser _user;
        public EmployeeCareerController(IEmployeeCareerService employeeCareerService, IHpFileDoc hpFileDoc, IUser user, IFileService fileService )
        {
            _employeeCareerService = employeeCareerService;
            _hpFileDoc = hpFileDoc;
            _user = user;
            _fileService = fileService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string id, string keyword = "", int page = 1, int size = 20)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseOutput.NotOk("请选择需要查看的信息");
            }
            var data = await _employeeCareerService.GetCareerPageList(id, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCEmployeeCareerEntity input)
        {
            if (input == null || string.IsNullOrEmpty(input.ID))
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var DbTypes = await _employeeCareerService.Query(a=>a.ID==input.ID && a.Papertype==input.Papertype);
            if (DbTypes.Any())
            {
                return ResponseOutput.NotOk("当前人员信息已存在相同证书信息");
            }

            var Entity = await _employeeCareerService.AddEntity(input);
            if (Entity != null)
            {
                if (!string.IsNullOrEmpty(input.Image))
                {
                    var fparam = new FileEntityParam();
                    fparam.GROUPID = 0;
                    fparam.SITEID = 0;
                    fparam.linkid = input.ECID.ToString();
                    fparam.filetype = FileEntity.FileType.Certificate;
                    var suc= await _hpFileDoc.UpdateUploadImgTmp(input.Image, fparam, _user.Name);
                    return suc? ResponseOutput.Ok():ResponseOutput.NotOk("信息保存成功，图片操作失败");
                }
                else
                {
                    return ResponseOutput.Ok();
                }
            }
            else
            {
                return ResponseOutput.NotOk("添加数据错误");
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int id)
        {
            if (id <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _employeeCareerService.DeleteById(id);
            if (isDel)
            {
                await _hpFileDoc.doDelete(0, 0, id.ToString(), "Certificate");
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("删除失败");
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCEmployeeCareerEntity input)
        {
            if (input == null || input.ECID <= 0 || string.IsNullOrEmpty(input.ID))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            bool suc = await _employeeCareerService.Update(input);
            if (suc)
            {
                if (!string.IsNullOrEmpty(input.Image))
                {
                    var fparam = new FileEntityParam();
                    fparam.GROUPID = 0;
                    fparam.SITEID = 0;
                    fparam.linkid = input.ECID.ToString();
                    fparam.filetype = FileEntity.FileType.Certificate;
                    await _hpFileDoc.doDelete(0, 0, input.ECID.ToString(), "Certificate");
                    suc = await _hpFileDoc.UpdateUploadImgTmp(input.Image, fparam, _user.Name);
                    return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("信息保存成功，图片操作失败");
                }
                else
                {
                    return ResponseOutput.Ok();
                }
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            var entity = await _employeeCareerService.QueryById(id);
            if (entity != null)
            {
                var Files = await _fileService.GetFileListByLindId(entity.ECID.ToString());
                if (Files.Any())
                {
                    entity.FileName = Files[0].filename;
                    entity.ImageTmpUrls = _fileService.GetImageUrl(Files[0], true);
                    entity.ImageUrls = new string[] { _fileService.GetImageUrl(Files[0]) };
                }
            }
            return ResponseOutput.Ok(entity);
        }
    }
}
