using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_SpecialEqpDoc")]
    public class GCSpecialEqpDoc
    {
        public Guid SEDOCID { get; set; }
        public int SEID { get; set; }
        public int filetype { get; set; }
        public string filename { get; set; }
        public int filesize { get; set; }
        public string updater { get; set; }
        public DateTime updatedate { get; set; }
    }

    public class SpecialEqpDocInputDto : GCSpecialEqpDoc
    {
        public List<ImgDto> Notify { get; set; }
        public List<ImgDto> Monitor { get; set; }
        public List<ImgDto> Licenses { get; set; }
        public List<ImgDto> Repair { get; set; }
        public List<ImgDto> Dismantle { get; set; }
        public string[] NotifyDel { get; set; }
        public string[] MonitorDel { get; set; }
        public string[] LicensesDel { get; set; }
        public string[] RepairDel { get; set; }
        public string[] DismantleDel { get; set; }
    }

    public class SpecialEqpDocOutputDto
    {
        public string name { get; set; }
        public string url { get; set; }
        public int filetype { get; set; }
    }
}
