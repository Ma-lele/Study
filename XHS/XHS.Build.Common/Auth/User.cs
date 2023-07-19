using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Common.Helps;

namespace XHS.Build.Common.Auth
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class User : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public User(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string Id
        {
            get
            {
                var id = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId);
                if (id != null && !string.IsNullOrEmpty(id.Value))
                {
                    return id.Value;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value;
                }

                return "";
            }
        }

        /// <summary>
        /// Group
        /// </summary>
        public int GroupId
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.GroupId);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return Convert.ToInt32(name.Value);
                }

                return 0;
            }
        }

        public string Roleid
        {
            get
            {
                var Obj = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.Role);

                if (Obj != null && !string.IsNullOrEmpty(Obj.Value))
                {
                    return Obj.Value;
                }

                return "";
            }
        }

        public string LoginName
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.LoginName);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value;
                }

                return string.Empty;
            }
        }

        public string Gender
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.Gender);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value;
                }

                return string.Empty;
            }
        }

        public bool IsAdmin
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.IsAdmin);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value.ToBool();
                }

                return false;
            }
        }

        public string userregion
        {
            get
            {
                var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserRegion);

                if (name != null && !string.IsNullOrEmpty(name.Value))
                {
                    return name.Value;
                }

                return "";
            }
        }

        private string _AnalystRegionID;
        public string AnalystRegionID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_AnalystRegionID))
                {
                    var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.AnalystRegionID);

                    if (name != null && !string.IsNullOrEmpty(name.Value))
                    {
                        _AnalystRegionID = name.Value;
                    }
                    else
                    {
                        _AnalystRegionID = "";
                    }
                }
                return _AnalystRegionID;
            }
            set
            {
                _AnalystRegionID = value;
            }
        }
        private string _UserSiteType;
        public string UserSiteType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_UserSiteType))
                {
                    var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserSiteType);

                    if (name != null && !string.IsNullOrEmpty(name.Value))
                    {
                        _UserSiteType = name.Value;
                    }
                    else
                    {
                        _UserSiteType = "";
                    }
                }
                return _UserSiteType;
            }
            set
            {
                _UserSiteType = value;
            }
        }
        private string _SiteId;
        public string SiteId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SiteId))
                {
                    var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.SiteId);

                    if (name != null && !string.IsNullOrEmpty(name.Value))
                    {
                        _SiteId = name.Value;
                    }
                    else
                    {
                        _SiteId = "";
                    }
                }
                return _SiteId;
            }
            set
            {
                _SiteId = value;
            }
        }
        private string _SiteName;
        public string SiteName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SiteName))
                {
                    var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.SiteName);

                    if (name != null && !string.IsNullOrEmpty(name.Value))
                    {
                        _SiteName = name.Value;
                    }
                    else
                    {
                        _SiteName = "";
                    }
                }
                return _SiteName;
            }
            set
            {
                _SiteName = value;
            }
        }
    }

    /// <summary>
    /// Claim属性
    /// </summary>
    public static class ClaimAttributes
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public const string UserId = "userid";

        /// <summary>
        /// 用户名
        /// </summary>
        public const string UserName = "username";

        /// <summary>
        /// 分组
        /// </summary>
        public const string GroupId = "groupid";

        /// <summary>
        /// 刷新有效期
        /// </summary>
        public const string RefreshExpires = "refresh";

        /// <summary>
        /// 角色
        /// </summary>
        public const string Role = "roleid";

        /// <summary>
        /// 登录名
        /// </summary>
        public const string LoginName = "loginname";

        public const string Gender = "gender";

        public const string IsAdmin = "isadmin";

        public const string AnalystRegionID = "analystregionid";

        public const string UserRegion = "userregion";


        public const string SiteId = "siteid";

        public const string SiteName = "sitename";
        public const string UserSiteType = "usersitetype";
    }
}
