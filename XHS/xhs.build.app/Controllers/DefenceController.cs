using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Defence;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 临边围挡
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DefenceController : ControllerBase
    {
        private readonly IDefenceService _defenceService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defenceService"></param>
        public DefenceController(IDefenceService defenceService, IUser user)
        {
            _defenceService = defenceService;
            _user = user;
        }

        /// <summary>
        /// 获取监测点下临边围挡
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<IResponseOutput> GetList(int SITEID)
        {
            return ResponseOutput.Ok(await _defenceService.getListBySite(SITEID));
        }

        /// <summary>
        /// 布防
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("shield")]
        public async Task<IResponseOutput> Shield(string dfcode)
        {
            return ResponseOutput.Ok(await _defenceService.doShield(dfcode));
        }

        /// <summary>
        /// 撤防
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("unshield")]
        public async Task<IResponseOutput> Unshield(string dfcode)
        {
            return ResponseOutput.Ok(await _defenceService.doUnshield(dfcode));
        }

        /// <summary>
        /// 区域布防
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="dfzone">片区</param>
        /// <returns></returns>
        [HttpGet]
        [Route("zoneshield")]
        public async Task<IResponseOutput> ZoneShield(int SITEID, string dfzone)
        {
            return ResponseOutput.Ok(await _defenceService.doZoneShield(SITEID, dfzone, 1));
        }

        /// <summary>
        /// 区域撤防
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="dfzone">片区</param>
        /// <returns></returns>
        [HttpGet]
        [Route("zoneunshield")]
        public async Task<IResponseOutput> ZoneUnshield(int SITEID, string dfzone)
        {
            return ResponseOutput.Ok(await _defenceService.doZoneShield(SITEID, dfzone, 0));
        }

        /// <summary>
        /// 临边围挡插入
        /// </summary>
        /// <returns>结果集</returns>
        [HttpPost]
        [Route("create")]
        public async Task<IResponseOutput> Create([FromForm] int SITEID, [FromForm] string dfcode, [FromForm] string bsheild, [FromForm] string dfname, [FromForm] string dfzone)
        {
            if (SITEID < 1 || string.IsNullOrEmpty(dfcode) || string.IsNullOrEmpty(bsheild) || string.IsNullOrEmpty(dfname) || string.IsNullOrEmpty(dfzone))
            {
                return ResponseOutput.NotOk("请填写正确的数据");
            }

            var param = new { GROUPID = _user.GroupId, SITEID = SITEID, dfcode = dfcode, bsheild = bsheild, dfname = dfname, dfzone = dfzone, @operator = _user.Name };

            return ResponseOutput.Ok(await _defenceService.doInsert(param));
        }

        /// <summary>
        /// 设置临边围挡信息
        /// </summary>
        /// <returns>设置结果</returns>
        [HttpPost]
        [Route("update")]
        public async Task<IResponseOutput> Update([FromForm] int DEFENCEID, [FromForm] int SITEID, [FromForm] string dfcode, [FromForm] string dfname, [FromForm] string dfzone)
        {
            if (DEFENCEID < 1 || SITEID < 1 || string.IsNullOrEmpty(dfcode) || string.IsNullOrEmpty(dfname) || string.IsNullOrEmpty(dfzone))
            {
                return ResponseOutput.NotOk("请填写正确的数据");
            }
            var param = new { DEFENCEID = DEFENCEID, GROUPID = _user.GroupId, SITEID = SITEID, dfcode = dfcode, dfname = dfname, dfzone = dfzone, @operator = _user.Name };

            return ResponseOutput.Ok(await _defenceService.doUpdate(param));
        }

        /// <summary>
        /// 删除临边围挡
        /// </summary>
        /// <param name="DEFENCEID">临边围挡ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IResponseOutput> Delete(int DEFENCEID)
        {
            return ResponseOutput.Ok(await _defenceService.doDelete(DEFENCEID));
        }
    }
}