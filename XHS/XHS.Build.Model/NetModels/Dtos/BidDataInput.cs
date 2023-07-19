using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class BidDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "安监备案号不能为空！")] 
        public string recordNumber { get; set; }
        /// <summary>
        /// 安监机构编号
        /// </summary>
        public string belongto { get; set; }
        /// <summary>
        /// 大项目名称
        /// </summary>
        public string projectname { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public string projectcode { get; set; }

        /// <summary>
        /// 单位类型
        /// </summary>
        public string companytype { get; set; }

        /// <summary>
        /// 招标标段名称
        /// </summary>
        public string bidprojectname { get; set; }

        /// <summary>
        /// 中标单位
        /// </summary>
        public string bidcompanyname { get; set; }

        /// <summary>
        /// 中标负责人
        /// </summary>
        public string bidperson { get; set; }

        /// <summary>
        /// 安监申报标段名称
        /// </summary>
        public string ajprojectname { get; set; }

        /// <summary>
        /// 安监状态
        /// </summary>
        public string ajstatus { get; set; }

        /// <summary>
        /// 施工许可单位名称
        /// </summary>
        public string buildcompanyname { get; set; }

        /// <summary>
        /// 施工许可项目负责人
        /// </summary>
        public string buildperson { get; set; }

        /// <summary>
        /// 与中标信息比对结果
        /// </summary>
        public int buildresult { get; set; }

        /// <summary>
        /// 与中标信息比对核查情况
        /// </summary>
        public string buildcheck { get; set; }

        /// <summary>
        /// 安监备案单位名称
        /// </summary>
        public string ajcompanyname { get; set; }

        /// <summary>
        /// 安监备案项目负责人
        /// </summary>
        public string ajperson { get; set; }

        /// <summary>
        /// 与施工许可比对结果
        /// </summary>
        public int ajresult { get; set; }

        /// <summary>
        /// 与施工许可比对核查情况
        /// </summary>
        public string ajcheck { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
    }
}
