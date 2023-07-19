using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.AISprayAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AISprayAction
{
    public class AISprayService : BaseServices<AISprayActionEntity>, IAISprayService
    {
        public readonly IBaseRepository<AISprayActionEntity> _repository;
        public AISprayService(IBaseRepository<AISprayActionEntity> repository)
        {
            base.BaseDal = repository;
            _repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> InsertSparyPROC(AISparyInputDto input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _repository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSprayActionInsert",
                new SugarParameter("@sprayprojid", input.projid),
                new SugarParameter("@spraystate", input.spraystate),
                new SugarParameter("@imgurl", input.imgurl),
                new SugarParameter("@thumburl", input.thumburl),
                new SugarParameter("@imgurlmarked", input.imgurlmarked),
                new SugarParameter("@createtime", input.createtime),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        public async Task<DataTable> GetSprayList(int SITEID, DateTime date)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteSprayList", new { SITEID = SITEID, billdate = date });
        }

        public async Task<DataTable> GetAiSprayDataCountAsync(int type, int siteid)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiSprayDataCount", new { type= type, siteid= siteid });
        }

        public async Task<DataTable> GetAiSprayDuringAnalysisAsync(int type, int siteid)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiSprayDuringAnalysis", new{type=type,siteid=siteid});
        }
    }
}
