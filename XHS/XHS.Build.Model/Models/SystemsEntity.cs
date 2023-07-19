using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 系统信息
    /// </summary>
    [SugarTable("T_Systems")]
    public class SystemsEntity : BaseEntity
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 说明备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
