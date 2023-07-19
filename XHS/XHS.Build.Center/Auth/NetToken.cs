using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;

namespace XHS.Build.Center.Auth
{
    public class NetToken
    {
        private readonly ICache _cache;
        private readonly ILogger<NetToken> _logger;
        private readonly IUserKey _userKey;
        private readonly IConfiguration _configuration;
        private static readonly Mutex mutex = new Mutex();
        private static readonly Mutex mutexcenter = new Mutex();
        public NetToken(ICache cache, ILogger<NetToken> logger, IUserKey userKey, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _userKey = userKey;
            _configuration = configuration;
        }

        /// <summary>
        /// 各个用户自己net的token
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string Token(string domain, string port)
        {
            string key = string.Format(CacheKey.UserCenterNetToken, _userKey.Key);
            string token = _cache.Get(key);
            if (string.IsNullOrEmpty(token))
            {
                try
                {
                    mutex.WaitOne();
                    token = _cache.Get(key);
                    if (!string.IsNullOrEmpty(token))
                    {
                        mutex.ReleaseMutex();
                        return token;
                    }
                    var keyApis = _cache.Get<KeySecretList>("keysecretconfig");
                    if (keyApis == null)
                    {
                        keyApis = new ConfigHelper().Get<KeySecretList>("keysecretconfig", "", true);
                        _cache.Set("keysecretconfig", keyApis, TimeSpan.FromHours(1));
                    }
                    var KA = keyApis.Items.FirstOrDefault(k => k.Key == _userKey.Key);
                    if (KA == null || string.IsNullOrEmpty(KA.Key) || string.IsNullOrEmpty(KA.Secret))
                    {
                        mutex.ReleaseMutex();
                        return string.Empty;
                    }
                    string Url = "http://" + domain + ":" + port + "/api/access/token";
                    string retString = HttpNetRequest.PostForm(Url, new Dictionary<string, object>() { { "key", KA.Key }, { "secret", KA.Secret } }, null);
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj != null && resObj.success)
                        {
                            _cache.Set(key, resObj.data["token"], TimeSpan.FromMinutes(Convert.ToDouble(resObj.data["expires"]) - 10));
                            mutex.ReleaseMutex();
                            return resObj.data["token"];
                        }
                        else
                        {
                            mutex.ReleaseMutex();
                            return string.Empty;
                        }
                    }
                    else
                    {
                        mutex.ReleaseMutex();
                        return string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(domain + ex.ToString());
                    mutex.ReleaseMutex();
                    return string.Empty;
                }

            }
            else
            {
                return token;
            }
        }
        /// <summary>
        /// 各个用户自己net的token
        /// </summary>
        public string Refresh(string domain, string port, string oldtoken)
        {
            string Url = "http://" + domain + ":" + port + "/api/access/refresh";
            string retString = HttpNetRequest.SendRequest(Url, new Dictionary<string, object>() { { "token", oldtoken } }, "GET", null);
            if (!string.IsNullOrEmpty(retString))
            {
                var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                if (resObj != null && resObj.success)
                {
                    string key = string.Format(CacheKey.UserCenterNetToken, _userKey.Key);
                    _cache.Set(key, resObj.data["token"], TimeSpan.FromMinutes(Convert.ToDouble(resObj.data["expires"])));
                    return resObj.data["token"];
                }
                else
                {
                    return Token(domain, port);
                }
            }
            else
            {
                return Token(domain, port);
            }
        }

