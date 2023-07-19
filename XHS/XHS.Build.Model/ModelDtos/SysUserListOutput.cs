using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 
    /// </summary>
    public class SysUserListOutput : BaseEntity
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 姓名、昵称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        public List<string> RIDs { get; set; }

        public string Mobile { get; set; }
        public int GroupId { get; set; }
        public bool IsShowinsite { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public List<string> RoleNames { get; set; }
    }


    public class UserAddInput : BaseEntity
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 姓名、昵称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        public List<string> RIDs { get; set; }

        public string Mobile { get; set; }
        public int GroupId { get; set; }
        public bool IsShowinsite { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
    }
}
