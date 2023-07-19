using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 角色表
    /// </summary>
    [SugarTable("T_SysRole")]
    public class SysRoleEntity : BaseEntity
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 系统角色
        /// </summary>
        public bool issys { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 所属分组
        /// </summary>
        public int GROUPID { get; set; }

    }
}
