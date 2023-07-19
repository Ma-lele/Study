using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using System.Linq;
using System.Data;

namespace XHS.Build.Services.Analyse
{
    public class EventService:BaseServices<AYEvent>,IEventService
    {
        private readonly IBaseRepository<AYEvent> _repository;

        public EventService(IBaseRepository<AYEvent> repository)
        {
            _repository = repository;
            BaseDal = repository;
        }


        public async Task<EventDetailDto> EventDetail(int EVENTID, string regionid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.EVENTID,a.GROUPID,a.SITEID,a.createdate,b.etname as 'ltypeName',c.etname as 'stypeName', a.devicecode,");
            sql.Append(" d.siteshortname,a.eventlevel,a.content,a.eventcode,a.status,a.handler,a.handledate,a.updatedate, ");
            sql.Append(" a.limitdate,a.spid,a.remark ");
            sql.Append(" from T_AY_Event a left join T_AY_EventType b on a.ltype = b.ETID ");
            sql.Append(" left join T_AY_EventType c on a.stype = c.ETID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Site d on a.SITEID = d.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region e on d.siteregion = e.regioncode ");
            sql.AppendFormat(" where a.EVENTID = {0}", EVENTID);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0})", regionid);
            }

            var entity = await _repository.Db.SqlQueryable<EventDetailDto>(sql.ToString()).FirstAsync();
            return entity;
        }


        public async Task<List<EventRadarDto>> EventRader(string regionid, int SITEID, string cityCode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.ETID,a.etname,b.eventlevel,count(1) as 'count'  ");
            sql.Append(" from (select etid,etname from T_AY_EventType where LEN(ETID) = 2) a ");
            sql.Append(" left join ( select a.*,c.REGIONID from [T_AY_Event] a inner join XJ_Env.[dbo].[T_GC_Site] d on a.SITEID = d.SITEID ");
            sql.Append(" inner join XJ_Env.[dbo].[T_GC_Region] c on d.siteregion = c.regioncode ");
            sql.AppendFormat(" where d.Status = 0 and d.sitecity = '{0}'  ", cityCode);
            if (SITEID > 0)
            {
                sql.AppendFormat(" and a.SITEID = {0} ", SITEID);
            }
            sql.Append(" ) b on a.ETID = b.ltype ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and b.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.ETID,a.etname,b.eventlevel ");
            var list = await _repository.Db.SqlQueryable<EventRadarDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<JObject> EventStatistic(string regionid, string cityCode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select count(1) as count,a.eventlevel,a.status ");
            sql.Append(" from T_AY_Event a inner join XJ_Env.[dbo].[T_GC_Site] d on a.SITEID = d.SITEID ");
            sql.Append(" inner join XJ_Env.[dbo].[T_GC_Region] c on d.siteregion = c.regioncode ");
            sql.AppendFormat(" where d.Status = 0 and d.sitecity = '{0}' ", cityCode);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.eventlevel,a.status ");

            StringBuilder sqltoday = new StringBuilder();
            sqltoday.Append(" select count(1) as count,a.eventlevel,a.status ");
            sqltoday.Append(" from T_AY_Event a inner join XJ_Env.[dbo].[T_GC_Site] d on a.SITEID = d.SITEID ");
            sqltoday.Append(" inner join XJ_Env.[dbo].[T_GC_Region] c on d.siteregion = c.regioncode ");
            sqltoday.AppendFormat(
                " where a.Status = 0 and d.sitecity = '{0}'  and CONVERT(varchar(10), a.createdate,120) = CONVERT(varchar(10), getdate(),120) "
                , cityCode);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sqltoday.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            sqltoday.Append(" group by a.eventlevel,a.status ");


            var list = await _repository.Db.SqlQueryable<EventStatisticDto>(sql.ToString()).ToListAsync();
            var today = await _repository.Db.SqlQueryable<EventStatisticDto>(sqltoday.ToString()).ToListAsync();



            JObject obj1 = new JObject();
            obj1["total1"] = 0;
            obj1["untreated1"] = 0;
            JObject obj2 = new JObject();
            obj2["total2"] = 0;
            obj2["untreated2"] = 0;
            JObject obj3 = new JObject();
            obj3["total3"] = 0;
            obj3["untreated3"] = 0;
            JObject obj4 = new JObject();
            obj4["total4"] = 0;
            obj4["untreated4"] = 0;


            list.ForEach(ii => {
                switch (ii.eventlevel)
                {
                    case 1:
                        obj1["total1"] = list.Where(jj => jj.eventlevel == 1).Sum(ss => ss.count);
                        obj1["untreated1"] = list.Where(jj => jj.eventlevel == 1 && jj.status == 1).Sum(ss => ss.count);
                        break;
                    case 2:
                        obj2["total2"] = list.Where(jj => jj.eventlevel == 2).Sum(ss => ss.count);
                        obj2["untreated2"] = list.Where(jj => jj.eventlevel == 2 && jj.status == 1).Sum(ss => ss.count);
                        break;
                    case 3:
                        obj3["total3"] = list.Where(jj => jj.eventlevel == 3).Sum(ss => ss.count);
                        obj3["untreated3"] = list.Where(jj => jj.eventlevel == 3 && jj.status == 1).Sum(ss => ss.count);
                        break;
                    case 4:
                        obj4["total4"] = list.Where(jj => jj.eventlevel == 4).Sum(ss => ss.count);
                        obj4["untreated4"] = list.Where(jj => jj.eventlevel == 4 && jj.status == 1).Sum(ss => ss.count);
                        break;
                }
            });
            JArray arr = new JArray {
                obj1,
                obj2,
                obj3,
                obj4
            };


            JObject obj = new JObject();
            obj["Statistic"] = Newtonsoft.Json.JsonConvert.SerializeObject(arr);
            obj["Today"] = Newtonsoft.Json.JsonConvert.SerializeObject(today);
            return obj;

        }


        public async Task<List<EventTypeStatisticDto>> EventTypeStatistic(string citycode, string typecode, int eventlevel, string regionid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.regionname groupshortname,b.eventlevel,count(1) as 'count' from ( ");
            sql.Append(" select b.SITEID,a.regionname from ");
            sql.Append(" XJ_Env.[dbo].[T_GC_Region] a inner join XJ_Env.dbo.T_GC_Site b on a.regioncode = b.siteregion ");
  
            sql.AppendFormat(" where b.sitecity = '{0}' ", citycode);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and a.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" ) a left join(select SITEID,eventlevel from XHS_Analyse.dbo.T_AY_Event ");
            sql.AppendFormat(" where ltype = '{0}' ", typecode);
            if (eventlevel > 0)
            {
                sql.AppendFormat(" and eventlevel = {0} ", eventlevel);
            } 
            sql.Append(" ) b on a.SITEID = b.SITEID group by a.regionname,b.eventlevel ");

            var list = await _repository.Db.SqlQueryable<EventTypeStatisticDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<List<EventCurveDto>> GetEventCurve(string typecode,int status,string regionid,int days, string cityCode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select count(1) as Count,a.EventLevel ,Convert(nvarchar,a.createdate,111) as CreationDate ");
            sql.Append(" from T_AY_Event a inner join XJ_Env.[dbo].[T_GC_Site] d on a.SITEID = d.SITEID ");
            sql.Append(" inner join XJ_Env.[dbo].[T_GC_Region] c on d.siteregion = c.regioncode ");
            sql.AppendFormat(" where d.Status = 0 and d.sitecity = '{0}' and DateDiff(dd,a.createdate,getdate()) < {1} ", cityCode, days);
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and a.ltype = '{0}' ", typecode);
            }
            if (status == 2)
            {
                sql.AppendFormat(" and a.status in (3,4) ");
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.eventlevel,Convert(nvarchar,a.createdate,111) ");


            var list = await _repository.Db.SqlQueryable<EventCurveDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<PageOutput<UntreatedEventDto>> GetUntreatedEvent(string typecode, int siteid,string keyword, int status, string regionid, int eventlevel, string cityCode,
            int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.EVENTID,a.GROUPID,a.SITEID,a.createdate,b.siteshortname,b.sitename,e.regionname groupshortname,d.etname,a.content, f.etname 'stypename', ");
            sql.Append(" case a.eventlevel when 1 then '低' when 2 then '中' when 3 then '高' when 4 then '超高' end 'eventlevel', ");
            sql.Append(" a.eventcode,a.status,a.handler ");
            sql.Append(" from XHS_Analyse.dbo.T_AY_Event a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" left join XHS_Analyse.dbo.T_AY_EventType d on a.ltype = d.ETID ");
            sql.Append(" inner join XJ_Env.[dbo].[T_GC_Region] e on b.siteregion = e.regioncode ");
            sql.Append(" left join XHS_Analyse.dbo.T_AY_EventType f on a.stype = f.ETID ");
            sql.AppendFormat(" where b.Status = 0 and b.sitecity = '{0}' ", cityCode);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql.AppendFormat(" and a.ltype = '{0}' ", typecode);
            }
            if (siteid > 0)
            {
                sql.AppendFormat(" and a.SITEID = {0} ", siteid);
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql.AppendFormat(" and a.content like '%{0}%' ", keyword);
            }
            if (status > 0)
            {
                sql.AppendFormat(" and a.status = {0} ", status);
            }
            if (eventlevel > 0)
            {
                sql.AppendFormat(" and a.eventlevel = {0} ", eventlevel);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0}) ", regionid);
            }
            // sql.Append(" order by a.createdate desc ");


            var result = await _repository.Db.SqlQueryable<UntreatedEventDto>(sql.ToString())
                .OrderBy(ii => ii.createdate, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<UntreatedEventDto>()
            {
                data = result,
                dataCount = totalCount
            };

            return data;
        }

        public async Task<PageOutput<ProjRiskDto>> ProjectRiskRank(string typecode, string regionid, string citycode, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select siteshortname,sitename,groupshortname,constructtype,projstatus,sitelat,sitelng,[1] 'lv1',[2] 'lv2',[3] 'lv3',[4] 'lv4' from ( ");
            sql.Append(" select b.siteshortname,b.sitename,d.regionname groupshortname,b.constructtype,b.projstatus,a.eventlevel,count(1) as 'count',b.sitelat,b.sitelng ");
            sql.Append(" from XHS_Analyse.dbo.T_AY_Event a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.[dbo].[T_GC_Region] d on b.siteregion = d.regioncode ");
            sql.AppendFormat(" where b.Status = 0 and a.status in (1,2) and  b.sitecity = '{0}' ", citycode);
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and a.ltype = '{0}' ", typecode);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }

            sql.Append(" group by b.siteshortname,b.sitename,b.constructtype,b.projstatus,a.eventlevel,d.regionname,b.sitelat,b.sitelng ");
            sql.Append(" ) t pivot(sum(count) for eventlevel in ([1],[2],[3],[4])) as s ");

            var list = await _repository.Db.SqlQueryable<ProjRiskDto>(sql.ToString())
                .OrderBy(ii => new { ii.lv4, ii.lv3, ii.lv2, ii.lv1 }, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<ProjRiskDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<PageOutput<EnterpriseRiskRankDto>> EnterpriseRiskRank(string typecode, string regionid, 
            string citycode,int companytype, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select companyname builder,lv1,lv2,lv3,lv4,(select count(1) from XJ_Env.dbo.T_GC_Site a inner join (select * from [XJ_Env].[dbo].[T_GC_SiteCompany] where companytype = 2 ) b on a.SITEID = b.SITEID ");
            sql.Append(" where b.companyname = t.companyname) projcount from ( select companyname,[1] lv1,[2] lv2,[3] lv3,[4] lv4 from ( ");
            sql.Append(" select e.companyname,a.eventlevel,count(1) 'count' from T_AY_Event a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.[dbo].[T_GC_Region] d on b.siteregion = d.regioncode ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_SiteCompany e on a.SITEID = e.SITEID ");
            sql.AppendFormat(" where b.Status = 0 and e.companytype = {0} ", companytype);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and b.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(typecode))
            { 
                sql.AppendFormat(" and a.ltype = '{0}' ", typecode);
            }


            sql.Append(" group by e.companyname,a.eventlevel ");
            sql.Append(" ) t pivot (sum(count) for eventlevel in ([1],[2],[3],[4])) as s ) t ");


            var list = await _repository.Db.SqlQueryable<EnterpriseRiskRankDto>(sql.ToString())
              .OrderBy(ii => new { ii.lv4, ii.lv3, ii.lv2, ii.lv1 }, OrderByType.Desc)
              .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<EnterpriseRiskRankDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<List<EventContentDto>> EventContentStatistic(string typecode, string regionid, string citycode, int eventlevel, int days)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.ETID,a.etname,a.nowcount,b.beforecount from( ");
            sql.Append(" select a.ETID,a.etname,count(b.EVENTID) as nowcount ");
            sql.Append(" from [dbo].[T_AY_EventType] a left join ");
            sql.Append(" (select a.* from T_AY_Event a inner join XJ_Env.dbo.T_GC_Site d on a.SITEID = d.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region c on d.siteregion = c.regioncode ");
            sql.Append(" where d.Status = 0 ");
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and d.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and a.ltype = '{0}' ", typecode);
            }
            if (eventlevel > 0)
            {
                sql.AppendFormat(" and a.eventlevel = {0} ", eventlevel);
            }
            if (days > 0)
            {
                sql.AppendFormat(" and DateDiff(dd,a.createdate,getdate()) between 0 and {0} ", days - 1);
            }
            sql.Append(" ) b on a.ETID = b.stype where len(a.etid) > 2 ");
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and left(a.etid,2) = '{0}' ", typecode);
            }
            sql.Append(" group by a.ETID,a.etname) a inner join ( ");
            sql.Append(" select  a.ETID,a.etname,count(b.EVENTID) as beforecount ");
            sql.Append(" from [dbo].[T_AY_EventType] a left join ");
            sql.Append(" (select a.* from T_AY_Event a inner join XJ_Env.dbo.T_GC_Site d on a.SITEID = d.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region c on d.siteregion = c.regioncode ");
            sql.Append(" where d.Status = 0 ");
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and d.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and a.ltype = '{0}' ", typecode);
            }
            if (eventlevel > 0)
            {
                sql.AppendFormat(" and a.eventlevel = {0} ", eventlevel);
            }
            if (days > 0)
            {
                sql.AppendFormat(" and DateDiff(dd,a.createdate,getdate()) between {0} and {1} ", days, days * 2 - 1);
            }
            sql.Append(" ) b on a.ETID = b.stype where len(a.etid) > 2 ");
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and left(a.etid,2) = '{0}' ", typecode);
            }
            sql.Append(" group by a.ETID,a.etname ) b on a.ETID = b.ETID ");

            var list = await _repository.Db.SqlQueryable<EventContentDto>(sql.ToString()).ToListAsync();
            return list;
        }

        public async Task<List<DistrictTrendDto>> DistrictTrend(string regionid,int days, string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT a.GROUPID,c.regionname groupshortname,max(a.siterisk) siterisk,Convert(nvarchar,a.billdate,111) billdate ");
            sql.Append(" FROM [XHS_Analyse].[dbo].[T_AY_SiteDailyRiskScore] a inner join XJ_Env.dbo.T_GC_Site d on a.SITEID = d.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region c on d.siteregion = c.regioncode ");
            sql.AppendFormat(" where d.Status = 0 and d.sitecity = '{0}' ", citycode);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            if (days > 0)
            {
                sql.AppendFormat(" and DateDiff(dd,a.billdate,getdate()) < {0} ", days);
            }
            sql.Append(" group by Convert(nvarchar,a.billdate,111),a.GROUPID,c.regionname ");

            var list = await _repository.Db.SqlQueryable<DistrictTrendDto>(sql.ToString()).ToListAsync();
            return list;
        }

        public async Task<List<ProjRiskStatisticDto>> ProjRiskStatistic(string regionid, string beforeDate)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select (total - isnull([4],0) - isnull([3],0) - isnull([2],0)) 'lv1',isnull([2],0) ");
            sql.Append(" 'lv2',isnull([3],0) 'lv3',isnull([4],0) 'lv4' ,'yesterday' createdate from ( ");
            sql.Append(" SELECT a.siterisk,count(1) count,(select count(1) from XJ_Env.dbo.T_GC_Site a inner join XJ_Env.dbo.T_GC_Region b on a.siteregion = b.regioncode ");
            sql.Append(" where a.status = 0 ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and b.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" ) total FROM [XHS_Analyse].[dbo].[T_AY_SiteDailyRiskScore] a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region c on b.siteregion = c.regioncode ");
            sql.AppendFormat(" where billdate = '{0}' ", beforeDate);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.siterisk ) t pivot(sum(count) for siterisk in ([1],[2],[3],[4])) as s union all ");
            sql.Append(" select (total - isnull([4],0) - isnull([3],0) - isnull([2],0)) 'lv1',isnull([2],0) 'lv2', ");
            sql.Append(" isnull([3],0) 'lv3',isnull([4],0) 'lv4' ,'today' createdate from (select max(eventlevel) eventlevel,(select count(1) from XJ_Env.dbo.T_GC_Site a ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region b on a.siteregion = b.regioncode ");
            sql.Append(" where a.status = 0 ");
          
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and b.REGIONID in ({0}) ", regionid);
            }
          
            sql.Append(" ) total from T_AY_Event a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region c on b.siteregion = c.regioncode ");
            sql.Append(" where a.status in (1,2) ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.SITEID) t pivot(count(eventlevel) for eventlevel in ([1],[2],[3],[4])) as s ");
            var list = await _repository.Db.SqlQueryable<ProjRiskStatisticDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<List<ProjRiskStatisticDto>> EnterpriseRiskStatistic(string regionid, string beforeDay)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select (total-isnull([4],0)-isnull([3],0)-isnull([2],0)) lv1,isnull([2],0) lv2,isnull([3],0) lv3,isnull([4],0) lv4, ");
            sql.Append(" 'yesterday' createdate  from ( select eventlevel,count(1) count,( select count(1) from ( ");
            sql.Append(" select distinct companytype,companycode,companyname  from XJ_Env.dbo.T_GC_SiteCompany where companytype = 2 ");
            sql.Append(" )t) total  from ( select max(siterisk) eventlevel from (select * from XJ_Env.dbo.T_GC_SiteCompany where companytype = 2) a ");
            sql.Append(" inner join T_AY_SiteDailyRiskScore b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Site c on b.SITEID = c.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region d on c.siteregion = d.regioncode ");
            sql.AppendFormat(" where b.billdate = '{0}' ",beforeDay);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }
       
            sql.Append(" group by companycode,companyname,companytype ) t group by t.eventlevel ");
            sql.Append(" ) t pivot(sum(count) for eventlevel in ([1],[2],[3],[4])) as s union all ");
            
            sql.Append(" select (total-isnull([4],0)-isnull([3],0)-isnull([2],0)) lv1,isnull([2],0) lv2,isnull([3],0) lv3,isnull([4],0) lv4, ");
            sql.Append(" 'today' createdate from( select count(1) count,t.eventlevel,( select count(1) from ( ");
            sql.Append(" select distinct companytype,companycode,companyname  from XJ_Env.dbo.T_GC_SiteCompany where companytype = 2 ");
            sql.Append(" )t) total from ( select a.companycode,a.companyname,a.companytype,max(b.eventlevel) eventlevel ");
            sql.Append(" from (select * from XJ_Env.dbo.T_GC_SiteCompany where companytype = 2) a inner join T_AY_Event b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Site c on b.SITEID = c.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region d on c.siteregion = d.regioncode ");
            sql.Append(" where b.status in (1,2) ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }
          
            sql.Append(" group by a.companycode,a.companyname,a.companytype) t group by t.eventlevel ");
            sql.Append(" ) t pivot(sum(count) for eventlevel in ([1],[2],[3],[4])) as s  "); 

            var list = await _repository.Db.SqlQueryable<ProjRiskStatisticDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<PageOutput<ProjListDto>> ProjectList(string typecode, string regionid, string citycode, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select SITEID,siteshortname,sitename,projtype,projstatus,[1] lv1,[2] lv2,[3] lv3,[4] lv4 from( ");
            sql.Append(" select a.SITEID,a.siteshortname,a.sitename,b.dataitem projtype,a.projstatus,c.eventlevel,count(1) count ");
            sql.Append(" from XJ_Env.dbo.T_GC_Site a left join XJ_Env.[dbo].[T_CC_DataDictionary] b on a.sitetype = b.DDID ");
            sql.Append(" left join T_AY_Event c on a.SITEID = c.SITEID ");
            sql.Append(" left join XJ_Env.dbo.T_GC_Region e on a.siteregion = e.regioncode ");
            sql.Append(" where a.status = 0 and c.status in (1,2) ");
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and c.ltype = '{0}' ", typecode);
            }
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and a.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.SITEID,a.siteshortname,a.sitename,b.dataitem,a.projstatus,c.eventlevel ");
            sql.Append(" ) t pivot(sum(count) for eventlevel in ([1],[2],[3],[4])) as s ");


            var list = await _repository.Db.SqlQueryable<ProjListDto>(sql.ToString())
                .OrderBy(oo => oo.lv4, OrderByType.Desc)
                .OrderBy(oo => oo.lv3, OrderByType.Desc)
                .OrderBy(oo => oo.lv2, OrderByType.Desc)
                .OrderBy(oo => oo.lv1, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<ProjListDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<ProjListDto> ProjectListCount(string typecode, string regionid, string citycode)
        { 
            StringBuilder sql = new StringBuilder();
            sql.Append(" select sum([1]) lv1,sum([2]) lv2,sum([3]) lv3,sum([4]) lv4 from( ");
            sql.Append(" select a.SITEID,a.siteshortname,b.dataitem projtype,a.projstatus,c.eventlevel,count(1) count ");
            sql.Append(" from XJ_Env.dbo.T_GC_Site a left join XJ_Env.[dbo].[T_CC_DataDictionary] b on a.sitetype = b.DDID ");
            sql.Append(" left join T_AY_Event c on a.SITEID = c.SITEID ");
            sql.Append(" left join XJ_Env.dbo.T_GC_Region e on a.siteregion = e.regioncode ");
            sql.Append(" where a.status = 0 and c.status in (1,2) ");
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and c.ltype = '{0}' ", typecode);
            }
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and a.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.SITEID,a.siteshortname,b.dataitem,a.projstatus,c.eventlevel ");
            sql.Append(" ) t pivot(sum(count) for eventlevel in ([1],[2],[3],[4])) as s ");


            var result = await _repository.Db.SqlQueryable<ProjListDto>(sql.ToString())
                .OrderBy(oo => oo.lv4, OrderByType.Desc)
                .OrderBy(oo => oo.lv3, OrderByType.Desc)
                .OrderBy(oo => oo.lv2, OrderByType.Desc)
                .OrderBy(oo => oo.lv1, OrderByType.Desc)
                .FirstAsync();

            return result;
        }


        public async Task<List<GroupRiskDto>> GroupRisk(string regionid, string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select groupshortname,[1] lv1,[2] lv2,[3] lv3,[4] lv4 from ( ");
            sql.Append(" select groupshortname,eventlevel,count(1) count from ( ");
            sql.Append(" select siteid,REGIONID,groupshortname,isnull(max(eventlevel),1) eventlevel from ( ");
            sql.Append(" select a.SITEID ,b.eventlevel,d.REGIONID,d.regionname groupshortname ");
            sql.Append(" from XJ_Env.dbo.T_GC_Region d  ");
            sql.Append(" left join XJ_Env.dbo.T_GC_Site a on d.regioncode = a.siteregion left join T_AY_Event b on a.SITEID = b.SITEID ");
            sql.Append(" where a.status = 0  ");
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and a.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.SITEID, b.eventlevel,d.REGIONID,d.regionname) t  ");
            sql.Append(" group by SITEID,REGIONID,groupshortname) t group by eventlevel,REGIONID,groupshortname ");
            sql.Append(" ) t pivot(sum(count) for eventlevel in ([1],[2],[3],[4])) as s ");

            var list = await _repository.Db.SqlQueryable<GroupRiskDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<ProjectDetailDto> ProjectDetail(string regionid, int siteid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.sitename, b.dataitem projtype, a.sitecode, a.siteajcode,a.status,d.regionname,d.regionname groupshortname, ");
            sql.Append(" a.siteaddr,a.sitelat,a.sitelng,a.bcityctrl,a.sitecost,a.planstartdate,a.planenddate,a.buildingarea,e.summary, ");
            sql.AppendFormat(" (select avg(sitescore) score from [dbo].[T_AY_SiteDailyRiskScore] where SITEID = {0} ) score, ", siteid);
            sql.Append(" (select companytype,companyname,companycontact,companytel from XJ_Env.[dbo].[T_GC_SiteCompany] where SITEID = a.SITEID ");
            sql.Append(" for json path,include_null_values) fivepart from XJ_Env.[dbo].T_GC_Site a left join XJ_Env.[dbo].[T_CC_DataDictionary] b on a.sitetype = b.DDID ");
            sql.Append(" left join XJ_Env.[dbo].T_GC_Region d on a.siteregion = d.regioncode ");
            sql.Append(" left join XJ_Env.[dbo].[T_GC_SiteSummary] e on a.SITEID = e.SITEID ");
            sql.AppendFormat(" where a.SITEID = {0} ", siteid);
 
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }

            var entity = await _repository.Db.SqlQueryable<ProjectDetailDto>(sql.ToString()).FirstAsync();
            return entity;
        }


        public async Task<PageOutput<AYPenalty>> PenaltyList(string regionid, int siteid, int page, int size)
        {
            RefAsync<int> totalCount = 0;

            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.* from T_AY_Penalty a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region c on b.siteregion = c.regioncode ");
            sql.AppendFormat(" where a.SITEID = {0} ", siteid);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }

            var list = await _repository.Db.SqlQueryable<AYPenalty>(sql.ToString())
                .OrderBy(ii => ii.penaltydate, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<AYPenalty>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<List<EventCurveDto>> RiskTrend(string regionid, int siteid, int year, int month)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select Convert(nvarchar,a.createdate,23) CreationDate,max(a.eventlevel) EventLevel from T_AY_Event a ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region c on b.siteregion = c.regioncode ");
            sql.Append(" where 1 = 1 ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            if (siteid > 0)
            {
                sql.AppendFormat(" and a.SITEID = {0} ", siteid);
            }
            sql.AppendFormat(" and DATEPART(yy,a.createdate) = {0} and DATEPART(mm,a.createdate) = {1} ", year, month);
            sql.Append(" group by Convert(nvarchar,a.createdate,23) ");

            var list = await _repository.Db.SqlQueryable<EventCurveDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<List<EventCurveDto>> EventCurveYear(string regionid, string typecode, string citycode)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select convert(nvarchar(5),DATEPART(yy,createdate)) + '-' + convert(nvarchar(5),DATEPART(mm,createdate)) CreationDate, ");
            sql.Append(" eventlevel,count(eventlevel) Count from T_AY_Event a ");
            sql.Append(" left join XJ_Env.dbo.T_GC_Site d on a.SITEID = d.SITEID left join XJ_Env.dbo.T_GC_Region c on c.regioncode = d.siteregion ");
            sql.Append(" where a.createdate >= DATEADD(year,-1,getdate()) and d.status = 0 ");
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and a.ltype = '{0}' ", typecode);
            }
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and d.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by convert(nvarchar(5),DATEPART(yy,createdate)) + '-' +convert(nvarchar(5),DATEPART(mm,createdate)),eventlevel ");

            var list = await _repository.Db.SqlQueryable<EventCurveDto>(sql.ToString()).ToListAsync();
            return list;
        }


        public async Task<PageOutput<EnterpriseRiskDto>> EnterpriseRiskList(string typecode, string regionid, string citycode, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select companycode,companyname,[1] lv1,[2] lv2,[3] lv3,[4] lv4 from ");
            sql.Append(" (select companycode,companyname,eventlevel,count(SITEID) eventcount from ( ");
            sql.Append(" select a.companycode,a.companyname,b.SITEID,max(c.eventlevel) eventlevel ");
            sql.Append(" from XJ_Env.dbo.T_GC_SiteCompany a left join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" left join T_AY_Event c on b.SITEID = c.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region e on b.siteregion = e.regioncode ");
            sql.Append(" where a.companytype = 2 and b.status = 0 ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0}) ", regionid);
            }
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and b.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and c.ltype = '{0}' ", typecode);
            }
            sql.Append(" group by a.companycode,a.companyname,b.SITEID) t group by companycode,companyname,eventlevel ");
            sql.Append(" ) t pivot(sum(eventcount) for eventlevel in ([1],[2],[3],[4])) as s  ");
            
            var list = await _repository.Db.SqlQueryable<EnterpriseRiskDto>(sql.ToString())
                .OrderBy(oo => oo.lv4, OrderByType.Desc)
                .OrderBy(oo => oo.lv3, OrderByType.Desc)
                .OrderBy(oo => oo.lv2, OrderByType.Desc)
                .OrderBy(oo => oo.lv1, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<EnterpriseRiskDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;

        }


        public async Task<EnterpriseRiskDto> EnterpriseRiskListCount(string typecode, string regionid, string citycode)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select sum([1]) lv1,sum([2]) lv2,sum([3]) lv3,sum([4]) lv4 from ");
            sql.Append(" (select companycode,companyname,eventlevel,count(SITEID) eventcount from ( ");
            sql.Append(" select a.companycode,a.companyname,b.SITEID,max(c.eventlevel) eventlevel ");
            sql.Append(" from XJ_Env.dbo.T_GC_SiteCompany a left join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" left join T_AY_Event c on b.SITEID = c.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region e on b.siteregion = e.regioncode ");
            sql.Append(" where a.companytype = 2 and b.status = 0 ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0}) ", regionid);
            }
            if (!string.IsNullOrWhiteSpace(citycode))
            {
                sql.AppendFormat(" and b.sitecity = '{0}' ", citycode);
            }
            if (!string.IsNullOrWhiteSpace(typecode))
            {
                sql.AppendFormat(" and c.ltype = '{0}' ", typecode);
            }
            sql.Append(" group by a.companycode,a.companyname,b.SITEID) t group by companycode,companyname,eventlevel ");
            sql.Append(" ) t pivot(sum(eventcount) for eventlevel in ([1],[2],[3],[4])) as s  ");

            var result = await _repository.Db.SqlQueryable<EnterpriseRiskDto>(sql.ToString())
                .FirstAsync();
          
            return result;

        }


        public async Task<SiteCompanyDto> EnterpriseDetail(string companycode, string companyname, string regionid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.companyname,a.companytype,a.companycode,a.companycontact,a.companytel,avg(b.sitescore) sitescore ");
            sql.Append(" from XJ_Env.dbo.T_GC_SiteCompany a left join [T_AY_SiteDailyRiskScore] b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Site c on a.SITEID = c.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region d on c.siteregion = d.regioncode ");
            sql.AppendFormat(" where a.companycode = '{0}' and a.companyname = '{1}' and a.companytype = 2  ", companycode, companyname);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by a.companyname,a.companytype,a.companycode,a.companycontact,a.companytel ");
            var entity = await _repository.Db.SqlQueryable<SiteCompanyDto>(sql.ToString()).FirstAsync();
            return entity;
        }


        public async Task<PageOutput<EnterpriseSiteDto>> EnterpriseSiteList(string companycode, string companyname, string regionid, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select b.sitename,e.dataitem sitetype,b.projstatus,max(c.eventlevel) eventlevel,avg(d.sitescore) sitescore ");
            sql.Append(" from XJ_Env.dbo.T_GC_SiteCompany a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region f on b.siteregion = f.regioncode ");
            sql.Append(" inner join XJ_Env.dbo.T_CC_DataDictionary e on b.sitetype = e.DDID left join T_AY_Event c on b.SITEID = c.SITEID ");
            sql.Append(" left join [T_AY_SiteDailyRiskScore] d on b.SITEID = d.SITEID ");
            sql.AppendFormat(" where a.companycode = '{0}' and a.companyname = '{1}' and c.status in (1,2)  ", companycode, companyname);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and f.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by b.SITEID,b.sitename,e.dataitem,b.projstatus ");
            var list = await _repository.Db.SqlQueryable<EnterpriseSiteDto>(sql.ToString())
                .OrderBy(oo => oo.eventlevel, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<EnterpriseSiteDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<PageOutput<EnterpriseEventDto>> EnterpriseEventList(string companycode, string companyname, string regionid, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select c.EVENTID,c.createdate,b.sitename, f.regionname groupshortname,e.etname,c.eventcode,c.eventlevel ");
            sql.Append(" from XJ_Env.dbo.T_GC_SiteCompany a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region f on b.siteregion = f.regioncode ");
            sql.Append(" inner join T_AY_Event c on b.SITEID = c.SITEID ");
            sql.Append(" inner join T_AY_EventType e on c.ltype = e.ETID ");
            sql.AppendFormat(" where a.companycode = '{0}' and a.companyname = '{1}' and a.companytype = 2 and b.status = 0  "
                , companycode, companyname);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and f.REGIONID in ({0}) ", regionid);
            }
           
            var list = await _repository.Db.SqlQueryable<EnterpriseEventDto>(sql.ToString())
                .OrderBy(oo => oo.eventlevel, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<EnterpriseEventDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<List<EnterpriseEventRadar>> EnterpriseEventRadar(string companycode, string companyname, string regionid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.etname,b.lv1,b.lv2,b.lv3,b.lv4 from T_AY_EventType a left join ( ");
            sql.Append(" select ETID,etname,[1] lv1,[2] lv2,[3] lv3,[4] lv4 from (select d.ETID,d.etname,b.eventlevel,count(1) eventcount ");
            sql.Append(" from XJ_Env.dbo.T_GC_SiteCompany a inner join T_AY_Event b on a.SITEID = b.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Site c on a.SITEID = c.SITEID inner join T_AY_EventType d on b.ltype = d.ETID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region e on c.siteregion = e.regioncode ");
            sql.AppendFormat(" where a.companycode = '{0}' and a.companyname = '{1}' and a.companytype = 2 and c.status = 0  "
                , companycode, companyname);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0}) ", regionid);
            }
            sql.Append(" group by d.ETID,d.etname,b.eventlevel ) t pivot(sum(eventcount) for eventlevel in ([1],[2],[3],[4])) as s ");
            sql.Append("  ) b on a.ETID = b.ETID where LEN(a.ETID) = 2 ");
            var list = await _repository.Db.SqlQueryable<EnterpriseEventRadar>(sql.ToString()).ToListAsync();
            return list;
        }

        public async Task<PageOutput<EnterprisePenalty>> EnterprisePenalty(string companycode, string companyname, string regionid, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select c.penaltydate,b.siteshortname,c.penaltycode,c.content ");
            sql.Append(" from XJ_Env.dbo.T_GC_SiteCompany a inner join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" inner join T_AY_Penalty c on b.SITEID = c.SITEID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region d on b.siteregion = d.regioncode ");
            sql.AppendFormat(" where a.companycode = '{0}' and a.companyname = '{1}' and a.companytype = 2 ", companycode, companyname);
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and d.REGIONID in ({0}) ", regionid);
            }

            var list = await _repository.Db.SqlQueryable<EnterprisePenalty>(sql.ToString()).ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<EnterprisePenalty>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<PageOutput<SiteScoreRank>> SiteScoreRank(string key,string regionid, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.SITEID,a.sitename,a.groupshortname,a.sitetype,a.projstatus,a.costdays,a.lv1, ");
            sql.Append(" a.lv2,a.lv3,a.lv4,avg(b.sitescore) sitescore from (select SITEID,sitename,groupshortname,sitetype, ");
            sql.Append(" projstatus,costdays,[1] lv1,[2] lv2,[3] lv3,[4] lv4 from (select a.SITEID,a.sitename, ");
            sql.Append(" e.regionname groupshortname,d.dataitem sitetype,a.projstatus,case when a.projstatus >= 4 then datediff(day,a.startdate,a.enddate) ");
            sql.Append(" else datediff(day,a.startdate,getdate()) end costdays,c.eventlevel,count(c.eventlevel) eventcount ");
            sql.Append(" from XJ_Env.dbo.T_GC_Site a ");
            sql.Append(" left join T_AY_Event c on a.SITEID = c.SITEID left join XJ_Env.dbo.T_CC_DataDictionary d on a.sitetype = d.DDID ");
            sql.Append(" inner join XJ_Env.dbo.T_GC_Region e on a.siteregion = e.regioncode ");
            sql.Append(" where a.sitecity = '320200' ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and e.REGIONID in ({0}) ", regionid);
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql.AppendFormat(" and a.sitename like '%{0}%' ",key);
            }
            sql.Append(" group by a.SITEID,a.sitename,e.regionname,d.dataitem,a.projstatus,a.startdate,enddate,c.eventlevel ");
            sql.Append(" ) t pivot(sum(eventcount) for eventlevel in ([1],[2],[3],[4])) as s ) a left join [T_AY_SiteDailyRiskScore] b ");
            sql.Append(" on a.SITEID = b.SITEID group by a.SITEID,a.sitename,a.groupshortname, ");
            sql.Append(" a.sitetype,a.projstatus,a.costdays,a.lv1,a.lv2,a.lv3,a.lv4 ");

            var list = await _repository.Db.SqlQueryable<SiteScoreRank>(sql.ToString())
                .OrderBy(oo=>oo.sitescore,OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<SiteScoreRank>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<PageOutput<EnterpriseScoreRank>> EnterpriseScoreRank(string key, string regionid, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.companycode,a.companyname,a.activecount,a.sitecount,avg(b.sitescore) sitescore from ( ");
            sql.Append(" select max(SITEID) SITEID,companycode, companyname,sum(activecount) activecount,sum(sitecount) sitecount from ( ");
            sql.Append(" select max(a.SITEID) SITEID,a.companycode,a.companyname,case when b.projstatus = 2 then count(b.siteid) end ");
            sql.Append(" activecount,count(b.siteid) sitecount from XJ_Env.dbo.T_GC_SiteCompany a left join XJ_Env.dbo.T_GC_Site b on a.SITEID = b.SITEID ");
            sql.Append(" left join XJ_Env.dbo.T_GC_Region c on b.siteregion = c.regioncode ");
            sql.Append(" where a.companytype = 2 ");
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and c.REGIONID in ({0}) ", regionid);
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql.AppendFormat(" and (a.companyname like '%{0}%' or a.companycode like '%{0}%' ) ", key);
            }
            sql.Append(" group by a.companycode,a.companyname,b.projstatus) a group by companycode,companyname ");
            sql.Append(" ) a left join T_AY_SiteDailyRiskScore b on a.SITEID = b.SITEID ");
            sql.Append(" group by a.companycode,a.companyname,a.activecount,a.sitecount ");
          

            var list = await _repository.Db.SqlQueryable<EnterpriseScoreRank>(sql.ToString())
                .OrderBy(oo => oo.sitescore, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<EnterpriseScoreRank>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }
    }
}
