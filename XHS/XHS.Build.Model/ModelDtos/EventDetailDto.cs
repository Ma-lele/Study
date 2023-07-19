using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class EventDetailDto
    {
        public int EVENTID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public DateTime createdate { get; set; }
        public string ltypeName { get; set; }
        public string stypeName { get; set; }
        public string devicecode { get; set; }
        public string siteshortname { get; set; }
        public string eventlevel { get; set; }
        public string content { get; set; }
        public string eventcode { get; set; }
        public int status { get; set; }
        public string handler { get; set; }
        public DateTime handledate { get; set; }
        public DateTime updatedate { get; set; }
        public DateTime limitdate { get; set; }
        public string spid { get; set; }
        public string remark { get; set; }

    }
}
