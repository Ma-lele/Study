using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 臭氧表
    /// </summary>
    [SugarTable("T_GC_Ozone")]
    public class GCOzone
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int OZID { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象ID
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string ozcode { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string ozname { get; set; }
        /// <summary>
        /// 数值
        /// </summary>
        public decimal o3 { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int bdel { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary> 
        public DateTime operatedate { get; set; }
    }
}
