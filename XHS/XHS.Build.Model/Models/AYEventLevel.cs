using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_AY_EventLevel")]
    public class AYEventLevel
    {
        public int ELID { get; set; }
        public string FTCODE { get; set; }
        public int srlevel { get; set; }
        public int eventlevel { get; set; }
    }
}
