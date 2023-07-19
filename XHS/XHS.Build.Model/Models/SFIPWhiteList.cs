using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// IP白名单
    /// </summary>
    [SugarTable("T_SF_IPWhiteList")]
    public class SFIPWhiteList
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 起始IP
        /// </summary>
        public string BeginIP { get; set; }
        /// <summary>
        /// 结束IP
        /// </summary>
        public string EndIP { get; set; }
        /// <summary>
        /// IP类型
        /// </summary>
        public string iptype { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updatedate { get; set; }
    }
}
