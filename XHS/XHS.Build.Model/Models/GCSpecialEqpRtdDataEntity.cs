using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SpecialEqpRtdData")]
    public class GCSpecialEqpRtdDataEntity
    {
        public int SERDID { get; set; }
        public int GROUPID { get; set; }
        public string secode { get; set; }
        public int setype { get; set; }
        public int alarmstate { get; set; }
        public string ID { get; set; }
        public string sedata { get; set; }
        public DateTime updatedate { get; set; }
    }
}
