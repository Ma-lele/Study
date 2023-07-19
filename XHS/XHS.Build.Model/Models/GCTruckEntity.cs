using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 实体类
    /// </summary>
    [SugarTable("T_GC_SiteTruck")]
    public class GCSiteTruckEntity
    {
        /// <summary>
        /// 数据编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int STID { get; set; }
        ///<summary>
        /// 分组ID
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测点ID
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 渣土车车牌
        /// </summary>
        public string truckno { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 运输公司名称
        /// </summary>
        public string transcomp { get; set; }
        /// <summary>
        /// 处置证号
        /// </summary>
        public string disposeno { get; set; }
        /// <summary>
        /// 有效期开始日
        /// </summary>
        public DateTime? truckstartdate { get; set; }
        /// <summary>
        /// 有效期结束日
        /// </summary>
        public DateTime? truckenddate { get; set; }
    }

    /// <summary>
    /// 水表列表实体类
    /// </summary>
    public class GCTruckPageListOutput : GCSiteTruckEntity
    {
        /// <summary>
        /// 分组简称
        /// </summary>
        //public string groupshortname { get; set; }
        /// <summary>
        /// 监测对象简称
        /// </summary>
        public string siteshortname { get; set; }
    }
}