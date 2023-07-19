using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 接口表
    /// </summary>
    [SugarTable("T_SysApis")]
    public class SysApisEntity : BaseEntity
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 接口url
        /// </summary>
        public string ApiUrl { get; set; }
    }
}
