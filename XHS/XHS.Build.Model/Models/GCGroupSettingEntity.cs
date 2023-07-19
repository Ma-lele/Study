using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 监测对象分组设定实体类
    /// </summary>
    [SugarTable("T_GC_GroupSetting")]
    public class GCGroupSettingEntity
    {
        /// <summary>
        /// 分组编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int GROUPID { get; set; }
        /// <summary>
        /// 键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
    }
}