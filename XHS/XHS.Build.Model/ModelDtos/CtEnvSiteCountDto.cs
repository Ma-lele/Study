using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 监测工地数实体
    /// </summary>
    public class CtEnvSiteCountDto
    {
        /// <summary>
        /// 扬尘工地数
        /// </summary>
        public int devicecount { get; set; }
        /// <summary>
        /// 车辆冲洗工地数
        /// </summary>
        public int washcount { get; set; }
        /// <summary>
        /// 密闭运输工地数
        /// </summary>
        public int airtightcount { get; set; }
        /// <summary>
        /// 裸土覆盖工地数
        /// </summary>
        public int soiltightcount { get; set; }
        /// <summary>
        /// 雾炮联动工地数
        /// </summary>
        public int fogcount { get; set; }
        /// <summary>
        /// 总工地数
        /// </summary>
        public int totalcount { get; set; }
    }
}
