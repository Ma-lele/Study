using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class TenantDto
    {
        public int TEID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public int TTID { get; set; }
        public string doorplate { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string typename { get; set; }
        public string contact { get; set; }
        public string tel { get; set; }
        public int tmpcount { get; set; }
        public int customizecount { get; set; }
        public int status { get; set; }
    }
}
