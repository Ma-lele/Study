
using System;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 移动巡检数据Dto
    /// </summary>
    public class MobileCheckDto
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
        /// 检查人姓名
        /// </summary>
        public string checkPeople { get; set; }

        /// <summary>
        /// 巡检描述
        /// </summary>
        public string checkContent { get; set; }

        /// <summary>
        /// 巡检照片 多张用,隔开
        /// </summary>
        public string picUrls { get; set; }

        /// <summary>
        /// 巡检时间（2019-07-07 12:24:34）
        /// </summary>
        public DateTime checkDate { get; set; }

    }
}
