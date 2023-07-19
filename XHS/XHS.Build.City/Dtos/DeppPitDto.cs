
using System;
using System.Collections.Generic;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 深基坑设备看板Dto
    /// </summary>
    public class DeppPitDto
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
        /// 采集时间
        /// </summary>
        public DateTime collectionTime { get; set; }

        /// <summary>
        /// 监测项
        /// </summary>
        public string monitorType { get; set; } = "";

        /// <summary>
        /// 预警阀值
        /// </summary>
        public int warnValue { get; set; } = 0;

        /// <summary>
        /// 报警阀值
        /// </summary>
        public int alarmValue { get; set; } = 0;

        /// <summary>
        /// 监测项
        /// </summary>
        public List<DeppPitData> data { get; set; }
    }

        public class DeppPitData
        {
        /// <summary>
        /// 监测点
        /// </summary>
        public string watchPoint { get; set; } = "";

        /// <summary>
        /// 值
        /// </summary>
        public double watchPointValue { get; set; } = 0;

        /// <summary>
        /// Y值
        /// </summary>
        public double watchPointYValue { get; set; } = 0;
    }
    }
