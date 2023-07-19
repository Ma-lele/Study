using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_SpecialEqpRecordProof")]
    public class GCSpecialEqpRecordProof
    {
        public Guid SERPROOFID { get; set; }
        public int SERID { get; set; }
        public string filename { get; set; }
        public int filesize { get; set; }
        public DateTime createtime { get; set; }
    }
}
