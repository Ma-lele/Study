using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Auth
{
    public class UserKey : IUserKey
    {
        private readonly IHttpContextAccessor _accessor;

        public UserKey(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Key
        {
            get
            {
                var key = _accessor?.HttpContext?.User?.FindFirst(KeyClaimAttributes.Key);
                if (key != null && !string.IsNullOrEmpty(key.Value))
                {
                    return key.Value;
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId
        {
            get
            {
                var id = _accessor?.HttpContext?.User?.FindFirst(KeyClaimAttributes.UserId);
                if (id != null && !string.IsNullOrEmpty(id.Value))
                {
                    return id.Value;
                }
                return string.Empty;
            }
        }
        //public string Secret
        //{
        //    get
        //    {
        //        var secret = _accessor?.HttpContext?.User?.FindFirst(KeyClaimAttributes.Secret);
        //        if (secret != null && !string.IsNullOrEmpty(secret.Value))
        //        {
        //            return secret.Value;
        //        }
        //        return string.Empty;
        //    }
        //}

        public int GroupId
        {
            get
            {
                var group = _accessor?.HttpContext?.User?.FindFirst(KeyClaimAttributes.GroupId);

                if (group != null && !string.IsNullOrEmpty(group.Value))
                {
                    return Convert.ToInt32(group.Value);
                }

                return 0;
            }
        }

        public string Name
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(KeyClaimAttributes.Name);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value;
                }

                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Claim属性
    /// </summary>
    public static class KeyClaimAttributes
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string Key = "key";

        /// <summary>
        /// 用户名
        /// </summary>
        public const string Name = "Name";

        public const string GroupId = "groupid";
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string UserId = "userid";
        /// <summary>
        /// 刷新有效期
        /// </summary>
        public const string RefreshExpires = "refresh";

    }
}
