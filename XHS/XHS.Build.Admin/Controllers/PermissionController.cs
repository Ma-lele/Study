using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Apis;
using XHS.Build.Services.Permission;
using XHS.Build.Services.Role;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 模块菜单
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private readonly IUserRoleService _userRoleService;
        private readonly IRolePermissionApiService _rolePermissionApiService;
        private readonly IApiService _apiService;
        private readonly IRoleService _roleService;
        public PermissionController(IPermissionService permissionService, IMapper mapper, IUser user, IUserRoleService userRoleService, IRolePermissionApiService rolePermissionApiService, IApiService apiService, IRoleService roleService)
        {
            _permissionService = permissionService;
            _mapper = mapper;
            _user = user;
            _userRoleService = userRoleService;
            _rolePermissionApiService = rolePermissionApiService;
            _apiService = apiService;
            _roleService = roleService;
        }

        /// <summary>
        /// 所有列表
        /// </summary>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20)
        {
            Expression<Func<SysPermissionEntity, bool>> whereExpression = a => (a.IsDeleted == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = whereExpression.And(b => b.Name.Contains(keyword));
            }
            var dbList = await _permissionService.QueryPage(whereExpression, page, size, " id desc");


            var data = new PageOutput<SysPermissionEntity>()
            {
                data = dbList.data,
                dataCount = dbList.dataCount
            };

            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 查询树形 Table
        /// </summary>
        /// <param name="parentid">父节点</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        [HttpGet]

        public async Task<IResponseOutput> GetTreeTable(string systemid, string parentid = "0", string key = "")
        {
            List<SysPermissionEntity> permissions = new List<SysPermissionEntity>();
            if (string.IsNullOrEmpty(systemid))
            {
                return ResponseOutput.Ok(permissions);
            }
            var apiList = await _apiService.Query();
            var permissionsList = await _permissionService.Query(d => d.IsDeleted == false && d.SystemId==systemid );
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            if (key != "")
            {
                permissions = permissionsList.Where(a => a.Name.Contains(key) 
                || a.Func != null && a.Func.Contains(key) 
                || a.Url != null && a.Url.Contains(key) 
                || a.MName != null && a.MName.Contains(key)).OrderBy(a => a.SortNo).ToList();
            }
            else
            {
                permissions = permissionsList.Where(a => a.ParentId == parentid).OrderBy(a => a.SortNo).ToList();
            }

            foreach (var item in permissions)
            {
                List<string> pidarr = new List<string> { };
                var parent = permissionsList.FirstOrDefault(d => d.Id == item.ParentId);

                while (parent != null)
                {
                    pidarr.Add(parent.Id);
                    parent = permissionsList.FirstOrDefault(d => d.Id == parent.ParentId);
                }

                //item.PidArr = pidarr.OrderBy(d => d).Distinct().ToList();

                pidarr.Reverse();
                pidarr.Insert(0, "0");
                item.PidArr = pidarr;

                //item.MName = apiList.FirstOrDefault(d => d.Id == item.ApiId)?.ApiUrl;
                item.hasChildren = permissionsList.Where(d => d.ParentId == item.Id).Any();

                
                if(item.ApiId != null) {
                    List<string> apis = new List<string>();
                    List<string> MNames = new List<string>();

                    item.ApiIds = item.ApiId.Split(',').ToList();
                    foreach(var id in item.ApiIds)
                    {
                        var MName = apiList.FirstOrDefault(d => d.Id == id)?.ApiUrl;
                        //string MNames = string.Join(",", MName);
                        MNames.Add(MName);
                    }
                    item.MName = string.Join("\r\n", MNames);
                }
            }

            return ResponseOutput.Ok(permissions);
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="needbtn"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPermissionTree(string pid = "0", bool needbtn = false)
        {
            List<SysPermissionEntity> permissions = new List<SysPermissionEntity>();
            if (_user.IsAdmin)//系统管理员
            {
                permissions = await _permissionService.Query(d => d.IsDeleted == false);
            }
            else
            {
                permissions = await _permissionService.GetPermissionsAsync();
            }
            var permissionTrees = (from child in permissions
                                   where child.IsDeleted == false
                                   orderby child.Id
                                   select new PermissionTree
                                   {
                                       value = child.Id,
                                       label = child.Name,
                                       Pid = child.ParentId,
                                       isbtn = child.IsButton,
                                       SortNo = child.SortNo,
                                       disabled = (_user.IsAdmin ? false : !child.Enabled)
                                   }).ToList();
            PermissionTree rootRoot = new PermissionTree
            {
                value = "0",
                Pid = "0",
                label = "根节点"
            };

            permissionTrees = permissionTrees.OrderBy(d => d.SortNo).ToList();


            PermissionService.LoopToAppendChildren(permissionTrees, rootRoot, pid, needbtn);
            return ResponseOutput.Ok(rootRoot);
        }


        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(PermissionAddInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写模块信息");
            }
            var entity = _mapper.Map<SysPermissionEntity>(input);
            if (entity.ApiIds != null && entity.ApiIds.Count > 0)
            {
                entity.ApiId = string.Join(",", entity.ApiIds);
            }
            var rows = await _permissionService.Add(entity);
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
            if (string.IsNullOrEmpty(id))
            {
                return ResponseOutput.NotOk("请选择需要删除的信息");
            }
            var entity = await _permissionService.QueryById(id);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到可删除的信息");
            }
            var subEntity = await _permissionService.Query(a => a.ParentId == entity.Id && a.IsDeleted == false);
            if (subEntity.Any())
            {
                return ResponseOutput.NotOk("存在子菜单或按钮未删除，请确认");
            }

            entity.IsDeleted = true;
            var suc = await _permissionService.Update(entity);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(PermissionUpdateInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Id) || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写需要修改的信息");
            }
            var entity = _mapper.Map<SysPermissionEntity>(input);
            if (entity.ApiIds.Count > 0)
            {
                entity.ApiId = string.Join(",", entity.ApiIds);
            }
            var suc = await _permissionService.Update(entity);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("更新信息失败");
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            return ResponseOutput.Ok(await _permissionService.QueryById(id));
        }

        /// <summary>
        /// 保存菜单权限分配
        /// </summary>
        /// <param name="assignView"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Assign(AssignView assignView)
        {
            if (assignView == null)
            {
                return ResponseOutput.NotOk("请先选择需要操作的数据");
            }
            if (!string.IsNullOrEmpty(assignView.rid))
            {
                var Role = await _roleService.QueryById(assignView.rid);
                if (Role == null)
                {
                    return ResponseOutput.NotOk("请先选择需要操作的数据");
                }
                if (Role.issys && !_user.IsAdmin)
                {
                    return ResponseOutput.NotOk("您无法操作系统角色");
                }

                var roleModulePermissions = await _rolePermissionApiService.Query(d => d.RoleId == assignView.rid);

                var remove = roleModulePermissions.Where(d => !assignView.pids.Contains(d.PermissionId)).Select(c => c.Id);
                if (remove.Any())
                {
                    await _rolePermissionApiService.DeleteByIds(remove.ToArray());
                }

                foreach (var item in assignView.pids)
                {
                    var rmpitem = roleModulePermissions.Where(d => d.PermissionId == item);
                    if (!rmpitem.Any())
                    {
                        var moduleid = (await _permissionService.Query(p => p.Id == item)).FirstOrDefault()?.ApiId;
                        RolePermissionApiEntity roleModulePermission = new RolePermissionApiEntity()
                        {
                            RoleId = assignView.rid,
                            ApiId = moduleid,
                            PermissionId = item,
                        };
                        await _rolePermissionApiService.Add(roleModulePermission);
                        //var apiIdsStr = (await _permissionService.Query(p => p.Id == item)).FirstOrDefault()?.ApiId;
                        //List<string> apiIds = new List<string>();
                        //if (apiIdsStr == null || apiIdsStr=="")
                        //{
                        //    apiIds.Add(apiIdsStr);
                        //}
                        //else
                        //{
                        //    apiIds = apiIdsStr.Split(",").ToList();
                        //}
                        //for (int i = 0; i < apiIds.Count; i++)
                        //{
                        //    RolePermissionApiEntity roleModulePermission = new RolePermissionApiEntity()
                        //    {
                        //        RoleId = assignView.rid,
                        //        ApiId = apiIds[i],
                        //        PermissionId = item,
                        //    };

                        //    await _rolePermissionApiService.Add(roleModulePermission);
                        //}

                    }
                }
            }
            return ResponseOutput.Ok();
        }

        /// <summary>
        /// 通过角色获取菜单【无权限】
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> GetPermissionIdByRoleId(string rid = "")
        {
            var data = new ResponseOutput<AssignShow>();

            var rmps = await _rolePermissionApiService.Query(d => d.RoleId == rid);
            var permissionTrees = (from child in rmps
                                   orderby child.Id
                                   select child.PermissionId).ToList();

            var permissions = await _permissionService.Query(d => d.IsDeleted == false);
            List<string> assignbtns = new List<string>();

            foreach (var item in permissionTrees)
            {
                var pername = permissions.FirstOrDefault(d => d.IsButton && d.Id == item)?.Name;
                if (!string.IsNullOrEmpty(pername))
                {
                    assignbtns.Add(item);
                }
            }

            return data.Ok(new AssignShow()
            {
                permissionids = permissionTrees,
                assignbtns = assignbtns,
            });
        }



        /// <summary>
        /// 获取路由树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> GetNavigationBar(string systemid="1")
        {
            if (_user.IsAdmin)
            {
                var rolePermissionMoudles = (await _permissionService.Query(d => d.IsDeleted == false && d.SystemId == systemid)).OrderBy(c => c.SortNo);
                var permissionTrees = (from child in rolePermissionMoudles
                                       where child.IsDeleted == false
                                       orderby child.Id
                                       select new NavigationBar
                                       {
                                           id = child.Id,
                                           name = child.Name,
                                           pid = child.ParentId,
                                           order = child.SortNo,
                                           path = child.Url,
                                           iconCls = child.Icon,
                                           Func = child.Func,
                                           IsHide = child.IsHide,
                                           IsButton = child.IsButton,
                                           meta = new NavigationBarMeta
                                           {
                                               requireAuth = true,
                                               title = child.Name,
                                               NoTabPage = child.HideTab,
                                               keepAlive = child.iskeepalive
                                           }
                                       }).ToList();


                NavigationBar rootRoot = new NavigationBar()
                {
                    id = "0",
                    pid = "0",
                    order = 0,
                    name = "根节点",
                    path = "",
                    iconCls = "",
                    meta = new NavigationBarMeta(),

                };

                permissionTrees = permissionTrees.OrderBy(d => d.order).ToList();

                PermissionService.LoopNaviBarAppendChildren(permissionTrees, rootRoot);
                return ResponseOutput.Ok(rootRoot);
            }
            var roleIds = (await _userRoleService.GetUserRoleList()).Select(b => b.Roleid).ToList();
            if (roleIds.Any())
            {
                var pids = (await _rolePermissionApiService.Query(d => roleIds.Contains(d.RoleId))).Select(d => d.PermissionId).Distinct();
                if (pids.Any())
                {
                    var rolePermissionMoudles = (await _permissionService.Query(d => pids.Contains(d.Id) && d.IsDeleted == false && d.SystemId == systemid)).OrderBy(c => c.SortNo);
                    var permissionTrees = (from child in rolePermissionMoudles
                                           where child.IsDeleted == false
                                           orderby child.Id
                                           select new NavigationBar
                                           {
                                               id = child.Id,
                                               name = child.Name,
                                               pid = child.ParentId,
                                               order = child.SortNo,
                                               path = child.Url,
                                               iconCls = child.Icon,
                                               Func = child.Func,
                                               IsHide = child.IsHide,
                                               IsButton = child.IsButton,
                                               meta = new NavigationBarMeta
                                               {
                                                   requireAuth = true,
                                                   title = child.Name,
                                                   NoTabPage = child.HideTab,
                                                   keepAlive = child.iskeepalive
                                               }
                                           }).ToList();


                    NavigationBar rootRoot = new NavigationBar()
                    {
                        id = "0",
                        pid = "0",
                        order = 0,
                        name = "根节点",
                        path = "",
                        iconCls = "",
                        meta = new NavigationBarMeta(),

                    };

                    permissionTrees = permissionTrees.OrderBy(d => d.order).ToList();

                    PermissionService.LoopNaviBarAppendChildren(permissionTrees, rootRoot);
                    return ResponseOutput.Ok(rootRoot);
                }
                else
                {
                    return ResponseOutput.NotOk("没有权限");
                }

            }
            else
            {
                return ResponseOutput.NotOk("没有权限");
            }
        }
    }
}
