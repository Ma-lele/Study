
using System;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 检查单数据看板Dto
    /// </summary>
    public class InspectionPointContentDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 巡检点id（唯一标识）
        /// </summary>
        public string inspectionId { get; set; }

        /// <summary>
        /// 巡检记录id
        /// </summary>
        public string inspectionContentId { get; set; }

        /// <summary>
        /// 巡检时间（2019-07-07 12:24:34）
        /// </summary>
        public DateTime inspectionTime { get; set; }

        /// <summary>
        /// 检查人姓名
        /// </summary>
        public string checkPerson { get; set; }

        /// <summary>
        /// 检查人身份证id
        /// </summary>
        public string checkPersonId { get; set; }

        /// <summary>
        /// 巡检描述
        /// </summary>
        public string checkContent { get; set; }

        /// <summary>
        /// 巡检照片
        /// </summary>
        public string[] urls { get; set; }

    }
}
