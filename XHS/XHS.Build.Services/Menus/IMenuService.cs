using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Menus
{
    public interface IMenuService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 当前用户菜单
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetCurrentUserMenus(string prefix = "M");

        Task<List<GCMenu>> GetAnalystUserMenu();
    }
}
