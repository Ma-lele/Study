using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{

    public class HfwSelectDto
    {
        public int HFWID { get; set; }
        public string hfwname { get; set; }
        public List<HfwSelectArea> areas { get; set; }
    }

    public class HfwSelectArea
    {
        public int HFWAID { get; set; }
        public string hfwaname { get; set; }
        public List<string> spotcode { get; set; }
    }


    public class HfwSelectData
    {
        public int HFWID { get; set; }
        public string hfwname { get; set; }
        public int HFWAID { get; set; }
        public string hfwaname { get; set; }
        public string spotcode { get; set; }
    }
}
