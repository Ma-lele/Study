using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_RolePermissionApi")]
    public class RolePermissionApiEntity:BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PermissionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApiId { get; set; }
    }
}
