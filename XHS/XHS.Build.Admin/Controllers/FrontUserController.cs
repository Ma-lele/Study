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
using XHS.Build.Services.FrontUser;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class FrontUserController : ControllerBase
    {
        private readonly IFrontUserService _frontUserService;
        private readonly IUser _user;
        /// <summary>
        /// 
        /// </summary>
        public FrontUserController(IFrontUserService frontUserService, IUser user)
        {
            _frontUserService = frontUserService;
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
            return ResponseOutput.Ok(await _frontUserService.QueryById(id));
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
            return ResponseOutput.Ok(await _frontUserService.GetPageList(keyword, page, size, GROUPID));
        }

        /// <summary>
        /// 获取每个分组用户的数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetGroupCount()
        {
            return ResponseOutput.Ok(await _frontUserService.GetGroupCount());
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCUserEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            input.@operator = _user.Name;
            var dbExist = await _frontUserService.Query(f => f.username == input.username && f.bdel == 0);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该用户已存在");
            }

            var rows = await _frontUserService.UserAdd(input);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            GCUserEntity input = new GCUserEntity();
            input.USERID = id;
            input.@operator = _user.Name;
            int result = await _frontUserService.UserDelete(input);
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
        public async Task<IResponseOutput> Put(GCUserEntity input)
        {
            if (input == null || String.IsNullOrEmpty(input.username))
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbExist = await _frontUserService.Query(f => f.username == input.username && f.bdel == 0 && f.USERID != input.USERID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该用户已存在");
            }

            input.@operator = _user.Name;
            int result = await _frontUserService.UserUpdate(input);
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
        public async Task<IResponseOutput> UpdateStatus(GCUserEntity input)
        {
            input.@operator = _user.Name;
            string strwhere = "";
            if (input.status == 1)//解冻
            {
                strwhere = string.Format(" USERID={0} and bdel=0", input.USERID.ToString());
            }
            else//冻结
            {
                strwhere = string.Format(" USERID={0}", input.USERID.ToString());
            }
            var suc = await _frontUserService.Update(input,
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
        public async Task<IResponseOutput> ResetPwd(GCUserEntity input)
        {
            input.@operator = _user.Name;
            var result = await _frontUserService.ResetPwd(input);
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
        /// 获取单个用户的负责工地列表
        /// </summary>
        /// <param name="USERID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> UserSiteList(int USERID)
        {
            return ResponseOutput.Ok(await _frontUserService.getUserSiteList(USERID));
        }

        /// <summary>
        /// 保存单个用户负责的工地
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> SaveUserSite(UserSiteInput input)
        {

            if (input == null || input.USERID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            input.@operator = _user.Name;
            int result = await _frontUserService.saveUserSite(input);
            if (result > 0)
            {
                return ResponseOutput.Ok(result);
            }
            else
            {
                return ResponseOutput.NotOk("保存失败");
            }
        }


    }
}
