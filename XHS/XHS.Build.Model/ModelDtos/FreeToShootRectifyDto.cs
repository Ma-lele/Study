using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 随手拍完成数据上传
    /// </summary>
    public class FreeToShootRectifyDto
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
        /// 完成时间（2019-07-07 12:24:34）
        /// </summary>
        public DateTime rectifyTime { get; set; }

        /// <summary>
        /// 整改负责人
        /// </summary>
        public string rectifyPerson { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string rectifyRemark { get; set; }

        /// <summary>
        /// 照片全路径，必须可访问
        /// </summary>
        public string[] urls { get; set; }

    }
}
