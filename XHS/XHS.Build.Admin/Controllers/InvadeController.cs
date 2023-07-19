using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Invade;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class InvadeController : ControllerBase
    {
        private readonly IInvadeService _invadeService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invadeService"></param>
        /// <param name="user"></param>
        public InvadeController(IInvadeService invadeService, IUser user)
        {
            _invadeService = invadeService;
            _user = user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> List(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _invadeService.GetInvadePageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _invadeService.GetGroupCount());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCInvadeEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbEntity = await _invadeService.Query(a => a.invadecode == input.invadecode);
            if (dbEntity.Any())
            {
                return ResponseOutput.NotOk("已存在设备编号");
            }
            input.updater = _user.Name;
            var rows = await _invadeService.Add(input);

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

            bool isDel = await _invadeService.DeleteById(id);
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
        public async Task<IResponseOutput> Put(GCInvadeEntity input)
        {
            if (input == null || input.INVADEID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var dbEntity = await _invadeService.Query(a => a.invadecode == input.invadecode && a.INVADEID != input.INVADEID);
            if (dbEntity.Any())
            {
                return ResponseOutput.NotOk("已存在设备编号");
            }
            bool suc = await _invadeService.Update(input);
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
            return ResponseOutput.Ok(await _invadeService.QueryById(id));
        }
    }
}
