using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Fog")]
    public class GCFogEntity
    {
        public int FOGID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string fogcode { get; set; }
        public string fogname { get; set; }
        public int fogtype { get; set; }
        public int fogstatus { get; set; }
        public int switchno { get; set; }
        public string delay { get; set; }
        public string bwaterauto { get; set; }

        [SugarColumn(IsOnlyIgnoreInsert =true)]
        public DateTime? checkintime { get; set; }

        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public DateTime? checkouttime { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }

    public class GCFogSitePageListOutput: GCFogEntity
    {
        public string groupshortname { get; set; }

        public string siteshortname { get; set; }
    }
}
