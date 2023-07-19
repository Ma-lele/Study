using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 黄土裸露
    /// </summary>
    [SugarTable("T_AI_SoilAction")]
    public class AISoilActionEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
        public long SCAID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        public string imgurl { get; set; }
        /// <summary>
        /// 照片缩略图地址
        /// </summary>
        public string thumburl { get; set; }
        /// <summary>
        /// 识别后带标签图
        /// </summary>
        public string imgurlmarked { get; set; }
        /// <summary>
        /// 裸土覆盖的百分比数值
        /// </summary>
        public decimal soilrate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime createtime { get; set; }
    }
}
