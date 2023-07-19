
using System;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 扬尘日监测数据Dto
    /// </summary>
    public class DayDustInfoDto
    {
        /// <summary>
        /// 监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 所属机构编号
        /// </summary>
        public string belongedTo { get; set; }

        /// <summary>
        /// 设备唯一id
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public double temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>
        public double humidity { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public double windSpeed { get; set; }
        /// <summary>
        /// 风向
        /// </summary>
        public string windDirection { get; set; }
        /// <summary>
        /// pm2.5
        /// </summary>

        public double pm2dot5 { get; set; }
        /// <summary>
        /// pm10
        /// </summary>
        public double pm10 { get; set; }
        /// <summary>
        /// 噪音
        /// </summary>
        public double noise { get; set; }

        /// <summary>
        /// 监测日期，例如：2019-01-24
        /// </summary>
        public string moniterTime { get; set; }
    }
}
