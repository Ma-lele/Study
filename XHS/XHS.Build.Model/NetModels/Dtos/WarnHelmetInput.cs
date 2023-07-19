using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class WarnHelmetInput
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Required(ErrorMessage = "projid不能为空！")]
        public string projid { get; set; }
        /// <summary>
        /// 发生位置
        /// </summary>
        [Required(ErrorMessage = "location不能为空！")]
        public string location { get; set; }
        /// <summary>
        /// 发生时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required(ErrorMessage = "createtime不能为空！")]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required(ErrorMessage = "imgurl不能为空！")]
        public string imgurl { get; set; }
        /// <summary>
        /// 小图地址数组
        /// </summary>
        [Required(ErrorMessage = "thumblist不能为空！")]
        public string[] thumblist { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class WarnStrangerInput
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Required(ErrorMessage = "projid不能为空！")]
        public string projid { get; set; }
        /// <summary>
        /// 发生时间	yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required(ErrorMessage = "createtime不能为空！")]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required(ErrorMessage = "imgurl不能为空！")]
        public string imgurl { get; set; }
        /// <summary>
        /// 小图地址数组
        /// </summary>
        [Required(ErrorMessage = "thumblist不能为空！")]
        public string[] thumblist { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class WarnTrespasserInput
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Required(ErrorMessage = "projid不能为空！")]
        public string projid { get; set; }
        /// <summary>
        /// 发生时间	yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required(ErrorMessage = "createtime不能为空！")]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required(ErrorMessage = "imgurl不能为空！")]
        public string imgurl { get; set; }
        /// <summary>
        /// 小图地址数组
        /// </summary>
        [Required(ErrorMessage = "thumblist不能为空！")]
        public string[] thumblist { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WarnFireInput
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Required(ErrorMessage = "projid不能为空！")]
        public string projid { get; set; }
        /// <summary>
        /// 发生时间	yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required(ErrorMessage = "createtime不能为空！")]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required(ErrorMessage = "imgurl不能为空！")]
        public string imgurl { get; set; }
        /// <summary>
        /// 小图地址数组
        /// </summary>
        [Required(ErrorMessage = "thumblist不能为空！")]
        public string[] thumblist { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public float temperature { get; set; } = 300f;
    }

    /// <summary>
    /// 
    /// </summary>
    public class SmokeInput
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Required(ErrorMessage = "projid不能为空！")]
        public string projid { get; set; }
        /// <summary>
        /// 发生时间	yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required(ErrorMessage = "createtime不能为空！")]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required(ErrorMessage = "imgurl不能为空！")]
        public string imgurl { get; set; }
        /// <summary>
        /// 小图地址数组
        /// </summary>
        [Required(ErrorMessage = "thumblist不能为空！")]
        public string[] thumblist { get; set; }
        /// <summary>
        /// 发生位置
        /// </summary>
        public string location { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WarnOverloadInput
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Required(ErrorMessage = "projid不能为空！")]
        public string projid { get; set; }
        /// <summary>
        /// 发生时间	yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required(ErrorMessage = "createtime不能为空！")]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required(ErrorMessage = "imgurl不能为空！")]
        public string imgurl { get; set; }
        /// <summary>
        /// 小图地址数组
        /// </summary>
        [Required(ErrorMessage = "thumblist不能为空！")]
        public string[] thumblist { get; set; }

        /// <summary>
        /// 升降机AI绑定设备编号
        /// </summary>
        [Required]
        public string liftovercode { get; set; }

        /// <summary>
        /// 达到人数
        /// </summary>
        [Required]
        public int numofpeople { get; set; }
    }

    public class WarnReflectiveVestInput
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        [Required(ErrorMessage = "projid不能为空！")]
        public string projid { get; set; }
        /// <summary>
        /// 发生时间	yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required(ErrorMessage = "createtime不能为空！")]
        public DateTime createtime { get; set; }
        /// <summary>
        /// 照片地址
        /// </summary>
        [Required(ErrorMessage = "imgurl不能为空！")]
        public string imgurl { get; set; }
        /// <summary>
        /// 小图地址
        /// </summary>
        [Required(ErrorMessage = "thumburl不能为空！")]
        public string thumburl { get; set; }
    }
}
