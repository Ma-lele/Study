using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleListOutput:BaseEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// 
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

        public int GROUPID { get; set; }
        public string GroupShortName { get; set; }
    }
}
