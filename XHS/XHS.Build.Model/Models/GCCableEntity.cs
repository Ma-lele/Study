using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_Cable")]
    public class GCCableEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int CSID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string sensorid { get; set; }
        public string cscode { get; set; }
        public string sensorname { get; set; }
        public int sensortype { get; set; }
        public int risklevel { get; set; }
        public int warned { get; set; }
        public string @operator { get; set; }
        public DateTime? operatedate { get; set; }
    }

    public class GCCablePageListOutput:GCCableEntity
    {
        public string siteshortname { get; set; }
    }
}
