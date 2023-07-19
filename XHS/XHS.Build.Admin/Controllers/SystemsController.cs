using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Systems;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class SystemsController : ControllerBase
    {
        private readonly ISystemsService _systemService;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemsService"></param>
        /// <param name="mapper"></param>
        public SystemsController(ISystemsService systemsService, IMapper mapper)
        {
            _systemService = systemsService;
            _mapper = mapper;
        }

        /// <summary>
        /// 系统列表
        /// </summary>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20)
        {
            Expression<Func<SystemsEntity, bool>> whereExpression = a => a.IsDeleted == false;
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = whereExpression.And(b => b.Name.Contains(keyword) && b.IsDeleted == false);
            }
            var dbList = await _systemService.QueryPage(whereExpression, page, size, " id desc");
            var data = new PageOutput<SystemsEntity>()
            {
                data = dbList.data,
                dataCount = dbList.dataCount
            };
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取全部系统信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> List()
        {
            return ResponseOutput.Ok(await _systemService.Query(a => a.IsDeleted == false));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(SystemsEntity input)
        {
            if (input == null || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写系统信息");
            }
            var rows = await _systemService.Add(input);
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
                return ResponseOutput.NotOk("请选择需要删除的信息");
            }
            var entity = await _systemService.QueryById(id);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到可删除的信息");
            }
            entity.IsDeleted = true;
            var suc = await _systemService.Update(entity);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(SystemsEntity input)
        {
            if (input == null || string.IsNullOrEmpty(input.Id) || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写需要修改的信息");
            }
            var suc = await _systemService.Update(input);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("更新信息失败");
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            return ResponseOutput.Ok(await _systemService.QueryById(id));
        }
    }
}
