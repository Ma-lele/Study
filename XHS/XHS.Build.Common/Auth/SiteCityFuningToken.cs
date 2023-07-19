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
    public class SiteCityFuningToken
    {
        private readonly ICache _cache;
        private readonly ILogger<SiteCityFuningToken> _logger;
        private readonly IConfiguration _configuration;
        private static readonly Mutex mutex = new Mutex();
        private static readonly Mutex mutexcenter = new Mutex();
        public SiteCityFuningToken(ICache cache, ILogger<SiteCityFuningToken> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }


        public string FormRequest(string url, string account, string password, string api, Dictionary<string, object> keyValues)
        {
          
                var token = getSiteCityToken(url, account, password);

                if (!string.IsNullOrEmpty(token))
                {
                    
                    var retString = HttpNetRequest.PostForm(url + api, keyValues, new Dictionary<string, string>() { { "accessToken",  token } });
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj.Code == 0)
                        {
                            if (resObj.success)
                            {
                                return retString;
                            }
                            else
                            {
                                _logger.LogInformation(url + "请求" + api + "获取返回内容" + retString);
                                //return ResponseOutput.NotOk(resObj.msg, resObj.data);
                            }
                        }
                        else
                        {
                            _logger.LogInformation(url + api + "接口信息错误：" + retString);
                                //return ResponseOutput.NotOk(db.Domain + api + "接口信息错误：" + retString);                            
                        }

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
        public string JsonRequest(string url, string account, string password, string api, string param)
        {
            var token = getSiteCityToken(url, account, password);
            if (!string.IsNullOrEmpty(token))
                {
               // WebHeaderCollection header = new WebHeaderCollection();
               // header.Add("Authorization", token);
                //string retString = UHttp.Post(url + api, param, UHttp.CONTENT_TYPE_JSON, header);
                string retString = HttpNetRequest.POSTSendJsonRequest(url + api, param, new Dictionary<string, string>() { });
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
        public string getSiteCityToken(string url, string account, string password)
        {
            url = "http://49.4.68.132:8094/api/";
            string key = string.Format(CacheKey.CenterNetToken, "FuningSiteCityToken:"+ account);
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
                    string Url = url + "getToken";
                    JObject jsonObject = new JObject();
                    string retString = "";                   
                    Url = url + "Security/Token?appkey=" + account + "&appsecret=" + password;
                    retString = UHttp.Get(Url);

                    //string retString = HttpNetRequest.POSTSendJsonRequest(Url, JsonConvert.SerializeObject(jsonObject), null);
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj != null && resObj.Code == 200)
                        {
                            string accessToken = resObj.data["accessToken"].ToString();
                            accessToken = accessToken.Replace("bearer ", "");
                            _cache.Set(key, accessToken, TimeSpan.FromMinutes(Convert.ToDouble(resObj.data["expireTime"])/1000 - 10));
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
                    if (ex is WebException && ((WebException)ex).Status == WebExceptionStatus.ProtocolError)
                    {
                        WebResponse errResp = ((WebException)ex).Response;
                        using (Stream respStream = errResp.GetResponseStream())
                        {
                            // read the error response
                            var reader = new StreamReader(respStream, Encoding.UTF8);
                            string ss = reader.ReadToEnd();
                            return ss;
                        }
                    }
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
