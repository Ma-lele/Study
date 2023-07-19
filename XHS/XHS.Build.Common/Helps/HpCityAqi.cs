using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Helps
{
    /// <summary>
    /// 城市的实时AQI助手
    /// </summary>
    public partial class HpCityAqi
    {

        //获取城市的实时AQI(PM25.in)
        //public static string GetCityAqiBk(string city)
        //{

        //    string serviceAddress = "http://www.pm25.in/api/querys/aqi_details.json?city=" + city + "&token=" + HpSystemSetting.getSettingValue("S021");
        //    System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(serviceAddress);
        //    request.Method = "GET";
        //    request.ContentType = "text/html;charset=UTF-8";
        //    System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
        //    System.IO.Stream myResponseStream = response.GetResponseStream();
        //    System.IO.StreamReader myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.UTF8);
        //    string retString = myStreamReader.ReadToEnd();
        //    myStreamReader.Close();
        //    myResponseStream.Close();
        //    return retString;
        //}

        //获取城市的实时AQI(和风天气)
        public static string GetCityAqi(string domain, string city, string key)
        {

            string serviceAddress = "https://" + domain + "/s6/air/now?location=" + city + "&key=" + key;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(serviceAddress);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream myResponseStream = response.GetResponseStream();
            System.IO.StreamReader myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        //获取城市的实时天气(和风天气)
        public static string GetCityWeather(string domain, string city, string key)
        {
            string serviceAddress = "https://" + domain + "/s6/weather/now?location=" + city + "&key=" + key;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(serviceAddress);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream myResponseStream = response.GetResponseStream();
            System.IO.StreamReader myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

    }
}
