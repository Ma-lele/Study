using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Device")]
    public class GCDeviceEntity
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int DEVICEID { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public int GROUPID { get; set; }
        /// <summary>
        /// 监测对象编号
        /// </summary>
        public int SITEID { get; set; } = 0;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string devicecode { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short bprime { get; set; } = 1;
        /// <summary>
        /// 自带摄像头编号
        /// </summary>
        public string cameracode { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        public string deviceip { get; set; }
        /// <summary>
        /// 设备端口
        /// </summary>
        public int deviceport { get; set; }
        /// <summary>
        /// 雾炮连动阀值
        /// </summary>
        public int fogkickline { get; set; } = 0;
        /// <summary>
        /// 状态 0:下线,1:在线 2：维护
        /// </summary>
        public short status { get; set; } = 0;
        /// <summary>
        /// 设备类型
        /// </summary>
        public short devicetype { get; set; }
        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime? checkintime { get; set; }
        /// <summary>
        /// 下线时间
        /// </summary>
        public DateTime? checkouttime { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? operatedate { get; set; }
        /// <summary>
        /// 是否无效 0：正常 1：无效
        /// </summary>
        public short bdel { get; set; } = 0;
        /// <summary>
        /// 是否已插入警告 0：未插入警告 1：已插入警告
        /// </summary>
        public short bsendwarning { get; set; } = 0;
        /// <summary>
        /// 参数
        /// </summary>
        public string param1 { get; set; }
        /// <summary>
        /// 1：自营  0：第三方
        /// </summary>
        public short isself { get; set; } = 1;
        /// <summary>
        /// 0：未发送过长时间扬尘离线报警  2：发送过2级的 3：发送过3级的
        /// </summary>
        public short bsendlongofflinewarn { get; set; } = 0;
        /// <summary>
        /// 供应商
        /// </summary>
        public string supplier { get; set; }
        /// <summary>
        /// 监测对象简称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string siteshortname { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_DeviceDY")]
    public class GCDeviceEntityDY
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int DEVICEID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string secret { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string aes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string prj_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string prj_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string owner_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string device_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contract_record_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string monitor_point { get; set; }
    }
}
