using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    /// 员工
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private readonly IEmployeeService _employeeService;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IFileService _fileService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeService"></param>
        /// <param name="mapper"></param>
        public EmployeeController(IEmployeeService employeeService, IMapper mapper, IUser user, IHpFileDoc hpFileDoc, IFileService fileService)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _user = user;
            _hpFileDoc = hpFileDoc;
            _fileService = fileService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="order"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20, string order = "", string ordertype = "")
        {
            var data = await _employeeService.GetEmployeePageList(keyword, page, size, order, ordertype);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 新增or修改（根据ID）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(EmployeeAddEditInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.ID) || string.IsNullOrEmpty(input.RealName) || string.IsNullOrEmpty(input.Sex) || string.IsNullOrEmpty(input.BirthDay) || string.IsNullOrEmpty(input.Address) || string.IsNullOrEmpty(input.Ethnic) || string.IsNullOrEmpty(input.Publisher))
            {
                return ResponseOutput.NotOk("请填写完整信息");
            }
            var entity = _mapper.Map<GCEmployeeEntity>(input);
            entity.OperateDate = DateTime.Now;
            entity.Operator = _user.Name;
            //entity.GroupID = _user.GroupId;
            entity.jsonall = JsonConvert.SerializeObject(input);
            var suc = await _employeeService.InsertOrUpdate(entity);
            if (suc > 0)
            {
                var fparam = new FileEntityParam();
                fparam.GROUPID = 0;
                fparam.SITEID = 0;
                fparam.linkid = input.ID;
                fparam.filetype = FileEntity.FileType.Employee;
                await _hpFileDoc.doDelete(0, 0, entity.ID, "employee");
                await _hpFileDoc.UpdateUploadImgTmp(input.Image, fparam, _user.Name);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("发生错误，请稍后再试");
            }
        }

        /// <summary>
        /// 单个信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Login]
        public async Task<IResponseOutput> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseOutput.NotOk("请选择需要查看的信息");
            }

            var entity = await _employeeService.QueryById(id);
            if (entity != null)
            {
                var Files = await _fileService.GetFileListByLindId(entity.ID);
                if (Files.Any())
                {
                    entity.FileImage = Files[0].filename;
                    entity.ImageTmpUrls =  _fileService.GetImageUrl(Files[0], true);
                    entity.ImageUrls = new string[] {  _fileService.GetImageUrl(Files[0]) };
                }
            }
            return ResponseOutput.Ok(entity);
        }
    }
}
