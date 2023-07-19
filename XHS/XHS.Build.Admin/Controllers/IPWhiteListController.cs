using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Cable;
using XHS.Build.Services.IPWhiteList;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class IPWhiteListController : ControllerBase
    {
        private readonly IIPWhiteListService _IPWhiteListService;
        private readonly IUser _user;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger<IPWhiteListController> _logger;
        private readonly string _privateKey = @"MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBALZt64FaIu7/eIthOunliw0e8RNvZEJL1Pw4XC0zjSxZAfxea2vmWKw+H+sf01dAIYsLk1W+Xq1U7xpI+ToOtEW2aLeuFAkhZfdkUxKosYk5z0hKzIPmefNqAJF7J3B6JRbOid7FezY8TLdmj8/dk/5DqylBoG1hNGUQ13nZTMlDAgMBAAECgYA4OpkkPYwW7ldRXp6yCTZazPaxbtwQMx9qvlRq+kDBMo4SI2go0c7zCBL+fci+U94C5YZ8Hzk/Y7Zu+58V7gJSFS0fPJG14VXdovnhtwtqkeqWkFiOIxKeDgkXFTSVFKmWTwBeSU1zhtH69kJX3d1W7zvIsPB96Jdn8ufCBMaIQQJBAN1VOEkTYM4/fdceRUJ9FO0SA0YqPEeyGhIfEKpf5lET0fKJ7GZ9ExwAi8mFoKV6dZpX4qJxbKz8RH6ThRlLCQ0CQQDTAMlijqsgAROL5PO2Ii8wLzi2k+b0CW55WWDMk3/nlsHCd53lLjwDVSvQLCwsx6eGt9oUKxAzzAryYhtSGOePAkArum6IYX+6v+iI274rSLMds3VaI3YNZC6qbeDJCHFki1nrZTyDcyqXWbREeFYo22zotXxdQ7OI6b5Ok660NJLxAkEAyVnH0ygHvpuoVmfd9SD73MRDH19WFexmIqCa0b2zYYEoVvWklA2hsSB2lcmBQh4oDFOnZHWPmY4NqCzcm2QumwJAT5m0GnCyTAuVo4PKHkYlfSzlHCtXkGebUJAAEM7jdA3PT50tOoe3aAnvaLVmo/Go4AWfm8vmZNHQ1iztUmd2eQ==";

        /// <summary>
        /// 
        /// </summary>
        public IPWhiteListController(IIPWhiteListService IPWhiteListService, IHttpContextAccessor context, ILogger<IPWhiteListController> logger, IUser user)
        {
            _IPWhiteListService = IPWhiteListService;
            _context = context;
            _logger = logger;
            _user = user;
        }


        /// <summary>
        /// 替换新合盛白IP
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IResponseOutput> Replace([FromForm] string data)
        {
            const string XHS = "XHS";
            string IP = string.Empty;
            try
            {
                string request = UEncrypter.DecryptByRSA16(data, _privateKey);
                DateTime date = DateTime.Parse(request);
                //时间在10分钟之内
                if (DateTime.Now.AddMinutes(-10) > date && DateTime.Now.AddMinutes(10) < date)
                    return ResponseOutput.NotOk("Bad Timing.");

                IP = IPHelper.GetIP(_context?.HttpContext?.Request);
                if (string.IsNullOrEmpty(IP))
                    return ResponseOutput.NotOk();

                SFIPWhiteList sfip = new SFIPWhiteList()
                {
                    BeginIP = IP,
                    EndIP = IP,
                    iptype = XHS,
                    updatedate = DateTime.Now
                };

                var ipw = (await _IPWhiteListService.Query(item => item.iptype == XHS)).FirstOrDefault();
                if (ipw == null)
                {//新增
                    await _IPWhiteListService.AddEntity(sfip);
                    return ResponseOutput.Ok();
                }

                //更新
                if (ipw.BeginIP == IP && ipw.EndIP == IP)
                    return ResponseOutput.Ok();//没变就拉倒

                sfip.ID = ipw.ID;
                var ret = await _IPWhiteListService.Update(sfip);
                if (ret)
                    return ResponseOutput.Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError($"{IP} : {ex.Message}");
            }

            return ResponseOutput.NotOk();
        }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="page">页码</param>
        /// <param name="size">行数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(string keyword = "", int page = 1, int size = 20)
        {
            Expression<Func<SFIPWhiteList, bool>> whereExpression = ii => true;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                whereExpression = whereExpression.And(ii => ii.BeginIP == keyword || ii.EndIP == keyword);
            }

            var list = await _IPWhiteListService.QueryPage(whereExpression, page, size, " ID desc");
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 新增IP白名单
        /// </summary>
        /// <param name="input">IPEntity</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Add(SFIPWhiteList input)
        {
            if (string.IsNullOrWhiteSpace(input.BeginIP) || string.IsNullOrWhiteSpace(input.BeginIP) || string.IsNullOrWhiteSpace(input.iptype))
            {
                return ResponseOutput.NotOk("必填项不能为空");
            }

            var entity = await _IPWhiteListService.Query(ii => ii.BeginIP == input.BeginIP && ii.EndIP == input.EndIP);
            if (entity.Any())
            {
                return ResponseOutput.NotOk("IP段已存在");
            }

            input.updatedate = DateTime.Now;
            int result = await _IPWhiteListService.Add(input);

            return result > 0 ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 编辑IP白名单
        /// </summary>
        /// <param name="input">entity</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Edit(SFIPWhiteList input)
        {
            if (input.ID <= 0 || string.IsNullOrWhiteSpace(input.BeginIP)
                || string.IsNullOrWhiteSpace(input.BeginIP) || string.IsNullOrWhiteSpace(input.iptype))
            {
                return ResponseOutput.NotOk("必填项不能为空");
            }

            var entity = await _IPWhiteListService.Query(ii => ii.BeginIP == input.BeginIP && ii.EndIP == input.EndIP && ii.ID != input.ID);
            if (entity.Any())
            {
                return ResponseOutput.NotOk("IP段已存在");
            }

            input.updatedate = DateTime.Now;
            bool result = await _IPWhiteListService.Update(input);

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("编辑失败");
        }


        /// <summary>
        /// 删除IP白名单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int id)
        {
            bool result = await _IPWhiteListService.DeleteById(id);

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }
    }
}
