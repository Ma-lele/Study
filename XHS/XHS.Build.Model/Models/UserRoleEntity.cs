using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 用户角色关系
    /// </summary>
    [SugarTable("T_UserRole")]
    public class UserRoleEntity : BaseEntity
    {
        public string Userid { get; set; }

        public string Roleid { get; set; }
    }
}
