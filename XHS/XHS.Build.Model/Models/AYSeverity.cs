using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_AY_Severity")]
    public class AYSeverity
    {
        public int SRID { get; set; }
        public string ETID { get; set; }
        public int srlevel { get; set; }
        public decimal srfrom { get; set; }
        public decimal srto { get; set; }
    }
}
