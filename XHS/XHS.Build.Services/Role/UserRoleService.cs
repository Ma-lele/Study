using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Role
{
    public class UserRoleService : BaseServices<UserRoleEntity>, IUserRoleService
    {
        private readonly IBaseRepository<UserRoleEntity> _baseRepository;
        private readonly IUser _user;
        public UserRoleService(IBaseRepository<UserRoleEntity> baseRepository, IUser user)
        {
            _baseRepository = baseRepository;
            base.BaseDal = baseRepository;
            _user = user;
        }

        public async Task<List<UserRoleEntity>> GetUserRoleList()
        {
            return await _baseRepository.Db.Queryable<UserRoleEntity, SysRoleEntity>((a, b) => new JoinQueryInfos(
                      JoinType.Inner, a.Roleid == b.Id)).Where((a, b) => a.Userid == _user.Id && b.IsDeleted == false && b.Enabled == true).Select<UserRoleEntity>().ToListAsync();
        }
    }
}
