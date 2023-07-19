using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Group;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// 获取组设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getSetting")]
        public async Task<IResponseOutput> GetSetting()
        {
            List<BnSetting> result = new List<BnSetting>();

            try
            {
                DataTable dt = await _groupService.getWarnline();
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSetting bs = new BnSetting();
                    bs.type = Convert.ToString(dt.Rows[i]["type"]);
                    bs.value = Convert.ToString(dt.Rows[i]["value"]);

                    result.Add(bs);
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