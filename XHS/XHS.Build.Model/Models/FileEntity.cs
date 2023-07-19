using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 文件实体类
    /// </summary>
    [SugarTable("T_GC_File")]
    public class FileEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public string FILEID { get; set; }
        //[Required]
        public int GROUPID { get; set; }
        //[Required]
        public int SITEID { get; set; }
        //[Required]
        public string linkid { get; set; }
        //[Required]
        public string filetype { get; set; }
        public string exparam { get; set; }
        //[Required]
        public string filename { get; set; }
        //[Required]
        public long filesize { get; set; }
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public short bdel { get; set; } = 1;
        //[Required]
        public int centerup { get; set; } = 0;
        //[Required]
        public string centerfileurl { get; set; }
        //[Required]
        public string creater { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string path { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string tmbpath { get; set; }

        [SugarColumn(IsIgnore = true)]
        public GCSecureHisEntity gCSecureHisEntity { get; set; }

        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public DateTime? createdate { get; set; }

        public class FileEntityParam
        {
            [Required(ErrorMessage = "组ID必须")]
            public int GROUPID { get; set; }
            [Required(ErrorMessage = "项目ID必须")]
            public int SITEID { get; set; }
            public FileType filetype { get; set; }
            public string linkid { get; set; }
            public string exparam { get; set; }
        }

        public enum FileType
        {
            /// <summary>
            /// 移动执法
            /// </summary>
            Inspection = 1,
            /// <summary>
            /// 移动巡检
            /// </summary>
            Round = 2,
            /// <summary>
            /// 巡更
            /// </summary>
            Security = 3,
            /// <summary>
            /// 员工
            /// </summary>
            Employee = 4,
            /// <summary>
            /// 证书
            /// </summary>
            Certificate = 5,
            /// <summary>
            /// 特设上下机
            /// </summary>
            SeAuth = 6,
            /// <summary>
            /// 高支模
            /// </summary>
            HighFormwork = 7,
            /// <summary>
            /// 深基坑
            /// </summary>
            DeepPit = 8,
            /// <summary>
            /// 资产巡检
            /// </summary>
            CaRound = 9,
            /// <summary>
            /// 工地平面图
            /// </summary>
            SiteMap = 10
        }
    }

    public class FileOutput
    {
        /// <summary>
        /// 上传的文件ID
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// 上传的原图URL
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 上传的缩略图（压缩图）URL
        /// </summary>
        public string FileUrlTemp { get; set; }
    }
}
