using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.RoleRight;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class CtRoleRightController : ControllerBase
    {
        private readonly IRoleRightService _roleRightService;
        //private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        public CtRoleRightController(IRoleRightService roleRightService, IUser user)
        {
            _roleRightService = roleRightService;
            //_user = user;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="ROLEID">角色</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAll(string ROLEID)
        {
            return ResponseOutput.Ok(await _roleRightService.GetCtAll(ROLEID));
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCRoleRightEntity input)
        {
            
            int result = await _roleRightService.Save(input.ROLEID, input.MENUIDS);
            if (result > 0)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("保存失败");
            }
        }

    }
}
