using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Services.Warning;
using XHS.Build.Common.Helps;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Common.Auth;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 告警分析
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    public class WarningController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IWarningService _warningService;


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="warningService"></param>
        public WarningController(IUser user,IWarningService warningService)
        {
            _user = user;
            _warningService = warningService;
        }


        /// <summary>
        /// 告警统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T071/[action]")]
        public async Task<IResponseOutput> WarningStatistic(DateTime startTime, DateTime endTime)
        {
            var result = await _warningService.spV2WarnStats(
                startTime.ToString("yyyy-MM-dd")
                , endTime.ToString("yyyy-MM-dd")
                , _user.SiteId.ToInt()
                );
             
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 告警分析-每日数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T071/[action]")]
        public async Task<IResponseOutput> WarningStatisticDaily(DateTime startTime, DateTime endTime)
        {
            int days = (endTime - startTime).TotalDays.ToInt();
            if (days <= 0 || days > 365)
            {
                return ResponseOutput.NotOk("日期超限");
            }

            var result = await _warningService.spV2WarnStatsDaily(
                startTime.ToString("yyyy-MM-dd")
                , endTime.ToString("yyyy-MM-dd")
                , _user.SiteId.ToInt()
                );
            var list = result.ToDataList<WarningDailyDto>();

            JArray arr = new JArray();
            for (int i = 0; i <= days; i++)
            {
                string tmpDay = startTime.AddDays(i).ToString("yyyy-MM-dd");
                JObject obj = new JObject();
                obj["Date"] = tmpDay;
                obj["Level1"] = list.Find(ii => ii.warnlevel == 1 && ii.createdate == tmpDay) == null ? 0 :
                    list.Find(ii => ii.warnlevel == 1 && ii.createdate == tmpDay).count;
                obj["Level2"] = list.Find(ii => ii.warnlevel == 2 && ii.createdate == tmpDay) == null ? 0 :
                    list.Find(ii => ii.warnlevel == 2 && ii.createdate == tmpDay).count;
                obj["Level3"] = list.Find(ii => ii.warnlevel == 3 && ii.createdate == tmpDay) == null ? 0 :
                    list.Find(ii => ii.warnlevel == 3 && ii.createdate == tmpDay).count;
                arr.Add(obj);
                 
            }

            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 告警统计（每日，按设备类型分）
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="deviceType">1 扬尘，2 摄像头，3 塔吊，4 升降机，5 卸料平台，6 深基坑，7 高支模，8 临边围挡（旧）,9 实名制</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        [Route("T033/[action]")]
        [Route("T037/[action]")]
        [Route("T038/[action]")]
        [Route("T071/[action]")]
        [Route("T083/[action]")]
        public async Task<IResponseOutput> WarningTypeDaily(DateTime startTime, DateTime endTime, int deviceType)
        {
            int days = (endTime - startTime).TotalDays.ToInt();
            if (days <= 0 || days > 365)
            {
                return ResponseOutput.NotOk("日期超限");
            }

            var result = await _warningService.spV2WarnTypeDaily(
                startTime.ToString("yyyy-MM-dd")
                , endTime.ToString("yyyy-MM-dd")
                , _user.SiteId.ToInt()
                , deviceType
                );

            //var list = result.ToDataList<WarningDailyDto>();

            //JArray arr = new JArray();
            //for (int i = 0; i <= days; i++)
            //{
            //    string tmpDay = startTime.AddDays(i).ToString("yyyy-MM-dd");
            //    JObject obj = new JObject();
            //    obj["Date"] = tmpDay;
            //    obj["Count"] = list.Find(ii => ii.createdate == tmpDay) == null ? 0 : list.Find(ii => ii.createdate == tmpDay).count;
            //    arr.Add(obj);
            //}

            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 通用告警-实时告警数据列表
        /// </summary>
        /// <param name="type">1 扬尘，2 摄像头，3 塔吊，4 升降机，5 卸料平台，6 深基坑，7 高支模，8 临边围挡（旧）,9 实名制</param>
        /// <param name="keyword">设备编号、名称、所属区域</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        [Route("T031/[action]")]
        [Route("T032/[action]")]
        [Route("T033/[action]")]
        [Route("T037/[action]")]
        [Route("T038/[action]")]
        [Route("T083/[action]")]
        public async Task<IResponseOutput> LiveList(int type, string keyword, DateTime startTime, DateTime endTime,
            int pageIndex = 1, int pageSize = 20)
        {
            var result = await _warningService.spV2WarnLiveList(type, _user.SiteId.ToInt(), keyword, startTime, endTime, pageSize, pageIndex);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 告警分析-分类统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T071/[action]")]
        public async Task<IResponseOutput> TypeStatistic(DateTime startTime, DateTime endTime)
        {
            int days = (endTime - startTime).TotalDays.ToInt();
            if (days <= 0 || days > 365)
            {
                return ResponseOutput.NotOk("日期超限");
            }

            var result = await _warningService.spV2WarnTypeStats(_user.SiteId.ToInt(), startTime, endTime);
            JObject objReturn = new JObject();
            objReturn["radar"] = Newtonsoft.Json.JsonConvert.SerializeObject(result.Tables[0]);

            var list = result.Tables[1]?.ToDataList<WarnTypeDto>();
            JArray arr = new JArray();
            for (int i = 0; i <= days; i++)
            {
                string tmpDay = startTime.AddDays(i).ToString("yyyy-MM-dd");
                JObject obj = new JObject();
                JArray arrValue = new JArray();
                obj["Date"] = tmpDay;
                list.Where(ii => ii.createdate.ToString("yyyy-MM-dd") == tmpDay)
                    .GroupBy(ii => ii.category).ToList()
                    .ForEach(ii =>
                    {
                        JObject objValue = new JObject();
                        objValue["type"] = ii.Key;
                        objValue["count"] = list.Find(jj => jj.category == ii.Key) == null ? 0 : list.Find(jj => jj.category == ii.Key).count;
                        arrValue.Add(objValue);
                    });
                obj["Value"] = arrValue;

                arr.Add(obj);
            }

            objReturn["line"] = arr;
            objReturn["type"] = Newtonsoft.Json.JsonConvert.SerializeObject(list.GroupBy(ii => ii.category).Select(ii => ii.Key));

            return ResponseOutput.Ok(objReturn);
        }


        /// <summary>
        /// 告警分析-实时数据下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T072/[action]")]
        public async Task<IResponseOutput> WarnLiveSelect()
        {
            var result = await _warningService.spV2WarnLiveSelect();
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 告警分析-实时数据(按模块分)
        /// </summary>
        /// <param name="keyword">关键词(设备编号)</param>
        /// <param name="MPID">模块ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T072/[action]")]
        public async Task<IResponseOutput> WarnTypeLiveList(string keyword, DateTime startDate, DateTime endDate,
            int MPID = -1, int pageIndex = 1, int pageSize = 20)
        {
            var result = await _warningService.spV2WarnTypeLive(_user.SiteId.ToInt(), keyword, MPID, startDate, endDate,
            pageIndex, pageSize);

            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 告警排行
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T002/[action]")]
        public async Task<IResponseOutput> WarnTypeRank(DateTime startDate, DateTime endDate)
        {
            int days = (endDate - startDate).TotalDays.ToInt();
            if (days <= 0 || days > 90)
            {
                return ResponseOutput.NotOk("日期超限");
            }

            var result = await _warningService.spV2WarnTypeRank(
                startDate.ToString("yyyy-MM-dd")
                , endDate.ToString("yyyy-MM-dd")
                , _user.SiteId.ToInt()
                );
           

            return ResponseOutput.Ok(result);
        }
    }
}
