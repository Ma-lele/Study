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
using XHS.Build.Services.CtRole;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class CtRoleController : ControllerBase
    {
        private readonly ICtRoleService _ctRoleService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        public CtRoleController(ICtRoleService ctRoleService, IUser user)
        {
            _ctRoleService = ctRoleService;
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
            return ResponseOutput.Ok(await _ctRoleService.GetPageList(keyword, page, size));
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAll()
        {
            return ResponseOutput.Ok(await _ctRoleService.Query());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(CTRoleEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbExist = await _ctRoleService.Query(f => f.rolename == input.rolename);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该角色已存在");
            }

            var rows = await _ctRoleService.CtRoleAdd(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _ctRoleService.DeleteById(id);
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
        public async Task<IResponseOutput> Put(CTRoleEntity input)
        {
            if (input == null || string.IsNullOrEmpty(input.ROLEID))
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbExist = await _ctRoleService.Query(f => f.rolename == input.rolename && f.ROLEID != input.ROLEID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该角色已存在");
            }

            bool suc = await _ctRoleService.Update(input);
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
