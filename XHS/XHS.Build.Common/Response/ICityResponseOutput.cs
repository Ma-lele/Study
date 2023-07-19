using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace XHS.Build.Common.Response
{
    /// <summary>
    /// 响应数据输出接口
    /// </summary>
    public interface ICityResponseOutput
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonIgnore]
        public int code { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string message { get; }
    }

    /// <summary>
    /// 响应数据输出泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICityResponseOutput<T> : ICityResponseOutput
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        T data { get; }
    }
}