        public void FormRequest(string api, List<DeviceBindOutput> outputs, Dictionary<string, object> keyValues)
        {
            foreach (var db in outputs)
            {
                var token = Token(db.Domain, db.Netport.ToString());

                if (!string.IsNullOrEmpty(token))
                {
                    string url = "http://" + db.Domain;
                    if (!string.IsNullOrEmpty(db.Netport))
                    {
                        url += ":" + db.Netport;
                    }
                    var retString = HttpNetRequest.PostForm(url + api, keyValues, new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj.Code == 0)
                        {
                            if (resObj.success)
                            {
                                continue;
                            }
                            else
                            {
                                _logger.LogInformation(db.Domain + "请求" + api + "获取返回内容" + retString);
                                //return ResponseOutput.NotOk(resObj.msg, resObj.data);
                            }
                        }
                        else
                        {
                            if (resObj.Code == 401)
                            {
                                var rToken = Refresh(db.Domain, db.Netport, token);
                                retString = HttpNetRequest.PostForm(url + api, keyValues, new Dictionary<string, string>() { { "Authorization", "Bearer " + rToken } });
                                if (!string.IsNullOrEmpty(retString))
                                {
                                    if (resObj.success)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        _logger.LogInformation(db.Domain + "请求" + api + "获取返回内容" + retString);
                                        //return ResponseOutput.NotOk(resObj.msg, resObj.data);
                                    }
                                }
                                else
                                {
                                    _logger.LogInformation(db.Domain + api + "接口信息返回空");
                                    //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                                }
                            }
                            else
                            {
                                _logger.LogInformation(db.Domain + api + "接口信息错误：" + retString);
                                //return ResponseOutput.NotOk(db.Domain + api + "接口信息错误：" + retString);
                            }
                        }

                    }
                    else
                    {
                        _logger.LogInformation(db.Domain + api + "接口信息返回空");
                        //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                    }
                }
                else
                {
                    _logger.LogInformation(db.Domain + api + "未获取到Token信息");
                    //return ResponseOutput.NotOk(db.Domain + api + "未获取到Token信息");
                }
            }
            //return ResponseOutput.Ok();

        }

        public void JsonRequest(string api, List<DeviceBindOutput> outputs, dynamic param)
        {
            foreach (var db in outputs)
            {
                var token = Token(db.Domain, db.Netport.ToString());
                if (!string.IsNullOrEmpty(token))
                {
                    string url = "http://" + db.Domain;
                    if (!string.IsNullOrEmpty(db.Netport))
                    {
                        url += ":" + db.Netport;
                    }
                    string retString = null;
                    try
                    {
                        retString = HttpNetRequest.POSTSendJsonRequest(url + api, JsonConvert.SerializeObject(param), new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(db.Domain + "请求" + api + "接口调用失败：" + ex.Message);
                    }
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj.Code == 0)
                        {
                            if (resObj.success)
                            {
                                continue;
                            }
                            else
                            {
                                _logger.LogInformation(db.Domain + "请求" + api + "获取返回内容" + retString);
                                //return ResponseOutput.NotOk(resObj.msg, resObj.data);
                            }
                        }
                        else
                        {
                            if (resObj.Code == 401)
                            {
                                var rToken = Refresh(db.Domain, db.Netport, token);
                                retString = HttpNetRequest.PostForm(url + api, JsonConvert.SerializeObject(param), new Dictionary<string, string>() { { "Authorization", "Bearer " + rToken } });
                                if (!string.IsNullOrEmpty(retString))
                                {
                                    if (resObj.success)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        _logger.LogInformation(db.Domain + "请求" + api + "获取返回内容" + retString);
                                        //return ResponseOutput.NotOk(resObj.msg, resObj.data);
                                    }
                                }
                                else
                                {
                                    _logger.LogInformation(db.Domain + api + "接口信息返回空");
                                    //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                                }
                            }
                            else
                            {
                                _logger.LogInformation(db.Domain + api + "接口信息错误：" + retString);
                                //return ResponseOutput.NotOk(db.Domain + api + "接口信息错误：" + retString);
                            }
                        }

                    }
                    else
                    {
                        _logger.LogInformation(db.Domain + api + "接口信息返回空");
                        //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                    }
                }
                else
                {
                    _logger.LogInformation(db.Domain + api + "未获取到Token信息");
                    //return ResponseOutput.NotOk(db.Domain + api + "未获取到Token信息");
                }
            }
            //return ResponseOutput.Ok();

        }

