using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 扬尘设备
    /// </summary>
    public class WaterDataInput
    {
        /// <summary>
        /// 设备编号 前8位以内标识生产厂,4位出厂日期,8位自动连号
        /// </summary>
        [Required(ErrorMessage = "Wmetercode不能为空！")]
        public string address { get; set; }

        /// <summary>
        /// 时间 
        /// </summary>
        public string add_time { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public float data_all { get; set; }
    }
}
