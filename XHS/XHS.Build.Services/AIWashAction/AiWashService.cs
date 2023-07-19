using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AIWash
{
    public class AiWashService : BaseServices<BaseEntity>, IAiWashService
    {

        private readonly IBaseRepository<BaseEntity> _aiWashService;

        public AiWashService(IBaseRepository<BaseEntity> aiWashRepository)
        {
            _aiWashService = aiWashRepository;
        }

        public async Task<DataTable> GetAiWashRecordListAsync(string month, string platecolor, int SiteId, int pageindex, int pagesize)
        {
            return await _aiWashService.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiWashRecordList", new { month = month, platecolor = platecolor, siteid = SiteId, pageindex = pageindex, pagesize = pagesize });
        }

        public async Task<DataRow> GetAiWashDataCompareAsync(int SiteId)
        {
            DataTable dt = await _aiWashService.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiWashDataCompare", new { siteid = SiteId });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataTable> GetAiWashDataCountAsync(int SiteId, int type)
        {
            return await _aiWashService.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiWashDataCount", new { siteid = SiteId, type = type });
        }

        public async Task<DataTable> GetAiWashDuringAnalysisAsync(int SiteId, int type)
        {
            return await _aiWashService.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiWashDuringAnalysis", new { siteid = SiteId, type = type });
        }
    }
}
