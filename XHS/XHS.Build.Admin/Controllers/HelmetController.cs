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
using XHS.Build.Services.Helmet;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class HelmetController : ControllerBase
    {
        private readonly IHelmetService _helmetService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="helmetService"></param>
        /// <param name="user"></param>
        public HelmetController(IHelmetService helmetService, IUser user)
        {
            _helmetService = helmetService;
            _user = user;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20, string order = "", string ordertype = "")
        {
            var data = await _helmetService.GetHelmetPage(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 安全帽 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _helmetService.GetGroupHelmetBuildCount());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCHelmetEntity input)
        {
            if (input == null || string.IsNullOrEmpty(input.helmetcode))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            var rows = await _helmetService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="buildid"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int buildid)
        {
            if (buildid <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _helmetService.DeleteById(buildid);
            if (isDel)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("删除失败");
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCHelmetEntity input)
        {
            if (input == null || input.HELMETID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _helmetService.Update(input);
            if (suc)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            return ResponseOutput.Ok(await _helmetService.QueryById(id));
        }
    }
}
