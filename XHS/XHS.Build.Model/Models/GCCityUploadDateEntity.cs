using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 推送上传
    /// </summary>
    [SugarTable("T_GC_CityUploadDate")]
    public class GCCityUploadDateEntity
    {
        /// <summary>
        /// 推送地址
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string uploadurl { get; set; }
        /// <summary>
        /// 推送方法
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string post { get; set; }
        /// <summary>
        /// 上传日期
        /// </summary>
        public DateTime? uploadtime { get; set; }
    }
}
