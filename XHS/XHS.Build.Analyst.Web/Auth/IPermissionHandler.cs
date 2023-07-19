using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XHS.Build.Analyst.Web.Auth
{
    /// <summary>
    /// 权限处理接口
    /// </summary>
    public interface IPermissionHandler
    {
        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="api"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        Task<bool> ValidateAsync(string api);
    }
}
