using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.HighFormwork;
using XHS.Build.Services.Warning;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 同步设备信息
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class HighFormworkController : ControllerBase
    {

        private readonly IHighFormworkService _highFormworkService;
        private readonly IWarningService _warningService;
        private readonly IMapper _mapper;
        public HighFormworkController(IHighFormworkService highFormworkService, IWarningService warningService, IMapper mapper)
        {
            _highFormworkService = highFormworkService;
            _warningService = warningService;
            _mapper = mapper;
        }

        /// <summary>
        /// 3.6.1 高支模实时数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="collectionTime">收集时间</param>
        /// <param name="Power">电量(%)</param>
        /// <param name="temperature">温度（℃）</param>
        /// <param name="load">立杆轴力(KN)</param>
        /// <param name="horizontalAngle">水平倾角（°）</param>
        /// <param name="coordinate">立杆倾角（°）</param>
        /// <param name="translation">水平位移（mm）</param>
        /// <param name="settlement">模板沉降（mm）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IResponseOutput> HighFormworkHistory(HighFormworkData dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
            //var highFormworkData = _mapper.Map<HighFormworkData>(dto);
            int result = await _highFormworkService.doRtdInsert(dto);

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
            }
            return ResponseOutput.NotOk(mJObj.ToString());
        }

        /// <summary>
        /// 3.6.2 高支模预警数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="warnExplain">报警类型(电量、温度、立杆轴力、水平倾角、立杆倾角、水平位移、模板沉降等)</param>
        /// <param name="warnContent">预警内容</param>
        /// <param name="happenTime">发生时间</param>
        /// <returns></returns>
        [HttpPost]
       	[Authorize]
        public async Task<IResponseOutput> HighFormworkAlarmInfo(HighFormworkAlarmInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId)|| string.IsNullOrEmpty(dto.warnExplain) || string.IsNullOrEmpty(dto.warnContent))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
            int result = await _warningService.doHighFormwork(dto);

            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
                return ResponseOutput.Ok(mJObj.ToString());
            }
            else if (result == -2)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。(设备编号未加白名单)");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return ResponseOutput.NotOk(mJObj.ToString());
        }
    }
}
