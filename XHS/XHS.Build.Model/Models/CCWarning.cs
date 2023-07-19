using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_CC_Warning")]
    public class CCWarning
    {
        public int WARNID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string devicecode { get; set; }
        /// <summary>
        /// 5:临边围挡报警,51:临边防护离线报警,52:临边线索断开报警,53:临边磁锁断开报警,54:临边人员靠近报警
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 默认1
        /// </summary>
        public int warnlevel { get; set; }
        /// <summary>
        /// 默认0
        /// </summary>
        public int bhandled { get; set; }
        public string content { get; set; }
        public DateTime operatedate { get; set; }
        /// <summary>
        /// 默认0
        /// </summary>
        public int warnstatus { get; set; }
        public string updater { get; set; }
        public DateTime updatedate { get; set; }
        public string paramjson { get; set; }
        public string jsonall { get; set; }
        /// <summary>
        /// 默认0
        /// </summary>
        public int bsendcmd { get; set; }
        public DateTime createdate { get; set; }
        public string remark { get; set; }
        /// <summary>
        /// 默认0
        /// </summary>
        public int WPID { get; set; }
        public string alarmid { get; set; }
    }
}
