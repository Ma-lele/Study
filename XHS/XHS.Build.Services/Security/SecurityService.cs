using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Security
{
    public class SecurityService : BaseServices<GCSecurityEntity>, ISecurityService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCSecurityEntity> _baseRepository;
        public SecurityService(IUser user, IBaseRepository<GCSecurityEntity> baseRepository)
        {
            _user = user;
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }
        public async Task<PageOutput<GCSiteSecurityPageOutput>> GetSecuritySitePageList(string keyword, int page, int size, string order, string ordertype)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCSiteEntity>()
                .WhereIF(!string.IsNullOrEmpty(keyword), s => s.sitename.Contains(keyword))
                .Where(s => s.GROUPID == _user.GroupId && SqlFunc.Subqueryable<GCSecurityEntity>().Where(b => s.SITEID == b.SITEID && b.bdel == 0).Any() && SqlFunc.Subqueryable<GCSiteUserEntity>().Where(c => c.SITEID == s.SITEID && c.USERID == _user.Id).Any())
                .OrderByIF(!string.IsNullOrEmpty(order), order + " " + ordertype)
                .OrderBy(s => s.SITEID, OrderByType.Desc)
            .Select(s => new GCSiteSecurityPageOutput()
            {
                Sum = SqlFunc.Subqueryable<GCSecureHisEntity>().Where(h => h.SITEID == s.SITEID && SqlFunc.DateIsSame(h.createddate, DateTime.Now)).Count(),
                OperateDate = SqlFunc.Subqueryable<GCSecureHisEntity>().Where(h => h.SITEID == s.SITEID).Max(it => it.createddate),
                SITEID = s.SITEID,
                SiteName = s.sitename,
                SiteLng =s.sitelng,
                SiteLat = s.sitelat,
                SiteShortName = s.siteshortname
            }).ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCSiteSecurityPageOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<SecurityListOutput>> GetTodaySecurityListCount(int siteid, DateTime date)
        {
            var list = await _baseRepository.Db.Queryable<GCSecurityEntity>().Where(u => u.SITEID == siteid && u.bdel == 0).Select(it => new SecurityListOutput
            {
                SCName = it.scname,
                SECURITYID = it.SECURITYID,
                remark = it.remark,
                Sum = SqlFunc.Subqueryable<GCSecureHisEntity>().Where(h => h.SITEID == siteid && h.SECURITYID == it.SECURITYID && SqlFunc.DateIsSame(h.createddate, date)).Count(),
            }).ToListAsync();
            return list.OrderByDescending(a => a.Sum).ToList();
        }

        public async Task<SiteSecurityOutput> GetSiteSecurityOutput(int securityid)
        {
            var entity = await _baseRepository.Db.Queryable<GCSecurityEntity, GCSiteEntity>((a, b) => new JoinQueryInfos(JoinType.Inner, a.SITEID == b.SITEID))
                .Where((a, b) => a.SECURITYID == securityid && a.bdel == 0).Select((a, b) => new SiteSecurityOutput()
                {
                    GROUPID = a.GROUPID,
                    operatedate = a.operatedate,
                    @operator = a.@operator,
                    remark = a.remark,
                    sclat = a.sclat,
                    sclng = a.sclng,
                    scname = a.scname,
                    SECURITYID = a.SECURITYID,
                    SITEID = a.SITEID,
                    SiteName = b.sitename,
                    SiteShortName = b.siteshortname,
                    Sum = SqlFunc.Subqueryable<GCSecureHisEntity>().Where(h => h.SITEID == a.SITEID && h.SECURITYID == securityid && SqlFunc.DateIsSame(h.createddate, DateTime.Now)).Count()
                }).FirstAsync();
            if (entity != null)
            {
                var UserSite = await _baseRepository.Db.Queryable<GCSiteUserEntity>().Where(s => s.SITEID == entity.SITEID && s.USERID == _user.Id).ToListAsync();
                if (UserSite != null && UserSite.Any())
                {
                    return entity;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<int> Insert(GCSecurityEntity entity)
        {
            List<SugarParameter> param = new List<SugarParameter>();
            param.Add(new SugarParameter("@GROUPID", entity.GROUPID));
            param.Add(new SugarParameter("@SITEID", entity.SITEID));
            param.Add(new SugarParameter("@scname", entity.scname));
            param.Add(new SugarParameter("@sclng", entity.sclng));
            param.Add(new SugarParameter("@sclat", entity.sclat));
            param.Add(new SugarParameter("@remark", entity.remark));
            param.Add(new SugarParameter("@operator", entity.@operator));
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _baseRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSecurityInsert", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> Update(GCSecurityEntity entity)
        {
            List<SugarParameter> param = new List<SugarParameter>();
            param.Add(new SugarParameter("@SECURITYID", entity.SECURITYID));
            param.Add(new SugarParameter("@scname", entity.scname));
            param.Add(new SugarParameter("@sclng", entity.sclng));
            param.Add(new SugarParameter("@sclat", entity.sclat));
            param.Add(new SugarParameter("@remark", entity.remark));
            param.Add(new SugarParameter("@operator", entity.@operator));
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _baseRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSecurityUpdate", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> Delete(int securityid,string username)
        {
            List<SugarParameter> param = new List<SugarParameter>();
            param.Add(new SugarParameter("@SECURITYID", securityid));
            param.Add(new SugarParameter("@username", username));
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _baseRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSecurityDelete", ps);
            return output.Value.ObjToInt();
        }

        public async Task<DataSet> spV2SecurityStats(int SITEID)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure()
               .GetDataSetAllAsync("spV2SecurityStats",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2SecurityPoint(int SITEID)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2SecurityPoint",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2SecurityHisCount(int SITEID, DateTime startDate, DateTime endDate)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2SecurityHisCount",
               new
               {
                   SITEID = SITEID,
                   StartDate = startDate,
                   EndDate = endDate
               });
        }

        public async Task<DataTable> spV2SecurityHisList(int SITEID, string keyword, DateTime startDate, DateTime endDate,
            int SECURITYID, int pageIndex, int pageSize)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2SecurityHisList",
               new
               {
                   SITEID = SITEID,
                   keyword = keyword,
                   SECURITYID = SECURITYID,
                   startdate = startDate,
                   enddate = endDate,
                   pagesize = pageSize,
                   pageindex = pageIndex
               });
        }

        public async Task<DataTable> spV2SecurityHisImage(int SITEID, int SCHISID)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2SecurityHisImage",
               new
               {
                   SITEID = SITEID,
                   SCHISID = SCHISID
               });
        }
    }
}
