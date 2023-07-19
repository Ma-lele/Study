using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.CtRole
{
    public interface ICtRoleService:IBaseServices<CTRoleEntity>
    {
        /// <summary>
        /// 获取一页角色
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<CTRoleEntity>> GetPageList(string keyword, int page, int size);

        /// <summary>
        /// 新建角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> CtRoleAdd(CTRoleEntity input);
    }
}
