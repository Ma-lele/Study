using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.CityAqi
{
    public class CityAqiService : BaseServices<BaseEntity>, ICityAqiService
    {
        private readonly IBaseRepository<BaseEntity> _baseRepository;

        public CityAqiService(IBaseRepository<BaseEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;

        }


        /// <summary>
        /// 插入实时AQI数据
        /// </summary>
        /// <param name="param">AQI数据信息</param>
        /// <returns>成功件数</returns>
        public int doInsert(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommand("spCityAqiInsert", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doInsert(SgParams sp)
        {
            await _baseRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCityAqiInsert", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 更新实时天气数据
        /// </summary>
        /// <param name="param">实时天气数据信息</param>
        /// <returns>成功件数</returns>
        public int doWeatherUpdate(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommand("spCityWeatherUpdate", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doWeatherUpdate(SgParams sp)
        {
            await _baseRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCityWeatherUpdate", sp.Params);
            return sp.ReturnValue;
        }
    }
}
