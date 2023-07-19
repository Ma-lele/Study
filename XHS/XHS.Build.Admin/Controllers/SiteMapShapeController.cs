using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Site;

namespace XHS.Build.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SiteMapShapeController : ControllerBase
    {
        private readonly IMapShareService _mapShareService;
        private readonly IUser _user;
        public SiteMapShapeController(IMapShareService mapShareService, IUser user)
        {
            _mapShareService = mapShareService;
            _user = user;
        }

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Pages(int SITEID, int pageindex = 1, int pagesize = 10)
        {
            var dbList = await _mapShareService.QueryPage(a => a.SITEID == SITEID, pageindex, pagesize);
            return ResponseOutput.Ok(dbList);
        }

        /// <summary>
        /// 增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCSiteMapShareEntity entity)
        {
            if (entity == null)
            {
                return ResponseOutput.NotOk("请输入信息");
            }
            entity.@operator = _user.Name;
            return ResponseOutput.Ok(await _mapShareService.Add(entity));
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCSiteMapShareEntity entity)
        {
            if (entity == null || entity.SMPID <= 0)
            {
                return ResponseOutput.NotOk("请输入信息");
            }
            entity.@operator = _user.Name;

            return (await _mapShareService.Update(entity)) ? ResponseOutput.Ok() : ResponseOutput.NotOk("修改失败");
        }

        /// <summary>
        /// 删
        /// </summary>
        /// <param name="SMPID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int SMPID)
        {
            if (SMPID <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }
            return (await _mapShareService.DeleteById(SMPID)) ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }
    }
}
