using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_SiteDoc")]
    public class GCSiteDoc
    {
        public Guid SITEDOCID { get; set; }
        public int SITEID { get; set; }
        public int filetype { get; set; }
        public string filename { get; set; }
        public int filesize { get; set; }
        public string updater { get; set; }
        public DateTime updatedate { get; set; }
    }
}
