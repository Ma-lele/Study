using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XHS.Build.City.Auth
{
    public interface IPermissionHandler
    {
        /// <summary>
        /// 接口权限验证
        /// </summary>
        /// <param name="api"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        Task<bool> ValidateAsync(string api, string ip);
    }
}
