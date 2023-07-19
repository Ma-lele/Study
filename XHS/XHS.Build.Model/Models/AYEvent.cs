using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_AY_Event")]
    public class AYEvent
    {
        public int EVENTID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string eventcode { get; set; }
        /// <summary>
        /// 事件等级 1:低 2:中 3:高 4:超高
        /// </summary>
        public int eventlevel { get; set; }
        public string devicecode { get; set; }
        public string linkid { get; set; }
        /// <summary>
        /// 对应eventtype表的大分类
        /// </summary>
        public string ltype { get; set; }
        /// <summary>
        /// 对应eventtype表的小分类
        /// </summary>
        public string stype { get; set; }
        public int warnlevel { get; set; }
        public string content { get; set; }
        public DateTime createdate { get; set; }
        /// <summary>
        /// 状态 1:未处理 2:处理中 3:已处理 4:已关闭
        /// </summary>
        public int status { get; set; }
        public string handler { get; set; }
        public DateTime handledate { get; set; }
        public string spid { get; set; }
        /// <summary>
        /// 监督检查类型 1:隐患整改单 2:局部停工单 3:全面停工单
        /// </summary>
        public int sptype { get; set; }
        public DateTime limitdate { get; set; }
        public string updater { get; set; }
        public DateTime updatedate { get; set; }
        public string remark { get; set; }

    }
}
