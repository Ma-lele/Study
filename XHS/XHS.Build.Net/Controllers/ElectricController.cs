using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.ElecMeter;
using XHS.Build.Services.Warning;

namespace XHS.Build.Net.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class ElectricController : ControllerBase
    {
        private readonly IElecMeterService _electricService;
        private readonly IWarningService _warningService;
        public ElectricController(IElecMeterService electricService, IWarningService warningService)
        {
            _electricService = electricService;
            _warningService = warningService;
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

            int result = await _electricService.RtdInsert(input);

            if (result > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else if(result == -1)
            {
                return ResponseOutput.NotOk("设备未添加白名单", 0);
            }
            else
            {
                return ResponseOutput.NotOk("上传数据失败", 0);
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
            if (input.type <= 21 || input.type > 29)
            {
                return ResponseOutput.NotOk("type参数错误", 0);
            }
            if (input.phase.Length !=4 || input.phase.Trim('0', '1') != string.Empty)
            {
                return ResponseOutput.NotOk("phase参数错误", 0);
            }
            int result = await _warningService.doElectric(input);

            if (result > 0)
            {
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk("上传数据失败", 0);
            }

        }

    }
}
