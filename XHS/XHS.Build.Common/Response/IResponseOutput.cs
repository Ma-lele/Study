using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace XHS.Build.Common.Response
{
    /// <summary>
    /// 响应数据输出接口
    /// </summary>
    public interface IResponseOutput
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonIgnore]
        bool success { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; }
    }

    /// <summary>
    /// 响应数据输出泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResponseOutput<T> : IResponseOutput
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        T data { get; }
    }
}
