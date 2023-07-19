using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Response;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.KeySecretConfig;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 登录认证
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [AllowAnonymous]
    public class AccessController : ControllerBase
    {
        private readonly ConfigHelper _configHelper;
        private readonly IUserKeyToken _userToken;
        private IConfiguration _configuration;
        private readonly ICache _cache;
        private readonly IKeySecretService _keySecretService;
        public AccessController(IConfiguration configuration, ICache cache,IUserKeyToken userToken, IKeySecretService keySecretService)
        {
            _configHelper = new ConfigHelper();
            _userToken = userToken;
            _configuration = configuration;
            _cache = cache;
            _keySecretService = keySecretService;
        }
        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Token([FromForm] string key, [FromForm] string secret)
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
            {
                return ResponseOutput.NotOk<string>("输入信息有误");
            }
            string dAppId = key;//UEncrypter.DecryptByRSA16(key, MyConfig.Webconfig.PrivateKey);
            string dSecret = secret;//UEncrypter.DecryptByRSA16(secret, MyConfig.Webconfig.PrivateKey);
            var keyList = _cache.Get<KeySecretList>("keysecretconfig");
            if (keyList == null)
            {
                var keyList1 = _keySecretService.Query();
                List<KeySecret> list = new List<KeySecret>();
                for (int i = 0; i < keyList1.Result.Count; i++)
                {
                    TCCKeySecretConfig con = keyList1.Result[i];
                    KeySecret keySecret1 = new KeySecret();
                    keySecret1.Key = con.Key;
                    keySecret1.Secret = con.Secret;
                    keySecret1.UserId = con.UserId;
                    keySecret1.GroupId = con.GroupId;
                    keySecret1.Name = con.Name;
                    keySecret1.Apis = con.Apis.Split(",");
                    list.Add(keySecret1);
                }
                keyList = new KeySecretList();
                keyList.Items = list;
                _cache.Set("keysecretconfig", keyList, TimeSpan.FromHours(1));
            }
            //var keyList = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
            if (keyList == null)
            {
                return ResponseOutput.NotOk<string>("输入信息有误");
            }
            var keySecret = keyList.Items.FirstOrDefault(k => k.Key == dAppId && k.Secret == dSecret);
            if (keySecret == null || string.IsNullOrEmpty(keySecret.Key) || string.IsNullOrEmpty(keySecret.Secret))
            {
                return ResponseOutput.NotOk<string>("输入信息有误");
            }

            var token = _userToken.Create(new[]
            {
                new Claim(KeyClaimAttributes.Key, keySecret.Key),
                new Claim(KeyClaimAttributes.GroupId, keySecret.GroupId==null?"":keySecret.GroupId),
                new Claim(KeyClaimAttributes.UserId, keySecret.UserId==null?string.Empty:keySecret.UserId),
                new Claim(KeyClaimAttributes.Name, keySecret.Name)
            });
            return ResponseOutput.Ok(new { Token = token, Expires = _configuration.GetSection("JWTConfig:expires").Value });
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Refresh(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return ResponseOutput.NotOk("请输入用户凭证");
            }

            var userClaims = _userToken.Decode(token);
            if (userClaims == null || userClaims.Length == 0)
            {
                return ResponseOutput.NotOk("获取正确的用户凭证");
            }

            var refreshExpiresValue = userClaims.FirstOrDefault(a => a.Type == KeyClaimAttributes.RefreshExpires)?.Value;
            if (string.IsNullOrEmpty(refreshExpiresValue))
            {
                return ResponseOutput.NotOk("登录信息已过期");
            }

            var refreshExpires = Convert.ToDateTime(refreshExpiresValue);
            if (refreshExpires <= DateTime.Now)
            {
                return ResponseOutput.NotOk("登录信息已过期");
            }

            var key = userClaims.FirstOrDefault(a => a.Type == KeyClaimAttributes.Key)?.Value;
            if (string.IsNullOrEmpty(key))
            {
                return ResponseOutput.NotOk("登录信息已过期");
            }
            var keyList = _cache.Get<KeySecretList>("keysecretconfig");
            if (keyList == null)
            {
                var keyList1 = _keySecretService.Query();
                List<KeySecret> list = new List<KeySecret>();
                for (int i = 0; i < keyList1.Result.Count; i++)
                {
                    TCCKeySecretConfig con = keyList1.Result[i];
                    KeySecret keySecret1 = new KeySecret();
                    keySecret1.Key = con.Key;
                    keySecret1.Secret = con.Secret;
                    keySecret1.UserId = con.UserId;
                    keySecret1.GroupId = con.GroupId;
                    keySecret1.Name = con.Name;
                    keySecret1.Apis = con.Apis.Split(",");
                    list.Add(keySecret1);
                }
                keyList = new KeySecretList();
                keyList.Items = list;
                _cache.Set("keysecretconfig", keyList, TimeSpan.FromHours(1));
            }
            //var keyList = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
            if (keyList == null)
            {
                return ResponseOutput.NotOk<string>("输入信息有误");
            }
            var keySecret = keyList.Items.FirstOrDefault(k => k.Key == key);
            if (keySecret == null || string.IsNullOrEmpty(keySecret.Key) || string.IsNullOrEmpty(keySecret.Secret))
            {
                return ResponseOutput.NotOk("未找到用户信息");
            }

            var r_token = _userToken.Create(new[]
            {
                new Claim(KeyClaimAttributes.Key, key),
                new Claim(KeyClaimAttributes.GroupId, keySecret.GroupId==null?string.Empty:keySecret.GroupId),
                new Claim(KeyClaimAttributes.UserId, keySecret.UserId==null?string.Empty:keySecret.UserId),
                new Claim(KeyClaimAttributes.Name, keySecret.Name)
            });

            return ResponseOutput.Ok(new { Token = token, Expires = _configuration.GetSection("JWTConfig:expires").Value });
        }

    }
}
