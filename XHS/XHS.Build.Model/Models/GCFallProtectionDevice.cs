using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_FallProtectionDevice")]
    public class GCFallProtectionDevice
    {
        public int FPDID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string deviceId { get; set; }
        public string name { get; set; }
        public DateTime creationtime { get; set; }
        /// <summary>
        /// 设备在线状态  0：离线  1：在线
        /// </summary>
        public int onlinestatus { get; set; }
        public string alarmId { get; set; }
        public int battery { get; set; }
        public string @long { get; set; }
        public string lat { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        public bool bdel { get; set; }
        public DateTime lastpushtime { get; set; }
    }
}
