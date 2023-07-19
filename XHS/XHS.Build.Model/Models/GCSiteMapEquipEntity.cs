using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SiteMapEquip")]
    public class GCSiteMapEquipEntity
    {
        [SugarColumn(IsPrimaryKey =true)]
        public int SMEID { get; set; }
        public int SITEID { get; set; }
        public int GROUPID { get; set; }
        public string equipcode { get; set; }
        public int equiptype { get; set; }
        public float equiplng { get; set; }
        public float equiplat { get; set; }
        public int equipheight { get; set; }
        public string @operator { get; set; }
        public DateTime? operatedate { get; set; } = DateTime.Now;
    }
}
