using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Water
{
    public class WaterService : BaseServices<BaseEntity>, IWaterService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _waterRepository;
        public WaterService(IUser user, IBaseRepository<BaseEntity> waterRepository)
        {
            _user = user;
            _waterRepository = waterRepository;
            BaseDal = waterRepository;
        }


        /// <summary>
        /// 实时数据插入
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> rtdInsert(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _waterRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWaterDataInsert", ps);
            return output.Value.ObjToInt();
        }

       
    }
}
