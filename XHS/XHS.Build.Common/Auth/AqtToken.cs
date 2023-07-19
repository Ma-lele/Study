using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;

namespace XHS.Build.Common.Auth
{
    public class AqtToken
    {
        private readonly ICache _cache;
        private readonly ILogger<AqtToken> _logger;
        private readonly IConfiguration _configuration;
        private static readonly Mutex mutex = new Mutex();
        private static readonly Mutex mutexcenter = new Mutex();
        private static readonly string tokenurl = "http://58.213.147.234:8000/api/";
        //private static readonly string url = "http://49.4.11.116:9876/api/";
        private static readonly string url = "http://49.4.11.116:8180/api/";  //正式URL
        private static readonly string geturl = "http://58.213.147.234:8000/ProvincialSystemDocking/api/";
        public AqtToken(ICache cache, ILogger<AqtToken> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }


        public string JsonRequest(string api, string param,string account = "",string password="")
        {
            string token = "";
            if (string.IsNullOrEmpty(account))
            {
                token = getAqtToken();
            }
            else
            {
                token = getAqtToken(account, password);
            }
            
            if (!string.IsNullOrEmpty(token))
                {
                   string retString = HttpNetRequest.POSTSendJsonRequest(url + api, param, new Dictionary<string, string>() { { "accessToken", token } });
                    if (!string.IsNullOrEmpty(retString))
                    {
                      return retString;
                      
                    }
                    else
                    {
                        _logger.LogInformation(url + api + "接口信息返回空");
                        //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                    }
                }
                else
                {
                    _logger.LogInformation(url + api + "未获取到Token信息");
                    //return ResponseOutput.NotOk(db.Domain + api + "未获取到Token信息");
                }

                return "";

        }

        public string UrlRequest(string api, Dictionary<string, object> keyValues,string apiurl = "")
        {
            if (string.IsNullOrEmpty(apiurl))
            {
                apiurl = geturl;
            }
                var token = getAqtToken();

                if (!string.IsNullOrEmpty(token))
                {
                   
                    var retString = HttpNetRequest.SendRequest(apiurl + api, keyValues, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });
                    if (!string.IsNullOrEmpty(retString))
                    {
                  
                    return retString;

                    }
                    else
                    {
                        _logger.LogInformation(url + api + "接口信息返回空");
                        //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                    }
                }
                else
                {
                    _logger.LogInformation(url + api + "未获取到Token信息");
                    //return ResponseOutput.NotOk(db.Domain + api + "未获取到Token信息");
                }
            return "";

        }

        /// <summary>
        /// center请求net的token
        /// </summary>
        public string getAqtToken(string account= "18961354088", string password= "BFCD9B999841963FFA7D3491C93515F9")
        {
            string key = string.Format(CacheKey.CenterNetToken, "AqtToken");
            string token = _cache.Get(key);
            if (string.IsNullOrEmpty(token))
            {
                try
                {
                    mutexcenter.WaitOne();
                    token = _cache.Get(key);
                    if (!string.IsNullOrEmpty(token))
                    {
                        mutexcenter.ReleaseMutex();
                        return token;
                    }
                    string Url = tokenurl + "GetToken";
                    JObject jsonObject = new JObject();
                    jsonObject.Add("account", account);
                    jsonObject.Add("password", password);
                    string retString = HttpNetRequest.POSTSendJsonRequest(Url, jsonObject.ToString(), null);
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj != null && resObj.Code == 0)
                        {
                            string accessToken = resObj.data["accessToken"].ToString();
                            accessToken = accessToken.Replace("bearer ", "");
                            _cache.Set(key, accessToken, TimeSpan.FromMinutes(30));
                            mutexcenter.ReleaseMutex();
                            return accessToken;
                        }
                        else
                        {
                            mutexcenter.ReleaseMutex();
                            return string.Empty;
                        }
                    }
                    else
                    {
                        mutexcenter.ReleaseMutex();
                        return string.Empty;
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    mutexcenter.ReleaseMutex();
                    return string.Empty;
                }
                
            }
            else
            {
                return token;
            }
        }
    }
}
