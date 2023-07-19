using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class SiteScoreRank
    {
        public int SITEID { get; set; }
        public string sitename { get; set; }
        public string groupshortname { get; set; }
        public string sitetype { get; set; }
        public int projstatus { get; set; }
        public int costdays { get; set; }
        public int lv1 { get; set; }
        public int lv2 { get; set; }
        public int lv3 { get; set; }
        public int lv4 { get; set; }
        public decimal sitescore { get; set; }
    }
}
