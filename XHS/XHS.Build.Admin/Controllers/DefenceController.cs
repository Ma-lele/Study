using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Defence;
using XHS.Build.Services.Device;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class DefenceController : ControllerBase
    {
        public readonly IDefenceService _defenceService;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly IDeviceService _deviceService;

        public DefenceController(IUser user, IDefenceService defenceService, IMapper mapper, IDeviceService deviceService)
        {
            _defenceService = defenceService;
            _user = user;
            _mapper = mapper;
            _deviceService = deviceService;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _defenceService.GetSiteDefencePageList(groupid, keyword, page, size);
            var d = _mapper.Map<List<DefenceOutputDto>>(data.data);

            var dataList = new PageOutput<DefenceOutputDto>()
            {
                data = d,
                dataCount = data.dataCount
            };

            return ResponseOutput.Ok(dataList);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _defenceService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(DefenceEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var DbEntity = await _defenceService.Query(a => a.dfcode == input.dfcode);
            if (DbEntity.Any())
            {
                return ResponseOutput.NotOk("已存在该设备编号");
            }
            input.@operator = _user.Name;
            input.operatedate = DateTime.Now;
            var rows = await _defenceService.Add(input);
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

            var entity = await _defenceService.QueryById(id);
            if (entity == null || entity.DEFENCEID <= 0)
            {
                return ResponseOutput.NotOk("数据不存在");
            }
            bool isDel = await _defenceService.DeleteById(id);
            if (isDel)
            {
                var delHistory = new GCDevDelHis
                {
                    GROUPID = entity.GROUPID,
                    SITEID = entity.SITEID,
                    DEVID = entity.DEFENCEID,
                    devcode = entity.dfcode,
                    devtype = 11,
                    devtypename = "临边防护",
                    @operator = _user.Name,
                    operatedate = DateTime.Now
                };
                await _deviceService.AddDevDelHis(delHistory);
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
        public async Task<IResponseOutput> Put(DefenceEntity input)
        {
            if (input == null || input.DEFENCEID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var DbEntity = await _defenceService.Query(a => a.dfcode == input.dfcode && a.DEFENCEID != input.DEFENCEID);
            if (DbEntity.Any())
            {
                return ResponseOutput.NotOk("已存在该设备编号");
            }
            input.operatedate = DateTime.Now;
            bool suc = await _defenceService.Update(input);
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
            return ResponseOutput.Ok(await _defenceService.QueryById(id));
        }
    }
}
