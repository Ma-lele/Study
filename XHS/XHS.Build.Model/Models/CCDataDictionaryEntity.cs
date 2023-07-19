using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 字典表
    /// </summary>
    [SugarTable("T_CC_DataDictionary")]
    public class CCDataDictionaryEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int DDID { get; set; }
        public string datatype { get; set; }
        public string dataitem { get; set; }
        public int sort { get; set; }
        public string dcode { get; set; }
    }
}
