using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Helmet
{
    public class HelmetService : BaseServices<GCHelmetEntity>, IHelmetService
    {
        private readonly IBaseRepository<GCHelmetEntity> _baseRepository;
        public HelmetService(IBaseRepository<GCHelmetEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        public async Task<DataTable> GetAiHelmentLocationListAsync(int siteid)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiHelmentLocationList", new { siteid = siteid });
        }

        public async Task<DataRow> GetAiHelmetDataCompareAsync(int siteid)
        {
            DataTable dt = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiHelmetDataCompare", new { siteid = siteid });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataTable> GetAiHelmetDataCountAsync(int type, int siteid)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiHelmetDataCount", new { type = type , siteid = siteid });
        }

        public async Task<DataTable> GetAiHelmetDuringAnalysisAsync(int type, int siteid)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiHelmetDuringAnalysis", new { type = type , siteid = siteid });
        }

        public async Task<DataTable> GetAiHelmetRecordListAsync(string keyword, string month, int siteid, int pageindex, int pagesize)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiHelmetRecordList", new { keyword = keyword, month= month, siteid= siteid, pageindex= pageindex, pagesize=pagesize });
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupHelmetBuildCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,isnull(A.count,0) as count from T_GC_Group G LEFT JOIN (SELECT D.GROUPID, count(1) as count from T_GC_Helmet D  GROUP BY D.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0 ORDER BY G.city, G.district, G.GROUPID");
        }

        public async Task<PageOutput<HelmetOutputList>> GetHelmetPage(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCHelmetEntity, GCSiteEntity, GCEmployeeEntity>((h, s, e) => new JoinQueryInfos(JoinType.Inner, h.SITEID == s.SITEID, JoinType.Left, h.ID == e.ID))
                .WhereIF(groupid > 0, (h, s, e) => h.GROUPID == groupid && s.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (h, s) => h.helmetcode.Contains(keyword) || s.sitename.Contains(keyword) || s.siteshortname.Contains(keyword))
                .OrderBy(" HELMETID desc")
            .Select<HelmetOutputList>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<HelmetOutputList>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }
        public async Task<DataSet> GetListBySiteId(int SITEID)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spNjjySiteHelmetPosition", new {SITEID = SITEID });

        }
    }
}
