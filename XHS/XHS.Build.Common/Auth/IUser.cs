using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Auth
{
    /// <summary>
    /// 用户信息接口
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 主键
        /// </summary>
       public string Id { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; }

        public int GroupId { get;  }

        /// <summary>
        /// 角色，（目前一个账号一个角色）
        /// </summary>
        public string Roleid { get; }

        public string LoginName { get; }


        public string Gender { get; }

        public bool IsAdmin { get; }

        /// <summary>
        /// 数据分析用的
        /// </summary>
        public string AnalystRegionID { get; set; }
        public string userregion { get; }
        public string UserSiteType { get; set; }

        public string SiteId { get; set; }
        public string SiteName { get; }
    }
}
