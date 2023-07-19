using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.AmDisclose;
using XHS.Build.Services.Group;
using XHS.Build.Services.NetData;
using XHS.Build.Services.SystemSetting;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 请求网络数据
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class NetDataController : ControllerBase
    {
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IGroupService _groupService;
        private readonly INetDataService _netDataService;
        private readonly IAmDiscloseService _amDiscloseService;
        public NetDataController(IHpSystemSetting hpSystemSetting, IAmDiscloseService amDiscloseService, IGroupService groupService, INetDataService netDataService)
        {
            _hpSystemSetting = hpSystemSetting;
            _groupService = groupService;
            _netDataService = netDataService;
            _amDiscloseService = amDiscloseService;
        }

        /// <summary>
        /// 车辆冲洗抓拍
        /// </summary>
        /// <param name="parkkey"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> WashCount(string parkkey)
        {
            var s60 = _hpSystemSetting.getSettingValue(Const.Setting.S060);
            if (string.IsNullOrEmpty(s60))
            {
                return ResponseOutput.NotOk("未获取到请求地址信息");
            }
            DataTable dtGroup = await _groupService.GroupGet();
            if (dtGroup == null || dtGroup.Rows.Count == 0)
            {
                return ResponseOutput.NotOk("用户信息错误");
            }
            string SecretKey = Convert.ToString(dtGroup.Rows[0]["washsecret"]);

            var retString = HttpNetRequest.PostSendRequestUrl(s60 + "getParkNoWashCount", new Dictionary<string, object>() { { "SecretKey", SecretKey }, { "parkKey", parkkey } });
            if (string.IsNullOrEmpty(retString))
            {
                return ResponseOutput.NotOk("返回内容为空");
            }
            var retObj= JsonConvert.DeserializeObject<WashNetDto>(retString);
            if (retObj.Success)
            {
                return ResponseOutput.Ok(retObj.Data);
            }
            else
            {
                return ResponseOutput.NotOk(retObj.Data);
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parkKey"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="washResult"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ParkOrderRecords(string parkKey, DateTime starttime, DateTime endtime, int washResult, int pageIndex, int pageSize)
        {
            var s60 = _hpSystemSetting.getSettingValue(Const.Setting.S060);
            if (string.IsNullOrEmpty(s60))
            {
                return ResponseOutput.NotOk("未获取到请求地址信息");
            }
            DataTable dtGroup = await _groupService.GroupGet();
            if (dtGroup == null || dtGroup.Rows.Count == 0)
            {
                return ResponseOutput.NotOk("用户信息错误");
            }
            string SecretKey = Convert.ToString(dtGroup.Rows[0]["washsecret"]);
            var retString = HttpNetRequest.PostSendRequestUrl(s60 + "parkOrderRecords",
                new Dictionary<string, object>() { { "washResult", washResult }, { "cstart", starttime }, { "cend", endtime }, { "SecretKey", SecretKey }, { "parkKey", parkKey }, { "pageIndex", pageIndex }, { "pageSize", pageSize } });
            if (string.IsNullOrEmpty(retString))
            {
                return ResponseOutput.NotOk("返回内容为空");
            }
            var retObj = JsonConvert.DeserializeObject<WashNetDto>(retString);
            if (retObj.Success)
            {
                return ResponseOutput.Ok(retObj.Data);
            }
            else
            {
                return ResponseOutput.NotOk(retObj.Data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="date">时间格式（yyyy-MM-dd）</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetMeetingData(string projId,string date,int page,int limit)
        {
            if (projId.Contains("-"))
            {
                var token = _netDataService.getPlateToken();
                if (string.IsNullOrEmpty(token))
                {
                    return ResponseOutput.NotOk("获取token错误");
                }
                var s147 = _hpSystemSetting.getSettingValue(Const.Setting.S147);
                if (string.IsNullOrEmpty(s147))
                {
                    return ResponseOutput.NotOk("未获取到请求地址信息");
                }
                var postData = new { workSiteId =projId, page =page, limit =limit, startTime =date+" 00:00:00", endTime =date+" 23:59:59"};
                var retString= HttpNetRequest.POSTSendJsonRequest(s147 + "getMeetingData", JsonConvert.SerializeObject(postData), new Dictionary<string, string>() { { "Authorization", token } });
                return ResponseOutput.Ok(JsonConvert.DeserializeObject<dynamic>(retString));
                }
            else
            {
                return ResponseOutput.Ok(JsonConvert.DeserializeObject<dynamic>(await _amDiscloseService.GetAmDiscloseByDay(projId, date, page, limit)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> CountMeetingData(string projId)
        {
            if (projId.Contains("-"))
            {
                var token = _netDataService.getPlateToken();
                if (string.IsNullOrEmpty(token))
                {
                    return ResponseOutput.NotOk("获取token错误");
                }
                var s147 = _hpSystemSetting.getSettingValue(Const.Setting.S147);
                if (string.IsNullOrEmpty(s147))
                {
                    return ResponseOutput.NotOk("未获取到请求地址信息");
                }
                var postData = new { workSiteId = projId };
                var retString = HttpNetRequest.POSTSendJsonRequest(s147 + "countMeetingData", JsonConvert.SerializeObject(postData), new Dictionary<string, string>() { { "Authorization", token } });
                return ResponseOutput.Ok(JsonConvert.DeserializeObject<dynamic>(retString));
            }
            else
            {
                return ResponseOutput.Ok(JsonConvert.DeserializeObject<dynamic>(await _amDiscloseService.GetCount(projId)));
            }
        }
    }
}
