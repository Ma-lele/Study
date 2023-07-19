using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.Site;
using XHS.Build.Services.UserService;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserKey _userKey;
        public UserController(IUserService userService, IUserKey userKey)
        {
            _userService = userService;
            _userKey = userKey;
        }

        /// <summary>
        /// 用户同步
        /// </summary>
        /// <param name="updatetime"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(DateTime? updatetime)
        {
           if(updatetime == null)
            {
                updatetime = DateTime.Now.AddYears(-2);
            }
            DataTable dt = await _userService.GetUserList(_userKey.GroupId, updatetime);
            return ResponseOutput.Ok(dt);
        }

      
    }
}
