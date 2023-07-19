using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Role
{
    public interface IUserRoleService : IBaseServices<UserRoleEntity>
    {
        Task<List<UserRoleEntity>> GetUserRoleList();
    }
}
