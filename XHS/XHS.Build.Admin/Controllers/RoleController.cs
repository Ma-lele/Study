using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Role;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 角色
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private readonly IUserRoleService _userRoleService;
        public RoleController(IRoleService roleService, IMapper mapper, IUser user, IUserRoleService userRoleService)
        {
            _roleService = roleService;
            _mapper = mapper;
            _user = user;
            _userRoleService = userRoleService;
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20, string order = "", string ordertype = "")
        {
            Expression<Func<SysRoleEntity, GcGroupEntity, bool>> whereExpression = (r, g) => (r.IsDeleted == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = whereExpression.Compose((r, g) => r.Name.Contains(keyword)
                , Expression.And);
            }
            if (groupid > 0)
            {
                whereExpression = whereExpression.Compose((r, g) => r.GROUPID == groupid || r.GROUPID == 0
                , Expression.And);
            }
            else if (_user.GroupId > 0)
            {
                whereExpression = whereExpression.Compose((r, g) => r.GROUPID == _user.GroupId
                , Expression.And);
            }
            var dbList = await _roleService.QueryRolePage(whereExpression, page, size, string.IsNullOrEmpty(order) ? " id desc" : order + " " + ordertype);
            var outputList = _mapper.Map<List<RoleListOutput>>(dbList.data);

            var data = new PageOutput<RoleListOutput>()
            {
                data = outputList,
                dataCount = dbList.dataCount
            };

            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Login]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _roleService.GetGroupCount());
        }

        /// <summary>
        /// 全部账号可见角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> GetAll()
        {
            Expression<Func<SysRoleEntity, bool>> whereExpression = a => (a.IsDeleted == false && a.Enabled == true);
            if (_user.GroupId > 0)//分组下的人员,获取非系统角色,groupid是0或者用户分组下的角色
            {
                whereExpression = whereExpression.And(b => b.issys == false && (b.GROUPID == 0 || b.GROUPID == _user.GroupId));
            }
            else
            {
                if (!_user.IsAdmin)//非系统管理员,即普通管理员可见非系统管理员意外的全部角色
                {
                    whereExpression = whereExpression.And(b => b.issys == false);
                }
                else
                {

                }
            }

            var dbList = await _roleService.Query(whereExpression);

            return ResponseOutput.Ok(dbList);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(RoleAddInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写角色信息");
            }

            var Role = await _roleService.Query(a => a.Name == input.Name && a.GROUPID == input.GROUPID);
            if (Role.Any())
            {
                return ResponseOutput.NotOk("该角色名称已存在");
            }

            var entity = _mapper.Map<SysRoleEntity>(input);
            entity.CreateTime = DateTime.Now;
            var rows = await _roleService.Add(entity);
            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseOutput.NotOk("请选择需要删除的角色");
            }
            var entity = await _roleService.QueryById(id);
            if (entity == null || entity.issys)
            {
                return ResponseOutput.NotOk("未找到可删除的角色信息或角色不可删除");
            }
            entity.IsDeleted = true;
            var suc = await _roleService.Update(entity);
            if (suc)
            {
                var ids = await _userRoleService.Query(r => r.Roleid == id);
                if (ids.Any())
                {
                    await _userRoleService.DeleteByIds(ids.Select(i => i.Id).ToArray());
                }
            }
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(RoleUpdateInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Id) || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写需要修改的角色信息");
            }
            var ExistRole = await _roleService.Query(a => a.Name == input.Name && a.GROUPID == input.GROUPID && a.Id != input.Id);
            if (ExistRole.Any())
            {
                return ResponseOutput.NotOk("该角色名称已存在");
            }
            var Role = await _roleService.QueryById(input.Id);
            var RU = await _userRoleService.Query(a => a.Roleid == input.Id);
            if (RU.Any() && Role.GROUPID != input.GROUPID)
            {
                return ResponseOutput.NotOk("当前角色已经分配，不可修改分组信息");
            }

            var entity = _mapper.Map<SysRoleEntity>(input);
            var suc = await _roleService.Update(entity, new List<string>() { "Name", "Enabled", "GROUPID" });
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("更新角色信息失败");
        }

        /// <summary>
        /// 获取角色详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            return ResponseOutput.Ok(await _roleService.QueryById(id));
        }
    }
}
