using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class DpMonitorDto
    {
        public DateTime collectionTime { get; set; }
        public string watchPoint { get; set; }
        public string devicename { get; set; }
        public decimal watchPointValue { get; set; }
    }
}
