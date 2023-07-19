using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class OzoneDto
    {

        public int OZID { get; set; }

        public int GROUPID { get; set; }
        public string groupshortname { get; set; }
        public int SITEID { get; set; }
        public string siteshortname { get; set; }
        public string ozcode { get; set; }

        public string ozname { get; set; }

        public decimal o3 { get; set; }

        public int bdel { get; set; }

        public DateTime createtime { get; set; }

        public string @operator { get; set; }

        public DateTime operatedate { get; set; }
    }
}
