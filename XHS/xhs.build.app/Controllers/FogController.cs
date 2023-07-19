using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Services.Fog;
using static XHS.Build.Common.Helps.HpFog;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 雾泡联动
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FogController : ControllerBase
    {
        public readonly IUser _user;
        private readonly IFogService _fogService;
        public FogController(IFogService fogService,IUser user)
        {
            _user = user;
            _fogService = fogService;
        }

        /// <summary>
        /// 获取监测点下雾泡
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getlist")]
        public async Task<IResponseOutput> GetList(int SITEID)
        {
            return ResponseOutput.Ok(await _fogService.getListBySite(SITEID));
        }

        /// <summary>
        /// 获取用户负责的所有雾泡
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getlistbyuser")]
        public async Task<IResponseOutput> GetListByUser()
        {
            return ResponseOutput.Ok(await _fogService.getListByUser());
        }

        /// <summary>
        /// 发送指令给雾炮
        /// </summary>
        /// <param name="bc"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendcommand")]
        public async Task<IResponseOutput> SendCommand(BnCmd bc)
        {
            bc.USERID = _user.Id;
            HpFog.SendCommand(bc);
            return ResponseOutput.Ok();
        }
    }
}