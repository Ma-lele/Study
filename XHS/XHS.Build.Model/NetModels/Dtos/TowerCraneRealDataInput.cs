using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 实时数据实体
    /// </summary>
    public class TowerCraneRealDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "SeCode不能为空！")]
        public string SeCode { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public float? Height { get; set; }
        /// <summary>
        /// 幅度
        /// </summary>
        public decimal? Margin { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal? Weight { get; set; }
        /// <summary>
        /// 回转
        /// </summary>
        public decimal? Rotation { get; set; }
        /// <summary>
        /// 力矩
        /// </summary>
        public decimal? Moment { get; set; }
        /// <summary>
        /// 力矩百分比
        /// </summary>
        public decimal? MomentPercent { get; set; }
        /// <summary>
        /// 风级
        /// </summary>
        public int? Wind { get; set; }
        /// <summary>
        /// 安全吊重
        /// </summary>
        public decimal? SafeLoad { get; set; }
        /// <summary>
        /// 报警类型(代码详见报警类型表)
        /// </summary>
        public int? Alarm { get; set; }
        /// <summary>
        /// 更新时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 是否已生成报告	0:没有,1:有
        /// </summary>
        public int? HasReport { get; set; }
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

        public decimal? LiJvMaxMargin { get; set; }

        public decimal? WindSpeed { get; set; }

    }
    /// <summary>
    /// 参数实体
    /// </summary>
    public class TowerCraneParamsDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "SeCode不能为空！")]
        public string SeCode { get; set; }
        /// <summary>
        /// 吊绳倍率
        /// </summary>
        public string BeiLv { get; set; }
        /// <summary>
        /// 最小高度
        /// </summary>
        public string MinHeight { get; set; }
        /// <summary>
        /// 最大高度
        /// </summary>
        public string MaxHeight { get; set; }
        /// <summary>
        /// 最小幅度
        /// </summary>
        public string MinMargin { get; set; }
        /// <summary>
        /// 最大幅度
        /// </summary>
        public string MaxMargin { get; set; }
        /// <summary>
        /// 最大起重量
        /// </summary>
        public string MaxWeight { get; set; }
        /// <summary>
        /// 最大起重量幅度
        /// </summary>
        public string MaxWeightMargin { get; set; }
        /// <summary>
        /// 最大幅度起重量
        /// </summary>
        public string MaxMarginWeight { get; set; }
        /// <summary>
        /// 左回转极限
        /// </summary>
        public string LeftRotation { get; set; }
        /// <summary>
        /// 右回转极限
        /// </summary>
        public string RightRotation { get; set; }
        /// <summary>
        /// 力距最大幅度
        /// </summary>
        public float LiJvMaxMargin { get; set; }
        /// <summary>
        /// 起重臂长
        /// </summary>
        public string Qzbc { get; set; }
        /// <summary>
        /// 平衡臂长
        /// </summary>
        public string Phbc { get; set; }
        /// <summary>
        /// 塔身高度
        /// </summary>
        public string Tsgd { get; set; }
        /// <summary>
        /// 塔帽高度
        /// </summary>
        public string Tmgd { get; set; }
        /// <summary>
        /// 报警类型(代码详见报警类型表)
        /// </summary>
        public string Alarm { get; set; }
        /// <summary>
        /// 更新时间  	yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string UpdateTime { get; set; }

        public string DriverId { get; set; }
        public string DriverCardNo { get; set; }
        public string DriverName { get; set; }

        [Newtonsoft.Json.JsonIgnore()]
        public string DriverImg { get; set; }

    }
    /// <summary>
    /// 工作循环实体
    /// </summary>
    public class TowerCraneWorkDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "SeCode不能为空！")]
        public string SeCode { get; set; }
        /// <summary>
        /// 起吊开始时间  	yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 起吊回转角度
        /// </summary>
        [Required]
        public float StartRotation { get; set; }
        /// <summary>
        /// 起吊小车幅度
        /// </summary>
        [Required]
        public float StartMargin { get; set; }
        /// <summary>
        /// 起吊高度
        /// </summary>
        [Required]
        public float StartHeight { get; set; }
        /// <summary>
        /// 卸吊结束时间  yyyy-MM-dd HH:mm:ss
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 卸吊回转角度
        /// </summary>
        [Required]
        public float EndRotation { get; set; }
        /// <summary>
        /// 卸吊小车幅度
        /// </summary>
        [Required]
        public float EndMargin { get; set; }
        /// <summary>
        /// 卸吊高度
        /// </summary>
        [Required]
        public float EndHeight { get; set; }
        /// <summary>
        /// 最大重量
        /// </summary>
        [Required]
        public float MaxWeight { get; set; }
        /// <summary>
        /// 最大力矩
        /// </summary>
        [Required]
        public float MaxMargin { get; set; }
        /// <summary>
        /// 司机身份证
        /// </summary>
        [Required(ErrorMessage = "司机身份证不能为空！")] 
        public string DriverCardNo { get; set; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        [Required(ErrorMessage = "司机姓名不能为空！")] 
        public string DriverName { get; set; }
    }

    /// <summary>
    /// 报警实体
    /// </summary>
    public class TowerCraneAlarmInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "SeCode不能为空！")]
        public string SeCode { get; set; }
    }
}
