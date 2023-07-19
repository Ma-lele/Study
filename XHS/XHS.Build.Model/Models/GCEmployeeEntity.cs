using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Employee")]
    public class GCEmployeeEntity
    {
        /// <summary>
        /// 身份证号码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BirthDay { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Ethnic { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string IdstartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string IdendDate { get; set; }
        /// <summary>
        /// 发证机关
        /// </summary>
        public string Publisher { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string County { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string Image { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string jsonall { get; set; }
        public string @Operator { get; set; }
        public DateTime? OperateDate { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string FileImage { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string[] ImageUrls { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string ImageTmpUrls { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeAddEditInput
    {
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BirthDay { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Ethnic { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string IdstartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string IdendDate { get; set; }
        /// <summary>
        /// 发证机关
        /// </summary>
        public string Publisher { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string County { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Image { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Employee")]
    public class EmployeeListOutput
    {
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        private string _birthday;
        public string BirthDay
        {
            get
            {
                if (!string.IsNullOrEmpty(_birthday))
                {
                    return _birthday.Split(' ')[0];
                }
                else
                {
                    return _birthday;
                }

            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _birthday = value.Split(' ')[0];
                }
                else
                {
                    _birthday = value;
                }

            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Ethnic { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        private string _IdstartDate;
        public string IdstartDate
        {
            get
            {
                if (!string.IsNullOrEmpty(_IdstartDate))
                {
                    return _IdstartDate.Split(' ')[0];
                }
                else
                {
                    return _IdstartDate;
                }

            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _IdstartDate = value.Split(' ')[0];
                }
                else
                {
                    _IdstartDate = value;
                }

            }
        }
        /// <summary>
        /// 结束日期
        /// </summary>
        private string _IdendDate;
        public string IdendDate
        {
            get
            {
                if (!string.IsNullOrEmpty(_IdendDate))
                {
                    return _IdendDate.Split(' ')[0];
                }
                else
                {
                    return _IdendDate;
                }

            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _IdendDate = value.Split(' ')[0];
                }
                else
                {
                    _IdendDate = value;
                }

            }
        }
        /// <summary>
        /// 发证机关
        /// </summary>
        public string Publisher { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string County { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string Image { get; set; }
    }
}
