using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_Server")]
    public class ServerEntity
    {
        public int SERVERID { get; set; }
        public string domain { get; set; }
        public int netport { get; set; }
        public string servername { get; set; }
        public string basever { get; set; }
        public string discription { get; set; }
        public string androidver { get; set; }
        public string androidurl { get; set; }
        public string iosver { get; set; }
        public string iosurl { get; set; }
        public DateTime updatedate { get; set; }
        public bool welcome { get; set; }


        public int bdeployed { get; set; }
    }
}
