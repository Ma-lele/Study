using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 角色实体类
    /// </summary>
    [SugarTable("T_GC_Role")]
    public class GCRoleEntity
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ROLEID { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string rolename { get; set; }
    }
}