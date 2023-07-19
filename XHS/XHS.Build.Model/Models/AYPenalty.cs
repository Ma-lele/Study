using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_AY_Penalty")]
    public class AYPenalty
    {
        public int PID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string penaltycode { get; set; }
        public string content { get; set; }
        public DateTime penaltydate { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }
}
