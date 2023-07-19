
using System;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 塔吊基本信息Dto
    /// </summary>
    public class UploadMachineryInfosDto
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
        /// 机械型号
        /// </summary>
        public string machineryModel { get; set; }
        /// <summary>
        /// 机械类型：塔式起重机 = 0,施工升降机 = 1,货运施工升降机 = 2,桥式起重机 = 3,门式起重机 = 4
        /// </summary>
        public int machineryType { get; set; }
        /// <summary>
        /// 设备信息号
        /// </summary>
        public string propertyRightsRecordNo { get; set; }
        /// <summary>
        /// 检测状态 未检测 = 0, 非我所检测 = 4, 检测中 = 5, 检测合格 = 6,检测不合格 = 7,复检中 = 17,复检合格 = 18,复检不合格 = 19
        /// </summary>
        public int machineryCheckState { get; set; }
        /// <summary>
        /// 机械状态：未安装告知 = 0,安装告知审核中 = 1,安装告知审核通过 = 2,安装告知审核不通过 = 3,检测合格 = 6,
        /// 办理使用登记审核中 = 8,办理使用登记未通过 = 9,办理使用登记通过 = 10,拆卸告知审核中 = 11,拆卸告知审核通过 = 12,
        /// 拆卸告知审核不通过 = 13,使用登记注销审核中 = 14, 使用登记注销审核通过 = 15,使用登记注销审核不通过 = 16
        /// </summary>
        public int checkState { get; set; }
        /// <summary>
        /// 检测合格日期
        /// </summary>
        public DateTime? reCheckReviewDate { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string oem { get; set; }
        /// <summary>
        /// 生产编号
        /// </summary>
        public string leaveTheFactoryNo { get; set; }
        /// <summary>
        /// 产权单位-社会统一信用代码
        /// </summary>
        public string propertyEntCode { get; set; }
        /// <summary>
        /// 产权单位-名称
        /// </summary>
        public string propertyEntName { get; set; }
        /// <summary>
        /// 使用单位-社会统一信用代码
        /// </summary>
        public string userEntCode { get; set; }
        /// <summary>
        /// 使用单位-名称
        /// </summary>
        public string userEntName { get; set; }
        /// <summary>
        /// 检测报告文件路径
        /// </summary>
        public string checkUrl { get; set; }
        /// <summary>
        /// 使用登记证编号
        /// </summary>
        public string useRecordNo { get; set; }
        /// <summary>
        /// 使用登记证文件全路径
        /// </summary>
        public string useRecordNoUrl { get; set; }

    }
}
