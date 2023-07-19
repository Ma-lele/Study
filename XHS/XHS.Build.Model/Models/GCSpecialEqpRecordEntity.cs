using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SpecialEqpRecord")]
    public class GCSpecialEqpRecordEntity
    {
        /// <summary>
        /// 特种设备备案连番
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SERID { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象ID
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 特种设备类型 1：塔吊 2：升降机
        /// </summary>
        public short setype { get; set; }
        /// <summary>
        /// 特种设备编号
        /// </summary>
        public int SEID { get; set; }
        /// <summary>
        /// 备案号
        /// </summary>
        public string recordno { get; set; }
        /// <summary>
        /// 产权备案号
        /// </summary>
        public string rightno { get; set; }
        /// <summary>
        /// 产权设备单位
        /// </summary>
        public string rightcompany { get; set; }
        /// <summary>
        /// 安装单位
        /// </summary>
        public string installer { get; set; }
        /// <summary>
        /// 告知日期
        /// </summary>
        public DateTime? notifydate { get; set; }
        /// <summary>
        /// 安装日期
        /// </summary>
        public DateTime installdate { get; set; }
        /// <summary>
        /// 检测合格情况
        /// </summary>
        public string checkinfo { get; set; }
        /// <summary>
        /// 检测合格日期
        /// </summary>
        public DateTime? checkdate { get; set; }
        /// <summary>
        /// 使用登记情况
        /// </summary>
        public string useinfo { get; set; }
        /// <summary>
        /// 使用登记时间
        /// </summary>
        public DateTime? usedate { get; set; }
        /// <summary>
        /// 否产权注销
        /// </summary>
        public short brightdel { get; set; } = 0;
        /// <summary>
        /// 更新者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? operatedate { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public string uninstaller { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? uninstalldate { get; set; }
        /// <summary>
        /// 机械型号
        /// </summary>
        public string machinerymodel { get; set; }
        /// <summary>
        /// 机械类型：塔式起重机 = 0,施工升降机 = 1,货运施工升降机 = 2,桥式起重机 = 3,门式起重机 = 4
        /// </summary>
        public int machinerytype { get; set; }
        /// <summary>
        /// 检测状态 未检测 = 0, 非我所检测 = 4, 检测中 = 5, 检测合格 = 6,检测不合格 = 7,复检中 = 17,复检合格 = 18,复检不合格 = 19
        /// </summary>
        public int machinerycheckstate { get; set; }
        /// <summary>
        /// 机械状态：未安装告知 = 0,安装告知审核中 = 1,安装告知审核通过 = 2,安装告知审核不通过 = 3,检测合格 = 6,办理使用登记审核中 = 8,办理使用登记未通过 = 9,办理使用登记通过 
        /// </summary>
        public int checkstate { get; set; }
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string oem { get; set; }
        /// <summary>
        /// 生产编号
        /// </summary>
        public string leavethefactoryno { get; set; }
        /// <summary>
        /// 产权单位-社会统一信用代码
        /// </summary>
        public string rightcompanyid { get; set; }
        /// <summary>
        /// 使用单位-社会统一信用代码
        /// </summary>
        public string userentcode { get; set; }
        /// <summary>
        /// 使用单位-名称
        /// </summary>
        public string userentname { get; set; }
        /// <summary>
        /// 检测报告文件路径
        /// </summary>
        public string checkurl { get; set; }
        /// <summary>
        /// 使用登记证文件全路径
        /// </summary>
        public string userecordnourl { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SpecialEqpRecordListOutput : GCSpecialEqpRecordEntity
    {
        /// <summary>
        /// 监测对象简称
        /// </summary>
        public string siteshortname { get; set; }
        /// <summary>
        /// 特种设备名称
        /// </summary>
        public string sename { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class SpecialEqpRecordInputDto : GCSpecialEqpRecordEntity
    {
        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileInput> fileList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SpecialEqpRecordUpdateDto : SpecialEqpRecordInputDto
    {
        /// <summary>
        /// 删除文件列表
        /// </summary>
        public List<string> fileDelList { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FileInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string id { get; set; }
    }
}
