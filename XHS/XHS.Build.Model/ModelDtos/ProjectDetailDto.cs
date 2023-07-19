using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class ProjectDetailDto
    {
        public string sitename { get; set; }
        public string projtype { get; set; }
        public string sitecode { get; set; }
        public string siteajcode { get; set; }
        public int status { get; set; }
        public string regionname { get; set; }
        public string groupshortname { get; set; }
        public string siteaddr { get; set; }
        public float sitelat { get; set; }
        public float sitelng { get; set; }
        public int bcityctrl { get; set; }
        public string sitecost { get; set; }
        public DateTime planstartdate { get; set; }
        public DateTime planenddate { get; set; }
        public string buildingarea { get; set; }
        public string summary { get; set; }
        public string fivepart { get; set; }
        public decimal score { get; set; }
    }
}
