using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Helmet")]
    public class GCHelmetEntity
    {
        public int HELMETID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string beaconcode { get; set; }
        public string helmetcode { get; set; }
        public string ID { get; set; }
        //public string useunit { get; set; }
        //public string city { get; set; }
        public string relayurl { get; set; }
        public string datatype { get; set; }
        public int power { get; set; }
        public decimal helmetpa { get; set; }
        public decimal helmetpaline { get; set; }
        public decimal hightdiff { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }

    public class HelmetOutputList:GCHelmetEntity
    {
        public string siteshortname { get; set; }
        public string realname { get; set; }
    }
}
