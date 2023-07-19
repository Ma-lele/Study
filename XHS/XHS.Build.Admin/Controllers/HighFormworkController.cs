using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Device;
using XHS.Build.Services.File;
using XHS.Build.Services.HighFormwork;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 高支模
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class HighFormworkController : ControllerBase
    {
        private readonly IHighFormworkService _highFormworkService;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IDeviceService _deviceService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="highFormworkService"></param>
        public HighFormworkController(IHighFormworkService highFormworkService, IUser user, IMapper mapper,
            IHpFileDoc hpFileDoc, IDeviceService deviceService)
        {
            _highFormworkService = highFormworkService;
            _user = user;
            _mapper = mapper;
            _hpFileDoc = hpFileDoc;
            _deviceService = deviceService;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCHighFormwork input)
        {
            input.bdel = 0;
            input.createtime = DateTime.Now;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var result = await _highFormworkService.Add(input);

            return result > 0 ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 检查同一group下code是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> CheckCode(string code, int groupid, int hfwid = 0)
        {
            bool exists = await _highFormworkService.CheckCode(code, groupid, hfwid);
            return ResponseOutput.Ok(exists);
        }


        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _highFormworkService.GetGroupCount());
        }


        /// <summary>
        /// 获取高支模分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _highFormworkService.GetHfwPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(GCHighFormwork input)
        {
            if (input == null || input.HFWID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _highFormworkService.Update(input);
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
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IResponseOutput> Delete(GCHighFormwork input)
        {
            if (input == null || input.HFWID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            input.bdel = 1;

            bool result = await _highFormworkService.Delete(input);

            if (result)
            {
                var delHistory = new GCDevDelHis
                {
                    GROUPID = input.GROUPID,
                    SITEID = input.SITEID,
                    DEVID = input.HFWID,
                    devcode = input.hfwcode,
                    devtype = 9,
                    devtypename = "高支模",
                    @operator = _user.Name,
                    operatedate = DateTime.Now
                };
                await _deviceService.AddDevDelHis(delHistory);
            }

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }


        /// <summary>
        /// 根据siteid获取下面所有高支模
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetHFWs(int siteid)
        {
            var result = await _highFormworkService.Query(ii => ii.SITEID == siteid && ii.bdel == 0);

            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 新增高支模区域
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddArea(HighFormworkAreaDto input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.fileid)
                || string.IsNullOrWhiteSpace(input.hfwaname))
            {
                return ResponseOutput.NotOk("请输入必填信息");
            }

            var entity = _mapper.Map<GCHighFormworkArea>(input);
            entity.bdel = 0;
            entity.createdate = DateTime.Now;
            entity.operatedate = DateTime.Now;
            entity.@operator = _user.Name;

            var result = await _highFormworkService.AddArea(entity);

            if (result == null || result.HFWAID <= 0)
            {
                return ResponseOutput.NotOk("新增失败");
            }

            var success = await _hpFileDoc.doUpdate(input.fileid,
                new FileEntity.FileEntityParam
                {
                    filetype = FileEntity.FileType.HighFormwork,
                    linkid = result.HFWAID.ToString(),
                    GROUPID = result.GROUPID,
                    SITEID = result.SITEID
                },
                _user.Name);

         
            return success ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 高支模分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> AreaGroups()
        {
            return ResponseOutput.Ok(await _highFormworkService.GetAreaGroupCount());
        }


        /// <summary>
        /// 获取高支模区域分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetAreaList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _highFormworkService.GetHfwaPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 修改高支模区域
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> PutArea(HighFormworkAreaUpdateDto input)
        {
            if (input == null || input.HFWAID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var entity = _mapper.Map<GCHighFormworkArea>(input);
            
            bool suc = await _highFormworkService.UpdateHFWA(entity);

            if (suc)
            {
                if (!string.IsNullOrWhiteSpace(input.FileID))
                {
                    var success = await _hpFileDoc.doUpdate(input.FileID,
                    new FileEntity.FileEntityParam
                    {
                        filetype = FileEntity.FileType.HighFormwork,
                        linkid = input.HFWAID.ToString(),
                        GROUPID = input.GROUPID,
                        SITEID = input.SITEID
                    },
                    _user.Name);
                }

                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }


        /// <summary>
        /// 删除高支模区域
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IResponseOutput> DeleteArea(GCHighFormworkArea input)
        {
            if (input == null || input.HFWID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            input.bdel = 1;

            bool result = await _highFormworkService.DeleteArea(input);

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }
    }
}
