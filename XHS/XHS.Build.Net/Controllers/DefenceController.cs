using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Response;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Defence;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 高处作业临边防护接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class DefenceController : ControllerBase
    {

        private readonly IDefenceService _defenceService;
        public DefenceController(IDefenceService defenceService)
        {
            _defenceService = defenceService;
        }

        /// <summary>
        /// 3.5.1缺失记录上传（可选） 
        /// 3.5.2恢复记录上传
        /// </summary>
        /// <param name="warnNumber">设备监测编号 (即deviceId)</param>
        /// <param name="defectPosition">缺失位置</param>
        /// <param name="defectWarnNumber">缺失预警模块编号（备用）</param>
        /// <param name="defectDate">发生时间（2020-01-10 10:00:00）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Fenceinterface/FenceAlarmInfo")]
        public async Task<IResponseOutput> FenceAlarmInfo(FenceAlarmInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.warnNumber))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
            int result = 0;
            if (string.IsNullOrEmpty(dto.defectPosition))
            {
                if (dto.recoveryDate.ToString().Equals("0001/1/1 0:00:00"))
                {
                    mJObj.Add("code", "10001");
                    mJObj.Add("message", "恢复记录上传时recoveryDate必须。");
                    return ResponseOutput.NotOk(mJObj.ToString());
                }
                result = await _defenceService.doFourRecover(dto.warnNumber, dto.recoveryDate);
            }
            else
            {
                if (dto.defectDate.ToString().Equals("0001/1/1 0:00:00"))
                {
                    mJObj.Add("code", "10001");
                    mJObj.Add("message", "缺失记录上传时defectDate必须。");
                    return ResponseOutput.NotOk(mJObj.ToString());
                }
                result = await _defenceService.doFourDisconnect(dto.warnNumber, "4", dto.defectPosition, dto.defectDate);
            }
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
                return ResponseOutput.Ok(mJObj.ToString());
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
        }
    }
}
