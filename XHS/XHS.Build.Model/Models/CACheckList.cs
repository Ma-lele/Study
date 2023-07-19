using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_CA_CheckList")]
    public class CACheckList
    {
        public int CLID { get; set; }
        /// <summary>
        /// 类别1：模板 2：自定义
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 模板ID或租户ID
        /// </summary>
        public int linkid { get; set; }
        public string name { get; set; }
        public int sort { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        /// <summary>
        /// 0:正常 1:删除
        /// </summary>
        public int bdel { get; set; }
    }
}
