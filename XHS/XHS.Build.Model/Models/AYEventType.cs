using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_AY_EventType")]
    public class AYEventType
    {
        public string ETID { get; set; }
        public string etname { get; set; }
        public DateTime lastdatetime { get; set; }
        public string etdatatype { get; set; }
        public int etsrtype { get; set; }
    }
}
