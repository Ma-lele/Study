using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace XHS.Build.Common.Wechat
{
    /// <summary>
    /// 微信网络提交数据工具类
    /// </summary>
    public class WClient : WebClient
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string CONTENT_TYPE_FORM = "application/x-www-form-urlencoded;charset=UTF-8";
        public const string CONTENT_TYPE_STREAM = "application/octet-stream;charset=UTF-8";
        public const string CONTENT_TYPE_JSON = "application/json;charset=UTF-8";
        public const string CONTENT_TYPE_XML = "application/xml;charset=UTF-8";
        public const string CONTENT_TYPE_TEXT = "application/text;charset=UTF-8";
        public string Url { get; set; }
        /// <summary>
        /// 字符编码
        /// </summary>
        public new Encoding Encoding
        {
            get { return _client.Encoding; }
            set { _client.Encoding = value; }
        }
        /// <summary>
        /// 请求标头
        /// </summary>
        public new WebHeaderCollection Headers
        {
            get { return _client.Headers; }
            set { _client.Headers = value; }
        }
        /// <summary>
        /// 客户端实例
        /// </summary>
        private WebClient _client = new WebClient();

        public WClient(string url)
            : this(url, CONTENT_TYPE_FORM) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">发送地址</param>
        public WClient(string url, string contentType)
        {
            Url = url;
            _client.Encoding = Encoding.UTF8;
            _client.Headers.Add("Content-Type", contentType);

        }

        /// <summary>
        /// 发送POST数据请求
        /// </summary>
        /// <param name="data">POST数据</param>
        /// <returns></returns>
        public string PostData(string data)
        {
            string result = string.Empty;
            byte[] postData = Encoding.GetBytes(data);

            Headers.Add("ContentLength", postData.Length.ToString());

            byte[] response = _client.UploadData(Url, POST, postData);//得到返回字符流
            result = Encoding.GetString(response);//转成字符串

            return result;
        }

        /// <summary>
        /// 发送GET数据请求，返回字符串
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            string result = _client.DownloadString(Url);
            return result;
        }

        /// <summary>
        /// 发送GET数据请求，返回字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetByteData()
        {
            byte[] result = _client.DownloadData(Url);
            return result;
        }

        /// <summary>
        /// 发送GET数据请求，返回流
        /// </summary>
        /// <returns></returns>
        public Stream GetStreamData()
        {
            Stream result = _client.OpenRead(Url);
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public new void Dispose()
        {
            _client.Dispose();
        }
    }
}
