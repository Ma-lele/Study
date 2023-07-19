using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Schedule
{
    public class ScheduleService : BaseServices<BaseEntity>, IScheduleService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _scheduleRepository;
        public ScheduleService(IUser user, IBaseRepository<BaseEntity> scheduleRepository)
        {
            _user = user;
            _scheduleRepository = scheduleRepository;
            BaseDal = scheduleRepository;
        }

        public async Task<int> doUpdate(object param)
        {
            return await _scheduleRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spScheduleNodeInfoUpdate", param);
        }

        public async Task<DataSet> getList(int SITEID)
        {
            return await _scheduleRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spScheduleNodeInfoList", new { SITEID = SITEID });
        }

        public async Task<DataSet> getSum(int SITEID)
        {
            return await _scheduleRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spScheduleNodeInfoSum", new { SITEID = SITEID });
        }
    }
}
