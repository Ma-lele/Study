using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_DeepPitDevice")]
    public class GCDeepPitDevice
    {
        public int DPDID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public int DPID { get; set; }
        public string dpcode { get; set; }
        public string deviceid { get; set; }
        public string devicename { get; set; }
        public int bdel { get; set; }
        public DateTime createtime { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }
}
