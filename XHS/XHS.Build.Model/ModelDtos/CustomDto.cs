using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class CustomDto
    {
        public int TEID { get; set; }
        public List<InspectionItems> conditions { get; set; }
    }
}
