
using System;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 高支模实时数据Dto
    /// </summary>
    public class HighFormworkDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 设备监测编号 (即deviceId)
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// 收集时间
        /// </summary>
        public DateTime collectionTime { get; set; }

        /// <summary>
        /// 电量(%)
        /// </summary>
        public double power { get; set; } = 0;

        /// <summary>
        /// 温度（℃）
        /// </summary>
        public double temperature { get; set; } = 0;

        /// <summary>
        /// 立杆轴力(KN)
        /// </summary>
        public double load { get; set; } = 0;

        /// <summary>
        /// 水平倾角（°）
        /// </summary>
        public double horizontalAngle { get; set; } = 0;

        /// <summary>
        /// 立杆倾角（°）
        /// </summary>
        public double coordinate { get; set; } = 0;

        /// <summary>
        /// 水平位移（mm）
        /// </summary>
        public double translation { get; set; } = 0;

        /// <summary>
        /// 模板沉降（mm）
        /// </summary>
        public double settlement { get; set; } = 0;
    }
}
