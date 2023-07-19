using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Response;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AccessController : ControllerBase
    {
        private readonly ConfigHelper _configHelper;
        private readonly IUserKeyToken _userToken;
        private IConfiguration _configuration;
        public AccessController(IConfiguration configuration, IUserKeyToken userToken)
        {
            _configHelper = new ConfigHelper();
            _userToken = userToken;
            _configuration = configuration;
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

            var keyList = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
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
            var keyList = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
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
                new Claim(KeyClaimAttributes.GroupId, keySecret.GroupId==null?"":keySecret.GroupId),
                new Claim(KeyClaimAttributes.Name, keySecret.Name)
            });

            return ResponseOutput.Ok(new { Token = token, Expires = _configuration.GetSection("JWTConfig:expires").Value });
        }

    }
}
