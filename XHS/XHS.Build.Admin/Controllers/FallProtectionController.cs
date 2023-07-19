using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.FallProtection;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 临边防护[新]
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class FallProtectionController : ControllerBase
    {
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IConfiguration _configuration;
        private readonly IFallProtectionService _fallProtectionService;
        private readonly IUser _user;
        


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="hpSystemSetting"></param>
        public FallProtectionController(IHpSystemSetting hpSystemSetting, IConfiguration configuration,
            IFallProtectionService fallProtectionService, IUser user)
        {
            _hpSystemSetting = hpSystemSetting;
            _configuration = configuration;
            _fallProtectionService = fallProtectionService;
            _user = user;
        }


        /// <summary>
        /// 设置服务器推送地址
        /// </summary>
        /// <param name="statusUrl"></param>
        /// <param name="dataUrl"></param>
        /// <param name="oldStatusUrl"></param>
        /// <param name="oldDataUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetPushUrl(string statusUrl, string dataUrl, string oldStatusUrl, string oldDataUrl)
        {
            if (string.IsNullOrWhiteSpace(FallProtectionHelper.Token) || FallProtectionHelper.TokenExpireTime <= DateTime.Now)
            {
                string account = _hpSystemSetting.getSettingValue("S198");
                string url = _configuration.GetSection("FallProtection:ServiceUrl").Value
                    + _configuration.GetSection("FallProtection:LoginApi").Value;
                if (!FallProtectionHelper.Login(url, account.Split(",")[0], account.Split(",")[1]))
                {
                    return ResponseOutput.NotOk("远程接口登录失败");
                }
            }


            string dataRegUrl = _configuration.GetSection("FallProtection:ServiceUrl").Value
                    + _configuration.GetSection("FallProtection:DataRegisterApi").Value;
            string dataUnRegUrl = _configuration.GetSection("FallProtection:ServiceUrl").Value
                    + _configuration.GetSection("FallProtection:DataUnregisterApi").Value;
            string statusRegUrl = _configuration.GetSection("FallProtection:ServiceUrl").Value
                    + _configuration.GetSection("FallProtection:StatusRegisterApi").Value;
            string statusUnRegUrl = _configuration.GetSection("FallProtection:ServiceUrl").Value
                    + _configuration.GetSection("FallProtection:StatusUnregisterApi").Value;


            FallProtectionHelper.Register(dataUnRegUrl, dataUrl);
            FallProtectionHelper.Register(statusUnRegUrl, statusUrl);

            if (!string.IsNullOrWhiteSpace(statusUrl) && statusUrl != oldStatusUrl)
            {
                var result = FallProtectionHelper.Register(statusRegUrl, statusUrl);
                if (!result)
                {
                    return ResponseOutput.NotOk("状态推送地址注册失败");
                }
            }
            if (!string.IsNullOrWhiteSpace(dataUrl) && dataUrl != oldDataUrl)
            {
                var result = FallProtectionHelper.Register(dataRegUrl, dataUrl);
                if (!result)
                {
                    return ResponseOutput.NotOk("数据推送地址注册失败");
                }
            }




            return ResponseOutput.Ok();
        }


        /// <summary>
        /// 获取服务器推送地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPushUrl()
        {
            var result = await _fallProtectionService.GetPushUrl();
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPageList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var list = await _fallProtectionService.GetPageListAsync(groupid, keyword, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _fallProtectionService.GetGroupCount());
        }


        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> AddDevice(GCFallProtectionDevice input)
        {
            if (input.GROUPID <= 0 || input.SITEID <= 0 || string.IsNullOrWhiteSpace(input.deviceId)
                || string.IsNullOrWhiteSpace(input.name))
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            input.bdel = false;
            input.creationtime = DateTime.Now;
            input.onlinestatus = 0;
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            input.lastpushtime = DateTime.Now;
            bool result = await _fallProtectionService.AddDevice(input);

            return result ? ResponseOutput.Ok("新增成功") : ResponseOutput.NotOk("新增失败");
        }


        /// <summary>
        /// 编辑设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> EditDevice(GCFallProtectionDevice input)
        {
            input.@operator = _user.Name;
            input.operatedate = DateTime.Now;
            bool result = await _fallProtectionService.EditDevice(input);
            return result ? ResponseOutput.Ok("修改成功") : ResponseOutput.NotOk("修改失败");
        }


        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="FPDID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeleteDevice(int FPDID)
        {
            bool result = await _fallProtectionService.DeleteDevice(FPDID, _user.Name);
            return result ? ResponseOutput.Ok("删除成功") : ResponseOutput.NotOk("删除失败");
        }


        /// <summary>
        /// 检查设备编号是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="FPDID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> CheckCode(string code, int FPDID = 0)
        {
            bool exists = await _fallProtectionService.CheckCode(code, FPDID);
            return ResponseOutput.Ok(exists);
        }
    }
}
