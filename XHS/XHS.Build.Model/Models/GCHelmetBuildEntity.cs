using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_HelmetBuild")]
    public class GCHelmetBuildEntity
    {
        public int BUILDID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string buildname { get; set; }
        public int buildfloor { get; set; }
        public decimal floorheight { get; set; }
        public string inbeaconcode { get; set; }
        public string outbeaconcode { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        public int underfloor { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HelmetBuildOutputList : GCHelmetBuildEntity
    {
        public string siteshortname { get; set; }
        public string sitename { get; set; }
    }
}
