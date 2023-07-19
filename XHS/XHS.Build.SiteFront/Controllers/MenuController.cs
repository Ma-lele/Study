using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Menus;
using XHS.Build.SiteFront.Attributes;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 页面菜单接口
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    [Permission]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="menuService"></param>
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T000/GetMenu")]
        public async Task<IResponseOutput> GetMenu()
        {
            var list = await _menuService.GetCurrentUserMenus("T");
            return ResponseOutput.Ok(list);
        }

    }
}
