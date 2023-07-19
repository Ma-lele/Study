using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Supervise
{
    public class Supervise : BaseServices<BaseEntity>, ISupervise
    {
        private readonly IBaseRepository<BaseEntity> _superviseRepository;

        public Supervise(IBaseRepository<BaseEntity> superviseRepository)
        {
            _superviseRepository = superviseRepository;
        }

        public async Task<DataTable> GetCloseCountAsync(int GROUPID, int datayear)
        {
            return await _superviseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperviseCloseCount", new { GROUPID = GROUPID, datayear = datayear });
        }

        public async Task<DataTable> GetCloseCountListAsync(int GROUPID, string yearmonth)
        {
            return await _superviseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperviseCloseCountList", new { GROUPID = GROUPID, yearmonth = yearmonth });
        }

        public async Task<DataTable> GetCountAsync(int GROUPID, string datamonth)
        {
            return await _superviseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperviseCount", new { GROUPID = GROUPID, datamonth = datamonth });
        }

        public async Task<DataTable> GetTypeCountAsync(int GROUPID, int datayear)
        {
            return await _superviseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperviseTypeCount", new { GROUPID = GROUPID, datayear = datayear });
        }

        public async Task<DataTable> GetTypeRankAsync(int GROUPID, string yearmonth)
        {
            return await _superviseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperviseTypeRank", new { GROUPID = GROUPID, yearmonth = yearmonth });
        }
    }
}
