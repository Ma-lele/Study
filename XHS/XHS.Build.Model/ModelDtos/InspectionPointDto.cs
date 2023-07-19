
namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 检查单数据看板Dto
    /// </summary>
    public class InspectionPointDto
    {
        /// <summary>
        /// 安全监督备案号
        /// </summary>
        public string recordNumber { get; set; }

        /// <summary>
        /// 巡检点id（唯一标识）
        /// </summary>
        public string inspectionId { get; set; }

        /// <summary>
        /// 巡检地点描述
        /// </summary>
        public string site { get; set; }
        /// <summary>
        /// 楼栋号
        /// </summary>
        public string building { get; set; } = "";

        /// <summary>
        /// 楼层号
        /// </summary>
        public int floor { get; set; } = 0;
    }
}
