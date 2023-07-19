
using System;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 设备基本信息Dto
    /// </summary>
    public class DeviceInfoDto
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
        /// 设备唯一id
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }

    }
}
