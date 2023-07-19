using System;
using System.ComponentModel.DataAnnotations;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class PenaltyDataInput
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [Required(ErrorMessage = "安监备案号不能为空！")] 
        public string recordNumber { get; set; }
        /// <summary>
        /// 处罚编号
        /// </summary>
        public string penaltycode { get; set; }
        /// <summary>
        /// 处罚内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 处罚时间
        /// </summary>
        public DateTime penaltydate { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string @operator { get; set; }

    }
}
