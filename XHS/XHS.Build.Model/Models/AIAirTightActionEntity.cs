using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 密闭运输
    /// </summary>
    [SugarTable("T_AI_AirTightAction")]
    public class AIAirTightActionEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long ATID { get; set; }
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
        /// 密闭状态.0:未密闭，1:密闭
        /// </summary>
        public int tightstatus { get; set; }
        /// <summary>
        /// 车辆信息
        /// </summary>
        public string cartype { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string carno { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createtime { get; set; }
    }
}
