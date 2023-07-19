using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Ozone;

namespace XHS.Build.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class OzoneController : ControllerBase
    {
        private readonly IOzoneService _ozoneService;
        private readonly IUser _user;

        public OzoneController(IOzoneService ozoneService, IUser user)
        {
            _ozoneService = ozoneService;
            _user = user;
        }


        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _ozoneService.GetGroupCount());
        }


        /// <summary>
        /// 检查同一group下code是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupid"></param>
        /// <param name="dpid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> CheckCode(string code, int groupid, int ozid = 0)
        {
            bool exists = await _ozoneService.CheckCode(code, groupid, ozid);
            return ResponseOutput.Ok(exists);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCOzone input)
        {
            input.bdel = 0;
            input.createtime = DateTime.Now;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var result = await _ozoneService.Add(input);

            return result > 0 ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 获取臭氧分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _ozoneService.GetOzPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCOzone input)
        {
            if (input == null || input.OZID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _ozoneService.Update(input);
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
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IResponseOutput> Delete(GCOzone input)
        {
            if (input == null || input.OZID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            input.bdel = 1;

            bool result = await _ozoneService.Delete(input);

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }
    }
}
