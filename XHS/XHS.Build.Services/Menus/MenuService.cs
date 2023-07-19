using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Menus
{
    public class MenuService : BaseServices<BaseEntity>, IMenuService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _menuRepository;

        public MenuService(IUser user, IBaseRepository<BaseEntity> menuRepository)
        {
            _user = user;
            _menuRepository = menuRepository;
            base.BaseDal = menuRepository;
        }


        public async Task<List<GCMenu>> GetAnalystUserMenu()
        {
            _menuRepository.CurrentDb = "XJ_Env";
            var list = await _menuRepository.Db.Queryable<GCRoleRightEntity, GCMenu>((r, m) => new JoinQueryInfos(
                     JoinType.Inner, r.MENUID == m.MENUID
                 ))
                .Where((r, m) => r.ROLEID.ToString() == _user.Roleid)
                .OrderBy((r, m) => m.sort)
                .Select((r, m) => new GCMenu
                {
                    name = m.name,
                    pageurl = m.pageurl,
                    sort = m.sort,
                    bmenu = m.bmenu
                })
                .ToListAsync();
            return list;
        }


        public async Task<DataTable> GetCurrentUserMenus(string prefix="M")
        {
            DataTable dt = await _menuRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoleRightMyMenuList", new { ROLEID = _user.Roleid, prefix = prefix });
            return dt;
            //return await _menuRepository.GetRoleMenus(_user.Roleid, "M");
        }

        
    }
}
