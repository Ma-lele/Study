
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 劳务人员奖惩信息Dto
    /// </summary>
    public class RewardPunishmentsDto
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
        /// 统一社会信用代码
        /// </summary>
        public string unifiedSocialCreditcode { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string workerIdNumber { get; set; }

        /// <summary>
        /// 奖惩内容
        /// </summary>
        public string eventContent { get; set; }

        /// <summary>
        /// 奖惩类型（奖励，惩罚）
        /// </summary>
        public string eventType { get; set; }

        /// <summary>
        /// 奖惩日期（2019-04-18）
        /// </summary>
        public string eventDate { get; set; }
    }
}
