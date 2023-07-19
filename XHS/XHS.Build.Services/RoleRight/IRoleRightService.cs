using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.RoleRight
{
    public interface IRoleRightService : IBaseServices<GCRoleRightEntity>
    {
        /// <summary>
        /// 获取指定角色的权限
        /// </summary>
        /// <param name="ROLEID"></param>
        /// <returns></returns>
        Task<DataTable> GetAll(int ROLEID);

        /// <summary>
        /// 保存指定角色的权限
        /// </summary>
        /// <param name="ROLEID"></param>
        /// <param name="MENUIDS"></param>
        /// <returns></returns>
        Task<int> Save(string ROLEID, string MENUIDS);

        /// <summary>
        /// 获取指定角色的权限（市平台）
        /// </summary>
        /// <param name="ROLEID"></param>
        /// <returns></returns>
        Task<DataTable> GetCtAll(string ROLEID);

        Task<IList<string>> GetFrontPermissionsAsync(string ROLEID, string prefix = "M");
    }
}
