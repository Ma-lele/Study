using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Device;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 设备
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        public readonly IUser _user;
        private readonly IDeviceService _deviceService;
        public DeviceController(IDeviceService deviceService, IUser user)
        {
            _deviceService = deviceService;
            _user = user;
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
            var data = await _deviceService.GetSiteDevicePageList(groupid, keyword, page, size);
            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _deviceService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(GCDeviceEntity input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var DBs = await _deviceService.Query(a => a.devicecode == input.devicecode);
            if (DBs.Any())
            {
                return ResponseOutput.NotOk("该设备编号已存在");
            }

            DBs = await _deviceService.Query(a => a.SITEID == input.SITEID && a.SITEID > 0);
            if (DBs.Any())
            {
                return ResponseOutput.NotOk("该监测对象已绑定过扬尘设备");
            }

            input.operatedate = DateTime.Now;
            input.@operator = _user.Name; 
            var rows = await _deviceService.Add(input);

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
            var entity = await _deviceService.QueryById(id);
            if (entity == null || entity.DEVICEID <= 0)
            {
                return ResponseOutput.NotOk("数据不存在");
            }

            bool isDel = await _deviceService.DeleteById(id);
            if (isDel)
            {
                //删除实时表中，设备编号+监测点已经不存在的数据
                await _deviceService.doRtdDelete();

                var delHistory = new GCDevDelHis { 
                    GROUPID = entity.GROUPID,
                    SITEID = entity.SITEID,
                    DEVID = entity.DEVICEID,
                    devcode = entity.devicecode,
                    devtype = 1,
                    devtypename = "扬尘监测",
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
        public async Task<IResponseOutput> Put(GCDeviceEntity input)
        {
            if (input == null || input.DEVICEID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var DBs = await _deviceService.Query(a => a.devicecode == input.devicecode && a.DEVICEID != input.DEVICEID);
            if (DBs.Any())
            {
                return ResponseOutput.NotOk("该设备编号已存在");
            }

            DBs = await _deviceService.Query(a => a.SITEID == input.SITEID && a.DEVICEID != input.DEVICEID && a.SITEID > 0);
            if (DBs.Any())
            {
                return ResponseOutput.NotOk("该监测对象已绑定过扬尘设备");
            }

            input.operatedate = DateTime.Now;
            bool suc = await _deviceService.Update(input);
            if (suc)
            {
                //删除实时表中，设备编号+监测点已经不存在的数据
                await _deviceService.doRtdDelete();
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
            return ResponseOutput.Ok(await _deviceService.QueryById(id));
        }


        /// <summary>
        /// 获取未绑定的设备（后台扬尘监测点绑定设备使用）
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetUnBind(int groupid)
        {
            return ResponseOutput.Ok(
                await _deviceService.Query(ii => ii.GROUPID == groupid && ii.bdel == 0 && ii.SITEID == 0)
                );
        }


        /// <summary>
        /// 绑定设备（后台扬尘监测点绑定）
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="siteid"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> BindDevice(int groupid, int siteid, string deviceCode)
        {
            var data = await _deviceService.Query(ii => ii.GROUPID == groupid && ii.bdel == 0 &&
                ii.SITEID == 0 && ii.devicecode == deviceCode);

            if (!data.Any())
            {
                return ResponseOutput.NotOk("设备不存在");
            }
            var device = data.FirstOrDefault();
            device.SITEID = siteid;
            device.@operator = _user.Name;
            device.operatedate = DateTime.Now;

            var result = await _deviceService.Update(device, new List<string>() { "SITEID", "operatedate", "operator" });
            return result ? ResponseOutput.Ok("绑定成功") : ResponseOutput.NotOk("绑定失败");
        }


        /// <summary>
        /// 解除设备绑定（后台扬尘监测点）
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="siteid"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> UnbindDevice(int groupid, int siteid, string deviceCode)
        {
            var data = await _deviceService.Query(ii => ii.GROUPID == groupid && ii.bdel == 0 &&
                ii.SITEID == siteid && ii.devicecode == deviceCode);

            if (!data.Any())
            {
                return ResponseOutput.NotOk("设备不存在");
            }
            var device = data.FirstOrDefault();
            device.SITEID = 0;
            device.@operator = _user.Name;
            device.operatedate = DateTime.Now;

            var result = await _deviceService.Update(device, new List<string>() { "SITEID", "operatedate", "operator" });
            await _deviceService.DeleteRtd(deviceCode, siteid);

            return result ? ResponseOutput.Ok("解绑成功") : ResponseOutput.NotOk("解绑失败");
        }
    }
}
