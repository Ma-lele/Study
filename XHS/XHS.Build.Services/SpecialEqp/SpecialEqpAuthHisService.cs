using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SpecialEqp
{
    public class SpecialEqpAuthHisService : BaseServices<GCSpecialEqpAuthHisEntity>, ISpecialEqpAuthHisService
    {
        private readonly IBaseRepository<GCSpecialEqpAuthHisEntity> _baseRepository;
        public SpecialEqpAuthHisService(IBaseRepository<GCSpecialEqpAuthHisEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        /// <summary>
        /// 获取特种设备的上下机数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="secode"></param>
        /// <param name="billdate"></param>
        /// <returns></returns>
        public async Task<DataTable> GetListAsync(int SITEID, string secode, DateTime billdate)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SpecialAuthHis", new { SITEID = SITEID, secode = secode, billdate = billdate });
        }
    }
}
