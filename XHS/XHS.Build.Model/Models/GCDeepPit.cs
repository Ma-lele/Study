using SqlSugar;
using System;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_DeepPit")]
    public class GCDeepPit
    {
        public int DPID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string dpcode { get; set; }
        public string dpname { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string dpurl { get; set; }
        public int bdel { get; set; }
        public DateTime createtime { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }
}
