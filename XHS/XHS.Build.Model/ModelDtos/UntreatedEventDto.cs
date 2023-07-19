using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class UntreatedEventDto
    {
        public int EVENTID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public DateTime createdate { get; set; }
        public string siteshortname { get; set; }
        public string sitename { get; set; }
        public string groupshortname { get; set; }
        public string etname { get; set; }
        public string content { get; set; }
        public string eventlevel { get; set; }
        public string eventcode { get; set; }
        public string status { get; set; }
        public string handler { get; set; }
        public string stypename { get; set; }
    }
}
