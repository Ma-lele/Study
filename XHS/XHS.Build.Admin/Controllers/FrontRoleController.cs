using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.FrontRole;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class FrontRoleController : ControllerBase
    {
        private readonly IFrontRoleService _frontRoleService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        public FrontRoleController(IFrontRoleService frontRoleService, IUser user)
        {
            _frontRoleService = frontRoleService;
            _user = user;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页件数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 10)
        {
            return ResponseOutput.Ok(await _frontRoleService.GetPageList(keyword, page, size));
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAll()
        {
            return ResponseOutput.Ok(await _frontRoleService.Query());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCRoleEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbExist = await _frontRoleService.Query(f => f.rolename == input.rolename);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该角色已存在");
            }

            var rows = await _frontRoleService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
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

            bool isDel = await _frontRoleService.DeleteById(id);
            if (isDel)
            {
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
        public async Task<IResponseOutput> Put(GCRoleEntity input)
        {
            if (input == null || input.ROLEID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbExist = await _frontRoleService.Query(f => f.rolename == input.rolename && f.ROLEID != input.ROLEID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该角色已存在");
            }

            bool suc = await _frontRoleService.Update(input);
            if (suc)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

    }
}
