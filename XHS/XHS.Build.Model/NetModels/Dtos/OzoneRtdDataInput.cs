using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 臭氧
    /// </summary>
    public class OzoneRtdDataInput 
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "ozcode不能为空！")]
        public string ozcode { get; set; }
        /// <summary>
        /// 臭氧值
        /// </summary>
        public double o3 { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime collectionTime { get; set; }
    }
}
