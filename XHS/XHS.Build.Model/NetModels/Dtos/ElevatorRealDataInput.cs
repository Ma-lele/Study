using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class ElevatorRealDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string SeCode { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public float? Height { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public float? Weight { get; set; }
        /// <summary>
        /// 报警状态
        /// </summary>
        public int? AlarmState { get; set; }
        /// <summary>
        /// 当前人数
        /// </summary>
        public int? NumOfPeople { get; set; }
        /// <summary>
        /// 设备时间
        /// </summary>
        public string DeviceTime { get; set; }
        /// <summary>
        /// 是否已生成报告 0:没有,1:有
        /// </summary>
        public int? HasReport { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public float? Speed { get; set; }
        /// <summary>
        /// 层数
        /// </summary>
        public float? Floor { get; set; }
        /// <summary>
        /// 司机工号
        /// </summary>
        public string DriverId { get; set; }
        /// <summary>
        /// 司机身份证
        /// </summary>
        public string DriverCardNo { get; set; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        public string DriverName { get; set; }

    }
}
