using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class BnAttend
    {
        public ulong ATTENDID { get; set; }
        public int USERID { get; set; }
        public DateTime attenddate { get; set; }
        public string address { get; set; }
    }
}
