using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.AssetInspection;
using XHS.Build.Common.Helps;
using Newtonsoft.Json.Linq;
using XHS.Build.Model.ModelDtos;
using AutoMapper;
using XHS.Build.Common;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 资产巡检
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class AssetInspectionController : ControllerBase
    {
        private readonly IAssetInspectionService _assetInspectionService;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        public AssetInspectionController(IUser user, IAssetInspectionService assetInspectionService, IMapper mapper)
        {
            _user = user;
            _assetInspectionService = assetInspectionService;
            _mapper = mapper;
        }


        /// <summary>
        /// 新增租户类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddTenantType(CATenantType input)
        {
            if (string.IsNullOrWhiteSpace(input.name))
            {
                return ResponseOutput.NotOk("请填写参数");
            }
            if (input.sort > 256)
            {
                return ResponseOutput.NotOk("排序数值不能大于255");
            }

            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            input.Status = 0;

            bool result = await _assetInspectionService.AddTenantType(input);
            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败，请检查是否重名");
        }


        /// <summary>
        /// 获取租户类型列表分页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTenantTypeList(string keyword, int page = 1, int size = 20)
        {
            Expression<Func<CATenantType, bool>> whereExpression = ii => ii.Status == 0;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                whereExpression = whereExpression.And(ii => ii.name.Contains(keyword));
            }

            var list = await _assetInspectionService.GetTenantTypeList(whereExpression, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 编辑租户类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> EditTenantType(CATenantType input)
        {
            input.@operator = _user.Name;
            input.operatedate = DateTime.Now;
            bool result = await _assetInspectionService.EditTenantType(input);
            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("编辑失败，请检查是否重名");
        }


        /// <summary>
        /// 删除租户类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> DeleteTenantType(int id)
        {
            CATenantType input = new CATenantType
            {
                TTID = id,
                Status = 1,
                @operator = _user.Name,
                operatedate = DateTime.Now
            };

            bool result = await _assetInspectionService.DeleteTenantType(input);
            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }


        /// <summary>
        /// 新增检查模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddInspectionTemplate(InspectionTemplateDto input)
        {
            if (string.IsNullOrWhiteSpace(input.name) || input.TTID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            if (!await _assetInspectionService.CheckTemplate(input.TTID))
            {
                return ResponseOutput.NotOk("此类型模板已存在");
            }

            var template = _mapper.Map<CACheckModel>(input);
            template.bdel = 0;
            template.@operator = _user.Name;
            template.operatedate = DateTime.Now;

            var result = await _assetInspectionService.AddTemplate(template);
            if (result == null || result.CMID <= 0)
            {
                return ResponseOutput.NotOk("新增失败");
            }

            _assetInspectionService.UpdateUser(result.CMID, result.TTID);


            var items = _mapper.Map<List<CACheckList>>(input.conditions);



            if (items.Count > 0)
            {
                items.ForEach(ii =>
                {
                    ii.bdel = 0;
                    ii.linkid = result.CMID;
                    ii.@operator = _user.Name;
                    ii.operatedate = DateTime.Now;
                    ii.sort = items.IndexOf(ii);
                    ii.type = 1;
                });

                int count = await _assetInspectionService.AddTemplateItems(items);
                if (count <= 0)
                {
                    return ResponseOutput.NotOk("新增失败");
                }
            }
            return ResponseOutput.Ok();
        }


        /// <summary>
        /// 获取检查模板列表分页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetInsTmpListAsync(string keyword, int page = 1, int size = 20)
        {
            Expression<Func<CACheckModel, bool>> whereExpression = model => true;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                whereExpression = whereExpression.And(model => model.name.Contains(keyword));
            }

            var list = await _assetInspectionService.GetInsTmpListAsync(whereExpression, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 获取模板检查项列表
        /// </summary>
        /// <param name="CMID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetCheckListAsync(int CMID)
        {
            var list = await _assetInspectionService.GetCheckListAsync(CMID);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 编辑检查模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> EditInsTmpAsync(InspectionTemplateDto input)
        {
            if (string.IsNullOrWhiteSpace(input.name) || input.TTID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            if (!await _assetInspectionService.CheckTemplate(input.TTID, input.CMID))
            {
                return ResponseOutput.NotOk("此类型模板已存在");
            }

            var template = _mapper.Map<CACheckModel>(input);
            template.bdel = 0;
            template.@operator = _user.Name;
            template.operatedate = DateTime.Now;

            bool result = await _assetInspectionService.EditTemplate(template);
            if (!result)
            {
                return ResponseOutput.NotOk("编辑失败");
            }
            var items = _mapper.Map<List<CACheckList>>(input.conditions);



            if (items.Count > 0)
            {
                items.ForEach(ii =>
                {
                    ii.bdel = 0;
                    ii.linkid = template.CMID;
                    ii.@operator = _user.Name;
                    ii.operatedate = DateTime.Now;
                    ii.sort = items.IndexOf(ii);
                    ii.type = 1;
                });

                int count = await _assetInspectionService.AddTemplateItems(items);
                if (count <= 0)
                {
                    return ResponseOutput.NotOk("编辑失败");
                }
            }
            return ResponseOutput.Ok();
        }


        /// <summary>
        /// 删除检查模板
        /// </summary>
        /// <param name="CMID"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> DelInsTmpAsync(int CMID)
        {
            if (CMID <= 0)
            {
                return ResponseOutput.NotOk("请选择一行");
            }
            bool result = await _assetInspectionService.DelTemplate(CMID, _user.Name);
            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }


        /// <summary>
        /// 获取租户列表分页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="groupid"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTenantListAsync(int groupid, string keyword, int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var list = await _assetInspectionService.GetTenantListAsync(groupid, keyword, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 登录名查重
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> CheckUsername(string username)
        {
            return ResponseOutput.Ok(await _assetInspectionService.CheckUsername(username));
        }


        /// <summary>
        /// 分组Count
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTenantGroupCount()
        {
            return ResponseOutput.Ok(await _assetInspectionService.GetTenantGroupCount());
        }


        /// <summary>
        /// 新增租户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddTenantAsync(CATenant input)
        {
            if (string.IsNullOrWhiteSpace(input.name) || input.TTID <= 0 || string.IsNullOrWhiteSpace(input.username))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            if (!await _assetInspectionService.CheckUsername(input.username))
            {
                return ResponseOutput.NotOk("此登录名已存在");
            }
            int CMID = await _assetInspectionService.GetCMID(input.TTID);
            input.CMID = CMID;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            input.pwd = "0x" + UEncrypter.EncryptBySHA1(input.tel).Replace("-", "").ToLower();

            var result = await _assetInspectionService.AddTenant(input);

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 编辑租户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> EditTenantAsync(CATenant input)
        {
            if (string.IsNullOrWhiteSpace(input.name) || input.TTID <= 0 || string.IsNullOrWhiteSpace(input.username))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            if (!await _assetInspectionService.CheckUsername(input.TEID, input.username))
            {
                return ResponseOutput.NotOk("此登录名已存在");
            }

            int CMID = await _assetInspectionService.GetCMID(input.TTID);
            input.CMID = CMID;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var result = await _assetInspectionService.EditTenant(input);

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("编辑失败");
        }


        /// <summary>
        /// 获取自定义检查项
        /// </summary>
        /// <param name="TEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetCustom(int TEID)
        {
            var list = await _assetInspectionService.GetCustom(TEID);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 设置自定义检查项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetCustom(CustomDto input)
        {
            if (input.TEID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var items = _mapper.Map<List<CACheckList>>(input.conditions);
            await _assetInspectionService.DelCustom(input.TEID, _user.Name);

            if (items.Count > 0)
            {
                items.ForEach(ii =>
                {
                    ii.bdel = 0;
                    ii.linkid = input.TEID;
                    ii.@operator = _user.Name;
                    ii.operatedate = DateTime.Now;
                    ii.sort = items.IndexOf(ii);
                    ii.type = 2;
                });

                int count = await _assetInspectionService.AddTemplateItems(items);
                if (count <= 0)
                {
                    return ResponseOutput.NotOk("新增失败");
                }
            }
            return ResponseOutput.Ok();
        }


        /// <summary>
        /// 获取资产巡检SITES
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetTenantSites(int GROUPID)
        {
            var list = await _assetInspectionService.GetTenantSites(GROUPID);
            return ResponseOutput.Ok(list);
        }
    }
}
