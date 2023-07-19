using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SiteMap")]
    public class GCSiteMapEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int SMID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 地图旋转角度
        /// </summary>
        public float heading { get; set; }
        /// <summary>
        /// 地图的倾斜角度
        /// </summary>
        public float tilt { get; set; }
        /// <summary>
        /// 视角中心点经度
        /// </summary>
        public double ctlng { get; set; }
        /// <summary>
        /// 视角中心点纬度
        /// </summary>
        public double ctlat { get; set; }
        /// <summary>
        /// 地图缩放级别
        /// </summary>
        public float mapzoom { get; set; }
        /// <summary>
        /// 平面图左上角对应的经度
        /// </summary>
        public double ltlng { get; set; }
        /// <summary>
        /// 平面图左上角对应的纬度
        /// </summary>
        public double ltlat { get; set; }
        /// <summary>
        /// 平面图右下角对应的经度
        /// </summary>
        public double rblng { get; set; }
        /// <summary>
        /// 平面图右下角对应的纬度
        /// </summary>
        public double rblat { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 平面图地址
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string planeurl { get; set; }

    }
}
