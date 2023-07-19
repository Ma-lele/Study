using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Services.SpecialEqp;

namespace XHS.Build.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class SpecialRealDataController : ControllerBase
    {
        private readonly IRealDataService _realDataService;
        public SpecialRealDataController(IRealDataService realDataService)
        {
            _realDataService = realDataService;
        }

        /// <summary>
        /// 特种设备 实时数据
        /// </summary>
        /// <param name="secode"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Pages(string secode, string starttime, string endtime, int pageindex = 1, int pagesize = 10)
        {
            return ResponseOutput.Ok(await _realDataService.GetPage(secode, starttime, endtime, pageindex, pagesize));
        }
    }
}
