using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class BnSiteFly
    {
        
        public int FLYID { get; set; }
        
        public int SITEID { get; set; }
        
        public string sitename { get; set; }
        
        public DateTime flydate { get; set; }
        
        public short bdel { get; set; }
        
        public DateTime operatedate { get; set; }
    }
}
