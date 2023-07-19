using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Services.AISprayAction.Dtos
{
    public class AISparyInputDto
    {
        /// <summary>
        /// 项目id
        /// </summary>
        [Required]
        public string projid { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required]
        public string imgurl { get; set; }
        /// <summary>
        /// 照片缩略图地址
        /// </summary>
        [Required]
        public string thumburl { get; set; }
        /// <summary>
        /// 识别后带标签图
        /// </summary>
        [Required]
        public string imgurlmarked { get; set; }
        /// <summary>
        /// 喷淋结果,0:关闭,1:开启
        /// </summary>
        [Required]
        public int spraystate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public DateTime? createtime { get; set; }
    }
}
