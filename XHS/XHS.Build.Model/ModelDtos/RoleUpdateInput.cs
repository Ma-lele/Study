using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class RoleUpdateInput
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

        public int GROUPID { get; set; }
    }
}
