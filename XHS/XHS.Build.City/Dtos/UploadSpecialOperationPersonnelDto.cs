
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 司机信息Dto
    /// </summary>
    public class UploadSpecialOperationPersonnelDto
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
        /// 设备信息号
        /// </summary>
        public string propertyRightsRecordNo { get; set; }
        /// <summary>
        /// 人员姓名
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 性别 0：男，1:女
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 传固定值2（代表使用人员）
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 详见工种字典
        /// </summary>
        public int workTypeCode { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string idCard { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

    }
}
