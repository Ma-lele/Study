using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    public class CenterToken
    {
        private readonly ICache _cache;
        private readonly ILogger<CenterToken> _logger;
        private readonly IConfiguration _configuration;
        private static readonly Mutex mutex = new Mutex();
        private static readonly Mutex mutexcenter = new Mutex();
        public CenterToken(ICache cache, ILogger<CenterToken> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }


        public string FormRequest(string url, string api, Dictionary<string, object> keyValues)
        {
          
                var token = GetCenterToken(url);

                if (!string.IsNullOrEmpty(token))
                {
                    
                    var retString = HttpNetRequest.PostSendRequestUrl(url + api, keyValues, "POST",new Dictionary<string, string>() { { "Authorization",  "Bearer " + token } });
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
        public string JsonRequest(string url, string api, string param)
        {
            var token = GetCenterToken(url);
            if (!string.IsNullOrEmpty(token))
                {
                WebHeaderCollection header = new WebHeaderCollection();
                header.Add("Authorization", "Bearer " +token);
                string retString = UHttp.Post(url + api, param, UHttp.CONTENT_TYPE_JSON, header);
                //string retString = HttpNetRequest.POSTSendJsonRequest(url + api, param, new Dictionary<string, string>() { { "accessToken", token } });
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
        public string GetCenterToken(string url)
        {
            string key = string.Format(CacheKey.CenterNetToken, "CenterToken:" + _configuration.GetSection("CenterKeySecret:Key"));
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
                    string Url = url + "/api/access/token";
                    string retString = null;
                    try
                    {
                        retString = HttpNetRequest.PostForm(Url, new Dictionary<string, object>() { { "key", _configuration.GetSection("CenterKeySecret:Key").Value }, { "secret", _configuration.GetSection("CenterKeySecret:Secret").Value } }, null);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
       
                    //string retString = HttpNetRequest.POSTSendJsonRequest(Url, JsonConvert.SerializeObject(jsonObject), null);
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj != null && resObj.success)
                        {
                            string accessToken = resObj.data["token"].ToString();
                            accessToken = accessToken.Replace("bearer ", "");
                            _cache.Set(key, accessToken, TimeSpan.FromMinutes(Convert.ToDouble(resObj.data["expires"])));
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
