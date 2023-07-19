using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XHS.Build.Common.Cache
{
    public static class CacheKey
    {
        /// <summary>
        /// 晨会交底token 缓存cache的 key
        /// 如果不共用，可以加个{0}==id
        /// </summary>
        public const string MorningMeetingKey = "MorningMeetingKey";
        /// <summary>
        /// DeviceBind 列表缓存
        /// </summary>
        public const string DeviceBindCenterKey = "DeviceBindCenterKey";

        /// <summary>
        /// 请求Net服务需要的token
        /// </summary>
        public const string UserCenterNetToken = "user:center:net:{0}";
        /// <summary>
        /// 请求Net服务需要的token
        /// </summary>
        public const string CenterNetToken = "center:net:{0}";

        /// <summary>
        /// 用户权限 admin:user:用户主键:permissions
        /// </summary>
        [Description("用户权限")]
        public const string UserPermissions = "admin:user:{0}:permissions";

        /// <summary>
        /// 用户权限 Front:user:用户主键:permissions
        /// </summary>
        [Description("用户权限")]
        public const string FrontUserPermissions = "front:user:{0}:permissions";
    }
}
