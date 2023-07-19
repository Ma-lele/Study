namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceDto
    {
        /// <summary>
        /// 安监备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 1:实名制 2:视频 3:塔机 4:升降机 5:卸料平台 6:扬尘 7:临边 8:深基坑 9:高支模 10:车辆冲洗 11:安全帽未佩戴 12:裸土 13:烟雾 14:非法车辆
        /// </summary>
        public int deviceType { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }


        /// <summary>
        /// 操作者
        /// </summary>
        public string updater { get; set; }

    }

}
