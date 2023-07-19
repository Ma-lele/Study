using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class DeepPitDto
    {
        public int DPID { get; set; }
        public int GROUPID { get; set; }
        public string groupshortname { get; set; }
        public int SITEID { get; set; }
        public string siteshortname { get; set; }
        public string dpcode { get; set; }
        public string dpname { get; set; }
        public int bdel { get; set; }
        public DateTime createtime { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        public string dpurl { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }

    }
}
