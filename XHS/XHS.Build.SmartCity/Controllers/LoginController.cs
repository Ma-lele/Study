using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Role;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.CtUser;
using XHS.Build.Services.RoleRight;
using XHS.Build.Model.Models;
using System.Collections.Generic;

namespace XHS.Build.Admin.Controllers
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
        private readonly IUser _user;
        private readonly ICache _cache;
        private readonly ICtUserService _ctUserService;
        private readonly IMapper _mapper;
        private readonly IRoleRightService _roleRightService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly JwtConfig _jwtConfig;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctUserService"></param>
        /// <param name="jwtConfig"></param>
        /// <param name="userToken"></param>
        /// <param name="user"></param>
        /// <param name="mapper"></param>
        /// <param name="roleRightService"></param>
        /// <param name="cache"></param>
        /// <param name="hpSystemSetting"></param>
        public LoginController(ICtUserService ctUserService, JwtConfig jwtConfig, IUserToken userToken, IUser user, IMapper mapper, IRoleRightService roleRightService, ICache cache, IHpSystemSetting hpSystemSetting)
        {
            _ctUserService = ctUserService;
            _cache = cache;
            _userToken = userToken;
            _user = user;
            _mapper = mapper;
            _roleRightService = roleRightService;
            _hpSystemSetting = hpSystemSetting;
            _jwtConfig = jwtConfig;
        }

        /// <summary>
        ///  发送前端用户登录验证码
        /// </summary>
        /// <param name="input">登录信息</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IResponseOutput> VerifyCode(LoginUserInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.LoginName) || string.IsNullOrEmpty(input.Password))
            {
                return ResponseOutput.NotOk("请输入用户名或密码");
            }
            //   var user = (await _sysUserService.Query(ii => ii.Mobile == mobile && ii.Status == "0" )).FirstOrDefault();
            var users = await _ctUserService.GetUserLogin(input.LoginName, input.Password);
            if (users.Rows.Count > 0)
            {

                var user = users.Rows[0];
                //var output = _mapper.Map<LoginUserOutput>(user);
                //var Roles = await _userRoleService.Query(r => r.Userid == user.Id);
                //if (Roles.Any())
                //{
                //    output.RoleIds = Roles.Select(a => a.Roleid).ToList();
                //}
                var mobile = user["mobile"];
                //bool falg = true;
                if (string.IsNullOrEmpty(user["mobile"].ToString()))
                {
                    return ResponseOutput.NotOk("未找到用户的手机号，无法进行短信验证。");
                }
                Random rdm = new Random((int)DateTime.Now.Ticks);
                string code = rdm.Next(1000, 9999).ToString();
                string ip = IPHelper.GetIP(HttpContext.Request);
                DateTime time;
                if (!string.IsNullOrEmpty(user["verifytime"].ToString()))
                {
                    time = (DateTime)user["verifytime"];
                    if (time.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        code = user["verifycode"].ToString();
                        //falg = false;
                    }
                }

                string key = $"web:user:{user["mobile"]}:verifycode";

                if (!string.IsNullOrWhiteSpace(await _cache.GetAsync<string>(key)))
                {
                    return ResponseOutput.NotOk("请求过于频繁，请稍后再试。");
                }
                else
                {
                    await _cache.SetAsync(key, code, new TimeSpan(0, 1, 0));

                    //todo 调接口发送手机验证码
                    JObject obj = new JObject();
                    obj["code"] = code;
                    HpAliSMS.SendSms(user["mobile"].ToString(), HpAliSMS.MOTION_VERIFYCODE, obj.ToString());
                    //if (falg)
                    //{
                    await _ctUserService.SetVerifyCode(user["mobile"].ToString(), code);
                    //}
                }

                return ResponseOutput.Ok(new { mobile });

            }
            else
            {
                return ResponseOutput.NotOk("用户名或密码错误");
            }


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
            var verifycodeflag = _hpSystemSetting.getSettingValue(Const.Setting.S192);
            if (string.IsNullOrEmpty(verifycodeflag) && string.IsNullOrEmpty(input.RegverifyCode))
            {
                return ResponseOutput.NotOk("请输入验证码");
            }
            else if (!verifycodeflag.Equals("1") && string.IsNullOrEmpty(input.RegverifyCode))
            {
                return ResponseOutput.NotOk("请输入验证码");
            }
            var users = await _ctUserService.GetUserLogin(input.LoginName, input.Password);
            if (users.Rows.Count > 0)
            {
                var user = users.Rows[0];
                var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, user["USERID"]);
                await _cache.DelAsync(key);
                var output = new CTUserEntity();
                output.username = user["username"].ToString();
                output.USERID = Convert.ToInt32(user["USERID"]);
                output.GROUPID = Convert.ToInt32(user["GROUPID"]);
                output.ROLEID = user["ROLEID"].ToString();
                output.userregion = user["userregion"].ToString();

                if (!string.IsNullOrEmpty(verifycodeflag) && verifycodeflag.Equals("1"))
                {

                }
                else
                {
                    string value = user["verifycode"].ToString();
                    if (string.IsNullOrWhiteSpace(value) || value != input.RegverifyCode)
                    {
                        return ResponseOutput.NotOk("验证码错误");
                    }
                    if (user["verifytime"] == null)
                    {
                        return ResponseOutput.NotOk("验证码已过期，请重新获取验证码");
                    }
                    DateTime time = (DateTime)user["verifytime"];
                    if (!time.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        return ResponseOutput.NotOk("验证码已过期，请重新获取验证码");
                    }
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
                var users = await _ctUserService.Query(a => a.username == loginname && a.status == 0);
                if (users.Any())
                {

                    var user = users.FirstOrDefault();
                    var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, user.USERID);
                    await _cache.DelAsync(key);
                    //var output = _mapper.Map<LoginUserOutput>(user);
                    //var Roles = await _userRoleService.Query(r => r.Userid == user.USERID);
                    //if (Roles.Any())
                    //{
                    //    output.RoleIds = Roles.Select(a => a.Roleid).ToList();
                    //}

                    return GetToken(user);

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
        /// 获得登录标题
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResponseOutput getLoginTitle()
        {
            JObject jso = new JObject();
            jso.Add("title", _hpSystemSetting.getSettingValue(Const.Setting.S007));
            string noverifycode = "0";
            if (!string.IsNullOrEmpty(_hpSystemSetting.getSettingValue(Const.Setting.S192)))
            {
                noverifycode = _hpSystemSetting.getSettingValue(Const.Setting.S192);
            }
            jso.Add("noverifycode", noverifycode);
            return ResponseOutput.Ok(jso);
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
            var user = await _ctUserService.QueryById(userId);
            if (user == null)
            {
                return ResponseOutput.NotOk("未找到用户信息");
            }
            //var output = _mapper.Map<LoginUserOutput>(user);
            //var Roles = await _userRoleService.Query(r => r.Userid == user.user);
            //if (Roles.Any())
            //{
            //    output.RoleIds = Roles.Select(a => a.Roleid).ToList();
            //}
            return GetToken(user);
        }


        /// <summary>
        /// 获得token
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private IResponseOutput GetToken(CTUserEntity output)
        {
            var token = _userToken.Create(new[]
            {
                new Claim(ClaimAttributes.UserId, output.USERID.ToString()),
                new Claim(ClaimAttributes.UserName, output.username),
                //new Claim(ClaimAttributes.LoginName, output.LoginName==null?"":output.LoginName),
                //new Claim(ClaimAttributes.Gender, output.Gender==null?"":output.Gender),
                new Claim(ClaimAttributes.GroupId, output.GROUPID.ToString()),
                new Claim(ClaimAttributes.Role,output.ROLEID),
                //new Claim(ClaimAttributes.IsAdmin,output.RoleIds==null?"false":(output.RoleIds.Contains("1")?"true":"false"))
            });
            var userId = output.USERID;
            var menuResult = _roleRightService.GetCtAll(output.ROLEID);
            var menus = menuResult.Result;
            var groupId = output.GROUPID;
            var userregion = output.userregion;
            return ResponseOutput.Ok(new { groupId, userId, userregion, token, menus, expires_in = _jwtConfig.Expires * 60 });
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ResetPassword(CTUserPwd input)
        {
            if (string.IsNullOrEmpty(input.oldpwd) || string.IsNullOrEmpty(input.newpwd))
            {
                return ResponseOutput.NotOk();
            }
            var result = await _ctUserService.ChangePwd(input);
            if (result > 0)
                return ResponseOutput.Ok();

            return ResponseOutput.NotOk("旧密码错误");
        }
    }
}
