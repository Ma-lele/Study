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
    [SugarTable("T_CA_Round")]
    public class CARound
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true,IsIdentity =true)]
        public int ROUNDID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public int TEID { get; set; }
        public string roundcode { get; set; }
        public string fellow { get; set; }
        public DateTime limitdate { get; set; }
        public int status { get; set; }
        public short bdel { get; set; }
        public DateTime createdate { get; set; }
        public DateTime operatedate { get; set; }
        public string operater { get; set; }
        public string solveduser { get; set; }
        public DateTime solveddate { get; set; }

        public class CARoundEntityParam
        {
            public int SITEID { get; set; }
            public int TEID { get; set; }
            public string fellow { get; set; } = "";
            public string roundcode { get; set; }
            public DateTime? limitdate { get; set; }
            public string checkOklist { get; set; }
            public List<CARoundDetailEntity> checklist { get; set; }

        }

        public class CARoundDetailEntity
        {
            public int TEID { get; set; }
            public int CLID { get; set; }
            public int RDID { get; set; }
            public int isqualified { get; set; } = 1;
            public string roundcode { get; set; } = "";
            public string remark { get; set; } = "";
            public string files { get; set; } = "[]";
        }
    }
}
