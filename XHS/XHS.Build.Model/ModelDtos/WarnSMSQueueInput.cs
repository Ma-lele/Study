using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class WarnSMSQueueInput
    {
        public int SITEID { get; set; }
            public ulong WPID { get; set; }
        public string phonenumber { get; set; }
        public string username { get; set; }
        public string sitename { get; set; }
        public string wpcode { get; set; }
        public string warnstatus { get; set; }
        public string uuids { get; set; } = "";
    }
}
