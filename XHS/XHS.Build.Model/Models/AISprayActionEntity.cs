using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 雾炮喷淋
    /// </summary>
    [SugarTable("T_AI_SprayAction")]
    public class AISprayActionEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public string FAID { get; set; }
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
        /// 喷淋结果,0:关闭,1:开启
        /// </summary>
        public string spraystate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createtime { get; set; }
    }
}
