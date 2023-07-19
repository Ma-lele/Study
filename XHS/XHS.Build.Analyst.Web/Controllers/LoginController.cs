using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Role;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.User;
using XHS.Build.Services.UserService;

namespace XHS.Build.Analyst.Web.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IUserToken _userToken;
        private readonly ICache _cache;
        private readonly IMapper _mapper;
        private readonly IUserRoleService _userRoleService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="sysUserService"></param>
        /// <param name="userToken"></param>
        /// <param name="mapper"></param>
        /// <param name="userRoleService"></param>
        /// <param name="cache"></param>
        public LoginController(ISysUserService sysUserService, IUserToken userToken, IMapper mapper, 
            IUserRoleService userRoleService, ICache cache, IConfiguration configuration, IUserService userService)
        {
            _cache = cache;
            _userToken = userToken;
            _mapper = mapper;
            _userRoleService = userRoleService;
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="input">登录信息</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseOutput> Login(LoginUserInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.LoginName) || string.IsNullOrEmpty(input.Password))
            {
                return ResponseOutput.NotOk("请输入用户名或密码");
            }

            var pwd = $"0x{UEncrypter.EncryptBySHA1(input.Password).Replace("-","").ToLower()}";
            var user = await _userService.QueryUser(input.LoginName, pwd);
            if (user != null && !string.IsNullOrWhiteSpace(user.UUID))
            {
                var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, user.UUID);
                await _cache.DelAsync(key);
                LoginUserOutput output = new LoginUserOutput
                {
                    GroupId = user.GROUPID,
                    Id = user.UUID.ToString(),
                    UserName = user.username,
                    RoleIds = new System.Collections.Generic.List<string> { user.ROLEID.ToString() },
                    userregion = user.userregion
                };
                return GetToken(output);
            }
            else
            {
                return ResponseOutput.NotOk("用户名或密码错误");
            }
        }


        /// <summary>
        /// API登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseOutput> ApiLogin(string data)
        { 
            if (string.IsNullOrWhiteSpace(data))
            {
                return ResponseOutput.NotOk("参数错误");
            }
 

            string privateKey = _configuration.GetSection("WebConfig").GetValue<string>("PrivateKey");
            string decryptStr = string.Empty;
            try
            {
                decryptStr = UEncrypter.DecryptByRSA16(data, privateKey);
            }
            catch {
                return ResponseOutput.NotOk("参数错误");
            }
            if (!decryptStr.TryParseJson(out JObject jData))
            {
                return ResponseOutput.NotOk("参数错误");
            }

            if (!jData.ContainsKey("username") || !jData.ContainsKey("timestamp")
                || string.IsNullOrWhiteSpace(jData["username"].ToString()) || string.IsNullOrWhiteSpace(jData["timestamp"].ToString()))
            {
                return ResponseOutput.NotOk("参数错误");
            }
           //var jData = JsonConvert.DeserializeObject<JObject>(decryptStr);

            string uuid = jData["username"].ToString();
            DateTime queryTime = jData["timestamp"].ToLong().ToDateTime(true);
            if ((DateTime.Now - queryTime).TotalSeconds > 600)
            {
                return ResponseOutput.NotOk("请求超时");
            }

            var user = await _userService.GetUserByUUID(uuid);
            if (user == null || string.IsNullOrWhiteSpace(user.USERID))
            {
                return ResponseOutput.NotOk("账户不存在");
            }

            LoginUserOutput output = new LoginUserOutput
            {
                GroupId = user.GROUPID,
                Id = user.UUID.ToString(),
                UserName = user.username,
                RoleIds = new System.Collections.Generic.List<string> { user.ROLEID.ToString() },
                userregion = user.userregion
            };
            return GetToken(output);
        }

        /// <summary>
        /// test
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseOutput> ApiLoginTest(string uuid)
        {
            string publicKey = _configuration.GetSection("WebConfig").GetValue<string>("PublicKey");
            JObject obj = new JObject();
            obj["username"] = uuid;
            obj["timestamp"] = DateTime.Now.ToTimestamp(true);

            string encryptStr = UEncrypter.EncryptByRSA16(obj.ToString(), publicKey);

           


            return ResponseOutput.Ok(encryptStr);
        }


        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> Refresh(string token)
        {
            var userClaims = _userToken.Decode(token);
            if (userClaims == null || userClaims.Length == 0)
            {
                return ResponseOutput.NotOk();
            }

            var refreshExpires = userClaims.FirstOrDefault(a => a.Type == ClaimAttributes.RefreshExpires)?.Value;
            if (string.IsNullOrEmpty(refreshExpires))
            {
                return ResponseOutput.NotOk();
            }

            if (refreshExpires.ToDateTime() <= DateTime.Now)
            {
                return ResponseOutput.NotOk("登录信息已过期");
            }

            var userId = userClaims.FirstOrDefault(a => a.Type == ClaimAttributes.UserId)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseOutput.NotOk();
            }
            var user = await _userService.GetUserByUUID(userId);
            if (user == null)
            {
                return ResponseOutput.NotOk("未找到用户信息");
            }


            LoginUserOutput output = new LoginUserOutput
            {
                GroupId = user.GROUPID,
                Id = user.UUID.ToString(),
                UserName = user.username,
                RoleIds = new System.Collections.Generic.List<string> { user.ROLEID.ToString() },
                userregion = user.userregion
            };
            return GetToken(output);
        }


        /// <summary>
        /// 获得token
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private IResponseOutput GetToken(LoginUserOutput output)
        {
            var token = _userToken.Create(new[]
            {
                new Claim(ClaimAttributes.UserId, output.Id==null?"":output.Id),
                new Claim(ClaimAttributes.UserName, output.UserName==null?"":output.UserName),
                new Claim(ClaimAttributes.LoginName, output.LoginName==null?"":output.LoginName),
                new Claim(ClaimAttributes.Gender, output.Gender==null?"":output.Gender),
                new Claim(ClaimAttributes.GroupId, output.GroupId.ToString()),
                new Claim(ClaimAttributes.Role,output.RoleIds==null?"": string.Join(',',output.RoleIds)),
                new Claim(ClaimAttributes.IsAdmin,output.RoleIds==null?"false":(output.RoleIds.Contains("1")?"true":"false")),
                new Claim(ClaimAttributes.UserRegion, output.userregion),
                new Claim(ClaimAttributes.AnalystRegionID, output.userregion)
            });

            return ResponseOutput.Ok(new { token, expires_in = _configuration.GetSection("JWTConfig:refreshExpires").Value.ObjToInt() * 60 });
        }
    }
}
