using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Dust;
using XHS.Build.Services.Group;
using XHS.Build.Services.Site;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 扬尘
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Permission]
    //[Authorize]
    public class DustController : ControllerBase
    {
        private readonly IDustService _dustService;
        private readonly IGroupService _groupService;
        private readonly ISiteService _siteService;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        public DustController(IDustService dustService, IGroupService groupService, ISiteService siteService, IUser user, IMapper mapper)
        {
            _dustService = dustService;
            _groupService = groupService;
            _siteService = siteService;
            _user = user;
            _mapper = mapper;
        }

        /// <summary>
        /// 扬尘 页面 Dataset
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPage()
        {
            return ResponseOutput.Ok(await _dustService.GetGroupListforSpot());
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="order"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 10, string order = "", string ordertype = "")
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _dustService.GetDustPageList(groupid, keyword, page, size,order,ordertype);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _dustService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(DustSiteAddInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.siteshortname) || input.PARENTSITEID < 0 || input.GROUPID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var parentEntity = await _siteService.QueryById(input.PARENTSITEID);
            if (parentEntity == null)
            {
                return ResponseOutput.NotOk("未找到所属监测对象");
            }
            input.sitename = parentEntity.sitename;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            var entity = _mapper.Map<GCSiteEntity>(input);
            entity.sitearea = parentEntity.sitearea;
            entity.sitetype = parentEntity.sitetype;
            var rows = await _siteService.Insert(entity);

            return rows > 0 ? ResponseOutput.Ok(rows) : ResponseOutput.NotOk("添加数据错误");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int siteid)
        {
            if (siteid <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }
            var entity = await _siteService.QueryById(siteid);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到相关数据");
            }
            entity.status = 3;
            entity.operatedate = DateTime.Now;
            entity.@operator = _user.Name;
            bool isDel = await _siteService.Update(entity, new List<string>() { "status", "operatedate", "operator" });
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
        public async Task<IResponseOutput> Put(DustSiteAddInput input)
        {
            if (input == null || input.SITEID <= 0 || string.IsNullOrEmpty(input.siteshortname) || input.GROUPID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            var entity = _mapper.Map<GCSiteEntity>(input);
            if (entity == null)
            {
                return ResponseOutput.NotOk("未找到监测对象");
            }
            var parentEntity = await _siteService.QueryById(input.PARENTSITEID);
            if (parentEntity == null)
            {
                return ResponseOutput.NotOk("未找到所属监测对象");
            }
            entity.sitename = parentEntity.sitename;
            entity.sitearea = parentEntity.sitearea;
            entity.sitetype = parentEntity.sitetype;
            bool suc = await _siteService.Update(entity);
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
            return ResponseOutput.Ok(await _siteService.QueryById(id));
        }
    }
}
