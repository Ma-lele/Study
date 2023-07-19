using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Services.AISoilAction.Dtos
{
    public class AISoilProcInputDto
    {
        /// <summary>
        /// 项目id
        /// </summary>
        public string projid { get; set; }
        /// <summary>
        /// 裸土覆盖的百分比数值
        /// </summary>
        public decimal soilrate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? createtime { get; set; }
    }
}
