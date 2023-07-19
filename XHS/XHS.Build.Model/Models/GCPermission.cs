using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_Permission")]
    public class GCPermission
    {
        public int ID { get; set; }
        public string MenuID { get; set; }
        public string ApiUri { get; set; }
    }
}