        public void UrlRequest(string api, string method, List<DeviceBindOutput> outputs, Dictionary<string, object> keyValues)
        {
            foreach (var db in outputs)
            {
                var token = Token(db.Domain, db.Netport.ToString());

                if (!string.IsNullOrEmpty(token))
                {
                    string url = "http://" + db.Domain;
                    if (!string.IsNullOrEmpty(db.Netport))
                    {
                        url += ":" + db.Netport;
                    }
                    var retString = HttpNetRequest.PostSendRequestUrl(url + api, keyValues, method, new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj.Code == 0)
                        {
                            if (resObj.success)
                            {
                                continue;
                            }
                            else
                            {
                                _logger.LogInformation(db.Domain + "请求" + api + "获取返回内容" + retString);
                                //return ResponseOutput.NotOk(resObj.msg, resObj.data);
                            }
                        }
                        else
                        {
                            if (resObj.Code == 401)
                            {
                                var rToken = Refresh(db.Domain, db.Netport, token);
                                retString = HttpNetRequest.PostSendRequestUrl(url + api, keyValues, method, new Dictionary<string, string>() { { "Authorization", "Bearer " + rToken } });
                                if (!string.IsNullOrEmpty(retString))
                                {
                                    if (resObj.success)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        _logger.LogInformation(db.Domain + "请求" + api + "获取返回内容" + retString);
                                        //return ResponseOutput.NotOk(resObj.msg, resObj.data);
                                    }
                                }
                                else
                                {
                                    _logger.LogInformation(db.Domain + api + "接口信息返回空");
                                    //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                                }
                            }
                            else
                            {
                                _logger.LogInformation(db.Domain + api + "接口信息错误：" + retString);
                                //return ResponseOutput.NotOk(db.Domain + api + "接口信息错误：" + retString);
                            }
                        }

                    }
                    else
                    {
                        _logger.LogInformation(db.Domain + api + "接口信息返回空");
                        //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                    }
                }
                else
                {
                    _logger.LogInformation(db.Domain + api + "未获取到Token信息");
                    //return ResponseOutput.NotOk(db.Domain + api + "未获取到Token信息");
                }
            }
            //return ResponseOutput.Ok();

        }

        /// <summary>
        /// center请求net的token
        /// </summary>
        public string CenterToken(string domain, string port)
        {
            string key = string.Format(CacheKey.CenterNetToken, domain);
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
                    string Url = "http://" + domain + ":" + port + "/api/access/token";
                    string retString = HttpNetRequest.PostForm(Url, new Dictionary<string, object>() { { "key", _configuration.GetSection("NetKeySecret:Key").Value }, { "secret", _configuration.GetSection("NetKeySecret:Secret").Value } }, null);
                    if (!string.IsNullOrEmpty(retString))
                    {
                        var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                        if (resObj != null && resObj.success)
                        {
                            _cache.Set(key, resObj.data["token"], TimeSpan.FromMinutes(Convert.ToDouble(resObj.data["expires"]) - 10));
                            mutexcenter.ReleaseMutex();
                            return resObj.data["token"];
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
                    _logger.LogError(domain + " : " + ex.ToString());
                    mutexcenter.ReleaseMutex();
                    return string.Empty;
                }

            }
            else
            {
                return token;
            }
        }

        /// <summary>
        /// center请求net的token
        /// </summary>
        public string CenterRefresh(string domain, string port, string oldtoken)
        {
            string Url = "http://" + domain + ":" + port + "/api/access/refresh";
            string retString = HttpNetRequest.SendRequest(Url, new Dictionary<string, object>() { { "token", oldtoken } }, "GET", null);
            if (!string.IsNullOrEmpty(retString))
            {
                var resObj = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(retString);
                if (resObj != null && resObj.success)
                {
                    string key = string.Format(CacheKey.CenterNetToken, domain);// _userKey.Key + CacheKey.CustomeNetToken;
                    _cache.Set(key, resObj.data["token"], TimeSpan.FromMinutes(Convert.ToDouble(resObj.data["expires"])));
                    return resObj.data["token"];
                }
                else
                {
                    return Token(domain, port);
                }
            }
            else
            {
                return Token(domain, port);
            }
        }
    }
}
