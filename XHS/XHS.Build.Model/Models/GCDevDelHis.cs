using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_DevDelHis")]
    public class GCDevDelHis
    {
        public int DDHID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public int DEVID { get; set; }
        public string devcode { get; set; }
        /// <summary>
        /// 1.扬尘监测 2.塔吊监控 3.实名制系统 4.远程监控 5.卸料平台 6.施工升降机 7.现场安全隐患 8.人员定位 9.高支模 10.深基坑监测 11.临边防护 
        /// </summary>
        public int devtype { get; set; }
        public string devtypename { get; set; }
        /// <summary>
        /// 0:失败，>=1 成功
        /// </summary>
        public int bsuccess { get; set; } = 1;
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }

    }


    public enum DevTypeName
    {
        扬尘监测 = 1,
        塔吊监控 = 2,
        实名制系统 = 3,
        远程监控 = 4,
        卸料平台 = 5,
        施工升降机 = 6,
        现场安全隐患 = 7,
        人员定位 = 8,
        高支模 = 9,
        深基坑监测 = 10,
        临边防护 = 11
    }
}
