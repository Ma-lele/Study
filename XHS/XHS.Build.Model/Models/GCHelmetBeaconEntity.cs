using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_HelmetBeacon")]
    public class GCHelmetBeaconEntity
    {
        [SugarColumn(IsIdentity =true, IsPrimaryKey =true)]
        public int BEACONID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string beaconcode { get; set; }
        public string beaconname { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }

    public class HelmetBeaconOutputList
    {
        public int BEACONID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string beaconcode { get; set; }
        public string beaconname { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }

        public string siteshortname { get; set; }
        public string sitename { get; set; }
    }
}
