
using System.Collections.Generic;

namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 场内人员不同工种数量Dto
    /// </summary>
    public class PeopleTypeCountDto
    {
        /// <summary>
        /// 监督备案号
        /// </summary>
        public string recordNumber { get; set; }
        /// <summary>
        /// 所属机构编号
        /// </summary>
        public string belongedTo { get; set; }
        /// <summary>
        /// 键为工种代码,值为人数
        /// </summary>
        public Dictionary<string,int> workTypeDic { get; set; }
        /// <summary>
        /// 记录时间（2019-04-18)
        /// </summary>
        public string recordDate { get; set; }

    }
}
