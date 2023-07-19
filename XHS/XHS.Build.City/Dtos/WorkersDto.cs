
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 劳务人员基本信息Dto
    /// </summary>
    public class WorkersDto
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
        /// 人员姓名
        /// </summary>
        public string workerName { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string idNumber { get; set; }
        /// <summary>
        /// 工种类型
        /// </summary>
        public int workType { get; set; }
        /// <summary>
        /// 安管职位 (项目经理、执行经理、安全人员)
        /// </summary>
        public string securityJob { get; set; }
        /// <summary>
        /// 是否离职（0否，1是）
        /// </summary>
        public int isResign { get; set; }
        /// <summary>
        /// 进场时间（2019-04-18）
        /// </summary>
        public string entryDate { get; set; }
        /// <summary>
        /// 离职时间（2019-05-05）
        /// </summary>
        public string exitDate { get; set; }

    }
}
