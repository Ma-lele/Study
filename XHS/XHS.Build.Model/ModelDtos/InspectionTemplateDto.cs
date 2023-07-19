using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class InspectionTemplateDto
    {
        public int CMID { get; set; }
        /// <summary>
        /// 模板name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 租户类型ID
        /// </summary>
        public int TTID { get; set; }
        /// <summary>
        /// 检查项
        /// </summary>
        public List<InspectionItems> conditions { get; set; }

    }

    public class InspectionItems
    {
        /// <summary>
        /// 检查项内容
        /// </summary>
        public string value { get; set; }
    }
}
