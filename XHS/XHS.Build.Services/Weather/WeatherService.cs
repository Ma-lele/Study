using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Weather
{
    public class WeatherService : BaseServices<BaseEntity>, IWeatherService
    {
        private readonly IBaseRepository<BaseEntity> _baseRepository;
        public WeatherService(IBaseRepository<BaseEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<GCCityWeather> GetCityWeather(string cityName)
        {
            return await _baseRepository.Db.SqlQueryable<GCCityWeather>
                ($" select [area],[weather],[updatedate] from [XJ_Env].[dbo].[T_GC_CityWeather] where [area] = '{cityName}'")
                .FirstAsync();
        }


        
    }
}
