using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Njjy
{
    public class NjjyService:BaseServices<BaseEntity>,INjjyService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _njjyRepository;
        public NjjyService(IUser user, IBaseRepository<BaseEntity> njjyRepository)
        {
            _user = user;
            _njjyRepository = njjyRepository;
            BaseDal = njjyRepository;
        }

        public async Task<DataTable> getPhaseHisList(int SITEID, DateTime operatedate)
        {
            return await _njjyRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySitePhaseHisList", new { SITEID = SITEID, operatedate = operatedate });
        }

        public async Task<DataTable> getPhaseHisList(int SITEID)
        {
            return await _njjyRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySitePhaseHisGet", new { SITEID = SITEID });
        }
    }
}
