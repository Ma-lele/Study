using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Cache
{
    public enum CacheType
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        Memory,
        /// <summary>
        /// Redis缓存
        /// </summary>
        Redis
    }
}
