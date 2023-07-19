using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.UserService;
using XHS.Build.Services.Site;
using System.Data;
using XHS.Build.Common.Configs;
using Newtonsoft.Json.Linq;

namespace XHS.Build.SiteFront.Controllers
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
        private readonly ISiteService _siteService;
        private readonly IMapper _mapper;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly JwtConfig _jwtConfig;
        private readonly IUserService _userService;
        /// <summary>
        /// 用户登录Controller
        /// </summary>
        /// <param name="siteService"></param>
        /// <param name="userService"></param>
        /// <param name="mapper"></param>
        /// <param name="cache"></param>
        /// <param name="hpSystemSetting"></param>
        public LoginController(JwtConfig jwtConfig, ISiteService siteService, IUserToken userToken, IUserService userService, IMapper mapper, ICache cache, IHpSystemSetting hpSystemSetting)
        {
            _jwtConfig = jwtConfig;
            _siteService = siteService;
            _userService = userService;
            _userToken = userToken;
            _cache = cache;
            _mapper = mapper;
            _hpSystemSetting = hpSystemSetting;
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

            var pwd = $"0x{UEncrypter.EncryptBySHA1(input.Password).Replace("-", string.Empty).ToLower()}";
            var user = await _userService.QueryUser(input.LoginName, pwd);
            if (user != null && !string.IsNullOrWhiteSpace(user.UUID))
            {
                var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, user.UUID);
                await _cache.DelAsync(key);
                //var output = _mapper.Map<LoginUserOutput>(user);
                LoginUserOutput output = new LoginUserOutput
                {
                    GroupId = user.GROUPID,
                    Id = user.USERID.ToString(),
                    UserName = user.username,
                    RoleIds = new System.Collections.Generic.List<string> { user.ROLEID.ToString() },
                    usersitetype = user.usersitetype
                };
                DataTable dataTable = await _siteService.getV2ListByUserId(user.USERID.ToInt());
                
                if (dataTable.Rows.Count > 0)
                {
                    output.siteid = dataTable.Rows[0]["SITEID"].ToInt();
                    output.sitename = dataTable.Rows[0]["sitename"].ToString();
                }

                return GetToken(output);
            }
            else
            {
                return ResponseOutput.NotOk("用户名或密码错误");
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="data">登录信息</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseOutput> SingleLogin(string data)
        {
            try
            {
                int expire = 0;
                if (!string.IsNullOrEmpty(_hpSystemSetting.getSettingValue(Const.Setting.S189)))
                {
                    expire = int.Parse(_hpSystemSetting.getSettingValue(Const.Setting.S189));
                }
                string dataDecryptstr = UEncrypter.DecryptByRSA(HttpUtility.UrlDecode(data), Const.Encryp.PRIVATE_KEY_OTHER);

                string loginname = dataDecryptstr.Split(";")[0];
                DateTime datetime = DateTime.Parse(dataDecryptstr.Split(";")[1]);
                if (datetime.AddMinutes(expire).CompareTo(DateTime.Now) < 0)
                {
                    return ResponseOutput.NotOk("链接已过期。");
                }
                var users = await _userService.Query(a => a.username == loginname && a.status == 0);
                if (users.Any())
                {

                    var user = users.FirstOrDefault();
                    var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, user.USERID);
                    await _cache.DelAsync(key);
                    LoginUserOutput output = new LoginUserOutput
                    {
                        GroupId = user.GROUPID,
                        Id = user.USERID.ToString(),
                        UserName = user.username,
                        RoleIds = new System.Collections.Generic.List<string> { user.ROLEID.ToString() },
                        usersitetype = user.usersitetype
                    };
                    DataTable dataTable = await _siteService.getV2ListByUserId(user.USERID.ToInt());
                    if (dataTable.Rows.Count > 0)
                    {
                        output.siteid = dataTable.Rows[0]["SITEID"].ToInt();
                        output.sitename = dataTable.Rows[0]["sitename"].ToString();
                    }
                    return GetToken(output);

                }
                else
                {
                    return ResponseOutput.NotOk("用户名或密码错误");
                }
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("无效链接。");
            }
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

            if (refreshExpires.ToLong() <= DateTime.Now.ToTimestamp())
            {
                return ResponseOutput.NotOk("登录信息已过期");
            }

            var userId = userClaims.FirstOrDefault(a => a.Type == ClaimAttributes.UserId)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseOutput.NotOk();
            }
            var user = await _userService.QueryById(userId);
            if (user == null)
            {
                return ResponseOutput.NotOk("未找到用户信息");
            }
            LoginUserOutput output = new LoginUserOutput
            {
                GroupId = user.GROUPID,
                Id = user.USERID.ToString(),
                UserName = user.username,
                RoleIds = new System.Collections.Generic.List<string> { user.ROLEID.ToString() },
                usersitetype = user.usersitetype
            };
            DataTable dataTable = await _siteService.getV2ListByUserId(user.USERID.ToInt());
            if (dataTable.Rows.Count > 0)
            {
                output.siteid = dataTable.Rows[0]["SITEID"].ToInt();
                output.sitename = dataTable.Rows[0]["sitename"].ToString();
            }
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
                new Claim(ClaimAttributes.UserId, output.Id==null?string.Empty:output.Id),
                new Claim(ClaimAttributes.UserName, output.UserName==null?string.Empty:output.UserName),
                new Claim(ClaimAttributes.GroupId, output.GroupId.ToString()),
                new Claim(ClaimAttributes.Role,output.RoleIds==null?string.Empty: string.Join(',',output.RoleIds)),
                new Claim(ClaimAttributes.UserSiteType, output.usersitetype.ToString()),
                new Claim(ClaimAttributes.SiteId, output.siteid.ToString()),
                new Claim(ClaimAttributes.SiteName, output.sitename)
            });

            return ResponseOutput.Ok(new { token, expires_in = _jwtConfig.Expires * 60 });
        }

        /// <summary>
        /// test
        /// </summary>
        /// <param name="devicecode"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IResponseOutput WsTest(string devicecode)
        {
            string publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDjaRXec5TSJo2beuuUmK/xBcpojvHpxvEoBXWErLfdj0JeBZ1p1nEs8B0arFbGAvlmeI+gj8R/PYsJm1wtMX/7h20rN+vlj2KSBMA/sBwfR7Ufa8RH6NbqECd9/LPuxhiuc6pkp1TAIb4BQfnLdfHh5oJWkjhMUp4bnEd9NdzUWQIDAQAB";
            JObject obj = new JObject();
            obj["DeviceCode"] = devicecode;
            obj["DateTime"] = DateTime.Now;

            string encryptStr = UEncrypter.EncryptByRSA16(obj.ToString(), publicKey);

            return ResponseOutput.Ok(encryptStr);
        }
    }
}
