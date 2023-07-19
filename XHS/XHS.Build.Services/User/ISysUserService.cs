using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.User
{
    public interface ISysUserService : IBaseServices<SysUserEntity>
    {
        Task<IList<string>> GetPermissionsAsync();

        Task<IList<string>> GetFrontPermissionsAsync();

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        Task<int> ResetPassword(string id, string passwordOld, string passwordNew);

        Task<int> SetVerifyCode(string mobile, string verifycode);
    }
}
