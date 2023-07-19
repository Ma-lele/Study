using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_DeviceBind")]
    public class DeviceBindEntity
    {
        /// <summary>
        /// 域名
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Domain { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string DeviceType { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn]
        public DateTime UpdateDate { get; set; }
    }

    public class DeviceBindOutput
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string DeviceCode { get; set; }

        public DateTime UpdateDate { get; set; }
        public string Netport { get; set; }
    }
}
