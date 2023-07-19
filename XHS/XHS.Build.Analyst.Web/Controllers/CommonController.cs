using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Analyst.Web.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Event;
using XHS.Build.Services.Group;
using XHS.Build.Services.Menus;
using XHS.Build.Services.Weather;

namespace XHS.Build.Analyst.Web.Controllers
{
    /// <summary>
    /// 通用接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class CommonController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IUser _user;
        private readonly IGroupService _groupService;
        private readonly IMenuService _menuService;
        private readonly IWeatherService _weatherService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="commonService"></param>
        public CommonController(IEventService eventService, IUser user, IGroupService groupService,
           IWeatherService weatherService, IMenuService menuService)
        {
            _eventService = eventService;
            _user = user;
            _groupService = groupService;
            _menuService = menuService;
            _weatherService = weatherService;
        }


        /// <summary>
        /// Group下拉框
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Login]
        public async Task<IResponseOutput> GroupSelect(string cityCode)
        {
            var result = await _groupService.GetGroupSelect(cityCode, _user.userregion);
          
            return ResponseOutput.Ok(result); 
        }


        /// <summary>
        /// 天气信息
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Weather(string cityName)
        {
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


        /// <summary>
        /// 切换区/市
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetGroup(string regionid)
        {
            if (_user.userregion == "0" || _user.userregion.Split(',').Contains(regionid))
            {
                _user.AnalystRegionID = regionid;
                return ResponseOutput.Ok();
            }

            return ResponseOutput.NotOk();
        }


        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetMenu()
        {
            var list = await _menuService.GetAnalystUserMenu();
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 获取事件类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetParentEventType()
        {
            var list = await _eventService.GetParentEventType();
            return ResponseOutput.Ok(list);
        }
    }
}
