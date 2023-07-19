using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SiteMapShape")]
    public class GCSiteMapShareEntity
    {
        [SugarColumn(IsPrimaryKey =true)]
        public int SMPID { get; set; }
        public int SITEID { get; set; }
        public int GROUPID { get; set; }
        public string points { get; set; }
        public int shapeheight { get; set; }
        public string @operator { get; set; }
        public DateTime? operatedate { get; set; } = DateTime.Now;
    }
}
