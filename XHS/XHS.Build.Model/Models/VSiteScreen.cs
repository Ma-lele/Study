using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 大屏视图
    /// </summary>
    public class VSiteScreen
    {
        public int SCREENID { get; set; }
        public int GROUPID { get; set; }
        public string groupname { get; set; }
        public string groupshortname { get; set; }
        public int SITEID { get; set; }
        public string sitename { get; set; }
        public string siteshortname { get; set; }
        public string phasename { get; set; }
        public string screencode { get; set; }
        public string screenname { get; set; }
        public string defaultnotice { get; set; }
        public string jsonparam { get; set; }
        public DateTime checkintime { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        public string groupnotice { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Screen")]
    public class GCScreenEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SCREENID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string screencode { get; set; }
        public string screenname { get; set; }
        public string defaultnotice { get; set; }
        public DateTime checkintime { get; set; }
        public string jsonparam { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
    }
}
