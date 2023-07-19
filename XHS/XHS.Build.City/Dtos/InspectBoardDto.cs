
namespace XHS.Build.City.Dtos
{
    /// <summary>
    /// 检查单数据看板Dto
    /// </summary>
    public class InspectBoardDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 检查单编号
        /// </summary>
        public string checkNumber { get; set; }

        /// <summary>
        /// 看板地址
        /// </summary>
        public string stereotacticBoardUrl { get; set; }
    }
}
