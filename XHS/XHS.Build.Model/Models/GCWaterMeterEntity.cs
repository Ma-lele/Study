using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 水表设备实体类
    /// </summary>
    [SugarTable("T_GC_WaterMeter")]
    public class GCWaterMeterEntity
    {
        /// <summary>
        /// 水表设备自增编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int WMID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 水表设备编号
        /// </summary>
        public string wmetercode { get; set; }
        /// <summary>
        /// 水表设备名称
        /// </summary>
        public string wmetername { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 0:正常 1:删除
        /// </summary>
        public int bdel { get; set; } = 0;
    }

    /// <summary>
    /// 水表列表实体类
    /// </summary>
    public class GCWaterMeterPageListOutput : GCWaterMeterEntity
    {
        /// <summary>
        /// 分组简称
        /// </summary>
        public string groupshortname { get; set; }
        /// <summary>
        /// 监测对象简称
        /// </summary>
        public string siteshortname { get; set; }
    }

    /// <summary>
    /// 分组水表数量实体
    /// </summary>
    public class GroupWaterMeterCount
    {
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 分组简称
        /// </summary>
        public string groupshortname { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string groupname { get; set; }
        /// <summary>
        /// 一个分组中水表的数量
        /// </summary>
        public int count { get; set; }
    }
}