
using System;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 视频监控点信息Dto
    /// </summary>
    public class VideoDto
    {
        /// <summary>
        /// 监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 所属机构编号
        /// </summary>
        public string belongedTo { get; set; }

        /// <summary>
        /// 视频地址的唯一id
        /// </summary>
        public string videoId { get; set; }

        /// <summary>
        /// 视频类型（0：扬尘监控，1：塔吊监控，2：其它）
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 安装位置
        /// </summary>
        public string site { get; set; }

        /// <summary>
        /// 视频跳转url
        /// </summary>
        public string url { get; set; }

    }
}
