
using System;
using System.Collections.Generic;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 检查单信息Dto
    /// </summary>
    public class InspectContentInfoDto
    {
        /// <summary>
        /// 监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 检查单编号
        /// </summary>
        public string checkNumber { get; set; }

        /// <summary>
        /// 检查时间（2019-07-07 10:22:22）
        /// </summary>
        public DateTime checkDate { get; set; }

        /// <summary>
        /// 检查人姓名，多人用;隔开
        /// </summary>
        public string checkPerson { get; set; }

        /// <summary>
        /// 检查人身份证号
        /// </summary>
        public string idCard { get; set; }

        /// <summary>
        /// 检查单类型：1：检查记录单 2：一般隐患单 3：严重隐患单
        /// </summary>
        public int checkNumType { get; set; }
        /// <summary>
        /// 是否符合省标准，0:否 1:是
        /// </summary>
        public int IsProvinStand { get; set; }

        /// <summary>
        /// 检查单内容列表
        /// </summary>
        public List<CheckListData> checkLists { get; set; }

        /// <summary>
        /// 是否需要整改：(1:是、0:否)
        /// </summary>
        public int isRectify { get; set; }

        /// <summary>
        /// 建议整改完成时间（2019-07-07 10:22:22）
        /// </summary>
        public DateTime? rectifyDate { get; set; }

        /// <summary>
        /// 检查备注
        /// </summary>
        public string remark { get; set; }

    }


    public class CheckListData
    {
        /// <summary>
        /// 检查项唯一id
        /// </summary>
        public string itemId { get; set; }

        /// <summary>
        /// 检查内容
        /// </summary>
        public string checkContent { get; set; }

        /// <summary>
        /// 检查人
        /// </summary>
        public string rectifyPerson { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string[] urls { get; set; }
    }
}
