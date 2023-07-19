using System.Text.Json.Serialization;


namespace XHS.Build.Common.Response
{
    /// <summary>
    /// 响应数据输出
    /// </summary>
    public class CityResponseOutput<T> : ICityResponseOutput<T>
    {
        /// <summary>
        /// 是否成功标记
        /// </summary>
        [JsonIgnore]
        public int code { get; private set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string message { get; private set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; private set; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="msg">消息</param>
        public CityResponseOutput<T> Ok(T ddata, string mmsg = null)
        {
            code = 0;
            data = ddata;
            message = mmsg;
           
            return this;
        }


    }

  
}
