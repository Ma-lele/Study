using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 特种设备工作循环
    /// </summary>
    [SugarTable("T_GC_SpecialEqpWorkData")]
    public class GCSpecialEqpWorkDataEntity
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SEWDID { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string secode { get; set; }
        /// <summary>
        /// 1:塔吊,2:升降机
        /// </summary>
        public int setype { get; set; } = 1;
        /// <summary>
        /// 司机身份证
        /// </summary>
        [Required(ErrorMessage = "司机身份证不能为空！")] 
        public string ID { get; set; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        [Required(ErrorMessage = "司机姓名不能为空！")] 
        public string DriverName { get; set; }
        /// <summary>
        /// 循环json数据
        /// </summary>
        public string workdata { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime starttime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endtime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updatedate { get; set; } = DateTime.Now;
    }
}
