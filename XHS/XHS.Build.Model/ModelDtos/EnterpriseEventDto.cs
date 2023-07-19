using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class EnterpriseEventDto
    {
        public int EVENTID { get; set; }
        public DateTime createdate { get; set; }
        public string sitename { get; set; }
        public string groupshortname { get; set; }
        public string etname { get; set; }
        public string eventcode { get; set; }
        public int eventlevel { get; set; } 
    }
}
