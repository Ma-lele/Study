using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    public class OpenApiOperateLog : MongoBase.BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Key { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string KeyCode { get; set; }

        /// <summary>
        /// api名称
        /// </summary>
        public string ApiMethod { get; set; }

        /// <summary>
        /// 接口路径
        /// </summary>
        public string ApiPath { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 接口msg
        /// </summary>
        public string NoteMsg { get; set; }

        /// <summary>
        /// 接口耗时
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Status { get; set; }

        public string Body { get; set; }

        public string UrlQueryString { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        public string GroupId { get; set; }
    }


    public class SpecialEqpData : MongoBase.BaseEntity
    {
        public string Platform { get; set; }
        public int Flag { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string SeCode { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public decimal Height { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 报警状态
        /// </summary>
        public int AlarmState { get; set; }
        /// <summary>
        /// 当前人数
        /// </summary>
        public int NumOfPeople { get; set; }
        /// <summary>
        /// 设备时间
        /// </summary>
        public string DeviceTime { get; set; }
        /// <summary>
        /// 是否已生成报告 0:没有,1:有
        /// </summary>
        public int HasReport { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public decimal Speed { get; set; }
        /// <summary>
        /// 层数
        /// </summary>
        public decimal Floor { get; set; }
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

        /// <summary>
        /// 幅度
        /// </summary>
        public double Margin { get; set; }
       
        /// <summary>
        /// 回转
        /// </summary>
        public double Rotation { get; set; }
        /// <summary>
        /// 力矩
        /// </summary>
        public decimal Moment { get; set; }
        /// <summary>
        /// 力矩百分比
        /// </summary>
        public decimal MomentPercent { get; set; }
        /// <summary>
        /// 风级
        /// </summary>
        public int Wind { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public decimal? WindSpeed { get; set; }
        /// <summary>
        /// 安全吊重
        /// </summary>
        public decimal SafeLoad { get; set; }
        /// <summary>
        /// 报警类型(代码详见报警类型表)
        /// </summary>
        public int Alarm { get; set; }

        /// <summary>
        /// 更新时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] 
        public DateTime UpdateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }

    public class CityOpenApiOperateLog : MongoBase.BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Key { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// api名称
        /// </summary>
        public string ApiMethod { get; set; }

        /// <summary>
        /// 接口路径
        /// </summary>
        public string ApiPath { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 接口msg
        /// </summary>
        public string NoteMsg { get; set; }

        /// <summary>
        /// 接口耗时
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }

        public string Body { get; set; }

        public string Result { get; set; }

        public string UrlQueryString { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        public string GroupId { get; set; }
    }

    public class CityUploadOperateLog : MongoBase.BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string url { get; set; }

        public string account { get; set; }

        /// <summary>
        /// api名称
        /// </summary>
        public string api { get; set; }

        public string result { get; set; }

        public string param { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime createtime { get; set; }
    }

    public class HighFormworkData : MongoBase.BaseEntity
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 设备监测编号 (即deviceId)
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// 点位编号
        /// </summary>
        public string pointId { get; set; }

        /// <summary>
        /// 电量(%)
        /// </summary>
        public double power { get; set; } = 0;

        /// <summary>
        /// 温度（℃）
        /// </summary>
        public double temperature { get; set; } = 0;

        /// <summary>
        /// 立杆轴力(KN)
        /// </summary>
        public double load { get; set; } = 0;

        /// <summary>
        /// 水平倾角（°）
        /// </summary>
        public double horizontalAngle { get; set; } = 0;

        /// <summary>
        /// 立杆倾角（°）
        /// </summary>
        public double coordinate { get; set; } = 0;

        /// <summary>
        /// 水平位移（mm）
        /// </summary>
        public double translation { get; set; } = 0;

        /// <summary>
        /// 模板沉降（mm）
        /// </summary>
        public double settlement { get; set; } = 0;

        /// <summary>
        /// 收集时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime collectionTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime createTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 臭氧
    /// </summary>
    public class OzoneRtdData : MongoBase.BaseEntity
    {
        public string Platform { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        /// 
        public string ozcode { get; set; }
        /// <summary>
        /// 臭氧值
        /// </summary>
        public double o3 { get; set; }

        /// <summary>
        /// 收集时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime collectionTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime createtime { get; set; } = DateTime.Now;
    }
}
