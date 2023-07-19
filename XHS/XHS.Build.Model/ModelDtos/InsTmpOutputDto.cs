using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class InsTmpOutputDto
    {
        public int CMID { get; set; }
        public string TmpName { get; set; }
        public int CheckCount { get; set; }
        public DateTime operatedate { get; set; }
        public string @operator { get; set; }
        public string TenantTypeName { get; set; }
        public int TTID { get; set; }
    }
}
