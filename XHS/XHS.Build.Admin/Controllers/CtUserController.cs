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
using XHS.Build.Services.CtUser;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class CtUserController : ControllerBase
    {
        private readonly ICtUserService _ctUserService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        public CtUserController(ICtUserService ctUserService, IUser user)
        {
            _ctUserService = ctUserService;
            _user = user;
        }

        /// <summary>
        /// 获取单个用户
        /// </summary>
        /// <param name="id">用户编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(int id)
        {
            return ResponseOutput.Ok(await _ctUserService.QueryById(id));
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页件数</param>
        /// <param name="GROUPID">分组</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int GROUPID, string keyword = "", int page = 1, int size = 10)
        {
            if (_user.GroupId > 0)
            {
                GROUPID = _user.GroupId;
            }
            return ResponseOutput.Ok(await _ctUserService.GetPageList(keyword, page, size, GROUPID));
        }

        /// <summary>
        /// 获取每个分组用户的数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetGroupCount()
        {
            return ResponseOutput.Ok(await _ctUserService.GetGroupCount());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(CTUserEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            input.@operator = _user.Name;
            var dbExist = await _ctUserService.Query(f => f.username == input.username && f.status <= 1);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该用户已存在");
            }

            var rows = await _ctUserService.UserAdd(input);

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

            CTUserEntity input = new CTUserEntity();
            input.USERID = id;
            input.@operator = _user.Name;
            int result = await _ctUserService.UserDelete(input);
            if (result > 0)
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
        public async Task<IResponseOutput> Put(CTUserEntity input)
        {
            if (input == null || String.IsNullOrEmpty(input.username))
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbExist = await _ctUserService.Query(f => f.username == input.username && f.status <=1 && f.USERID != input.USERID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该用户已存在");
            }

            input.@operator = _user.Name;
            int result = await _ctUserService.UserUpdate(input);
            if (result > 0)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

        /// <summary>
        /// 冻结/解冻用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> UpdateStatus(CTUserEntity input)
        {
            input.@operator = _user.Name;
            string strwhere = "";
            if (input.status == 0)//解冻
            {
                strwhere = string.Format(" USERID={0} and status=1", input.USERID.ToString());
            }
            else//冻结
            {
                strwhere = string.Format(" USERID={0} and status=0", input.USERID.ToString());
            }
            var suc = await _ctUserService.Update(input,
                new List<string>() { "USERID", "operator", "status" },
                null,
                strwhere);
            if (suc)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("状态更新失败");
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> ResetPwd(CTUserEntity input)
        {
            input.@operator = _user.Name;
            var result = await _ctUserService.ResetPwd(input);
            if (result > 0)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("密码重置失败");
            }
        }

        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetRegions(int GROUPID)
        {
            return ResponseOutput.Ok(await _ctUserService.GetRegions(GROUPID));
        }

    }
}
