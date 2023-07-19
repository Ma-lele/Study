using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 城市天气
    /// </summary>
    [SugarTable("T_GC_CityWeather")]
    public class GCCityWeather
    {
        public string area { get; set; }
        public string weather { get; set; }
        public DateTime updatedate { get; set; }
    }
}
