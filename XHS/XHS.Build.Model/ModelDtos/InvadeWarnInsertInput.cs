using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class InvadeWarnInsertInput
    {
        [Required(ErrorMessage = "invadecode不能为空")]
        public string invadecode { get; set; }
        [Required(ErrorMessage = "imgurl不能为空")]
        public string imgurl { get; set; }
        [Required(ErrorMessage = "thumburl不能为空")]
        public string thumburl { get; set; }
        [Required(ErrorMessage = "videourl不能为空")]
        public string videourl { get; set; }
        [Required(ErrorMessage = "invadename不能为空")]
        public string invadename { get; set; }
        [Required(ErrorMessage = "createtime不能为空")]
        public DateTime? createtime { get; set; }
    }
}
