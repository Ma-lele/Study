using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class RoleAddInput
    {
        public string Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Enabled { get; set; }

        public int GROUPID { get; set; }
    }
}
