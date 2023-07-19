using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Permission
{
    public class PermissionService:BaseServices<SysPermissionEntity>,IPermissionService
    {
        private readonly IBaseRepository<SysPermissionEntity> _baseRepository;
        private readonly IUser _user;
        public PermissionService(IBaseRepository<SysPermissionEntity> baseRepository, IUser user)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
            _user = user;
        }

        public static void LoopNaviBarAppendChildren(List<NavigationBar> all, NavigationBar curItem)
        {

            var subItems = all.Where(ee => ee.pid == curItem.id).ToList();

            if (subItems.Count > 0)
            {
                curItem.children = new List<NavigationBar>();
                curItem.children.AddRange(subItems);
            }
            else
            {
                curItem.children = null;
            }


            foreach (var subItem in subItems)
            {
                LoopNaviBarAppendChildren(all, subItem);
            }
        }

        public static void LoopToAppendChildren(List<PermissionTree> all, PermissionTree curItem, string pid, bool needbtn)
        {

            var subItems = all.Where(ee => ee.Pid == curItem.value).ToList();

            var btnItems = subItems.Where(ss => ss.isbtn == true).ToList();
            if (subItems.Count > 0)
            {
                curItem.btns = new List<PermissionTree>();
                curItem.btns.AddRange(btnItems);
            }
            else
            {
                curItem.btns = null;
            }

            if (!needbtn)
            {
                subItems = subItems.Where(ss => ss.isbtn == false).ToList();
            }
            if (subItems.Count > 0)
            {
                curItem.children = new List<PermissionTree>();
                curItem.children.AddRange(subItems);
            }
            else
            {
                curItem.children = null;
            }

            if (curItem.isbtn)
            {
                //curItem.label += "按钮";
            }

            foreach (var subItem in subItems)
            {
                if (subItem.value == pid )
                {
                    //subItem.disabled = true;//禁用当前节点
                }
                LoopToAppendChildren(all, subItem, pid, needbtn);
            }
        }

        public async Task<List<SysPermissionEntity>> GetPermissionsAsync()
        {
            return await _baseRepository.Db.Queryable<SysPermissionEntity, RolePermissionApiEntity, UserRoleEntity>((a, b, c) => new JoinQueryInfos(
                     JoinType.Inner, a.Id == b.PermissionId, JoinType.Inner, b.RoleId == c.Roleid)).Where((a, b, c) => a.IsDeleted == false && c.Userid == _user.Id).Select<SysPermissionEntity>().ToListAsync();
        }
    }
}
