using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Inspection")]
    public class InspectionEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true,IsIdentity =true)]
        public int INSPID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public int USERID { get; set; }
        public string inspcode { get; set; }
        public string insplevel { get; set; }
        public string items { get; set; }
        public int itemnum { get; set; }
        public DateTime limitdate { get; set; }
        public string processjson { get; set; }
        public int processstatus { get; set; }
        public int insppoint { get; set; }
        public int reformpoint { get; set; }
        public short bdel { get; set; }
        public DateTime createdate { get; set; }
        public DateTime reformdate { get; set; }
        public int CHARGERID { get; set; }
        public int SOLVERID { get; set; }
        public DateTime solveddate { get; set; }
        public DateTime updatedate { get; set; }

        public class InspectionEntityParam
        {
            public int SITEID { get; set; }
            public string items { get; set; } = "[[]]";
            public int itemnum { get; set; } = 0;
            public string remark { get; set; } = "";
            public string inspcode { get; set; }
            public int insppoint { get; set; } = 0;
            public int CHARGERID { get; set; } = 0;
            public int insptype { get; set; } = 2;
            public string insplevel { get; set; }
            public DateTime? limitdate { get; set; }
            public int deductpoint { get; set; } = 0;
            public string deletefiles { get; set; } = "[]";
            public string files { get; set; } = "[]";
        }
    }
}
