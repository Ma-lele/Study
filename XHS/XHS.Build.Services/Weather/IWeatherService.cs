using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Weather
{
    public interface IWeatherService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 获取指定城市天气信息
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        Task<GCCityWeather> GetCityWeather(string cityName);
        
    }
}
