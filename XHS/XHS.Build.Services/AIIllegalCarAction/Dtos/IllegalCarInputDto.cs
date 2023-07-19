using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Services.AIIllegalCarAction.Dtos
{
    public class IllegalCarInputDto
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
        /// 视频地址
        /// </summary>
        [Required]
        public string videourl { get; set; }
        /// <summary>
        /// 识别后带标签图
        /// </summary>
        [Required]
        public string imgurlmarked { get; set; }
        /// <summary>
        /// 密闭状态.0:未密闭，1:密闭
        /// </summary>
        [Required]
        public int status { get; set; }
        /// <summary>
        /// 车辆信息
        /// </summary>
        [Required]
        public string cartype { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [Required]
        public string carno { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime? createtime { get; set; }
    }
}
