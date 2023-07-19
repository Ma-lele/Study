using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Services.RoleRight;
using XHS.Build.Services.User;

namespace XHS.Build.SiteFront.Auth
{
    /// <summary>
    /// 权限处理
    /// </summary>
    [SingleInstance]
    public class PermissionHandler : IPermissionHandler
    {
        private readonly IRoleRightService _roleRightService;
        private readonly IUser _user;

        public PermissionHandler(IRoleRightService roleRightService, IUser user)
        {
            _roleRightService = roleRightService;
            _user = user;
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="api">接口路径</param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string api)
        {
            //放行共通菜单
            if (api.Contains("/T000/", StringComparison.OrdinalIgnoreCase))
                return true;

            var permissions = await _roleRightService.GetFrontPermissionsAsync(_user.Roleid,"T");
            var isValid = permissions.Any(m => m != null && api.Contains(m, StringComparison.OrdinalIgnoreCase));
            return isValid;
        }
    }
}
