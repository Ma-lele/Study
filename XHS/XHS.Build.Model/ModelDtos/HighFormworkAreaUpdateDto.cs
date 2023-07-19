using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class HighFormworkAreaUpdateDto
    {
        public int HFWAID { get; set; }
        public int GROUPID { get; set; }
        public int SITEID { get; set; }
        public int HFWID { get; set; }
        public string hfwaname { get; set; }
        public int bactive { get; set; }
        public int bdel { get; set; }
        public DateTime createdate { get; set; }
        public string @operator { get; set; }
        public DateTime operatedate { get; set; }
        public string FileID { get; set; }
    }
}
