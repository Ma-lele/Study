using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 角色菜单模块关系
    /// </summary>
    [SugarTable("T_RolePermission")]
    public class RolePermissionEntity:BaseEntity
    {
        public string Roleid { get; set; }

        public string Permissionid { get; set; }
    }
}
