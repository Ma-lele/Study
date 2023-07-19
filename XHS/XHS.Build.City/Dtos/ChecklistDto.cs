
using System;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 检查单信息Dto
    /// </summary>
    public class ChecklistDto
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
        /// 检查单编号
        /// </summary>
        public string checkNumber { get; set; }

        /// <summary>
        /// 是否符合省标准，0:否 1:是
        /// </summary>
        public int IsProvinStand { get; set; }

        /// <summary>
        /// 检查时间（2019-07-07 10:22:22）
        /// </summary>
        public DateTime checkDate { get; set; }

        /// <summary>
        /// 是否需要整改（是，否）
        /// </summary>
        public string isNeedToRectify { get; set; }

        /// <summary>
        /// 建议整改完成时间（2019-07-07 10:22:22）
        /// </summary>
        public DateTime recommendFinishDate { get; set; }

        /// <summary>
        /// 检查备注
        /// </summary>
        public string checkComment { get; set; }

        /// <summary>
        /// 检查人姓名 多人用,隔开
        /// </summary>
        public string checkPeople { get; set; }

    }
}
