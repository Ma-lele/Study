
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 深基坑设备看板Dto
    /// </summary>
    public class DeepPitRtdDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 结构物编号
        /// </summary>
        public string dpCode { get; set; }

        /// <summary>
        /// 设备监测编号 (即deviceId)
        /// </summary>
        [Required(ErrorMessage = "设备编号不能为空！")]
        public string deviceId { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        [Required(ErrorMessage = "采集时间不能为空！")]
        public DateTime collectionTime { get; set; }

        /// <summary>
        /// 监测项
        /// </summary>
        [Required(ErrorMessage = "监测项不能为空！")]
        public int monitorType { get; set; }

        /// <summary>
        /// 预警阀值
        /// </summary>
        public int warnValue { get; set; } = 0;

        /// <summary>
        /// 报警阀值
        /// </summary>
        public int alarmValue { get; set; } = 0;

        /// <summary>
        /// 监测项值
        /// </summary>
        [Required(ErrorMessage = "监测项值不能为空！")]
        public List<DeepPitData> data { get; set; }
    }
    public enum MonitorType
    {
        离线 = 0,
        支撑锚轴力 = 1,
        深层水平位移 = 2,
        地表水平位移 = 3,
        地表竖向位移 = 4,
        建筑物倾斜 = 5,
        建筑物沉降 = 6,
        地表及建筑物裂缝 = 7,
        地下水位 = 8,
        围护墙边坡顶部水平位移 = 9,
        围护墙边坡顶部竖向位移 = 10,
        立柱竖向位移 = 11,
        周边管线及设施竖向位移 = 12,
        坑底隆起回弹 = 13,
    }

    public class DeepPitData
        {
        /// <summary>
        /// 监测点
        /// </summary>
        [Required(ErrorMessage = "监测点不能为空！")]
        public string watchPoint { get; set; } = "";

        /// <summary>
        /// 值
        /// </summary>
        [Required(ErrorMessage = "监测点值不能为空！")]
        public double watchPointValue { get; set; }

        /// <summary>
        /// Y值
        /// </summary>
        public double watchPointExValue { get; set; } = 0;
    }
    }
