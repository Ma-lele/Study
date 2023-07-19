using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.UserService;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IUserToken _userToken;
        private readonly JwtConfig _jwtConfig;
        private readonly IUser _user;
        private readonly IUserService _userService;
        private const string SINGLELOGIN_PUBLIC_KEY = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDIQta6nGt7Tpltgh2mEpIjAEfKi5rtFvOBo5ZN1mVjnURbHs9NfuGGDuKdY+RzwHoZzYtj2QbXsZSDzYRv7YcbEXEqzFsUpW2QtDYKlEJwUP9DT2+WIXGAWKRiZRfkz2Izh0ms7LnJsoEEiWK4gmbPXypi5B2pY0xo3Lmt3xmc/wIDAQAB";
        private const string SINGLELOGIN_PRIVATE_KEY = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAMhC1rqca3tOmW2CHaYSkiMAR8qLmu0W84Gjlk3WZWOdRFsez01+4YYO4p1j5HPAehnNi2PZBtexlIPNhG/thxsRcSrMWxSlbZC0NgqUQnBQ/0NPb5YhcYBYpGJlF+TPYjOHSazsucmygQSJYriCZs9fKmLkHaljTGjcua3fGZz/AgMBAAECgYAYocgBqg26W/+ZLaDx9WTOM1GhQyyqLuGCt5lcN5u+9fAbeR2sfYfF0nzjOQ83gZxDIjob7OzfiPMohxXcIo40eWlD+05jypsJx1DPKq8kgSNAPsBS1DU32KxjxKnPqOS9CzilVhm6dtyuV8TSvxAvMbcxpn3Lp+6pAEi16Ee9oQJBAOdHnGv2Dqd3yx/F0WSRWB1DQNjNdXaHu2OJ9k3323FQzt1HKjGtDxYW0/p2m6IKeCGPIeICQAlF/e+EMOlMgW8CQQDdqnsEkiXwyTQN3Jj76qpf4rXU54AFAjifef/Tfomlku96mu49MPp9MTWoMj/gja8uafRCby8dzVnXt9D1v7VxAkEAxNjqhekzp84KEMzp39LlUGLBesXEyFHWaG4wOURQfi3tI+FCRG2rfX2IhpEU/eIzRTzx9c88eagc7hNxHeCD+QJAN6+sV+mfPm5NdW4FwMOjKZN6upGtx5RXRTkQ28DeM4pGPzbMwvKa+vhx0l9NTMxLRg24HzhvTb1Y5Kh5BkOzAQJBAOK8zYlAqAH3e0X08NC5wtFisiMQ/q4zehFAoLeTk0HS4TMSAX9HNha6qFhLhh0MdOYg/DubjmNnkkEisbkRoho=";

        public LoginController(IUserToken userToken, JwtConfig jwtConfig, IUser user, IUserService userService)
        {
            _userToken = userToken;
            _jwtConfig = jwtConfig;
            _user = user;
            _userService = userService;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <param name="logintype">0:智慧工地用户；1:租户</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Login(LoginRequest loginRequest)
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return ResponseOutput.NotOk<string>("用户名或密码为空");
            }
            DataTable dt = null;
            if (loginRequest.LoginType =="tenant")
            {
                 dt = await _userService.GetTenantUserByLogin(loginRequest);
            }
            else
            {
                dt = await _userService.GetUserByLogin(loginRequest);
            }
            
            if (dt == null)
            {
                return ResponseOutput.NotOk<string>("用户名或密码错误");
            }

            var token = _userToken.Create(new[]
            {
                new Claim(ClaimAttributes.UserId, dt.Rows[0]["userid"].ToString()),
                new Claim(ClaimAttributes.UserName, dt.Rows[0]["UserName"].ToString()),
                new Claim(ClaimAttributes.Role, dt.Rows[0]["ROLEID"].ToString()),
                new Claim(ClaimAttributes.GroupId, dt.Rows[0]["GROUPID"].ToString())
            });

            dt.Columns.Add("token", Type.GetType("System.String"));
            dt.Columns.Add("expires", Type.GetType("System.Int32"));
            dt.Columns.Add("refreshexpires", Type.GetType("System.Int32"));
            dt.Rows[0]["token"] = token;
            dt.Rows[0]["expires"] = _jwtConfig.Expires;
            dt.Rows[0]["refreshexpires"] = _jwtConfig.RefreshExpires;
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 单点登录
        /// </summary>
        /// <param name="encrptstr">RSA加密字符串</param>
        /// <returns></returns>
        [HttpPost]
        [Route("doSingleLogin")]
        public async Task<IResponseOutput> doSingleLogin([FromForm] string encrptstr)
        {
           // KeyValuePair<string, string> str = UEncrypter.CreateRSAKey(true);
            string decrptstr = UEncrypter.DecryptByRSA(encrptstr, SINGLELOGIN_PRIVATE_KEY);
            JObject mJObj = new JObject();
            mJObj = JObject.Parse(decrptstr);
            string username = (string)mJObj.GetValue("username");
            DateTime updatedate = (DateTime)mJObj.GetValue("datetime");
            if (string.IsNullOrEmpty(username))
            {
                return ResponseOutput.NotOk<string>("用户名为空");
            }
            if (updatedate < DateTime.Now.AddMinutes(-5) || updatedate > DateTime.Now.AddMinutes(5))
            {
                return ResponseOutput.NotOk<string>("日期不合法");
            }
            try
            {
                //updatedate = updatedate.AddHours(DateTime.Now.DayOfYear).AddMinutes(-1 * DateTime.Now.DayOfYear);
                //long timestamp = (updatedate.ToUniversalTime().Ticks - 621355968000000000) / 10000;
                //string userName = UEncrypter.DecryptDecECB(username, timestamp.ToString());
                LoginRequest loginRequest = new LoginRequest();
                loginRequest.UserName = username;
                var dt = await _userService.GetUserByLogin(loginRequest);
                if (dt == null)
                {
                    return ResponseOutput.NotOk<string>("用户名或密码错误");
                }

                var token = _userToken.Create(new[]
                {
                new Claim(ClaimAttributes.UserId, dt.Rows[0]["userid"].ToString()),
                new Claim(ClaimAttributes.UserName, dt.Rows[0]["UserName"].ToString()),
                new Claim(ClaimAttributes.Role, dt.Rows[0]["ROLEID"].ToString()),
                new Claim(ClaimAttributes.GroupId, dt.Rows[0]["GROUPID"].ToString())
                 });

                dt.Columns.Add("token", Type.GetType("System.String"));
                dt.Columns.Add("expires", Type.GetType("System.Int32"));
                dt.Columns.Add("refreshexpires", Type.GetType("System.Int32"));
                dt.Rows[0]["token"] = token;
                dt.Rows[0]["expires"] = _jwtConfig.Expires;
                dt.Rows[0]["refreshexpires"] = _jwtConfig.RefreshExpires;
                return ResponseOutput.Ok(dt);

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk();
            }

          
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("refresh")]
        public async Task<IResponseOutput> Refresh(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return ResponseOutput.NotOk("请输入用户凭证");
            }

            var userClaims = _userToken.Decode(token);
            if (userClaims == null || userClaims.Length == 0)
            {
                return ResponseOutput.NotOk("获取用户信息失败");
            }

            var refreshExpiresValue = userClaims.FirstOrDefault(a => a.Type == ClaimAttributes.RefreshExpires)?.Value;
            if (string.IsNullOrEmpty(refreshExpiresValue))
            {
                return ResponseOutput.NotOk("登录信息已过期");
            }

            var refreshExpires = Convert.ToDateTime(refreshExpiresValue);
            if (refreshExpires <= DateTime.Now)
            {
                return ResponseOutput.NotOk("登录信息已过期");
            }

            string userId = userClaims.FirstOrDefault(a => a.Type == ClaimAttributes.UserId)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseOutput.NotOk();
            }
            var Users = await _userService.Query(u => u.USERID == userId);
            if (!Users.Any())
            {
                return ResponseOutput.NotOk("未找到用户信息");
            }
            var R_Entity = new LoginRequest() { UserName = Users.FirstOrDefault().username, Password = Users.FirstOrDefault().pwd };
            var dt = await _userService.GetUserByLogin(R_Entity);

            var r_token = _userToken.Create(new[]
            {
                new Claim(ClaimAttributes.UserId, dt.Rows[0]["userid"].ToString()),
                new Claim(ClaimAttributes.UserName, dt.Rows[0]["UserName"].ToString()),
                new Claim(ClaimAttributes.Role, dt.Rows[0]["ROLEID"].ToString()),
                new Claim(ClaimAttributes.GroupId, dt.Rows[0]["GROUPID"].ToString())
            });

            return ResponseOutput.Ok(r_token);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="pwdNew"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("changePwd")]
        public async Task<IResponseOutput> ChangePwd(PasswordInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.pwd) || string.IsNullOrEmpty(input.pwdNew))
                return ResponseOutput.NotOk("请输入用户密码");

            int result = 0;
            try
            {
                LoginRequest loginRequest = new LoginRequest() { Password = UEncrypter.DecryptByRSA(input.pwd, Const.Encryp.PRIVATE_KEY_OTHER), UserName = _user.Name };
                DataTable dt = null;
                //租户
                if (input.LoginType == "tenant")
                {
                    dt = await _userService.GetTenantUserByLogin(loginRequest);
                    if (dt != null && dt.Rows.Count > 0 && input.teid == Convert.ToString(dt.Rows[0]["TEID"]))
                    {
                      
                       result = await _userService.changeTenantUserPwd(input.teid, UEncrypter.DecryptByRSA(input.pwdNew, Const.Encryp.PRIVATE_KEY_OTHER));
                       
                    }
                    else
                    {
                        return ResponseOutput.NotOk("用户名或密码错误");
                    }
                }
                else
                {
                    dt = await _userService.GetUserByLogin(loginRequest);
                    if (dt != null && dt.Rows.Count > 0 && _user.Id == Convert.ToString(dt.Rows[0]["USERID"]))
                    {
                       
                        result = await _userService.changePwd(_user.Id, UEncrypter.DecryptByRSA(input.pwdNew, Const.Encryp.PRIVATE_KEY_OTHER));
                       
                    }
                    else
                    {
                        return ResponseOutput.NotOk("用户名或密码错误");
                    }
                }
                
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }
    }
}