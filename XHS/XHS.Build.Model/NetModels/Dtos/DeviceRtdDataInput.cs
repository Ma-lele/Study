using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 扬尘设备
    /// </summary>
    public class DeviceRtdDataInput
    {
        /// <summary>
        /// 设备编号 前8位以内标识生产厂,4位出厂日期,8位自动连号
        /// </summary>
        [Required(ErrorMessage = "DeviceCode不能为空！")]
        public string devicecode { get; set; }

        /// <summary>
        /// 纬度 
        /// </summary>
        public float lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public float lng { get; set; }
        /// <summary>
        /// PM10
        /// </summary>
        public float pm10 { get; set; }
        /// <summary>
        /// PM2.5
        /// </summary>
        public float pm2_5 { get; set; }
        /// <summary>
        /// TSP
        /// </summary>
        public float tsp { get; set; }
        /// <summary>
        /// 噪音
        /// </summary>
        public float noise { get; set; }
        /// <summary>
        /// 湿度
        /// </summary>
        public float dampness { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public float temperature { get; set; }
        /// <summary>
        /// 大气压
        /// </summary>
        public float atmos { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public float speed { get; set; }
        /// <summary>
        /// 风向0 -359
        /// </summary>
        public int direction { get; set; }

    }
}
