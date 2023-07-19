using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.NetModels.Dtos
{
    public class DeepPitInput
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 结构物编号
        /// </summary>
        [Required(ErrorMessage = "结构物编号不能为空！")]
        public string dpCode { get; set; }
        /// <summary>
        /// 结构物名
        /// </summary>
        [Required(ErrorMessage = "结构物名称不能为空！")]
        public string dpName { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double longitude { get; set; }
        /// <summary>
        /// 结构物图片url
        /// </summary>
        [Required(ErrorMessage = "结构物图片url不能为空！")]
        public string dpPicUrl { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        [Required(ErrorMessage = "结构物至少绑定一个设备！")]
        public List<DeviceData> devices { get; set; }

    }

    public class DeviceData
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "设备编号不能为空！")]
        public string deviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Required(ErrorMessage = "设备名称不能为空！")]
        public string deviceName { get; set; }

    }
}
