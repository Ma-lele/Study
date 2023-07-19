using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    public class UnloadDataInput 
    {
        /// <summary>
        /// 设备id
        /// </summary>
        [Required(ErrorMessage = "unload_id不能为空！")]
        public string unload_id { get; set; }
        /// <summary>
        /// 电量百分比
        /// </summary>
        public float electric_quantity { get; set; }
        /// <summary>
        /// 重量，单位千克
        /// </summary>
        public float weight { get; set; }
        /// <summary>
        /// 偏置值(-100~100)
        /// </summary>
        public float bias { get; set; }
        /// <summary>
        /// 数据状态 0正常 1预警 2 重量报警 3 偏置报警
        /// </summary>
        public int upstate { get; set; }
        /// <summary>
        /// 预警重量
        /// </summary>
        public float early_warning_weight { get; set; }
        /// <summary>
        /// 报警重量
        /// </summary>
        public float alarm_weight { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updatetime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; } = DateTime.Now;
    }
}
