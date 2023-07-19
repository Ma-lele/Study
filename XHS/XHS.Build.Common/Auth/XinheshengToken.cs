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
    public class XinheshengToken
    {
        private readonly ICache _cache;
        private readonly ILogger<XinheshengToken> _logger;
        private readonly IConfiguration _configuration;
        private static readonly Mutex mutex = new Mutex();
        private static readonly Mutex mutexcenter = new Mutex();
        public XinheshengToken(ICache cache, ILogger<XinheshengToken> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }


        public string FormRequest(string url, string account, string password, string api, Dictionary<string, object> keyValues)
        {

            var token = getCityToken(url, account, password);

            if (!string.IsNullOrEmpty(token))
            {

                var retString = HttpNetRequest.PostForm(url + api, keyValues, new Dictionary<string, string>() { { "accessToken", token } });
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
            string retString = "";
            Dictionary<string, object> requestParams = new Dictionary<string, object>();
            var token = getCityToken(url, account, password);
            if (!string.IsNullOrEmpty(token))
            {

                retString = HttpNetRequest.POSTSendJsonRequest(url + api, param, new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });

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
        public string getCityToken(string url, string account, string password)
        {
            string key = string.Format(CacheKey.CenterNetToken, "CityToken:" + account);
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
                    string Url = url + "api/Access/Token";
                    Dictionary<string, object> ParamsValues = new Dictionary<string, object>();
                    ParamsValues.Add("key", account);
                    ParamsValues.Add("secret", password);
                    string retString = "";
                    retString = HttpNetRequest.PostForm(Url, ParamsValues, null);


                    //string retString = HttpNetRequest.POSTSendJsonRequest(Url, JsonConvert.SerializeObject(jsonObject), null);
                    if (!string.IsNullOrEmpty(retString))
                    {
                        JObject resObj = JObject.Parse(retString);

                        if (resObj != null && (bool)resObj["success"] == true)
                        {
                            string accessToken = resObj["data"]["token"].ToString();
                            accessToken = accessToken.Replace("bearer ", "");
                            _cache.Set(key, accessToken, TimeSpan.FromMinutes(Convert.ToDouble(resObj["data"]["expires"]) - 60));
                            mutexcenter.ReleaseMutex();
                            return accessToken;
                            //resObj["result"]["access_token"]
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
                catch (Exception ex)
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
