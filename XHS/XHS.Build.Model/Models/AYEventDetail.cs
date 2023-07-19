using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_AY_EventDetail")]
    public class AYEventDetail
    {
        public int EDID { get; set; }
        public string stype { get; set; }
        public string linkid { get; set; }
        public string detail { get; set; }
    }
}
