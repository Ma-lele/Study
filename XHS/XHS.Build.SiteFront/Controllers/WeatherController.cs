using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Group;
using XHS.Build.Services.Menus;
using XHS.Build.Services.Weather;
using XHS.Build.SiteFront.Attributes;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 天气接口
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    [Permission]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IUser _user;
        private readonly IGroupService _groupService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="weatherService"></param>
        /// <param name="user"></param>
        /// <param name="groupService"></param>
        /// <param name="menuService"></param>
        public WeatherController(IWeatherService weatherService, IUser user, IGroupService groupService)
        {
            _weatherService = weatherService;
            _user = user;
            _groupService = groupService;
        }


        /// <summary>
        /// 天气信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T000/Weather")]
        public async Task<IResponseOutput> Weather()
        {
            string cityName="";
            DataSet ds = await _groupService.GetAreas(_user.GroupId);
            if(ds != null && ds.Tables.Count>0)
            {
                cityName = ds.Tables[0].Rows[0]["label"].ToString().Replace("市","");
            }
            var data = await _weatherService.GetCityWeather(cityName);
            if (data == null)
            {
                return ResponseOutput.NotOk("无数据");
            }
            data.weather = data.weather.Replace("\\r", "").Replace("\\n", "").Replace("&quot;", "");
            string[] temp = data.weather.TrimStart('{').TrimEnd('}').Split(',');
            JArray arr = new JArray();
            foreach (var i in temp)
            {
                JObject obj = new JObject();
                obj[i.Split(':')[0].Trim()] = i.Split(':')[1].Trim();
                arr.Add(obj);

            }
            return ResponseOutput.Ok(arr);
        }

    }
}
