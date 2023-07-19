using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 公告Bean
    /// </summary>
    public class BnNotice
    {
        public int NOTICEID { get; set; }
        public int GROUPID { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public string content { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public DateTime operatedate { get; set; }
    }
}
