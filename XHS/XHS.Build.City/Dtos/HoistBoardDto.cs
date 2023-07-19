
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 升降机信息看板Dto
    /// </summary>
    public class HoistBoardDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 所属机构编号
        /// </summary>
        public string belongedTo { get; set; }

        /// <summary>
        /// 看板地址
        /// </summary>
        public string hoistBoardUrl { get; set; }
    }
}
