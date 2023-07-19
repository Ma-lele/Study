using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SettingController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;
        public SettingController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        /// <summary>
        /// 系统配置列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="order"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20, string order = "", string ordertype = "")
        {
            Expression<Func<CCSystemSettingEntity, bool>> whereExpression = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = b => b.itemname.Contains(keyword) || b.SETTINGID.Contains(keyword);
            }
            var data = await _systemSettingService.QueryPage(whereExpression, page, size, string.IsNullOrEmpty(order) ? " SETTINGID desc " : order + " " + ordertype);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(CCSystemSettingEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            var dbExist = await _systemSettingService.Query(f => f.SETTINGID == input.SETTINGID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("已存在相同数据信息");
            }

            var rows = await _systemSettingService.Add(input);
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

            bool isDel = await _systemSettingService.DeleteById(id);
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
        public async Task<IResponseOutput> Put(CCSystemSettingEntity input)
        {
            if (input == null || string.IsNullOrEmpty(input.SETTINGID))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            bool suc = await _systemSettingService.Update(input);
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
            return ResponseOutput.Ok(await _systemSettingService.QueryById(id));
        }
    }
}
