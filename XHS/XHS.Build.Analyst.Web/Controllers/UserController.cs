using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Analyst.Web.Attributes;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Common.Helps;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Role;
using XHS.Build.Services.User;

namespace XHS.Build.Analyst.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ISysUserService _sysUserService;
        private readonly IMapper _mapper;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(ISysUserService sysUserService, IUser user, IMapper mapper, IUserRoleService userRoleService, IRoleService roleService, IUnitOfWork unitOfWork)
        {
            _sysUserService = sysUserService;
            _user = user;
            _mapper = mapper;
            _userRoleService = userRoleService;
            _roleService = roleService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            Expression<Func<SysUserEntity, bool>> whereExpression = a => true;
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = whereExpression.And(b => b.UserName.Contains(keyword) || b.LoginName.Contains(keyword) || b.Mobile.Contains(keyword));
            }
            if (groupid > 0)
            {
                whereExpression = whereExpression.And(b => b.GroupId == groupid);
            }
            else if (_user.GroupId > 0)
            {
                whereExpression = whereExpression.And(b => b.GroupId == _user.GroupId);
            }
            var dbList = await _sysUserService.QueryPage(whereExpression, page, size, " id desc");

            var output = _mapper.Map<List<SysUserListOutput>>(dbList.data);
            // 这里可以封装到多表查询，此处简单处理
            var allUserRoles = await _userRoleService.Query();
            var allRoles = await _roleService.Query(d => d.IsDeleted == false);

            foreach (var item in output)
            {
                var currentUserRoles = allUserRoles.Where(d => d.Userid == item.Id).Select(d => d.Roleid).ToList();
                item.RIDs = currentUserRoles;
                item.RoleNames = allRoles.Where(d => currentUserRoles.Contains(d.Id)).Select(d => d.Name).ToList();
            }

            var data = new PageOutput<SysUserListOutput>()
            {
                data = output,
                dataCount = dbList.dataCount
            };
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 用户列表分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Login]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _sysUserService.GetGroupCount());
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(UserAddInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.LoginName))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var exist = await _sysUserService.Query(a => a.LoginName == input.LoginName);
            if (exist != null && exist.Count > 0)
            {
                return ResponseOutput.NotOk("登录名已存在");
            }
            try
            {
                _unitOfWork.BeginTran();
                var entity = _mapper.Map<SysUserEntity>(input);
                entity.CreateTime = DateTime.Now;
                entity.OperateDate = DateTime.Now;
                if (input.RIDs != null && input.RIDs.Any())
                {
                    var userRolsAdd = new List<UserRoleEntity>();
                    input.RIDs.ForEach(rid =>
                    {
                        userRolsAdd.Add(new UserRoleEntity() { Roleid = rid, Userid = entity.Id });
                    });

                    await _userRoleService.Add(userRolsAdd);
                }
                entity.Password = UEncrypter.SHA256(entity.Password);
                var rows = await _sysUserService.Add(entity);
                _unitOfWork.CommitTran();
                return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
            }
            catch
            {
                _unitOfWork.RollbackTran();
                return ResponseOutput.NotOk("添加数据错误");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseOutput.NotOk("请选择需要删除的信息");
            }
            if (id == "1")
            {
                return ResponseOutput.NotOk("系统用户不可删除");
            }
            var entity = await _sysUserService.QueryById(id);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到可删除的信息");
            }
            var suc = await _sysUserService.DeleteById(id);
            if (suc)
            {
                var ids = await _userRoleService.Query(r => r.Userid == id);
                if (ids.Any())
                {
                    await _userRoleService.DeleteByIds(ids.Select(i => i.Id).ToArray());
                }
            }
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(UserAddInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Id) || string.IsNullOrEmpty(input.LoginName))
            {
                return ResponseOutput.NotOk("请填写需要修改的信息");
            }
            var exist = await _sysUserService.Query(a => a.Id != input.Id && a.LoginName == input.LoginName);
            if (exist != null && exist.Count > 0)
            {
                return ResponseOutput.NotOk("登录名已存在");
            }
            try
            {
                _unitOfWork.BeginTran();
                var dbEntity = await _sysUserService.QueryById(input.Id);
                var entity = _mapper.Map(input, dbEntity);
                entity.OperateDate = DateTime.Now;
                if (input.RIDs != null && input.RIDs.Any())
                {
                    var usreroles = (await _userRoleService.Query(d => d.Userid == entity.Id)).Select(d => d.Id).ToArray();
                    if (usreroles.Count() > 0)
                    {
                        var isAllDeleted = await _userRoleService.DeleteByIds(usreroles);
                    }

                    var userRolsAdd = new List<UserRoleEntity>();
                    input.RIDs.ForEach(rid =>
                    {
                        userRolsAdd.Add(new UserRoleEntity() { Roleid = rid, Userid = entity.Id });
                    });

                    await _userRoleService.Add(userRolsAdd);
                }
                else
                {
                    var usreroles = (await _userRoleService.Query(d => d.Userid == entity.Id)).Select(d => d.Id).ToArray();
                    await _userRoleService.DeleteByIds(usreroles);
                }
                var DbUser = await _sysUserService.QueryById(entity.Id);
                if (DbUser == null)
                {
                    _unitOfWork.RollbackTran();
                    return ResponseOutput.NotOk("未找到用户信息");
                }
                if (string.IsNullOrWhiteSpace(entity.Password) || DbUser.Password == entity.Password)
                {
                    entity.Password = DbUser.Password;
                }
                entity.Password = UEncrypter.SHA256(entity.Password);
                var suc = await _sysUserService.Update(entity);
                _unitOfWork.CommitTran();
                return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("更新信息失败");
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTran();
                return ResponseOutput.NotOk("更新信息错误");
            }
        }


        /// <summary>
        /// 获取用户详情
        /// 【无权限】
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> GetInfoByToken()
        {
            return ResponseOutput.Ok(await _sysUserService.QueryById(_user.Id));
        }
    }
}
