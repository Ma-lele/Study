using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Phase
{
    public class PhaseService:BaseServices<BaseEntity>,IPhaseService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _phaseRepository;
        public PhaseService(IUser user, IBaseRepository<BaseEntity> phaseRepository)
        {
            _user = user;
            _phaseRepository = phaseRepository;
            BaseDal = phaseRepository;
        }

        public async Task<long> doUpdate(int SITEID, int constructphase, int phasepercent, DateTime phasedate)
        {
            var param1 = new SugarParameter("@SITEID", SITEID);
            var param2 = new SugarParameter("@USERID", _user.Id);
            var param3 = new SugarParameter("@constructphase", constructphase);
            var param4 = new SugarParameter("@phasepercent", phasepercent);
            var param5 = new SugarParameter("@phasedate", phasedate);
            var param6 = new SugarParameter("@operator", _user.Name);
            var output = new SugarParameter("@output", null, System.Data.DbType.String, ParameterDirection.Output);//isOutput=true
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);//isOutput=true
            await _phaseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSitePhaseUpdate", param1, param2, param3, param4, param5, param6, output, returnvalue);
            int returnValue = Convert.ToInt32(returnvalue.Value);
            long result;
            string outputValue = Convert.ToString(output.Value);
            if (long.TryParse(outputValue, out result))
                return result;
            else
                return Convert.ToInt64(returnValue);
        }

        public async Task<DataTable> getAll()
        {
            return await _phaseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spPhaseAll");
        }

        public async Task<DataTable> getHisList(int SITEID, DateTime operatedate)
        {
            return await _phaseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSitePhaseHisGet", new { SITEID = SITEID, operatedate = operatedate });
        }

        public async Task<DataTable> getPhotoList(long PHASEHISID)
        {
            return await _phaseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spPhasePhotoList", new { PHASEHISID = PHASEHISID });
        }

        public async Task<DataTable> GetSitePhaseList(int SITEID)
        {
            return await _phaseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSitePhaseGet", new { SITEID = SITEID });
        }


        public async Task<int> UpdateSitePhase(int SITEID, string PHASEIDS, string phasedates, int phasedatetype)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("SITEID", SITEID);
            param.Add("PHASEIDS", PHASEIDS);
            param.Add("phasedates", phasedates);
            param.Add("phasedatetype", phasedatetype);
            param.Add("operator", _user.Name);
            return await _phaseRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSitePhaseSave", param);
        }
    }
}
