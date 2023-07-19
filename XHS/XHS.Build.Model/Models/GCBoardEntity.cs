using SqlSugar;
using System;
namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 实体类
    /// </summary>
    [SugarTable("T_GC_Board")]
    public class GCBoardEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string boardurl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string boardmemo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MENUID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string boardtype { get; set; }
    }
}