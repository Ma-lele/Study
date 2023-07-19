using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.CityAqi
{
    public interface ICityAqiService : IBaseServices<BaseEntity>
    {
        int doInsert(params SugarParameter[] param);

        int doWeatherUpdate(params SugarParameter[] param);

        Task<int> doInsert(SgParams sp);

        Task<int> doWeatherUpdate(SgParams sp);
    }
}
