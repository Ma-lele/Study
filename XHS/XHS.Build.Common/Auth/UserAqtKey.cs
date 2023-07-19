using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Auth
{
    public class UserAqtKey : IUserAqtKey
    {
        private readonly IHttpContextAccessor _accessor;

        public UserAqtKey(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
      

        public int SiteId
        {
            get
            {
                var group = _accessor?.HttpContext?.User?.FindFirst(AppKeyClaimAttributes.SiteId);

                if (group != null && !string.IsNullOrEmpty(group.Value))
                {
                    return Convert.ToInt32(group.Value);
                }

                return 0;
            }
        }

        public string Appkey
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(AppKeyClaimAttributes.Appkey);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value;
                }

                return string.Empty;
            }
        }
    

    public string RecordNumber
    {
        get
        {
            var name = _accessor?.HttpContext?.User?.FindFirst(AppKeyClaimAttributes.RecordNumber);

            if (name != null && !string.IsNullOrEmpty(name.Value))
            {
                return name.Value;
            }

            return string.Empty;
        }
    }


        public string Attenduserpsd
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(AppKeyClaimAttributes.Attenduserpsd);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value;
                }

                return string.Empty;
            }
        }

        public string AccessToken
        {
            get
            {
                var token = _accessor?.HttpContext?.User?.FindFirst(AppKeyClaimAttributes.AccessToken);

                if (token != null && !string.IsNullOrEmpty(token.Value))
                {
                    return token.Value;
                }

                return string.Empty;
            }
        }
    }

/// <summary>
/// Claim属性
/// </summary>
public static class AppKeyClaimAttributes
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string Appkey = "appkey";

        public const string SiteId = "siteid";

        public const string RecordNumber = "recordNumber";

        public const string Attenduserpsd = "attenduserpsd";

        public const string AccessToken = "accessToken";

        /// <summary>
        /// 刷新有效期
        /// </summary>
        public const string RefreshExpires = "refresh";

    }
}
