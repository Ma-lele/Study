using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_CA_CheckModel")]
    public class CACheckModel
    {
        public int CMID { get; set; }
        public string name { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        /// <summary>
        /// 租户类型ID
        /// </summary>
        public int TTID { get; set; }
        /// <summary>
        /// 0:正常 1:删除
        /// </summary>
        public int bdel { get; set; }
    }
}
