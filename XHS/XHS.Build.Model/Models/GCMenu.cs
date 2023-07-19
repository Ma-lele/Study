using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_Menu")]
    public class GCMenu
    {
        public string MENUID { get; set; }
        public string name { get; set; }
        public string pageurl { get; set; }
        public string posturl { get; set; }
        public string cssclass { get; set; }
        public int bmenu { get; set; }
        public string sort { get; set; }
    }
}
