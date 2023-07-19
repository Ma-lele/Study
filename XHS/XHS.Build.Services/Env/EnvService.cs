using System;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Env
{
    public class EnvService : BaseServices<BaseEntity>, IEnvService
    {
        private readonly IBaseRepository<BaseEntity> _envRepository;

        public EnvService(IBaseRepository<BaseEntity> envRepository)
        {
            _envRepository = envRepository;
        }

        public async Task<DataRow> GetSiteCountAsync(int GROUPID)
        {
            DataTable dt = await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvSiteCount", new { GROUPID = GROUPID });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataTable> GetWarnCountAsync(int GROUPID,int type)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvWarnCount", new { GROUPID = GROUPID, type= type });
        }

        public async Task<DataSet> GetWarnCountTotalAsync(int GROUPID)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spCtEnvWarnCountTotal", new { GROUPID = GROUPID});
        }

        public async Task<DataRow> GetWashCountAsync(int GROUPID, int type)
        {
            DataTable dt = await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvWashCount", new { GROUPID = GROUPID,type=type });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataRow> GetAirTightCountAsync(int GROUPID, int type)
        {
            DataTable dt = await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvAirTightCount", new { GROUPID = GROUPID, type = type });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataRow> GetSoilCountAsync(int GROUPID, int type)
        {
            DataTable dt = await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvSoilCount", new { GROUPID = GROUPID, type = type });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataTable> GetSitePmAsync(int GROUPID)
        {
            return await  _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvSitePm", new { GROUPID = GROUPID});
        }

        public async Task<DataTable> GetPmRtdRankAsync(int GROUPID)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvPmRtdRank", new { GROUPID = GROUPID });
        }

        public async Task<DataTable> GetPmHourRankAsync(int GROUPID)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvPmHourRank", new { GROUPID = GROUPID });
        }

        public async Task<DataTable> GetPmDailyRankAsync(int GROUPID)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvPmDailyRank", new { GROUPID = GROUPID });
        }

        public async Task<DataTable> GetPmDailyListAsync(int GROUPID, string billdate, string keyword, int pageindex, int pagesize)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvPmDailyList", new { GROUPID = GROUPID, billdate= billdate, keyword= keyword, pageindex= pageindex, pagesize= pagesize });
        }

        public async Task<DataTable> GetPmHourListAsync(int GROUPID, string billdate, string keyword, int pageindex, int pagesize)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvPmHourList", new { GROUPID = GROUPID, billdate = billdate, keyword = keyword, pageindex = pageindex, pagesize = pagesize });
        }

        public async Task<DataTable> GetCtEnvWarnListAsync(int GROUPID, int SITEID, DateTime startdate, DateTime enddate, int type, int pageindex, int pagesize)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtEnvWarnList", new { GROUPID = GROUPID, SITEID = SITEID, startdate = startdate, enddate = enddate, type = type, pageindex = pageindex, pagesize = pagesize });
        }

        #region  项目端2.0
        public async Task<DataTable> GetPmSiteRtdData(int SITEID)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2PmSiteRtdData", new { SITEID = SITEID });
        }

        public async Task<DataSet> GetPmSiteChart(int SITEID)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spV2PmSiteChart", new { SITEID = SITEID });
        }
        public async Task<DataSet> GetSiteO3Chart(int SITEID)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteO3Chart", new { SITEID = SITEID });
        }

        public async Task<DataTable> GetPmSiteDayWarnCount(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2PmSiteDayWarnCount", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetPmOnlineBarChart(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2PmOnlineBarChart", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetSiteDataHis(int SITEID, DateTime startdate, DateTime enddate, int datatype, int pageindex, int pagesize)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteDataHis", new { SITEID = SITEID, startdate = startdate, enddate = enddate, datatype = datatype, pageindex = pageindex, pagesize = pagesize });
        }

        public async Task<DataTable> GetSiteO3His( int SITEID, DateTime startdate, DateTime enddate, int datatype, int pageindex, int pagesize)
        {
            return await _envRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteO3His", new { SITEID = SITEID, startdate = startdate, enddate = enddate, datatype = datatype, pageindex = pageindex, pagesize = pagesize });
        }
        #endregion
    }
}