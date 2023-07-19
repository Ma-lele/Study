using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using Microsoft.Extensions.DependencyInjection;
using XHS.Build.Common.Helps;
using XHS.Build.Aqt365.Auth;

namespace XHS.Build.Aqt365.Attributes
{
    /// <summary>
    /// 启用权限
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter, IAsyncAuthorizationFilter
    {
        private async Task PermissionAuthorization(AuthorizationFilterContext context)
        {
            //排除匿名访问
            if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(AllowAnonymousAttribute)))
                return;

            //登录验证
            var user = context.HttpContext.RequestServices.GetService<IUserAqtKey>();
            if (user == null || string.IsNullOrEmpty(user.Appkey))
            {
                context.Result = new ChallengeResult();
                return;
            }

            //权限验证
            var httpMethod = context.HttpContext.Request.Method;
            var api = context.ActionDescriptor.AttributeRouteInfo.Template;
            var ip = IPHelper.GetIP(context.HttpContext.Request);
            var permissionHandler = context.HttpContext.RequestServices.GetService<IPermissionHandler>();
            var isValid = await permissionHandler.ValidateAsync(api, ip);
            if (!isValid)
            {
                context.Result = new JsonResult(new { code = "403", msg = "没有权限" });//new ForbidResult();
            }
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            await PermissionAuthorization(context);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await PermissionAuthorization(context);
        }
    }
}
