
using System;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 视频信息数据Dto
    /// </summary>
    public class FenceAlarmInfoDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 视频地址的唯一id
        /// </summary>
        public string warnNumber { get; set; }

        /// <summary>
        /// 缺失位置
        /// </summary>
        public string defectPosition { get; set; }

        /// <summary>
        /// 缺失预警模块编号（备用）
        /// </summary>
        public string defectWarnNumber { get; set; }

        /// <summary>
        /// 发生时间（2020-01-10 10:00:00）
        /// </summary>
        public DateTime defectDate { get; set; }

        /// <summary>
        /// 发生时间（2020-01-10 10:00:00）
        /// </summary>
        public DateTime recoveryDate { get; set; }
    }
}
