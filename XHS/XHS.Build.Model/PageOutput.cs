using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace XHS.Build.Model
{
    /// <summary>
    /// 分页信息输出
    /// </summary>
    public class PageOutput
    {
        /// <summary>
        /// 数据总数
        /// </summary>
        public int Total { get; set; } = 0;

        /// <summary>
        /// 数据
        /// </summary>
        public DataTable List { get; set; }
    }

    /// <summary>
    /// 分页信息输出
    /// </summary>
    public class PageOutput<T>
    {
        /// <summary>
        /// 数据总数
        /// </summary>
        public long dataCount { get; set; } = 0;

        /// <summary>
        /// 数据
        /// </summary>
        public IList<T> data { get; set; }
    }
}
