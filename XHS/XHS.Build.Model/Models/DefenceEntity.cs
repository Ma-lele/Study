using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 临边围挡
    /// </summary>
    [SugarTable("T_GC_Defence")]
    public class DefenceEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int DEFENCEID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string dfcode { get; set; }
        public string dfname { get; set; }
        public int dfstatus { get; set; }
        public int bsheild { get; set; }
        public string dfzone { get; set; }
        public DateTime checkintime { get; set; }
        public DateTime checkouttime { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }

        [SugarColumn(IsIgnore =true)]
        public string siteshortname { get; set; }
    }
}
