using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Role
{
    public class RolePermissionApiService : BaseServices<RolePermissionApiEntity>, IRolePermissionApiService
    {
        private readonly IBaseRepository<RolePermissionApiEntity> _baseRepository;
        public RolePermissionApiService(IBaseRepository<RolePermissionApiEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            base.BaseDal = baseRepository;
        }
    }
}
