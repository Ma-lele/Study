using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using XHS.Build.Common.Response;
using XHS.Build.Model.MongoModels;
using XHS.Build.Services.TestMongoDBService;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 测试MongoDB增删改查
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LogController : ControllerBase
    {
        private readonly ILogService _testService;
        public LogController(ILogService testService)
        {
            _testService = testService;
        }

        ///// <summary>
        ///// 增
        ///// </summary>
        ///// <param name="testEntity"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<TestEntity> Post(TestEntity testEntity)
        //{
        //    return await _testService.InsertAsync(testEntity);
        //}

        /// <summary>
        /// 删
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            return await _testService.Delete(id);
        }

        /// <summary>
        /// 改
        /// </summary>
        /// <param name="testEntity"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> Update([FromForm] string testEntity)
        {
            return await _testService.Update(testEntity);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("pages")]
        public async Task<string> Pages(string name = "", int pageindex = 1, int pagesize = 10)
        {
            var list = await _testService.Pages(name, pageindex, pagesize);
            //list.TotalCount
            return list.ToArray().ToJson();
        }


        /// <summary>
        /// 根据name搜索
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("namelist")]
        public async Task<string> GetListNyName(string name)
        {
            var list = await _testService.GetTestEntityByName(name);
            return list.ToJson();
        }
        /// <summary>
        /// 所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<string> GetList()
        {
            var list = await _testService.GetTestEntities();
            return list.ToJson();
        }

        /// <summary>
        /// 不固定属性？
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddJson")]
        public async Task<bool> AddJson([FromForm] string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }
            await _testService.InsertObjectAsync(json);
            return true;
        }

        /// <summary>
        /// 批量插入 不固定属性？
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddJsons")]
        public async Task<bool> AddJsons([FromForm] string json)
        {
            await _testService.InsertObjectsAsync(json);
            return true;
        }
    }
}