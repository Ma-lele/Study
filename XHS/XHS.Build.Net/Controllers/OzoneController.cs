using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.Ozone;

namespace XHS.Build.Net.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class OzoneController : ControllerBase
    {
        private readonly IOzoneService _ozoneService;
        private readonly IMapper _mapper;
        private readonly IOperateLogService _operateLogService;
        public OzoneController(IOzoneService ozoneService, IMapper mapper, IOperateLogService operateLogService)
        {
            _ozoneService = ozoneService;
            _mapper = mapper;
            _operateLogService = operateLogService;
        }
        [HttpPost]
        public async Task<IResponseOutput> RealData( OzoneRtdDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.ozcode))
            {
                return ResponseOutput.NotOk("未找到相应设备或状态异常", 0);
            }

            int retDB = await _ozoneService.doRtdInsert(input);
            if (retDB > 0)
            {
                var seData = _mapper.Map<OzoneRtdData>(input);
                seData.Platform = "net";
                await _operateLogService.AddOzoneDataLog(seData);
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

    }
}
