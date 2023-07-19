using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Analyst.Web.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Analyse;
using XHS.Build.Services.Site;

namespace XHS.Build.Analyst.Web.Controllers
{
    /// <summary>
    /// 事件分析
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class EventController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IEventService _eventService;

        /// <summary>
        /// ctor
        /// </summary>
        public EventController(IUser user, IEventService eventService)
        {
            _user = user;
            _eventService = eventService;
        }


        /// <summary>
        /// 事件类型雷达图
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EventRadar(int SITEID = 0,string cityCode = "320200")
        {
            var list = await _eventService.EventRader(_user.AnalystRegionID, SITEID, cityCode);

            JArray arr = new JArray();
            list.GroupBy(gg => gg.etname).ToList().ForEach(ii => {
                JObject obj = new JObject();
                obj["name"] = ii.Key;
                obj["lv1"] = list.Where(ww => ww.etname == ii.Key && ww.eventlevel == 1).Sum(ss => ss.count);
                obj["lv2"] = list.Where(ww => ww.etname == ii.Key && ww.eventlevel == 2).Sum(ss => ss.count);
                obj["lv3"] = list.Where(ww => ww.etname == ii.Key && ww.eventlevel == 3).Sum(ss => ss.count);
                obj["lv4"] = list.Where(ww => ww.etname == ii.Key && ww.eventlevel == 4).Sum(ss => ss.count);
                arr.Add(obj);
            });

            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 事件统计+今日事件
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EventStatisticNToday(string cityCode = "320200")
        {
            return ResponseOutput.Ok(await _eventService.EventStatistic(_user.AnalystRegionID, cityCode));
        }


        /// <summary>
        /// 事件曲线图
        /// </summary>
        /// <param name="eventStatus"></param>
        /// <param name="days"></param>
        /// <param name="typecode"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EventCurve(int eventStatus,int days, string typecode = "", string cityCode = "320200")
        {
            var result = await _eventService.GetEventCurve(typecode,eventStatus, _user.AnalystRegionID, days, cityCode);
            List<DateTime> dateList = new List<DateTime>();
            for (int i = -29; i <= 0; i++)
            {
                dateList.Add(DateTime.Now.AddDays(i));
            }

            JArray arrList = new JArray();
            for (int i = 1; i < 5; i++)
            {
                JObject objLevel = new JObject();
                objLevel["level"] = i;

                JArray arr = new JArray();
                dateList.ForEach(ii =>
                {
                    JObject obj = new JObject();
                    obj["count"] = result.Find(jj => jj.EventLevel == i && jj.CreationDate == ii.ToString("yyyy/MM/dd")) == null ? 0 :
                    result.Find(jj => jj.EventLevel == i && jj.CreationDate == ii.ToString("yyyy/MM/dd")).Count;
                    obj["date"] = ii.ToString("yyyy-MM-dd");
                    obj["currentLv"] =
                          result.Find(jj => jj.CreationDate == ii.ToString("yyyy/MM/dd") && jj.EventLevel == 4 && jj.Count > 0) == null ?
                          (result.Find(jj => jj.CreationDate == ii.ToString("yyyy/MM/dd") && jj.EventLevel == 3 && jj.Count > 0) == null ?
                          (result.Find(jj => jj.CreationDate == ii.ToString("yyyy/MM/dd") && jj.EventLevel == 2 && jj.Count > 0) == null ? 1 : 2) : 3) : 4;
                    arr.Add(obj);
                });

                objLevel["data"] = arr;

                arrList.Add(objLevel);
            }

            return ResponseOutput.Ok(arrList);
        }


        /// <summary>
        /// 事件列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="siteid"></param>
        /// <param name="status"></param>
        /// <param name="eventlevel"></param>
        /// <param name="cityCode"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> UntreatedEvent(string typecode,string keyword, int siteid = 0, int status = 0, int eventlevel = 0, string cityCode = "320200",
            int page = 1, int size = 20)
        {
            var result = await _eventService.GetUntreatedEvent(typecode,siteid, keyword, status, _user.AnalystRegionID, eventlevel, cityCode, page, size);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 事件详情
        /// </summary>
        /// <param name="EVENTID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EventDetail(int EVENTID)
        {
            var result = await _eventService.EventDetail(EVENTID, _user.AnalystRegionID);

           // result.stypeName = result.stypeName.Contains('-') ? (result.stypeName.Split('-')[0] + result.devicecode) : "";
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 事件分析-类型子菜单-风险统计
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="eventlevel"></param>
        /// <param name="citycode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EventTypeStatistic(string typecode,int eventlevel = 0,string citycode= "320200")
        { 
                var result = await _eventService.EventTypeStatistic(citycode, typecode, eventlevel, _user.AnalystRegionID);
                JArray arr = new JArray();
                result.GroupBy(gg => gg.groupshortname).ToList().ForEach(ii =>
                {
                    JObject obj = new JObject();
                    obj["groupshortname"] = ii.Key;
                    obj["lv1"] = result.Where(ww => ww.eventlevel == 1 && ww.groupshortname == ii.Key).Sum(ss => ss.count);
                    obj["lv2"] = result.Where(ww => ww.eventlevel == 2 && ww.groupshortname == ii.Key).Sum(ss => ss.count);
                    obj["lv3"] = result.Where(ww => ww.eventlevel == 3 && ww.groupshortname == ii.Key).Sum(ss => ss.count);
                    obj["lv4"] = result.Where(ww => ww.eventlevel == 4 && ww.groupshortname == ii.Key).Sum(ss => ss.count);
                    obj["current"] = obj["lv4"].ToString() == "0" ? (obj["lv3"].ToString() == "0" ? (obj["lv2"].ToString() == "0" ? 1 : 2) : 3) : 4;
                    arr.Add(obj);
                });

                return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 项目风险排行
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="citycode"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ProjectRiskRank(string typecode,string citycode = "320200", int page = 1, int size = 20)
        {
            var list = await _eventService.ProjectRiskRank(typecode, _user.AnalystRegionID, citycode, page, size);

            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 企业风险排行
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="citycode"></param>
        /// <param name="companytype"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseRiskRank(string typecode, string citycode = "320200", int companytype = 2, int page = 1, int size = 20)
        {
            var list = await _eventService.EnterpriseRiskRank(typecode, _user.AnalystRegionID, citycode, companytype, page, size);

            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 触发事件内容统计
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="eventlevel"></param>
        /// <param name="days"></param>
        /// <param name="citycode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EventContentStatistic(string typecode, int eventlevel, int days, string citycode = "320200")
        {
            var list = await _eventService.EventContentStatistic(typecode, _user.AnalystRegionID, citycode, eventlevel, days);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 区域发展趋势
        /// </summary>
        /// <param name="days"></param>
        /// <param name="citycode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> DistrictTrend(int days, string citycode = "320200")
        {
            var list = await _eventService.DistrictTrend(_user.AnalystRegionID, days, citycode);
            List<DateTime> dateList = new List<DateTime>();
            for (int i = -days + 1; i <= 0; i++)
            {
                dateList.Add(DateTime.Now.AddDays(i));
            }
            JArray arr = new JArray();
            list.GroupBy(gg => gg.groupshortname).ToList().ForEach(ii =>
            {
                JObject obj = new JObject();
                obj["groupshortname"] = ii.Key;
                JArray arrRisk = new JArray();
                foreach (var dd in dateList)
                {
                    JObject riskObj = new JObject();
                    var tmp = list.Find(ww => ww.groupshortname == ii.Key
                            && ww.billdate == dd.ToString("yyyy/MM/dd"));

                    riskObj["risk"] = tmp == null ? 1 : tmp.siterisk;
                    riskObj["date"] = dd.ToString("yyyy/MM/dd");
                    arrRisk.Add(riskObj);
                }
                obj["siterisk"] = arrRisk;
                arr.Add(obj);
            });

            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 当前项目风险统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ProjRiskStatistic()
        {
            var list = await _eventService.ProjRiskStatistic(_user.AnalystRegionID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 当前企业风险统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseRiskStatistic()
        {

            var list = await _eventService.EnterpriseRiskStatistic(_user.AnalystRegionID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="citycode"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ProjectList(string typecode, string citycode = "320200", int page = 1, int size = 20)
        {
            var list = await _eventService.ProjectList(typecode,_user.AnalystRegionID, citycode, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 项目列表上面的统计
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="citycode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ProjectListCount(string typecode, string citycode = "320200")
        {
            var list = await _eventService.ProjectListCount(typecode, _user.AnalystRegionID, citycode);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 当前区域风险统计
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GroupRisk(string citycode = "320200")
        {
            var list = await _eventService.GroupRisk(_user.AnalystRegionID, citycode);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 项目信息看板-基本信息
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> ProjectDetail(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("参数不能为空");
            }
            var entity = await _eventService.ProjectDetail(_user.AnalystRegionID, SITEID);
            return ResponseOutput.Ok(entity);
        }


        /// <summary>
        /// 项目信息看板-行政处罚
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> PenaltyList(int SITEID, int page = 1, int size = 20)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("参数不能为空");
            }

            var list = await _eventService.PenaltyList(_user.AnalystRegionID, SITEID, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 项目信息看板-风险趋势
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> RiskTrend(int SITEID, int year, int month)
        {
            if (SITEID <= 0 || year <= 0 || month <= 0)
            {
                return ResponseOutput.NotOk("参数不能为空");
            }

            var list = await _eventService.RiskTrend(_user.AnalystRegionID, SITEID, year, month);
            DateTime date = new DateTime(year, month, 1);
            int days = DateTime.DaysInMonth(year, month);

            JArray arr = new JArray();
            for (int i = 0; i < days; i++)
            {
                JObject obj = new JObject();
                obj["CreationDate"] = date.AddDays(i).ToString("yyyy-MM-dd");
                obj["EventLevel"] = list.Find(ii => ii.CreationDate == date.AddDays(i).ToString("yyyy-MM-dd")) == null ?
                    1 : list.Find(ii => ii.CreationDate == date.AddDays(i).ToString("yyyy-MM-dd")).EventLevel;

                arr.Add(obj);
            }

            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 事件分析-子类-事件风险年统计
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="citycode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EventCurveYear(string typecode, string citycode = "320200")
        {
            if (string.IsNullOrWhiteSpace(typecode))
            {
                return ResponseOutput.NotOk("参数不能为空");
            }

            var list = await _eventService.EventCurveYear(_user.AnalystRegionID, typecode, citycode);
            DateTime date = DateTime.Now;
            JArray arr = new JArray();
            for (int i = 0; i < 12; i++)
            {
                var temp = list.Where(ii => ii.CreationDate == date.AddMonths(-i).ToString("yyyy-MM"));

                JObject obj = new JObject();
                obj["CreationDate"] = date.AddMonths(-i).ToString("yyyy-MM");
                obj["lv1"] = temp.Any() ?
                    (temp.Where(ii => ii.EventLevel == 1).Any() ? temp.Where(ii => ii.EventLevel == 1).FirstOrDefault().Count : 0)
                    : 0;
                obj["lv2"] = temp.Any() ?
                    (temp.Where(ii => ii.EventLevel == 2).Any() ? temp.Where(ii => ii.EventLevel == 2).FirstOrDefault().Count : 0)
                    : 0;
                obj["lv3"] = temp.Any() ?
                    (temp.Where(ii => ii.EventLevel == 3).Any() ? temp.Where(ii => ii.EventLevel == 3).FirstOrDefault().Count : 0)
                    : 0;
                obj["lv4"] = temp.Any() ?
                    (temp.Where(ii => ii.EventLevel == 4).Any() ? temp.Where(ii => ii.EventLevel == 4).FirstOrDefault().Count : 0)
                    : 0;

                arr.Add(obj);
            }


            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 企业风险列表
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="citycode"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseRiskList(string typecode, string citycode = "320200", int page = 1, int size = 20)
        {
            var list = await _eventService.EnterpriseRiskList(typecode,_user.AnalystRegionID, citycode, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 企业风险列表上面的统计
        /// </summary>
        /// <param name="typecode"></param>
        /// <param name="citycode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseRiskListCount(string typecode, string citycode = "320200")
        {
            var list = await _eventService.EnterpriseRiskListCount(typecode, _user.AnalystRegionID, citycode);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 企业信息看板-基本信息
        /// </summary>
        /// <param name="companycode"></param>
        /// <param name="companyname"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseDetail(string companycode, string companyname)
        {
            if (string.IsNullOrWhiteSpace(companyname) || string.IsNullOrWhiteSpace(companycode))
            {
                return ResponseOutput.NotOk("参数错误");
            }

            var entity = await _eventService.EnterpriseDetail(companycode, companyname, _user.AnalystRegionID);
            return ResponseOutput.Ok(entity);
        }


        /// <summary>
        /// 企业信息看板-承接项目
        /// </summary>
        /// <param name="companycode"></param>
        /// <param name="companyname"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseSiteList(string companycode, string companyname, int page = 1, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(companyname) || string.IsNullOrWhiteSpace(companycode))
            {
                return ResponseOutput.NotOk("参数错误");
            }

            var list = await _eventService.EnterpriseSiteList(companycode, companyname, _user.AnalystRegionID, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 企业信息看板-发生事件
        /// </summary>
        /// <param name="companycode"></param>
        /// <param name="companyname"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseEventList(string companycode, string companyname, int page = 1, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(companyname) || string.IsNullOrWhiteSpace(companycode))
            {
                return ResponseOutput.NotOk("参数错误");
            }

            var list = await _eventService.EnterpriseEventList(companycode, companyname, _user.AnalystRegionID, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 企业信息看板-发生事件-雷达图
        /// </summary>
        /// <param name="companycode"></param>
        /// <param name="companyname"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseEventRadar(string companycode, string companyname)
        {
            if (string.IsNullOrWhiteSpace(companyname) || string.IsNullOrWhiteSpace(companycode))
            {
                return ResponseOutput.NotOk("参数错误");
            }

            var list = await _eventService.EnterpriseEventRadar(companycode, companyname, _user.AnalystRegionID);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 企业信息看板-行政处罚
        /// </summary>
        /// <param name="companycode"></param>
        /// <param name="companyname"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterprisePenalty(string companycode, string companyname, int page = 1, int size = 20)
        {
            if (string.IsNullOrWhiteSpace(companyname) || string.IsNullOrWhiteSpace(companycode))
            {
                return ResponseOutput.NotOk("参数错误");
            }
            var entity = await _eventService.EnterprisePenalty(companycode, companyname, _user.AnalystRegionID, page, size);
            return ResponseOutput.Ok(entity);
        }


        /// <summary>
        /// 综合评分-项目评分排行
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> SiteScoreRank(string key = "",int page = 1, int size = 20)
        {
            var list = await _eventService.SiteScoreRank(key,_user.AnalystRegionID, page, size);
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 综合评分-企业评分排行
        /// </summary>
        /// <param name="key"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> EnterpriseScoreRank(string key = "", int page = 1, int size = 20)
        {
            var list = await _eventService.EnterpriseScoreRank(key, _user.AnalystRegionID, page, size);
            return ResponseOutput.Ok(list);
        }
    }
}
