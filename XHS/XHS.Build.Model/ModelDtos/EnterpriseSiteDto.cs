using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class EnterpriseSiteDto
    {
        public string sitename { get; set; }
        public string sitetype { get; set; }
        public int projstatus { get; set; }
        public int eventlevel { get; set; }
        public decimal sitescore { get; set; }
    }
}
