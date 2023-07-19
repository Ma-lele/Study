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
using XHS.Build.Services.Unload;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 卸料
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class UnloadController : ControllerBase
    {
        private readonly IUnloadService _unloadService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unloadService"></param>
        /// <param name="user"></param>
        public UnloadController(IUnloadService unloadService, IUser user)
        {
            _unloadService = unloadService;
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
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _unloadService.GetSiteUnloadPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _unloadService.GetGroupCount());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCUnloadEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var dbExist = await _unloadService.Query(f => f.unloadid == input.unloadid && f.bdel == 0);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该设备编号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            var rows = await _unloadService.Add(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ulid"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int ulid)
        {
            if (ulid <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _unloadService.DeleteById(ulid);
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
        public async Task<IResponseOutput> Put(GCUnloadEntity input)
        {
            if (input == null || input.ULID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var dbExist = await _unloadService.Query(f => f.unloadid == input.unloadid && f.bdel == 0 && f.ULID != input.ULID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该设备编号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _unloadService.Update(input);
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
            return ResponseOutput.Ok(await _unloadService.QueryById(id));
        }
    }
}
