using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Role;
using XHS.Build.Services.User;

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
        private readonly ICache _cache;
        private readonly ISysUserService _sysUserService;
        private readonly IMapper _mapper;
        private readonly JwtConfig _jwtConfig;
        private readonly IUserRoleService _userRoleService;
        public LoginController(JwtConfig jwtConfig, ISysUserService sysUserService, IUserToken userToken, IMapper mapper, IUserRoleService userRoleService, ICache cache)
        {
            _jwtConfig = jwtConfig;
            _sysUserService = sysUserService;
            _cache = cache;
            _userToken = userToken;
            _mapper = mapper;
            _userRoleService = userRoleService;
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

            var users = await _sysUserService.Query(a => a.LoginName == input.LoginName && a.Password == UEncrypter.SHA256(input.Password));
            if (users.Any())
            {

                var user = users.FirstOrDefault();
                var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, user.Id);
                await _cache.DelAsync(key);
                var output = _mapper.Map<LoginUserOutput>(user);
                var Roles = await _userRoleService.Query(r => r.Userid == user.Id);
                if (Roles.Any())
                {
                    output.RoleIds = Roles.Select(a => a.Roleid).ToList();
                }
                return GetToken(output);
            }
            else
            {
                return ResponseOutput.NotOk("用户名或密码错误");
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
            var user = await _sysUserService.QueryById(userId);
            if (user == null)
            {
                return ResponseOutput.NotOk("未找到用户信息");
            }
            var output = _mapper.Map<LoginUserOutput>(user);
            var Roles = await _userRoleService.Query(r => r.Userid == user.Id);
            if (Roles.Any())
            {
                output.RoleIds = Roles.Select(a => a.Roleid).ToList();
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
                new Claim(ClaimAttributes.LoginName, output.LoginName==null?string.Empty:output.LoginName),
                new Claim(ClaimAttributes.Gender, output.Gender==null?string.Empty:output.Gender),
                new Claim(ClaimAttributes.GroupId, output.GroupId.ToString()),
                new Claim(ClaimAttributes.Role,output.RoleIds==null?string.Empty: string.Join(',',output.RoleIds)),
                new Claim(ClaimAttributes.IsAdmin,output.RoleIds==null?"false":(output.RoleIds.Contains("1")?"true":"false"))
            });

            return ResponseOutput.Ok(new { token, expires_in = _jwtConfig.Expires * 60 });
        }
    }
}
