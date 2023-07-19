using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class SysRoleEntityDto
    {
        public string Id { get; set; }
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

        /// <summary>
        /// 所属分组简写名称
        /// </summary>
        public string GroupShortName { get; set; }
    }
}
