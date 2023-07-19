using System;


namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 随手拍数据上传Dto
    /// </summary>
    public class FreeToShootDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 随手拍唯一编号
        /// </summary>
        public string checkNumber { get; set; }

        /// <summary>
        /// 拍摄时间（2019-07-07 12:24:34）
        /// </summary>
        public DateTime shootTime { get; set; }

        /// <summary>
        /// 拍摄人
        /// </summary>
        public string shootPerson { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phoneNumber { get; set; }

        /// <summary>
        /// 隐患描述内容
        /// </summary>
        public string CheckContent { get; set; }

        /// <summary>
        /// 照片全路径，必须可访问
        /// </summary>
        public string[] urls { get; set; }
    }
}
