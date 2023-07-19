using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace XHS.Build.Common.Helps
{
    public class HttpNetRequest
    {
        private static int timeOut = 100000;//设置连接超时时间，默认10秒
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url">请求URL，如果需要传参，在URL末尾加上“？+参数名=参数值”即可</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            //创建
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //设置请求方法
            httpWebRequest.Method = "GET";
            //请求超时时间
            httpWebRequest.Timeout = 20000;
            //发送请求
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //利用Stream流读取返回数据
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            //获得最终数据，一般是json
            string responseContent = streamReader.ReadToEnd();
            streamReader.Close();
            httpWebResponse.Close();
            return responseContent;
        }

        /// <summary>
        /// 发送post json
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public static string POSTSendJsonRequest(string url, string jsonstring, Dictionary<string, string> headers)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.Timeout = timeOut;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1;SV1)";
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8;";
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers.Keys)
                {
                    request.Headers.Add(item, headers[item]);
                }
            }

            var memStream = new MemoryStream();
            var paramsByte = Encoding.GetEncoding("utf-8").GetBytes(jsonstring);
            memStream.Write(paramsByte, 0, paramsByte.Length);
            request.ContentLength = memStream.Length;
            var requestStream = request.GetRequestStream();

            memStream.Position = 0;
            var tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            var response = request.GetResponse();
            using (var s = response.GetResponseStream())
            {
                var reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// get(拼接url) post form
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParams"></param>
        /// <param name="requestMethod">GET POST</param>
        /// <returns></returns>
        public static string SendRequest(string url, Dictionary<string, object> requestParams, string requestMethod, Dictionary<string, string> headers)
        {
            if (requestMethod == "GET")
            {
                var paramStr = "";
                foreach (var key in requestParams.Keys)
                {
                    paramStr += string.Format("{0}={1}&", key, HttpUtility.UrlEncode(requestParams[key].ToString()));
                }
                paramStr = paramStr.TrimEnd('&');
                url += (url.EndsWith("?") ? "&" : "?") + paramStr;
            }

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.Timeout = timeOut;
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers.Keys)
                {
                    request.Headers.Add(item, headers[item]);
                }
            }
            if (requestMethod == "POST")
            {
                request.Method = requestMethod;
                request.ContentType = "multipart/form-data";

                var memStream = new MemoryStream();

                var strBuf = new StringBuilder();
                foreach (var key in requestParams.Keys)
                {
                    strBuf.Append("Content-Disposition: form-data; name=\"" + key + "\"\r\n\r\n");
                    strBuf.Append(requestParams[key].ToString());
                }
                var paramsByte = Encoding.GetEncoding("utf-8").GetBytes(strBuf.ToString());
                memStream.Write(paramsByte, 0, paramsByte.Length);
                request.ContentLength = memStream.Length;

                var requestStream = request.GetRequestStream();

                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
            }
            try
            {
                var response = request.GetResponse();
                using (var s = response.GetResponseStream())
                {
                    var reader = new StreamReader(s, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 默认表单提交
        /// </summary>
        /// <param name="requestUri">提交路径</param>
        /// <param name="postData">提交数据</param>
        /// <param name="cookie">Cookie容器对象</param>
        /// <returns>字符串结果</returns>
        public static string PostForm(string requestUri, Dictionary<string, object> postData, Dictionary<string, string> headers)
        {
            HttpWebRequest request = WebRequest.CreateHttp(requestUri);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers.Keys)
                {
                    request.Headers.Add(item, headers[item]);
                }
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string key in postData.Keys)
            {
                stringBuilder.AppendFormat("&{0}={1}", key, postData[key]);
            }
            byte[] buffer = Encoding.UTF8.GetBytes(stringBuilder.ToString().Trim('&'));
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return reader.ReadToEnd();
        }
        public static string PostSendRequestUrl(string url, Dictionary<string, object> requestParams, string method = "POST", Dictionary<string, string> headers = null)
        {
            var paramStr = "";
            foreach (var key in requestParams.Keys)
            {
                paramStr += string.Format("{0}={1}&", key, HttpUtility.UrlEncode(requestParams[key].ToString()));
            }
            paramStr = paramStr.TrimEnd('&');
            url += (url.EndsWith("?") ? "&" : "?") + paramStr;

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method;
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.Timeout = timeOut;
            if (headers != null && headers.Count > 0)
            {
                foreach (var item in headers.Keys)
                {
                    request.Headers.Add(item, headers[item]);
                }
            }
            var response = request.GetResponse();
            using (var s = response.GetResponseStream())
            {
                var reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
    }
}
