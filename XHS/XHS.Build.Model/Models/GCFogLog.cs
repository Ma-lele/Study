using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 雾炮日志
    /// </summary>
    [SugarTable("T_GC_FogLog")]
    public class GCFogLog
    {
        public long FOGLOGID { get; set; }
        public int GROUPID { get; set; }
        public int USERID { get; set; }
        public string fogcode { get; set; }
        public string direct { get; set; }
        public string cmd { get; set; }
        public string msg { get; set; }
        public string command { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }
}
