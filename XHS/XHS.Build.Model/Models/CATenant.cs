using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_CA_Tenant")]
    public class CATenant
    {
        public int TEID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string tel { get; set; }
        public string pwd { get; set; }
        public string doorplate { get; set; }
        public int status { get; set; }
        public int TTID { get; set; }
        public int CMID { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        public string username { get; set; }
    }
}
