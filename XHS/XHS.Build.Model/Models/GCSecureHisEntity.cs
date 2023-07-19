using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_SecureHis")]
    public class GCSecureHisEntity
    {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int SCHISID { get; set; }
        public int SECURITYID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string USERID { get; set; }
        public float shlng { get; set; }
        public float shlat { get; set; }
        public string remark { get; set; }
        public DateTime createddate { get; set; }

        public string UserName { get; set; }


        /// <summary>
        /// 巡更点名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string scname { get; set; }

        [SugarColumn(IsIgnore =true)]
        public List<FileEntity> Files { get; set; }
    }

    public class SecureHisListOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public int SCHISID { get; set; }
        /// <summary>
        /// 人员id
        /// </summary>
        public string USERID { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 巡更点名称
        /// </summary>
        public string scname { get; set; }
        public float shlng { get; set; }
        public float shlat { get; set; }

        public DateTime createddate { get; set; }
    }

    public class MonthCount
    {
        public  string createddate { get; set; }
        public int count { get; set; }
    }
}
