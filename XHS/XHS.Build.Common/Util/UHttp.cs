using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace XHS.Build.Common.Util
{
    /// <summary>
    /// Http请求处理类
    /// </summary>
    public class UHttp
    {
        public const string CONTENT_TYPE_FORM = "application/x-www-form-urlencoded";
        public const string CONTENT_TYPE_JSON = "application/json";
        public const string CONTENT_TYPE_TEXT_XML = "text/xml; charset=utf-8";
        public static string Post(string postUrl, string postData)
        {
            return Post(postUrl, postData, CONTENT_TYPE_FORM, new WebHeaderCollection());
        }
        public static string Post(string postUrl, string postData, string contentType)
        {
            return Post(postUrl, postData, contentType, new WebHeaderCollection());
        }
        public static string Post(string postUrl, string postData, WebHeaderCollection headers)
        {
            return Post(postUrl, postData, CONTENT_TYPE_FORM, headers);
        }
        //public static string Put(string postUrl, string postData,string contenttype,  WebHeaderCollection headers)
        //{
        //    return Put(postUrl, postData, contenttype, headers);
        //}
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="postData">请求数据</param>
        /// <param name="contentType">contentType</param>
        /// <param name="headers">头数据</param>
        /// <returns></returns>
        public static string Post(string postUrl, string postData, string contentType, WebHeaderCollection headers)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            string content = null;
            byte[] dataByte = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(postUrl) as HttpWebRequest;
                request.Headers = headers;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = dataByte.Length;

                if (postUrl.IndexOf("https") >= 0)
                {
                    ServicePointManager.ServerCertificateValidationCallback = OnValidateCertificate;
                    ServicePointManager.Expect100Continue = true;
                }
                //System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();

                using (Stream outstream = request.GetRequestStream())
                {
                    outstream.Write(dataByte, 0, dataByte.Length);
                    outstream.Close();
                }

                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                using (Stream instream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(instream, encoding))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                return content;
            }
            catch (Exception ex)
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
        public static string PostForm(string postUrl, NameValueCollection postData)
        {
            HttpWebRequest request = WebRequest.CreateHttp(postUrl);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            if (postUrl.IndexOf("https") >= 0)
            {
                ServicePointManager.ServerCertificateValidationCallback = OnValidateCertificate;
                ServicePointManager.Expect100Continue = true;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string key in postData.Keys)
            {
                stringBuilder.AppendFormat("&{0}={1}", key, postData.Get(key));
            }
            byte[] buffer = Encoding.UTF8.GetBytes(stringBuilder.ToString().Trim('&'));
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                                      SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="postData">请求数据</param>
        /// <returns></returns>
        public static string Post(string postUrl, string postData, string contentType, Cookie cookie)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            string content = null;
            byte[] dataByte = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(postUrl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                if (cookie != null)
                {
                    cookieContainer.Add(cookie);
                }
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = dataByte.Length;
                using (Stream outstream = request.GetRequestStream())
                {
                    outstream.Write(dataByte, 0, dataByte.Length);
                    outstream.Close();
                }

                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                using (Stream instream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(instream, encoding))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                return content;
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ex.Message;
            }
        }

        public static string Put(string postUrl, string postData, string contentType, WebHeaderCollection headers)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            string content = null;
            byte[] dataByte = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(postUrl) as HttpWebRequest;
                request.Headers = headers;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "PUT";
                request.ContentType = contentType;
                request.ContentLength = dataByte.Length;
                using (Stream outstream = request.GetRequestStream())
                {
                    outstream.Write(dataByte, 0, dataByte.Length);
                    outstream.Close();
                }

                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                using (Stream instream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(instream, encoding))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                return content;
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ex.Message;
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="postData">请求数据</param>
        /// <returns></returns>
        public static HttpWebResponse PostAll(string postUrl, string postData, string contentType, Cookie cookie)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] dataByte = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(postUrl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                if (cookie != null)
                {
                    cookieContainer.Add(cookie);
                }
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = dataByte.Length;
                using (Stream outstream = request.GetRequestStream())
                {
                    outstream.Write(dataByte, 0, dataByte.Length);
                    outstream.Close();
                }

                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;

                return response;
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return null;
            }
        }

        public static string Get(string getUrl)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                request = (HttpWebRequest)WebRequest.Create(getUrl);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 5000;

                response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ex.Message;
            }
        }
    }

    //public class MyPolicy : ICertificatePolicy
    //{
    //    public bool CheckValidationResult(
    //          ServicePoint srvPoint
    //        , X509Certificate certificate
    //        , WebRequest request
    //        , int certificateProblem)
    //    {

    //        //Return True to force the certificate to be accepted.
    //        return true;

    //    } // end CheckValidationResult
    //} // class MyPolicy
}
