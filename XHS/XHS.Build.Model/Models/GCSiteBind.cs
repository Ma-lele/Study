using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 监测对象简介实体类
    /// </summary>
    [SugarTable("T_GC_Site")]
    public class GCSiteBindEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SITEID { get; set; }
        /// <summary>
        /// 车辆冲洗编号
        /// </summary>
        public string parkkey { get; set; }
        /// <summary>
        /// 对接考勤系统的项目编号
        /// </summary>
        public string attendprojid { get; set; }
        /// <summary>
        /// 1：群耀  2：大运  3：比利时 4：2.0 5:新合盛
        /// </summary>
        public short attendprojtype { get; set; } = 0;
        /// <summary>
        /// 对接安全帽识别的项目编号
        /// </summary>
        public string helmetprojid { get; set; } = "";
        /// <summary>
        /// 对接人车分流的项目编号
        /// </summary>
        public string trespasserprojid { get; set; } = "";
        /// <summary>
        /// 对接陌生人的项目编号
        /// </summary>
        public string strangerprojid { get; set; } = "";
        /// <summary>
        /// 对接火警识别的项目编号
        /// </summary>
        public string fireprojid { get; set; } = "";
        /// <summary>
        /// 对接升降机人数识别的项目编号
        /// </summary>
        public string liftoverprojid { get; set; } = "";
        /// <summary>
        /// 晨会交底对接项目编号
        /// </summary>
        public string amdiscloseprojid { get; set; } = "";
        
        /// <summary>
        /// 深圳安全帽对接账户（用户::密码）
        /// </summary>
        public string szhelmetnamepwd { get; set; }
        /// <summary>
        /// 对接信息更新时间
        /// </summary>
        public DateTime? apiupdatedate { get; set; }
        /// <summary>
        /// 密闭运输绑定的工地ID
        /// </summary>
        public string airtightprojid { get; set; } = "";
        /// <summary>
        /// 围挡喷淋绑定的工地ID
        /// </summary>
        public string sprayprojid { get; set; } = "";
        /// <summary>
        /// 反光衣绑定的工地ID
        /// </summary>
        public string vestprojid { get; set; } = "";
        /// <summary>
        /// 裸土覆盖绑定的工地ID
        /// </summary>
        public string soilprojid { get; set; } = "";
    }
}