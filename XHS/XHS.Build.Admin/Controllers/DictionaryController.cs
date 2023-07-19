using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Dictionary;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 公共字典 数据
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Permission]
    [Authorize]
    public class DictionaryController : ControllerBase
    {
        private readonly IGCRegionService _gCRegionService;
        private readonly IDictionaryService _dictionaryService;
        public DictionaryController(IGCRegionService gCRegionService, IDictionaryService dictionaryService)
        {
            _gCRegionService = gCRegionService;
            _dictionaryService = dictionaryService;
        }

        /// <summary>
        /// 字典分页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="order"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
       [HttpGet]
        public async Task<IResponseOutput> GetPages(string keyword = "", int page = 1, int size = 20, string order = "", string ordertype = "")
        {
            Expression<Func<CCDataDictionaryEntity, bool>> whereExpression = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = b => b.dataitem.Contains(keyword);
            }
            var dbList = await _dictionaryService.QueryPage(whereExpression, page, size, string.IsNullOrEmpty(order) ? "DDID desc,datatype desc,sort desc" : order +" "+ ordertype);
            var data = new PageOutput<CCDataDictionaryEntity>()
            {
                data = dbList.data,
                dataCount = dbList.dataCount
            };
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(CCDataDictionaryEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var rows = await _dictionaryService.Add(input);

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

            bool isDel = await _dictionaryService.DeleteById(id);
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
        public async Task<IResponseOutput> Put(CCDataDictionaryEntity input)
        {
            if (input == null || input.DDID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            bool suc = await _dictionaryService.Update(input);
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
            return ResponseOutput.Ok(await _dictionaryService.QueryById(id));
        }


        /// <summary>
        /// 获取市区街道
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> City(int pid = 0)
        {
            return ResponseOutput.Ok(await _gCRegionService.Query(g => g.ParentId == pid));
        }

        /// <summary>
        /// 获取所有配置字典
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> List(string datatype = "")
        {
            if (string.IsNullOrEmpty(datatype))
            {
                return ResponseOutput.Ok(await _dictionaryService.Query());
            }
            else
            {
                return ResponseOutput.Ok(await _dictionaryService.Query(a => a.datatype == datatype));
            }
        }
    }
}
