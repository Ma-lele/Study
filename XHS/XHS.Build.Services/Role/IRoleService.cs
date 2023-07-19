using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Role
{
    public interface IRoleService:IBaseServices<SysRoleEntity>
    {
        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        Task<PageOutput<SysRoleEntityDto>> QueryRolePage(Expression<Func<SysRoleEntity, GcGroupEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
    }
}
