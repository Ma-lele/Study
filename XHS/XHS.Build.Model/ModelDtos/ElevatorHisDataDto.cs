using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class ElevatorHisDataDto
    {
        public int total { get; set; }
        public dynamic rows { get; set; }
        public int page { get; set; }
        public int records { get; set; }
    }
}
