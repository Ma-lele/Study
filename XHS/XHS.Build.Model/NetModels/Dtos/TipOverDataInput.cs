using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class TipOverDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "SeCode不能为空！")] 
        public string SeCode { get; set; }
        /// <summary>
        /// 开始时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间yyyy-MM-dd HH:mm: ss
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 报警循环时长  秒
        /// </summary>
        public int Timelong
        { get; set; }
        /// <summary>
        /// 倾翻报警类型
        /// </summary>
        public int Alarm { get; set; }
        /// <summary>
        /// 更新时间yyyy-MM-dd HH:mm: ss 
        /// </summary>
        public DateTime UpdateTime { get; set; }

    }
}
