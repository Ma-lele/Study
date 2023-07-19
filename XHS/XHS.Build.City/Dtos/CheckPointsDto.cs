
using System;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 移动巡检点信息Dto
    /// </summary>
    public class CheckPointsDto
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
        /// 巡检点id（唯一标识）
        /// </summary>
        public string checkPointId { get; set; }

        /// <summary>
        /// 巡检地点描述
        /// </summary>
        public string summary { get; set; }

        /// <summary>
        /// 楼栋号
        /// </summary>
        public string building { get; set; }

        /// <summary>
        /// 楼层号 默认0
        /// </summary>
        public int floor { get; set; } = 0;

    }
}
