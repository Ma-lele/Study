using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 颗粒物分布Bean
    /// </summary>
    public class BnPmDist
    {
        public int SITEID { get; set; }
        public string sitename { get; set; }
        public string siteshortname { get; set; }
        public int lv1 { get; set; }
        public int lv2 { get; set; }
        public int lv3 { get; set; }
        public int lv4 { get; set; }
        public int lv5 { get; set; }
        public int lv6 { get; set; }
        /// <summary>
        /// 0:Pm10;1:Pm2.5
        /// </summary>
        public int type { get; set; }
    }
}
