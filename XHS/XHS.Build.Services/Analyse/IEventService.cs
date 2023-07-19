using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Analyse
{
    public interface IEventService : IBaseServices<AYEvent>
    {

        Task<PageOutput<UntreatedEventDto>> GetUntreatedEvent(string typecode, int siteid, string keyword, int status, string regionid, int eventlevel, string cityCode,
            int page, int size);

        Task<List<EventCurveDto>> GetEventCurve(string typecode, int status, string regionid, int days, string cityCode);

        Task<EventDetailDto> EventDetail(int EVENTID, string regionid);

        Task<JObject> EventStatistic(string regionid, string cityCode);

        Task<List<EventRadarDto>> EventRader(string regionid, int SITEID, string cityCode);

        Task<List<EventTypeStatisticDto>> EventTypeStatistic(string citycode, string typecode, int eventlevel, string regionid);

        Task<PageOutput<ProjRiskDto>> ProjectRiskRank(string typecode, string regionid, string citycode, int page, int size);

        Task<PageOutput<EnterpriseRiskRankDto>> EnterpriseRiskRank(string typecode, string regionid, string citycode, int companytype,int page, int size);

        Task<List<EventContentDto>> EventContentStatistic(string typecode, string regionid, string citycode, int eventlevel, int days);

        Task<List<DistrictTrendDto>> DistrictTrend(string regionid, int days, string citycode);

        Task<List<ProjRiskStatisticDto>> ProjRiskStatistic(string regionid, string beforeDate);

        Task<List<ProjRiskStatisticDto>> EnterpriseRiskStatistic(string regionid, string beforeDay);

        Task<PageOutput<ProjListDto>> ProjectList(string typecode, string regionid, string citycode, int page, int size);

        Task<ProjListDto> ProjectListCount(string typecode, string regionid, string citycode);

        Task<List<GroupRiskDto>> GroupRisk(string regionid, string citycode);

        Task<ProjectDetailDto> ProjectDetail(string regionid, int siteid);

        Task<PageOutput<AYPenalty>> PenaltyList(string regionid, int siteid, int page, int size);

        Task<List<EventCurveDto>> RiskTrend(string regionid, int siteid, int year, int month);

        Task<List<EventCurveDto>> EventCurveYear(string regionid, string typecode, string citycode);

        Task<PageOutput<EnterpriseRiskDto>> EnterpriseRiskList(string typecode, string regionid, string citycode, int page, int size);

        Task<EnterpriseRiskDto> EnterpriseRiskListCount(string typecode, string regionid, string citycode);

        Task<SiteCompanyDto> EnterpriseDetail(string companycode, string companyname, string regionid);

        Task<PageOutput<EnterpriseSiteDto>> EnterpriseSiteList(string companycode, string companyname, string regionid, int page, int size);

        Task<PageOutput<EnterpriseEventDto>> EnterpriseEventList(string companycode, string companyname, string regionid, int page, int size);

        Task<List<EnterpriseEventRadar>> EnterpriseEventRadar(string companycode, string companyname, string regionid);

        Task<PageOutput<EnterprisePenalty>> EnterprisePenalty(string companycode, string companyname, string regionid, int page, int size);

        Task<PageOutput<SiteScoreRank>> SiteScoreRank(string key,string regionid, int page, int size);

        Task<PageOutput<EnterpriseScoreRank>> EnterpriseScoreRank(string key,string regionid, int page, int size);
    }
}
