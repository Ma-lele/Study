using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 摄像头Bean
    /// </summary> 
    public class BnCamera
    {
        /// <summary>
        /// 摄像头ID
        /// </summary> 
        public int CAMERAID { get; set; }

        /// <summary>
        /// 摄像头ID
        /// </summary> 
        public int bonline { get; set; }
        /// <summary>
        /// 摄像头名称
        /// </summary> 
        public string cameraname { get; set; }
        /// <summary>
        /// 摄像头编号
        /// </summary> 
        public string cameracode { get; set; }
        /// <summary>
        /// 通道号
        /// </summary> 
        public int channel { get; set; } = 1;
        /// <summary>
        /// 备用参数
        /// </summary> 
        public string cameraparam { get; set; }
        /// <summary>
        /// 摄像头类型
        /// </summary> 
        public int cameratype { get; set; } 
        /// <summary>
        /// 码流类型(0-主码流,1-子码流),未填默认为主码流
        /// </summary> 
        public string streamtype { get; set; } = "0";
        /// <summary>
        /// 协议类型（rtsp-rtsp协议,rtmp-rtmp协议,hLS-hLS协议，默认为rtsp）
        /// </summary> 
        public string protocol { get; set; } = "rtsp";
        /// <summary>
        /// 协议类型( 0-udp，1-tcp),默认为tcp
        /// </summary> 
        public string transmode { get; set; } = "1";
        /// <summary>
        /// 开始时间（回看时用）
        /// </summary> 
        public DateTime? begintime { get; set; }
        /// <summary>
        /// 结束时间（回看时用）
        /// </summary> 
        public DateTime? endtime { get; set; }
        /// <summary>
        /// 录像存储位置（0-中心存储，1-设备存储）,默认设备存储
        /// </summary> 
        public string recordLocation { get; set; }

        /// <summary>
        /// 开始或停止操作(1 开始 0 停止)
        /// </summary> 
        public string action { get; set; }

        /// <summary>
        /// 控制命令(不区分大小写) 说明： LEFT 左转 RIGHT 右转 UP 上转 DOWN 下转 ZOOM_IN 焦距变大 ZOOM_OUT 焦距变小 
        /// LEFT_UP 左上 LEFT_DOWN 左下 RIGHT_UP 右上 RIGHT_DOWN 右下 FOCUS_NEAR 焦点前移 FOCUS_FAR 焦点后移 
        /// IRIS_ENLARGE 光圈扩大 IRIS_REDUCE 光圈缩小 以下命令presetIndex不可为空： GOTO_PRESET到预置点
        /// </summary> 
        public string command { get; set; }

        /// <summary>
        /// 预置点编号(取值范围为1-128)
        /// </summary> 
        public string presetIndex { get; set; }

        /// <summary>
        /// 云台速度(取值范围为1-100，默认40)
        /// </summary> 
        public string speed { get; set; }
    }
}
