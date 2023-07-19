using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 监测点施工阶段Bean
    /// </summary>
    public class BnSitePhase
    {
        public int PHASEID { get; set; }
        public int parentid { get; set; }
        public int phaseorder { get; set; }
        public string phasename { get; set; }
    }
}
