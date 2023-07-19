using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Apis;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class ApisController : ControllerBase
    {
        private readonly IApiService _apiService;

        public ApisController(IApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20)
        {
            Expression<Func<SysApisEntity, bool>> whereExpression = a => true;
            if (!string.IsNullOrEmpty(keyword))
            {
                whereExpression = whereExpression.And(b => b.Name.Contains(keyword) || b.ApiUrl.Contains(keyword));
            }
            var dbList = await _apiService.QueryPage(whereExpression, page, size, " id desc");
            var data = new PageOutput<SysApisEntity>()
            {
                data = dbList.data,
                dataCount = dbList.dataCount
            };
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IResponseOutput> GetAll()
        {
            var dbList = await _apiService.Query();
            return ResponseOutput.Ok(dbList);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(SysApisEntity input)
        {
            if (input == null || string.IsNullOrEmpty(input.Name))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var rows = await _apiService.Add(input);
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
            var entity = await _apiService.QueryById(id);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到可删除的信息");
            }
            var suc = await _apiService.DeleteById(id);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(SysApisEntity entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.Id) || string.IsNullOrEmpty(entity.Name))
            {
                return ResponseOutput.NotOk("请填写需要修改的信息");
            }
            var suc = await _apiService.Update(entity);
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
            return ResponseOutput.Ok(await _apiService.QueryById(id));
        }

        /// <summary>
        /// 同步所有接口数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IResponseOutput> AsyncApi()
        {
            //route可能自定义  反射不到
            //var assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable().Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            //assembly.ForEach(r =>
            //{
            //    //当前命名空间下的方法，排除父类的
            //    var Methods = r.GetMethods().Where(m => m.DeclaringType.Namespace == MethodBase.GetCurrentMethod().DeclaringType.Namespace);
            //    foreach (var methodInfo in Methods)
            //    {
            //        foreach (Attribute attribute in methodInfo.GetCustomAttributes())
            //        {
            //            var sss = attribute;
            //        }
            //    }
            //});

            var ApiJson = UHttp.Get(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/swagger/V1/swagger.json");
            JObject obj = (JObject)JsonConvert.DeserializeObject(ApiJson);
            List<ApisInputDto> EntityList = new List<ApisInputDto>();
            if (obj != null)
            {

                var PathObjs = obj["paths"];
                JObject ApiObjs = (JObject)JsonConvert.DeserializeObject(Convert.ToString(PathObjs));
                JArray apiList = new JArray();
                foreach (var item in ApiObjs.Properties())
                {
                    var apidetail = item.Name;
                    var ss = item.Value.FirstOrDefault().First;
                    var summary = ss["summary"];

                    EntityList.Add(new ApisInputDto() { ApiUrl = Convert.ToString(apidetail).ToLower(), Name = summary.ToString() });
                    //JObject apiItem = new JObject();
                    //apiItem["ApiUrl"] = Convert.ToString(apidetail).ToLower();
                    //apiItem["Name"] = summary.ToString();

                    //apiList.Add(apiItem);
                }
                return await _apiService.AsyncApi(EntityList);
            }
            return ResponseOutput.Ok();
        }
    }
}
