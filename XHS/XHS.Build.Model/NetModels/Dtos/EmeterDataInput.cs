using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 智能电表设备
    /// </summary>
    public class EmeterDataInput
    {
        /// <summary>
        /// 设备编号 前8位以内标识生产厂,4位出厂日期,8位自动连号
        /// </summary>
        [Required(ErrorMessage = "emetercode不能为空！")]
        public string emetercode { get; set; }

        /// <summary>
        /// 时间 
        /// </summary>
        public string electime { get; set; }

        /// <summary>
        /// 总电能
        /// </summary>
        public float elecdata { get; set; }


        /// <summary>
        /// 消音状态
        /// </summary>
        public int muteStatus { get; set; }

        /// <summary>
        /// 继电器输出状态
        /// </summary>
        public int relayOutputStatus { get; set; }

        /// <summary>
        /// 缺相状态
        /// </summary>
        public int phaseLossStatus { get; set; }

        /// <summary>
        /// 断电状态
        /// </summary>
        public int powerOffStatus { get; set; }

        /// <summary>
        /// 漏电状态
        /// </summary>
        public int electricityLeakageStatus { get; set; }

        /// <summary>
        /// A相温度状态
        /// </summary>
        public int aPhaseTemperatureStatus { get; set; }

        /// <summary>
        /// B相温度状态
        /// </summary>
        public int bPhaseTemperatureStatus { get; set; }

        /// <summary>
        /// C相温度状态
        /// </summary>
        public int cPhaseTemperatureStatus { get; set; }

        /// <summary>
        /// N相温度状态
        /// </summary>
        public int nPhaseTemperatureStatus { get; set; }

        /// <summary>
        /// 过压状态
        /// </summary>
        public int overVoltageStatus { get; set; }

        /// <summary>
        /// 欠压状态
        /// </summary>
        public int underVoltageStatus { get; set; }

        /// <summary>
        /// A相过载状态
        /// </summary>
        public int aPhaseOverloadStatus { get; set; }

        /// <summary>
        /// B相过载状态
        /// </summary>
        public int bPhaseOverloadStatus { get; set; }

        /// <summary>
        /// C相过载状态
        /// </summary>
        public int cPhaseOverloadStatus { get; set; }

        /// <summary>
        /// A相电压
        /// </summary>
        public float aPhaseVoltage { get; set; }

        /// <summary>
        /// B相电压
        /// </summary>
        public float bPhaseVoltage { get; set; }

        /// <summary>
        /// C相电压
        /// </summary>
        public float cPhaseVoltage { get; set; }

        /// <summary>
        /// A相频率
        /// </summary>
        public float aPhaseFrequence { get; set; }

        /// <summary>
        /// B相频率
        /// </summary>
        public float bPhaseFrequence { get; set; }
        /// <summary>
        /// C相频率
        /// </summary>
        public float cPhaseFrequence { get; set; }
        /// <summary>
        /// A相电流
        /// </summary>
        public float aPhaseCurrent { get; set; }
        /// <summary>
        /// B相电流
        /// </summary>
        public float bPhaseCurrent { get; set; }
        /// <summary>
        /// C相电流
        /// </summary>
        public float cPhaseCurrent { get; set; }
        /// <summary>
        /// 漏电流
        /// </summary>
        public float leakCurrent { get; set; }
        /// <summary>
        /// A相温度
        /// </summary>
        public float aPhaseTemperature { get; set; }
        /// <summary>
        /// B相温度
        /// </summary>
        public float bPhaseTemperature { get; set; }
        /// <summary>
        /// C相温度
        /// </summary>
        public float cPhaseTemperature { get; set; }
        /// <summary>
        /// N相温度
        /// </summary>
        public float nPhaseTemperature { get; set; }
        /// <summary>
        /// A相电能
        /// </summary>
        public float aPhaseElectricalEnergy { get; set; }
        /// <summary>
        /// B相电能
        /// </summary>
        public float bPhaseElectricalEnergy { get; set; }
        /// <summary>
        /// C相电能
        /// </summary>
        public float cPhaseElectricalEnergy { get; set; }
    }
}
