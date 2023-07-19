using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 电表设备实体类
    /// </summary>
    [SugarTable("T_GC_ElecMeter")]
    public class GCElecMeterEntity
    {
        /// <summary>
        /// 电表设备自增编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int EMID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 电表设备编号
        /// </summary>
        public string emetercode { get; set; }
        /// <summary>
        /// 电表设备名称
        /// </summary>
        public string emetername { get; set; }
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
    /// 电表列表实体类
    /// </summary>
    public class GCElecMeterPageListOutput : GCElecMeterEntity
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
    /// 分组电表数量实体
    /// </summary>
    public class GroupElecMeterCount
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
        /// 一个分组中电表的数量
        /// </summary>
        public int count { get; set; }
    }
}