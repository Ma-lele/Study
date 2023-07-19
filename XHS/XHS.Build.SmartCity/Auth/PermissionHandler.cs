using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Attributes;
using XHS.Build.Services.User;

namespace XHS.Build.SmartCity.Auth
{
    /// <summary>
    /// 权限处理
    /// </summary>
    [SingleInstance]
    public class PermissionHandler : IPermissionHandler
    {
        private readonly ISysUserService _userService;

        public PermissionHandler(ISysUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="api">接口路径</param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string api)
        {
            var permissions = await _userService.GetPermissionsAsync();

            var isValid = permissions.Any(m => m != null && m.Equals($"/{api}", StringComparison.OrdinalIgnoreCase));
            return isValid;
        }
    }
}
