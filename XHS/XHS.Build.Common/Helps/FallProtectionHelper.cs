using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
 

namespace XHS.Build.Common.Helps
{
    public class FallProtectionHelper
    {
        public static DateTime TokenExpireTime = DateTime.MinValue;
        public static string Token = "";
 

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Login(string url,string username, string password)
        {
            JObject postData = new JObject();
            postData["userName"] = username;
            postData["passWord"] = password;

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Content-Type", "application/json");
 
            string result = HttpNetRequest.POSTSendJsonRequest(url, postData.ToString(), header);
            var objResult = JObject.Parse(result);
            if (objResult.SelectToken("resultCode") != null 
                && objResult.SelectToken("resultCode").ToString() == "0"
                && objResult.SelectToken("data.token") != null)
            {
                Token = objResult.SelectToken("data.token").ToString();
                TokenExpireTime = DateTime.Now.AddHours(12);
                return true;
            } 
            return false;
        }



        public static bool Register(string url,string pushUrl)
        {
            JObject postData = new JObject();
            postData["devType"] = "a1DqYiZ9OVf";
            postData["destUrl"] = pushUrl;

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Content-Type", "application/json");
            header.Add("WL-APART-TOKEN", Token);

            string result = HttpNetRequest.POSTSendJsonRequest(url, postData.ToString(), header);
            var objResult = JObject.Parse(result);



            return false;
        }


      

    }
}
