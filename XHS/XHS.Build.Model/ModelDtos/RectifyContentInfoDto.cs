
using System;
using System.Collections.Generic;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 检查单信息Dto
    /// </summary>
    public class RectifyContentInfoDto
    {
        /// <summary>
        /// 监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 检查单编号
        /// </summary>
        public string checkNumber { get; set; }

        /// <summary>
        /// 整改完成时间
        /// </summary>
        public DateTime finalRectifyDate { get; set; }

        /// <summary>
        /// 整改审批人
        /// </summary>
        public string rectifyApprover { get; set; }

        /// <summary>
        /// 整改内容
        /// </summary>
        public List<rectifyContentsData> rectifyContents { get; set; }

    }

    public class rectifyContentsData
    {
        /// <summary>
        /// 检查项唯一id
        /// </summary>
        public string itemId { get; set; }

        /// <summary>
        /// 整改备注
        /// </summary>
        public string rectifyRemark { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string[] urls { get; set; }
    }
}
