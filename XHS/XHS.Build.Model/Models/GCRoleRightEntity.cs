using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 角色权限实体类
    /// </summary>
    [SugarTable("T_GC_RoleRight")]
    public class GCRoleRightEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long RIGHTID { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        public string ROLEID { get; set; }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string MENUID { get; set; }
        /// <summary>
        /// 菜单编号(多个)
        /// </summary>
        public string MENUIDS { get; set; }
    }
}