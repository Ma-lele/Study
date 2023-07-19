using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.ModelDtos
{
    public class EmeterWarnDataInput
    {
        [Required(ErrorMessage = "emetercode不能为空")]
        public string emetercode { get; set; }
        [Required(ErrorMessage = "报警类型不能为空")]
        public int type { get; set; }
        [Required(ErrorMessage = "phase不能为空")]
        public string phase { get; set; }
        [Required(ErrorMessage = "preSetVa不能为空")]
        public float preSetVa { get; set; }
        [Required(ErrorMessage = "actionVa不能为空")]
        public float actionVa { get; set; }
        [Required(ErrorMessage = "createtime不能为空")]
        public DateTime? createtime { get; set; }
    }
}
