using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SpecialEqp")]
    public class GCSpecialEqpEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SEID { get; set; }
        public int SETYPEID { get; set; }
        public int SITEID { get; set; }
        public string sename { get; set; }
        public string secode { get; set; }
        public int setype { get; set; }
        public int alarmstate { get; set; }
        public DateTime? alarmdate { get; set; } = DateTime.Now;
        public string paramjson { get; set; }
        public DateTime? lastcheckday { get; set; }
        public DateTime? productdate { get; set; }
        public string maker { get; set; }
        public string remark { get; set; }
        public int bdel { get; set; }
        public int sestatus { get; set; }
        public string liftovercode { get; set; }
        public int hasreport { get; set; }
        public DateTime? checkintime { get; set; }
        public DateTime? checkouttime { get; set; }
        public string @operator { get; set; }
        public DateTime? operatedate { get; set; }

        public int GROUPID { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SpecialEqpType")]
    public class GCSpecialEqpTypeEntity
    {
        public int SETYPEID { get; set; }
        public int GROUPID { get; set; }
        public string setypename { get; set; }
        public int checktime { get; set; }
        public int checktype { get; set; }
        public int bdel { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SpecialEqpListOutput : GCSpecialEqpEntity
    {
        public string setypename{get;set;}

        public DateTime? nextcheckday { get; set; }

        public string siteshortname { get; set; }

        public int daycount { get; set; }
    }
}
