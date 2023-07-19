
using System;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 高支模预警数据Dto
    /// </summary>
    public class HighFormworkAlarmInfoDto
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
        /// 点位编号 
        /// </summary>
        public string pointId { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime happenTime { get; set; }

        /// <summary>
        /// 报警类型(电量、温度、立杆轴力、水平倾角、立杆倾角、水平位移、模板沉降等)
        /// </summary>
        public string warnExplain { get; set; }

        /// <summary>
        /// 预警内容
        /// </summary>
        public string warnContent { get; set; }

        
    }
}
