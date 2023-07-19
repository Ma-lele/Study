using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_File")]
    public class CenterFileEntity
    {

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long FILEID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid UUID { get; set; } = Guid.NewGuid();
        /// <summary>
        /// 照片地址
        /// </summary>
        public string fileurl { get; set; }
        /// <summary>
        /// 照片缩略图地址（非图片文件为null）
        /// </summary>
        public string tmburl { get; set; }
        /// <summary>
        /// 原始文件地址
        /// </summary>
        public string originalurl { get; set; }
        /// <summary>
        /// 照片压缩图地址（非图片文件为null）
        /// </summary>
        public string compurl { get; set; }
        /// <summary>
        /// 源文件路径
        /// </summary>
        public string physicalpath { get; set; }
        /// <summary>
        /// 0:失败,1:成功
        /// </summary>
        public short bsuccess { get; set; } = 0;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; } = DateTime.Now;
    }
}
