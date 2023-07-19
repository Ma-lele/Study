using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class BnAttendNote
    {
        public int ATTENDNOTEID { get; set; }
        public int USERID { get; set; }
        public DateTime attenddate { get; set; }
        public DateTime operatedate { get; set; }
        public string operatenote { get; set; }
    }
}
