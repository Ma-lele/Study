using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Invade")]
    public class GCInvadeEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
        public int INVADEID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string invadecode { get; set; }
        public string invadename { get; set; }
        public double invadelng { get; set; }
        public double invadelat { get; set; }
        public string updater { get; set; }
        public DateTime updatedate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 
    /// </summary>
    public class GCInvadeListOutput: GCInvadeEntity
    {
        public string siteshortname { get; set; }
    }
}
