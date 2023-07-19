using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// PMXXXBean
    /// </summary> 
    public class BnPmMonth
    { 
        public int SITEID { get; set; } 
        public string sitename { get; set; } 
        public string siteshortname { get; set; }
        /// <summary>
        /// 0:全部;
        /// </summary> 
        public int sitetype { get; set; } 
        public Decimal monthpm10 { get; set; } 
        public Decimal monthpm2_5 { get; set; }
    }
}
