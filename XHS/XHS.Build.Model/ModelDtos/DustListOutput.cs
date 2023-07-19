using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class DustListOutput
    {
        public int SITEID { get; set; }
        public string siteshortname { get; set; }
        public string devicecode { get; set; }
        public int GROUPID { get; set; }
        public string groupshortname { get; set; }
        public string parentshortname { get; set; }
    }
}
