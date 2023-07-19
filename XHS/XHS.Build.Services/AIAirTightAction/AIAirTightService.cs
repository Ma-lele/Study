using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.AIAirTightAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AIAirTightAction
{
    public class AIAirTightService : BaseServices<AIAirTightActionEntity>, IAIAirTightService
    {
        public readonly IBaseRepository<AIAirTightActionEntity> _repository;
        public AIAirTightService(IBaseRepository<AIAirTightActionEntity> repository)
        {
            base.BaseDal = repository;
            _repository = repository;
        }

        public async Task<int> WarnInsertForAirTight(AirTightProcInputDto input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _repository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForAirTight",
                new SugarParameter("@projid", input.projid),
                new SugarParameter("@cartype", input.cartype),
                new SugarParameter("@carno", input.carno),
                new SugarParameter("@createtime", input.createtime),
                new SugarParameter("@jsonall", JsonConvert.SerializeObject(input)),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        public async Task<DataTable> GetAirTightList(int SITEID, DateTime date)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteAirTightList", new { siteid = SITEID, billdate = date });
        }

        public async Task<DataTable> GetAiAirtightRecordListAsync(string month, int SiteId, string keyword, int pageindex, int pagesize)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiAirtightRecordList", new { month = month, siteid = SiteId, keyword = keyword, pageindex = pageindex, pagesize = pagesize });
        }

        public async Task<DataRow> GetAiAirtightDataCompareAsync(int SiteId)
        {
            DataTable dt = await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiAirtightDataCompare", new {siteid = SiteId });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }                                 

        public async Task<DataTable> GetAiAirtightDataCountAsync(int SiteId, int type)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiAirtightDataCount", new { siteid = SiteId, type = type });
        }

        public async Task<DataTable> GetAiAirtightDuringAnalysisAsync(int SiteId, int type)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiAirtightDuringAnalysis", new { siteid = SiteId, type = type });
        }
    }
}
