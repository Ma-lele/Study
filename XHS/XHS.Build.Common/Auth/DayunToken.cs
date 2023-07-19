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
using System.util;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;

namespace XHS.Build.Common.Auth
{
    public class DayunToken
    {
        private readonly ICache _cache;
        private readonly ILogger<DayunToken> _logger;
        private readonly IConfiguration _configuration;
        private static readonly Mutex mutex = new Mutex();
        private static readonly Mutex mutexcenter = new Mutex();
        public DayunToken(ICache cache, ILogger<DayunToken> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }




        /// <summary>
        /// 获取时间戳(1970年1月1日零点整至当前时间的毫秒数)
        /// </summary>
        /// <returns></returns>
        private static long Timestamp
        {
            get { return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000; }
        }

        private string _GetPass()
        {
            string result = string.Empty;
            result = string.Format("{0}:{1}", "tfFW6dBPDVTcigf2", Timestamp);
            result = UEncrypter.EncryptAES128ECBPKCS5Padding(result, "9lJl4fSvWBjAb5qR");
            return result;
        }


        /// <summary>
        /// 获取token
        /// </summary>
        private string GetToken(string website,bool reflash = false)
        {
            string key = string.Format(CacheKey.CenterNetToken, "DayunToken:" + website);
            string token = _cache.Get(key);
            if (reflash)
            {
                token = "";
            }
            if (string.IsNullOrEmpty(token))
            {
                try
                {
                    mutexcenter.WaitOne();
                    token = _cache.Get(key);
                    if (reflash)
                    {
                        token = "";
                    }
                    if (!string.IsNullOrEmpty(token))
                    {
                        mutexcenter.ReleaseMutex();
                        return token;
                    }
                    string pass = "{\"pass\":\"" + _GetPass() + "\"}";
                    string tokenresp = UHttp.Post($"{website}/rest/Token/get/sysZzJI5X6RBSsWAPCa", pass, UHttp.CONTENT_TYPE_JSON);
                    if (!string.IsNullOrEmpty(tokenresp))
                    {
                        JObject resObj = new JObject();
                        resObj = JObject.Parse(tokenresp);
                        if (resObj.ContainsKey("result"))
                        {
                            string accessToken = Convert.ToString(resObj["result"]["access_token"]);
                            _cache.Set(key, accessToken, TimeSpan.FromMilliseconds(Convert.ToDouble(Convert.ToString(resObj["result"]["expiresIn"]))- 3600000));
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

        public string JsonRequest(string website, string api, JObject data, bool reflash = false)
        {
            var token = GetToken(website, reflash);
            if (!string.IsNullOrEmpty(token))
            {
                SortedDictionary<string, string> jparam = new SortedDictionary<string, string>();
                StringBuilder sb = new StringBuilder(string.Empty);
                jparam = new SortedDictionary<string, string>();
                sb = new StringBuilder(string.Empty);
                foreach (JProperty jProperty in data.Properties())
                {
                    jparam.Add(jProperty.Name, jProperty.Value.ToString());
                }
                foreach (var item in jparam)
                {
                    sb.Append(item.Value);
                }
                sb.Append(token);
                string sign = UEncrypter.SHA256(sb.ToString());
                string rtdUrl = string.Format("{0}/{1}/{2}/{3}", website, api, "sysZzJI5X6RBSsWAPCa", sign);
                string response = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(jparam), UHttp.CONTENT_TYPE_JSON);
                if (!string.IsNullOrEmpty(response))
                {
                    try
                    {
                        JObject job = JObject.Parse(response);
                        if (job.ContainsKey("flag") && ("0010".Equals(job.GetValue("flag").ToString())
                            || "0007".Equals(job.GetValue("flag").ToString())))
                        {
                            if (reflash)
                            {
                                return response;
                            }
                            else
                            {
                                JsonRequest(website, api, data, true);
                            }
                            
                        }
                        else
                        {
                            return response;
                        }
                    }
                    catch(Exception ex)
                    {
                        return response;
                    }
                          
                    
                }
                else
                {
                    _logger.LogInformation(website + api + "接口信息返回空");
                    //return ResponseOutput.NotOk(db.Domain + api + "接口信息返回空");
                }
            }
            else
            {
                _logger.LogInformation(website + api + "未获取到Token信息");
                //return ResponseOutput.NotOk(db.Domain + api + "未获取到Token信息");
            }

            return "";
        }


        /// <summary>
        /// 获取code
        /// </summary>
        public string getCode(string website, string uploadapi, JObject jObject)
        {
            string key = string.Format(CacheKey.CenterNetToken, "DayunCode:" + website);
            string strCode = _cache.Get(key);
            if (string.IsNullOrEmpty(strCode))
            {
                try
                {
                    mutexcenter.WaitOne();
                    strCode = _cache.Get(key);
                    if (!string.IsNullOrEmpty(strCode))
                    {
                        mutexcenter.ReleaseMutex();
                        return strCode;
                    }
                    string tokenresp = JsonRequest(website, uploadapi, jObject);
                    if (!string.IsNullOrEmpty(tokenresp))
                    {
                        JArray jar = new JArray();
                        JArray jarCode = new JArray();
                        jar = JArray.Parse(tokenresp);
                        foreach (JObject item in jar)
                        {
                            jarCode.Add(item.GetValue("contract_record_code"));
                        }
                        string strcode = JsonConvert.SerializeObject(jarCode);
                        _cache.Set(key, strcode, TimeSpan.FromDays(Convert.ToDouble(1)));
                        mutexcenter.ReleaseMutex();
                        return strcode;
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
                return strCode;
            }
        }
    }
}
