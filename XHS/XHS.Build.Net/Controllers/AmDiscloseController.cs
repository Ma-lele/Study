using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.AmDisclose;
using XHS.Build.Services.Site;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 晨会交底
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class AmDiscloseController : ControllerBase
    {
        private readonly IAmDiscloseService _amDiscloseService;
        private readonly ISiteService _siteService;
        public AmDiscloseController(IAmDiscloseService amDiscloseService, ISiteService siteService)
        {
            _amDiscloseService = amDiscloseService;
            _siteService = siteService;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCAmDiscloseEntity entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.projid))
            {
                return ResponseOutput.NotOk("请填写正确的数据");
            }
            var siteList = await _siteService.QuerySiteExist(a => a.amdiscloseprojid == entity.projid);
            if (!siteList)
            {
                return ResponseOutput.NotOk("未找到工地项目信息");
            }
            entity.createdate = DateTime.Now;
            var res = await _amDiscloseService.Add(entity);
            return res > 0 ? ResponseOutput.Ok(1) : ResponseOutput.NotOk("新增晨会交底失败，请稍后再试");
        }
    }
}
