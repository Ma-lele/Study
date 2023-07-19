using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Unload")]
    public class GCUnloadEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ULID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; } = 0;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string unloadid { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string unloadname { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        public int bdel { get; set; } = 0;
    }


    /// <summary>
    /// 
    /// </summary>
    public class GCUnloadPageListOutput : GCUnloadEntity
    {
        /// <summary>
        /// 监测对象简称
        /// </summary>
        public string siteshortname { get; set; }
    }
}
