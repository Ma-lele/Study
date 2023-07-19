
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 劳务单位基本信息Dto
    /// </summary>
    public class CompaniesDto
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
        /// 企业名称
        /// </summary>
        public string companyName { get; set; }

    }
}
