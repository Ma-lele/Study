using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class WashNetDto
    {
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public string Code { get; set; }
        public bool Success { get; set; }
    }

    /// <summary>
    /// 车辆冲洗
    /// </summary>
    public class CarWashInsertDto
    {
        /// <summary>
        /// 冲洗状态 1:冲洗;2:未冲洗
        /// </summary>
        [Required] 
        public int washresult { get; set; }
        /// <summary>
        /// 车冲编号
        /// </summary>
        [Required]
        public string cwcode { get; set; }
        /// <summary>
        /// 车冲名称
        /// </summary>
        public string cwname { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        [Required] 
        public int channelid { get; set; }
        /// <summary>
        /// 出场时间
        /// </summary>
        public DateTime? outtime { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        [Required]
        public string carno { get; set; }
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public string platecolor { get; set; }
        /// <summary>
        /// 冲洗时长（分钟）
        /// </summary>
        [Required]
        public int cwminute { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        public string imgpath { get; set; }
        /// <summary>
        /// 视频地址
        /// </summary>
        public string videopath { get; set; }
    }
}
