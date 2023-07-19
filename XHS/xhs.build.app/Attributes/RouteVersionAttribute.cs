using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xhs.build.app.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RouteVersionAttribute : RouteAttribute, IApiDescriptionGroupNameProvider
    {
        /// <summary>
        /// 分组名称,是来实现接口 IApiDescriptionGroupNameProvider
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 自定义路由构造函数，继承基类路由
        /// </summary>
        /// <param name="actionName"></param>
        public RouteVersionAttribute(string actionName = "[action]") : base("/api/{version}/[controller]/" + actionName)
        {
        }
        /// <summary>
        /// 自定义版本+路由构造函数，继承基类路由
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="version"></param>
        public RouteVersionAttribute(ApiVersions version, string actionName = "") : base($"/api/{version.ToString()}/[controller]/{actionName}")
        {
            GroupName = version.ToString();
        }
    }

    /// <summary>
    /// 接口版本
    /// </summary>
    public enum ApiVersions
    {
        /// <summary>
        /// V1 版本
        /// </summary>
        V1 = 1,
        /// <summary>
        /// 太湖新城
        /// </summary>
        THXC = 2,
    }
}
