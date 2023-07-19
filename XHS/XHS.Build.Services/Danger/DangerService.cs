using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Danger
{
    public class DangerService : BaseServices<BaseEntity>, IDangerService
    {


        private readonly IBaseRepository<BaseEntity> _DangerRepository;

        public DangerService(IBaseRepository<BaseEntity> DangerRepository)
        {
            _DangerRepository = DangerRepository;
        }

        public Task<DataTable> GetDevCountAsync(int GROUPID)
        {
            return _DangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtDangerDevCount", new { GROUPID = GROUPID });

        }

        public async Task<DataRow> GetSiteCountAsync(int GROUPID)
        {
            DataTable dt = await _DangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtDangerSiteCount", new { GROUPID = GROUPID });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public Task<DataTable> GetWarnAreaCountAsync(int GROUPID, int type)
        {
            return _DangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtDangerWarnAreaCount", new { GROUPID = GROUPID, type = type });
        }

        public Task<DataTable> GetWarnCountAsync(int GROUPID, int type)
        {
            return _DangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtDangerWarnCount", new { GROUPID = GROUPID, type = type });
        }

        public Task<DataTable> GetWarnRankAsync(int GROUPID, int type)
        {
            return _DangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtDangerWarnRank", new { GROUPID = GROUPID, type = type });
        }

        public Task<DataTable> GetDevList(int GROUPID, int type, int pageindex, int pagesize, string keyword)
        {
            return _DangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtDangerDevList", new { GROUPID = GROUPID, type = type, pageindex = pageindex, pagesize = pagesize, keyword = keyword });
        }

        public async Task<DataTable> spV2DangerList(int SITEID)
        {
            return await _DangerRepository.Db.Ado.UseStoredProcedure()
                .GetDataTableAsync("spV2DangerList",
                new
                {
                    SITEID = SITEID
                });
        }

        public async Task<string> spV2DangerFile(int SITEID, string SDID)
        {
            return await _DangerRepository.Db.Ado.UseStoredProcedure()
                .GetStringAsync("spV2DangerFile",
                new
                {
                    SITEID = SITEID,
                    SDID = SDID
                });
        }

        public async Task<DataTable> spV2DangerTypeStats(int SITEID)
        {
            return await _DangerRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2DangerTypeStats",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2DangerTrend(int SITEID, int days)
        {
            return await _DangerRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2DangerTrend",
               new
               {
                   SITEID = SITEID,
                   days = days
               });
        }
    }
}
