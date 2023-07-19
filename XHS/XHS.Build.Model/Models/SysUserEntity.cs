using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [SugarTable("T_SysUser")]
    public class SysUserEntity : BaseEntity
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

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

        public string Mobile { get; set; }
        public int GroupId { get; set; }
        public DateTime OperateDate { get; set; }
        public bool IsShowinsite { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }

        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public string platform { get; set; } = "010";
        public string Verifycode { get; set; }
        public DateTime? Verifytime { get; set; }
    }
}
