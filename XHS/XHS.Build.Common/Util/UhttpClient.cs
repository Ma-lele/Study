using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;

namespace Util
{
    public class UhttpClient
    {
        private const string CONTENT_TYPE_FORM = "application/x-www-form-urlencoded";
        private const string CONTENT_TYPE_JSON = "application/json";

        /// <summary>
        /// 提交json数据
        /// </summary>
        /// <param name="postUrl">地址</param>
        /// <param name="jsonParam">json数据</param>
        /// <returns></returns>
        public static string PostJson(string postUrl, string jsonParam)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent httpContent = new StringContent(jsonParam, Encoding.UTF8);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(CONTENT_TYPE_JSON);

                HttpResponseMessage response = httpClient.PostAsync(postUrl, httpContent).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        /// <summary>
        /// 提交一般表单数据
        /// </summary>
        /// <param name="postUrl">地址</param>
        /// <param name="dicParam">表单</param>
        /// <returns></returns>
        public static string Post(string postUrl, Dictionary<string, string> dicParam)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent httpContent = new FormUrlEncodedContent(dicParam);

                HttpResponseMessage response = httpClient.PostAsync(postUrl, httpContent).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        /// <summary>
        /// 提交一般表单数据
        /// </summary>
        /// <param name="postUrl">地址</param>
        /// <param name="dicParam">表单</param>
        /// <returns></returns>
        public static string Post(string postUrl, Dictionary<string, string> dicParam, Dictionary<string, string> headers = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent httpContent = new FormUrlEncodedContent(dicParam);
                if (headers != null && headers.Count > 0)
                {
                    foreach (var item in headers.Keys)
                    {
                        httpClient.DefaultRequestHeaders.Add(item, headers[item]);
                    }
                }
                HttpResponseMessage response = httpClient.PostAsync(postUrl, httpContent).Result;

                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        /// <summary>
        /// 提交一般表单数据
        /// </summary>
        /// <param name="postUrl">地址</param>
        /// <param name="dicParam">表单</param>
        /// <returns></returns>
        public static string PostXM(string postUrl, string jsonParam, Dictionary<string, string> headers = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent httpContent = new StringContent(jsonParam, Encoding.UTF8);
                if (headers != null && headers.Count > 0)
                {
                    foreach (var item in headers.Keys)
                    {
                        httpClient.DefaultRequestHeaders.Add(item, headers[item]);
                    }
                }
                HttpResponseMessage response = httpClient.PostAsync(postUrl, httpContent).Result;
                if(!response.IsSuccessStatusCode)
                {
                    string nonce = null;
                    string authorization = GetSingleHeaderValue(response.Headers, "Authorization");
                    if (string.IsNullOrEmpty(authorization))
                    {
                        return nonce;
                    }
                    string str1 = authorization.Substring(authorization.IndexOf("Digest") + 7);
                    string[] str2 = str1.Split(",".ToCharArray());
                    for(int i=0;i< str2.Length; i++)
                    {
                        string str = str2[i];
                        if(str.IndexOf("nonce") != -1)
                        {
                            nonce = str;
                        }
                    }
                    if (!string.IsNullOrEmpty(nonce))
                    {
                        return nonce;
                    }
                }
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        public static string GetSingleHeaderValue(HttpResponseHeaders responseHeaders,
          string keyName)
        {
            if (responseHeaders.Contains(keyName))
                return responseHeaders.First(ph => ph.Key == keyName).Value.First();

            return null;
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="dicParam">参数</param>
        /// <returns></returns>
        public static string Get(string url, Dictionary<string, object> dicParam, Dictionary<string, string> headers = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var paramStr = "";
                if (dicParam != null && dicParam.Count > 0)
                {
                    foreach (var key in dicParam.Keys)
                    {
                        paramStr += string.Format("{0}={1}&", key, HttpUtility.UrlEncode(dicParam[key].ToString()));
                    }
                }
                paramStr = paramStr.TrimEnd('&');
                url += (url.EndsWith("?") ? "&" : "?") + paramStr;
                
                if (headers != null && headers.Count > 0)
                {
                    foreach (var item in headers.Keys)
                    {
                        httpClient.DefaultRequestHeaders.Add(item, headers[item]);
                    }
                }

                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
        }
    }
}
