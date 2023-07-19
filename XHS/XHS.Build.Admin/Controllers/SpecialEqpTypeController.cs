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
using XHS.Build.Services.SpecialEqp;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class SpecialEqpTypeController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ISpecialEqpTypeService _specialEqpService;
        public SpecialEqpTypeController(IUser user, ISpecialEqpTypeService specialEqpService)
        {
            _specialEqpService = specialEqpService;
            _user = user;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            var data = await _specialEqpService.GetSiteSpecialEqpPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _specialEqpService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCSpecialEqpTypeEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var rows = await _specialEqpService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int id)
        {
            if (id <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _specialEqpService.DeleteById(id);
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
        public async Task<IResponseOutput> Put(GCSpecialEqpTypeEntity input)
        {
            if (input == null || input.SETYPEID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            bool suc = await _specialEqpService.Update(input);
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
            return ResponseOutput.Ok(await _specialEqpService.QueryById(id));
        }
    }
}
