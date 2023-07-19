using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_AY_FrequencyType")]
    public class AYFrequencyType
    {
        public string FTCODE { get; set; }
        public decimal ftfrom { get; set; }
        public decimal ftto { get; set; }
    }
}
