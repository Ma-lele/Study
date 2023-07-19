using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 总件数、OK件数实体
    /// </summary>
    public class CtEnvOkCountDto
    {
        /// <summary>
        /// OK件数
        /// </summary>
        public int okcount { get; set; }
        /// <summary>
        /// 总件数
        /// </summary>
        public int totalcount { get; set; }
    }
}
