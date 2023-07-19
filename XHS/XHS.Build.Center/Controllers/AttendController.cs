using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Center.Attributes;
using XHS.Build.Center.Auth;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.DeviceBind;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class AttendController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;
        private readonly ConfigHelper _configHelper;
        private readonly IUserKey _userKey;
        private readonly ICache _cache;
        public AttendController(NetToken netToken, IDeviceBindService deviceBindService, IUserKey userKey, ICache cache)
        {
            _netToken = netToken;
            _deviceBindService = deviceBindService;
            _configHelper = new ConfigHelper();
            _userKey = userKey;
            _cache = cache;
        }

        /// <summary>
        /// 同步人员基本信息
        /// </summary>
        [HttpPost]
        public async Task<IResponseOutput> Employee(EmployeeInput entity)
        {
            //var keyList = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
            var keyList = _cache.Get<KeySecretList>("keysecretconfig");
            if (keyList == null)
            {
                keyList = new ConfigHelper().Get<KeySecretList>("keysecretconfig", "", true);
                _cache.Set("keysecretconfig", keyList, TimeSpan.FromHours(1));
            }
            if (keyList == null || keyList.Items == null || keyList.Items.Count == 0)
            {
                return ResponseOutput.NotOk<string>("同步信息未配置");
            }
            var domains = keyList.Items.FirstOrDefault(k => k.Key == _userKey.Key).Domains;
            if (domains == null || domains.Length == 0)
            {
                return ResponseOutput.NotOk<string>("同步信息未配置");
            }
            if (entity == null || string.IsNullOrEmpty(entity.ID))
            {
                return ResponseOutput.NotOk("请填写正确的数据");
            }
            var DBList = new List<DeviceBindOutput>() { };//await _deviceBindService.GetDeviceBindByCodeType(entity.ID, "attend");
            foreach (var domain in domains)
            {
                DBList.Add(new DeviceBindOutput() { Domain = domain, Netport = "9037" });
            }
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, entity);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 同步进出场实时记录
        /// </summary>
        [HttpPost]
        public async Task<IResponseOutput> EmployeePass(EmployeePassHisInsertInput entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.attendprojid))
            {
                return ResponseOutput.NotOk("请填写正确的数据");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(entity.attendprojid, "attend");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, entity);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }

        /// <summary>
        /// 用户站点信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> EmployeeSite(GCEmployeeSiteEntity entity)
        {
            if (entity == null || string.IsNullOrEmpty(entity.attendprojid))
            {
                return ResponseOutput.NotOk("请填写正确的数据");
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(entity.attendprojid, "attend");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, entity);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }
        }
    }
}
