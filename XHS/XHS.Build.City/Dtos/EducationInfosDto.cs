
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 劳务人员安全教育信息Dto
    /// </summary>
    public class EducationInfosDto
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
        /// 教育内容
        /// </summary>
        public string educationContent { get; set; }

        /// <summary>
        /// 教育类型(安全教育,入场教育,退场教育,技能培训,班前教育,VR安全教育,其它)
        /// </summary>
        public string educationType { get; set; }

        /// <summary>
        /// 教育地点
        /// </summary>
        public string educationLocation { get; set; }
        
        /// <summary>
        /// 教育日期（2019-04-18）
        /// </summary>
        public string educationDate { get; set; }
       
        /// <summary>
        /// 教育时长(分钟)
        /// </summary>
        public int educationDuration { get; set; }

    }
}
