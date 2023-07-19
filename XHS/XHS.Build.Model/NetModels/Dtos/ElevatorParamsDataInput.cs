using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class ElevatorParamsDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "SeCode不能为空！")] 
        public string SeCode { get; set; }
        /// <summary>
        /// 最小高度
        /// </summary>
        public float MinHeight { get; set; }
        /// <summary>
        /// 最大高度
        /// </summary>
        public float MaxHeight { get; set; }
        /// <summary>
        /// 最小层数
        /// </summary>
        public int MaxFloor { get; set; }
        /// <summary>
        /// 最大人数
        /// </summary>
        public int MaxPerson { get; set; }
        /// <summary>
        /// 最大重量
        /// </summary>
        public float MaxWeight { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        public string DriverId { get; set; }
        //[Required(ErrorMessage = "司机身份证不能为空！")] 
        public string DriverCardNo { get; set; }
        //[Required(ErrorMessage = "司机姓名不能为空！")] 
        public string DriverName { get; set; }

        [Newtonsoft.Json.JsonIgnore()]
        public string DriverImg { get; set; }
    }
}
