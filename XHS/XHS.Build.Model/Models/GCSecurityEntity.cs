using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 监测对象巡更点
    /// </summary>
    [SugarTable("T_GC_Security")]
    public class GCSecurityEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int SECURITYID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 巡更点名称
        /// </summary>
        public string scname { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double sclng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double sclat { get; set; }
        /// <summary>
        /// 备注 说明
        /// </summary>
        public string remark { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }

        public int bdel { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GCSiteSecurityPageOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public int SITEID { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 站点简称
        /// </summary>
        public string SiteShortName { get; set; }
        /// <summary>
        /// 站点经度
        /// </summary>
        public float SiteLng { get; set; }
        /// <summary>
        /// 站点纬度
        /// </summary>
        public float SiteLat { get; set; }
        /// <summary>
        /// 今日巡更数
        /// </summary>
        public int Sum { get; set; }

        public DateTime? OperateDate { get; set; }
    }

    public class SecurityListOutput
    {
        public int SECURITYID { get; set; }

        /// <summary>
        /// 巡更点名称
        /// </summary>
        public string SCName { get; set; }

        /// <summary>
        /// 巡更点描述
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 当前巡更点今日打卡次数
        /// </summary>
        public int Sum { get; set; }
    }

    public class SiteSecurityOutput : GCSecurityEntity
    {
        /// <summary>
        /// 站点名称
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 站点简称
        /// </summary>
        public string SiteShortName { get; set; }

        public int Sum { get; set; }
    }
}
