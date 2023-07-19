using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.AIIllegalCarAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AIIllegalCarAction
{
    public class AIIllegalCarService : BaseServices<AIIllegalCarActionEntity>, IAIIllegalCarService
    {
        public readonly IBaseRepository<AIIllegalCarActionEntity> _repository;
        public AIIllegalCarService(IBaseRepository<AIIllegalCarActionEntity> repository)
        {
            base.BaseDal = repository;
            _repository = repository;
        }

        public async Task<int> WarnInsertForIllegalCar(IllegalCarProcInputDto input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _repository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForIllegalCar",
                new SugarParameter("@projid", input.projid),
                new SugarParameter("@cartype", input.cartype),
                new SugarParameter("@carno", input.carno),
                new SugarParameter("@createtime", input.createtime),
                new SugarParameter("@jsonall", JsonConvert.SerializeObject(input)),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        public async Task<DataTable> GetIllegalCarList(int SITEID, DateTime date)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteIllegalCarList", new { SITEID = SITEID, billdate = date });
        }

        public async Task<DataTable> GetAiIllegalRecordListAsync(string month, string keyword, int SiteId, int pageindex, int pagesize)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiIllegalRecordList", new { month= month, keyword= keyword, siteid= SiteId, pageindex= pageindex, pagesize= pagesize });
        }

        public async Task<DataRow> GetAiIllegalDataCompareAsync(int SiteId)
        {
           DataTable dt=  await  _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiIllegalDataCompare", new { siteid= SiteId });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataTable> GetAiIllegalDataCountAsync(int SiteId, int type)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiIllegalDataCount", new { siteid = SiteId, type=type});
        }

        public async Task<DataTable> GetAiIllegalDuringAnalysisAsync(int SiteId, int type)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiIllegalDuringAnalysis", new { siteid = SiteId, type = type });
        }
    }
}
