using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;

namespace XHS.Build.Common.Auth
{
    public class SiteCityXuweiToken
    {
        private readonly ILogger<SiteCityXuweiToken> _logger;
        public SiteCityXuweiToken(ICache cache, ILogger<SiteCityXuweiToken> logger, IConfiguration configuration)
        {
            _logger = logger;
        }
        private static readonly string apiurl = "http://47.95.217.172:8100";
        private static readonly string prourl = "http://123.57.205.192:6527";
        private static readonly string appid = "acfa7ac7925f3af0";
        private static readonly string secret = "a45685faacfa7ac7925f3af093d334e8";
        private static readonly string realnameurl = "http://123.57.205.192:7890/webapi/v1.0/";
        private static readonly string token = "1136216092441556992";
        private static readonly string provider = "13299284113967325635";


        public string JsonRequest(int type, string projectId, string api, string param)
        {
            string url = apiurl;
            string apiparam = secret + "appid" + appid + "projectId" + projectId + secret;
            string apiuri = "?appid=" + appid + "&projectId=" + projectId + "&sign=";
            // WebHeaderCollection header = new WebHeaderCollection();
            // header.Add("Authorization", token);
            //string retString = UHttp.Post(url + api, param, UHttp.CONTENT_TYPE_JSON, header);

            if (type == 2)
            {
                JObject jobparam = new JObject();
                if (!string.IsNullOrEmpty(param))
                {
                    jobparam = JObject.Parse(param);
                }


                SortedDictionary<string, dynamic> jparam = new SortedDictionary<string, dynamic>();
                jparam = new SortedDictionary<string, dynamic>();
                foreach (JProperty jProperty in jobparam.Properties())
                {
                    jparam.Add(jProperty.Name, jProperty.Value);
                }
                jobparam = new JObject();
                //string[] arrKeys = jparam.Keys.ToArray();
                //Array.Sort(arrKeys, string.CompareOrdinal);
                foreach (var item in jparam)
                {
                    jobparam.Add(item.Key, item.Value  /*jparam[item]*/);
                }

                url = realnameurl;
                if (api.Contains("program/uploadProgram"))
                {
                    url = prourl;
                }
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                apiparam = token + "body" + JsonConvert.SerializeObject(jobparam) + "provider" + provider + "timestamp" + timestamp + "token" + token + token;
                apiuri = "?token=" + token + "&provider=" + provider + "&timestamp=" + timestamp + "&signature=";
            }
            string signature = UEncrypter.EncryptByMD5(apiparam);
            apiuri = apiuri + signature;
            JObject paramjob = JObject.Parse(param);
            paramjob.Add("Appid", appid);
            paramjob.Add("Sign", signature);
            string retString = HttpNetRequest.POSTSendJsonRequest(url + api + apiuri, JsonConvert.SerializeObject(JArray.Parse("[" + paramjob + "]")), new Dictionary<string, string>() { });
            if (!string.IsNullOrEmpty(retString))
            {
                return retString;
            }
            else
            {
                _logger.LogInformation(url + api + "接口信息返回空");
            }

            return "";
        }




    }
}
