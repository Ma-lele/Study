using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 监测对象简介实体类
    /// </summary>
    [SugarTable("T_GC_SiteSummary")]
    public class GCSiteSummaryEntity
    {
        /// <summary>
        /// 分组编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int SITEID { get; set; }
        /// <summary>
        /// 监测对象简介
        /// </summary>
        public string summary { get; set; } = "";
    }
}