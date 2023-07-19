using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using XHS.Build.Common.Helps;

namespace XHS.Build.Common.Response
{
    /// <summary>
    /// 响应数据输出
    /// </summary>
    public class ResponseOutput<T> : IResponseOutput<T>
    {
        /// <summary>
        /// 是否成功标记
        /// </summary>
        [JsonIgnore]
        public bool success { get; private set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; private set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; private set; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="msg">消息</param>
        public ResponseOutput<T> Ok(T ddata, string mmsg = null)
        {
            success = true;
            data = ddata;
            msg = mmsg;
            //if (CheckIfAnonymousType(ddata.GetType()) || ddata.GetType().IsValueType || ddata is String || ddata == null)
            //{
            //    data = ddata;
            //}
            //else
            //{
            //    ddata = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(HtmlHelper.filter(Newtonsoft.Json.JsonConvert.SerializeObject(ddata)));
            //}
            return this;
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public ResponseOutput<T> NotOk(string mmsg = null, T ddata = default(T))
        {
            success = false;
            msg = mmsg;
            data = ddata;

            return this;
        }

        private static bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }

    /// <summary>
    /// 响应数据静态输出
    /// </summary>
    public static partial class ResponseOutput
    {
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static IResponseOutput Ok<T>(T data = default(T), string msg = null)
        {
            return new ResponseOutput<T>().Ok(data, msg);
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        public static IResponseOutput Ok()
        {
            return Ok<string>();
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static IResponseOutput NotOk<T>(string msg = null, T data = default(T))
        {
            return new ResponseOutput<T>().NotOk(msg, data);
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static IResponseOutput NotOk(string msg = null)
        {
            return new ResponseOutput<string>().NotOk(msg);
        }

        /// <summary>
        /// 根据布尔值返回结果
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public static IResponseOutput Result<T>(bool success)
        {
            return success ? Ok<T>() : NotOk<T>();
        }

        /// <summary>
        /// 根据布尔值返回结果
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public static IResponseOutput Result(bool success)
        {
            return success ? Ok() : NotOk();
        }
    }

    public class DeviceResponse<T>
    {
        public bool success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }

        public int Code { get; set; }
        public string Msg { get; set; }
    }
}
