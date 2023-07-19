using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_EmployeeSite")]
    public class GCEmployeeSiteEntity
    {
        /// <summary>
        /// 身份证id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        public string attendprojid { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string socialcreditcode { get; set; }
        /// <summary>
        /// 班组名称
        /// </summary>
        public string shiftname { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>
        public string workertype { get; set; }
        /// <summary>
        /// 管理人员岗位类别
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 工种类别
        /// </summary>
        public string jobtype { get; set; }
        /// <summary>
        /// 工种名称
        /// </summary>
        public string jobname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? startdate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? enddate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? updatedate { get; set; }
    }
}
