using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Center.Attributes;
using XHS.Build.Services.DeviceBind;
using XHS.Build.Center.Auth;
using XHS.Build.Services.OperateLogS;
using AutoMapper;
using System.Linq;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class ElectricController : ControllerBase
    {
        private readonly IDeviceBindService _deviceBindService;
        private readonly NetToken _netToken;
        public ElectricController(IDeviceBindService deviceBindService, NetToken netToken)
        {
            _deviceBindService = deviceBindService;
            _netToken = netToken;
        }

        /// <summary>
        /// 上传实时数据
        /// </summary>
        /// <param name="input">json数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> RtdData(EmeterDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.emetercode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.emetercode, "elecmeter");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, input);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }

        }

        /// <summary>
        /// 上传告警数据
        /// </summary>
        /// <param name="input">json数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> WarnData(EmeterWarnDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.emetercode))
            {
                return ResponseOutput.NotOk("emetercode参数错误", 0);
            }
            if (input.type<=21 || input.type>29)
            {
                return ResponseOutput.NotOk("type参数错误", 0);
            }
            if (input.phase.Length != 4 || input.phase.Trim('0', '1') != string.Empty)
            {
                return ResponseOutput.NotOk("phase参数错误", 0);
            }
            
            var DBList = await _deviceBindService.GetDeviceBindByCodeType(input.emetercode, "elecmeter");
            if (DBList.Any())
            {
                var netApi = "/api/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"].ToString();
                _netToken.JsonRequest(netApi, DBList, input);
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("未获取到服务器信息");
            }

        }

    }
}
