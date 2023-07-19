
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 场内人员进出数信息Dto
    /// </summary>
    public class AttendancesDto
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
        /// 记录日期（2019-04-18）
        /// </summary>
        public string recordDate { get; set; }
        
        /// <summary>
        /// 记录小时（0-23）
        /// </summary>
        public int recordHour { get; set; }
        
        /// <summary>
        /// 进场人数
        /// </summary>
        public int inPeople { get; set; }
       
        /// <summary>
        /// 出场人数
        /// </summary>
        public int outPeople { get; set; }
    }
}
