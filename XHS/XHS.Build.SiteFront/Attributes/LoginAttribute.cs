using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XHS.Build.SiteFront.Attributes
{
    /// <summary>
    /// 登录后不走权限
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LoginAttribute : Attribute
    {
    }
}
