using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XHS.Windows.Server
{
    public class HttpNetRequest
    {
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            try
            {
                //创建
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //设置请求方法
                httpWebRequest.Method = "GET";
                //请求超时时间
                httpWebRequest.Timeout = 30000;
                //发送请求
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //利用Stream流读取返回数据
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                //获得最终数据，一般是json
                string responseContent = streamReader.ReadToEnd();
                streamReader.Close();
                httpWebResponse.Close();
                LogHelper.Info(responseContent);
                return responseContent;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return null;
            }
        }

    }
}
