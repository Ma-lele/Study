using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("T_GC_Region")]
    public class GCRegionEntity
    {
        public int RegionId { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public int ParentId { get; set; }
        public int RegionLevel { get; set; }
        public int RegionOrder { get; set; }
        public string @Operator { get; set; }
        public DateTime OperateDate { get; set; }
    }
}
