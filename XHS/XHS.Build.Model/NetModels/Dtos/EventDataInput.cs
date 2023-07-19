using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class EventDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "安监备案号不能为空！")] 
        public string recordNumber { get; set; }
        /// <summary>
        /// 事件单号
        /// </summary>
        public string eventcode { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string handler { get; set; }

        /// <summary>
        /// 监督检查单号
        /// </summary>
        public string SPID { get; set; }

        /// <summary>
        /// 监督检查类型
        /// </summary>
        public int SPtype { get; set; }

        /// <summary>
        /// 整改期限
        /// </summary>
        public DateTime limitdate { get; set; }

        /// <summary>
        /// 监督检查单号
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }
    }
}
