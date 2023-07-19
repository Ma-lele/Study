using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Inspection
{
    public class CaRoundService : BaseServices<InspectionEntity>, ICaRoundService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<InspectionEntity> _InspectionRepository;
        public CaRoundService(IUser user, IBaseRepository<InspectionEntity> InspectionRepository)
        {
            _user = user;
            _InspectionRepository = InspectionRepository;
            BaseDal = InspectionRepository;
        }

        public async Task<int> doDelete(string roundcode)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionDelete", new { roundcode = roundcode, USERID = _user.Id });
        }

        public async Task<DataSet> getOne(string roundcode)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spCaRoundGet", new { roundcode = roundcode });
        }


        public async Task<string> doInsert(List<SugarParameter> param)
        {

            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetStringAsync("spCaRoundInsert", param);
            
        }

        public async Task<int> doDetailInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _InspectionRepository.Db.Ado.UseStoredProcedure().GetStringAsync("spCaRoundDetailCheckInsert", param);
            return output.Value.ObjToInt();
        }


        public async Task<int> doDetailOkInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _InspectionRepository.Db.Ado.UseStoredProcedure().GetStringAsync("spCaRoundDetailCheckOkInsert", param);
            return output.Value.ObjToInt();
        }

        public async Task<int> doRemarkAdd(List<SugarParameter> param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCaRoundRemarkAdd", param);
        }


        public async Task<int> doSolve(List<SugarParameter> param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCaRoundSolve", param);
        }

        public async Task<int> doFinish(List<SugarParameter> param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCaRoundFinish", param);
        }

        public async Task<DataTable> GetOrderTenantList(DateTime startdate, DateTime enddate, int siteid = 0, int teid = 0, string keyword = "", int sort = 1, int tenanttype = 0, int pageindex = 1, int pagesize = 10)
        {
                      
            var dt = await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCaRoundCountList", new { GROUPID = _user.GroupId, USERID = _user.Id, SITEID = siteid, teid = teid, keyword = keyword, sort = sort, tenanttype = tenanttype, startdate = startdate, enddate= enddate,pageindex = pageindex, pagesize = pagesize });
            
            return dt;
        }

        public async Task<DataTable> GetOrderList(DateTime startdate, DateTime enddate, int teid = 0, int isoverhouronly = 0, int status = 0, string keyword = "", int pageindex = 1, int pagesize = 10)
        {
           var dt = await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCaRectifyRoundList", new { GROUPID = _user.GroupId, USERID = _user.Id, teid = teid, isoverhouronly= isoverhouronly,status = status, keyword = keyword, startdate = startdate, enddate = enddate, pageindex = pageindex, pagesize = pagesize });

            return dt;
        }

        public async Task<DataTable> GetCheckListByTenant(int teid)
        {
            var dt = await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCaCheckList", new { teid = teid });

            return dt;
        }

    }
}
