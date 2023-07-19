using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class ProjRiskDto
    {
        public string siteshortname { get; set; }
        public string sitename { get; set; }
        public string groupshortname { get; set; }
        public string constructtype { get; set; }
        public int projstatus { get; set; }
        public float sitelat { get; set; }
        public float sitelng { get; set; }
        public int lv1 { get; set; }
        public int lv2 { get; set; }
        public int lv3 { get; set; }
        public int lv4 { get; set; }
    }
}
