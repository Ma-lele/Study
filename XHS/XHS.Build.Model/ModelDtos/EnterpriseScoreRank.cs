using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class EnterpriseScoreRank
    {
        public string companycode { get; set; }
        public string companyname { get; set; }
        public int activecount { get; set; }
        public int sitecount { get; set; }
        public decimal sitescore { get; set; }
    }
}
