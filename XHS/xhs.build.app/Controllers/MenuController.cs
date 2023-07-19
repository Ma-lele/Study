using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Menus;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuController(IUser user, IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// 当前用户的APP Menus
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("usermenus")]
        public async Task<IResponseOutput> GetUserMenus()
        {
            return ResponseOutput.Ok(await _menuService.GetCurrentUserMenus());
        }
    }
}