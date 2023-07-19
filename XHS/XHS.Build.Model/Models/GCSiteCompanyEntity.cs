using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 实体类
    /// </summary>
    [SugarTable("T_GC_SiteCompany")]
    public class GCSiteCompanyEntity
    {
        /// <summary>
        /// 编辑
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SCID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 五方类型 1：建设单位，2：施工单位，3:监理单位，4：勘察单位，5：设计单位
        /// </summary>
        public int companytype { get; set; }
        /// <summary>
        /// 五方单位名称
        /// </summary>
        public string companyname { get; set; } = "";
        /// <summary>
        /// 社会东一信用代码
        /// </summary>
        public string companycode { get; set; } = "";
        /// <summary>
        /// 联系人
        /// </summary>
        public string companycontact { get; set; } = "";
        /// <summary>
        /// 联系方式
        /// </summary>
        public string companytel { get; set; } = "";
    }
}