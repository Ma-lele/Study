using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 晨会交底
    /// </summary>
    [SugarTable("T_GC_AmDisclose")]
    public class GCAmDiscloseEntity
    {
        /// <summary>
        /// 绑定项目编号
        /// </summary>
        public string projid { get; set; }
        /// <summary>
        /// 图片URL
        /// </summary>
        [SugarColumn(DefaultValue ="")]
        public string imgurl { get; set; }
        /// <summary>
        /// 缩略图URL
        /// </summary>
        [SugarColumn(DefaultValue = "[]")]
        public string thumburl { get; set; }
        /// <summary>
        /// 视频录像URL
        /// </summary>
        [SugarColumn(DefaultValue = "")] 
        public string videourl { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public int numofpeople { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startdate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? enddate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? createdate { get; set; }
    }
}
