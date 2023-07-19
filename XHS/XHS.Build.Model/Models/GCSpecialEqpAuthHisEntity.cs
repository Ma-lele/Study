using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SpecialEqpAuthHis")]
    public class GCSpecialEqpAuthHisEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int SEAHID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string secode { get; set; }
        public int ison { get; set; }
        public string imagefile { get; set; }
        public string ID { get; set; }
        public string realname { get; set; }
        public DateTime updatedate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AuthData
    {
        /// <summary>
        /// 设备
        /// </summary>
        [Required]
        public string SeCode { get; set; }
        /// <summary>
        /// 是否上机0：下机，1：上机
        /// </summary>
        [Required]
        public int IsOn { get; set; }
        /// <summary>
        /// 司机刷脸照片
        /// </summary>
        [Required]
        public string Image64 { get; set; }
        /// <summary>
        /// 司机身份证
        /// </summary>
        [Required(ErrorMessage = "司机身份证不能为空！")] 
        public string DriverCardNo { get; set; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        [Required(ErrorMessage = "司机姓名不能为空！")] 
        public string DriverName { get; set; }

        public string Certification { get; set; }

        public string CheckTime { get; set; }
    }
}
