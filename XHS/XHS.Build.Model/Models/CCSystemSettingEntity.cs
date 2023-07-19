using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_CC_SystemSetting")]
    public class CCSystemSettingEntity
    {
        [SugarColumn(IsPrimaryKey =true)]
        public string SETTINGID { get; set; }
        public string itemname { get; set; }
        public string type { get; set; } = "text";
        public string value { get; set; }
        public string optionvalue { get; set; }
    }
}
