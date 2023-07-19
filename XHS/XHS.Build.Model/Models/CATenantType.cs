using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_CA_TenantType")]
    public class CATenantType
    {
        public int TTID { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 状态 0:正常，1:停用
        /// </summary>
        public int Status { get; set; }
        public int sort { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }
}
