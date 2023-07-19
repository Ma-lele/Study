using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.DeepPit;
using XHS.Build.Services.Device;
using XHS.Build.Services.File;
using XHS.Build.Services.SystemSetting;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 深基坑
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class DeepPitController : ControllerBase
    {
        private readonly IDeepPitService _deepPitService;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IHpFileDoc _hpFileDoc;
        private readonly IDeviceService _deviceService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deepPitService"></param>
        /// <param name="user"></param>
        /// <param name="mapper"></param>
        /// <param name="hpSystemSetting"></param>
        /// <param name="hpFileDoc"></param>
        public DeepPitController(IDeepPitService deepPitService, IUser user, IMapper mapper, IHpSystemSetting hpSystemSetting,
            IHpFileDoc hpFileDoc, IDeviceService deviceService)
        {
            _deepPitService = deepPitService;
            _user = user;
            _mapper = mapper;
            _hpSystemSetting = hpSystemSetting;
            _hpFileDoc = hpFileDoc;
            _deviceService = deviceService;
        }


        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _deepPitService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(DeepPitInputDto input)
        {
            input.bdel = 0;
            input.createtime = DateTime.Now;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var entity = _mapper.Map<GCDeepPit>(input);

            var result = await _deepPitService.AddEntity(entity);
            if (result != null && result.DPID > 0)
            {
                var suc = await _hpFileDoc.doUpdate(input.fileid,
                     new FileEntity.FileEntityParam
                     {
                         filetype = FileEntity.FileType.DeepPit,
                         linkid = result.DPID.ToString(),
                         GROUPID = result.GROUPID,
                         SITEID = result.SITEID
                     },
                     _user.Name
                     );
                if (suc)
                {
                    result.dpurl = $"http://{_hpSystemSetting.getSettingValue("S034")}/resourse/{result.GROUPID}/DeepPit/{result.SITEID}/{result.DPID}/{input.fileid}";
                    var success = await _deepPitService.Update(result);
                }
            }

            return (result != null && result.DPID > 0) ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 检查同一group下code是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupid"></param>
        /// <param name="dpid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> CheckCode(string code, int groupid, int dpid = 0)
        {
            bool exists = await _deepPitService.CheckCode(code, groupid, dpid);
            return ResponseOutput.Ok(exists);
        }


        /// <summary>
        /// 获取深基坑分页数据
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
            var data = await _deepPitService.GetDpPageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(DeepPitInputDto input)
        {
            if (input == null || input.DPID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            if (!string.IsNullOrWhiteSpace(input.fileid))
            {
                input.dpurl = $"http://{_hpSystemSetting.getSettingValue("S034")}/resourse/{input.GROUPID}/DeepPit/{input.SITEID}/{input.DPID}/{input.fileid}";
            }

            var entity = _mapper.Map<GCDeepPit>(input);
            bool suc = await _deepPitService.Update(entity);
            if (suc)
            {
                if (!string.IsNullOrWhiteSpace(input.fileid))
                {
                    var success = await _hpFileDoc.doUpdate(input.fileid,
                     new FileEntity.FileEntityParam
                     {
                         filetype = FileEntity.FileType.DeepPit,
                         linkid = input.DPID.ToString(),
                         GROUPID = input.GROUPID,
                         SITEID = input.SITEID
                     },
                     _user.Name
                     );
                }
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int id)
        {
            if (id <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }
            GCDeepPit input = new GCDeepPit();
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            input.bdel = 1;
            input.DPID = id;

            var entity = await _deepPitService.QueryById(id);
            if (entity == null || entity.DPID <= 0)
            {
                return ResponseOutput.NotOk("数据不存在");
            }

            bool result = await _deepPitService.Delete(input);
            if (result)
            {
                var delHistory = new GCDevDelHis
                {
                    GROUPID = entity.GROUPID,
                    SITEID = entity.SITEID,
                    DEVID = entity.DPID,
                    devcode = entity.dpcode,
                    devtype = 10,
                    devtypename = "深基坑监测",
                    @operator = _user.Name,
                    operatedate = DateTime.Now
                };
                await _deviceService.AddDevDelHis(delHistory);
            }

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }


        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddDevice(GCDeepPitDevice input)
        {
            input.bdel = 0;
            input.createtime = DateTime.Now;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;

            var dp = await _deepPitService.Query(ii => ii.DPID == input.DPID);
            if (!dp.Any())
            {
                return ResponseOutput.NotOk("找不到深基坑");
            }
            input.dpcode = dp.FirstOrDefault().dpcode;
            input.GROUPID = dp.FirstOrDefault().GROUPID;
            input.SITEID = dp.FirstOrDefault().SITEID;

            var result = await _deepPitService.AddDevice(input);


            return result > 0 ? ResponseOutput.Ok() : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 编辑设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> EditDevice(GCDeepPitDevice input)
        {
            if (input == null || input.DPDID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            var dp = await _deepPitService.Query(ii => ii.DPID == input.DPID);
            if (!dp.Any())
            {
                return ResponseOutput.NotOk("找不到深基坑");
            }
            input.dpcode = dp.FirstOrDefault().dpcode;
            input.GROUPID = dp.FirstOrDefault().GROUPID;
            input.SITEID = dp.FirstOrDefault().SITEID;

            bool suc = await _deepPitService.EditDevice(input);


            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("修改失败");
        }


        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="DPDID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeleteDevice(int DPDID)
        {
            if (DPDID <= 0)
            {
                return ResponseOutput.NotOk("请选中一行");
            }
            var entity = new GCDeepPitDevice { 
                DPDID = DPDID,
                bdel = 1,
                operatedate = DateTime.Now,
                @operator = _user.Name
            };
            bool suc = await _deepPitService.DeleteDevice(entity);
            return suc ? ResponseOutput.Ok() : ResponseOutput.NotOk("删除失败");
        }

        /// <summary>
        /// 获取深基坑设备分页数据
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetDeviceList(string keyword = "", int page = 1, int size = 20)
        {
            var data = await _deepPitService.GetDevicePageList( keyword, page, size);
            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 检查深基坑设备编号是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> CheckDeviceCode(string code)
        {
            bool exists = await _deepPitService.CheckDeviceCode(code);
            return ResponseOutput.Ok(exists);
        }


        /// <summary>
        /// 获取所有深基坑列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetDpSelect()
        {
            var result = await _deepPitService.Query(ii => ii.bdel == 0);
            return ResponseOutput.Ok(result);
        }
    }
}
