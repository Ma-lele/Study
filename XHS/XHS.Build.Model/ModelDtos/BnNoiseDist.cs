using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 噪声分布Bean
    /// </summary>
    public class BnNoiseDist
    {
        public int SITEID { get; set; }
        public string sitename { get; set; }
        public string siteshortname { get; set; }
        public Decimal percentok { get; set; }
        public Decimal percentng { get; set; }
        /// <summary>
        /// 0:全天;1:白天;2:夜间
        /// </summary>
        public int type { get; set; }
    }
}
