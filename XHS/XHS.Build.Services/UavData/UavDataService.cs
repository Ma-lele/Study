using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.UavData
{
    public class UavDataService:BaseServices<BaseEntity>,IUavDataService
    {
        private readonly IBaseRepository<BaseEntity> _uavDataRepository;
        public UavDataService(IBaseRepository<BaseEntity> uavDataRepository)
        {
            _uavDataRepository = uavDataRepository;
            BaseDal = uavDataRepository;
        }

        public async Task<DataTable> getList(int SITEID, DateTime createddate)
        {
            return await _uavDataRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteUavDataList", new { SITEID = SITEID, createddate = createddate });
        }
    }
}
