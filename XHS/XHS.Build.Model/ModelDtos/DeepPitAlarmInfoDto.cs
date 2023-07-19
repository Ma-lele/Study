
using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 高支模预警数据Dto
    /// </summary>
    public class DeepPitAlarmInfoDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 结构物编号
        /// </summary>
        [Required(ErrorMessage = "结构物编号不能为空！")]
        public string dpCode { get; set; }

        /// <summary>
        /// 设备监测编号 (即deviceId)
        /// </summary>
        [Required(ErrorMessage = "设备编号不能为空！")]
        public string deviceId { get; set; }

        /// <summary>
        /// 点位编号 
        /// </summary>
        public string watchPoint { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime happenTime { get; set; }

        /// <summary>
        /// 报警类型(电量、温度、立杆轴力、水平倾角、立杆倾角、水平位移、模板沉降等)
        /// </summary>
        public int alarmExplain { get; set; }

        /// <summary>
        /// 预警内容
        /// </summary>
        public string alarmContent { get; set; }

        
    }
}
