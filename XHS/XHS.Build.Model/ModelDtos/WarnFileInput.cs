using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class WarnFileInput
    {
       public int SITEID { get; set; }
        public long WPID { get; set; }
        public int bsolved { get; set; }
        public string filename { get; set; }
        public string fileString { get; set; }
        public int filesize { get; set; }
    }
}
