using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;

namespace XHS.Build.Model.ModelDtos
{
    public class SeverityDto:AYSeverity
    {
        public string etname { get; set; }
        public DateTime lastdatetime { get; set; }
        public string etdatatype { get; set; }
        public int etsrtype { get; set; }
    }
}
