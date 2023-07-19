using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.DeepPit;
using XHS.Build.Services.Warning;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 深基坑信息接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class DeepPitController : ControllerBase
    {

        private readonly IDeepPitService _deepPitService;
        private readonly IWarningService _warningService;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IMapper _mapper;
        public DeepPitController(IDeepPitService deepPitService, IAqtUploadService aqtUploadService, IWarningService warningService, IMapper mapper)
        {
            _deepPitService = deepPitService;
            _warningService = warningService;
            _aqtUploadService = aqtUploadService;
            _mapper = mapper;
        }

        /// <summary>
        /// 深基坑结构物数据上传
        /// </summary>
        /// <param name="dto">深基坑结构物Dto</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeepPit(DeepPitInput dto)
        {
            JObject mJObj = new JObject();
            string deviceIds = string.Empty;
            string deviceNames = string.Empty;
            if (dto.devices.Count > 0)
            {
                foreach (DeviceData d in dto.devices)
                {
                    deviceIds = deviceIds + "," + d.deviceId.Replace(":", string.Empty);
                    deviceNames = deviceNames + "," + d.deviceName;
                }
                deviceIds = deviceIds.Substring(deviceIds.IndexOf(",") + 1);
                deviceNames = deviceNames.Substring(deviceNames.IndexOf(",") + 1);
            }


            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.Remove("recordNumber");
            sp.Remove("devices");
            sp.Add("operator", "第三方");
            sp.Add("deviceIds", deviceIds);
            sp.Add("deviceNames", deviceNames);
            sp.NeetReturnValue();

            await _deepPitService.Insert(sp);

            if (sp.ReturnValue > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
                return ResponseOutput.Ok(mJObj.ToString());
            }
            else if (sp.ReturnValue == -2)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。(深基坑结构物未加白名单。)");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return ResponseOutput.NotOk(mJObj.ToString());
        }

        /// <summary>
        /// 深基坑实时数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="collectionTime">采集时间</param>
        /// <param name="monitorType">监测项</param>
        /// <param name="warnValue">预警阀值</param>
        /// <param name="alarmValue">报警阀值</param>
        /// <param name="data">数值</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeepPitHistory(DeepPitRtdDto dto)
        {
            JObject mJObj = new JObject();
            if (dto.monitorType <= 0 || dto.monitorType > 13 || (string.IsNullOrEmpty(dto.recordNumber) && string.IsNullOrEmpty(dto.dpCode)))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
            MonitorType type = (MonitorType)dto.monitorType;

            string watchPoints = string.Empty;
            string watchPointValues = string.Empty;
            string watchPointExValues = string.Empty;
            if (dto.data.Count > 0)
            {
                foreach (DeepPitData d in dto.data)
                {
                    watchPoints = watchPoints + "," + d.watchPoint.Replace(":", string.Empty);
                    watchPointValues = watchPointValues + "," + d.watchPointValue;
                    watchPointExValues = watchPointExValues + "," + d.watchPointExValue;
                }
                watchPoints = watchPoints.Substring(watchPoints.IndexOf(",") + 1);
                watchPointValues = watchPointValues.Substring(watchPointValues.IndexOf(",") + 1);
                watchPointExValues = watchPointExValues.Substring(watchPointExValues.IndexOf(",") + 1);
            }
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.Add("deviceId", dto.deviceId.Replace(":", string.Empty));
            sp.Add("monitorType", type.ToString());
            sp.Add("data", JsonConvert.SerializeObject(dto.data));
            sp.Add("operator", "第三方");
            sp.Add("watchPoints", watchPoints);
            sp.Add("watchPointValues", watchPointValues);
            sp.Add("watchPointExValues", watchPointExValues);
            sp.NeetReturnValue();



            int result = await _deepPitService.RtdInsert(sp);

            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
                return ResponseOutput.Ok(mJObj.ToString());
            }
            else if (result == -2)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。(未找到该设备。)");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return ResponseOutput.NotOk(mJObj.ToString());
        }


        /// <summary>
        /// 深基坑预警数据上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="deviceId">设备监测编号 (即deviceId)</param>
        /// <param name="warnExplain">报警类型(电量、温度、立杆轴力、水平倾角、立杆倾角、水平位移、模板沉降等)</param>
        /// <param name="warnContent">预警内容</param>
        /// <param name="happenTime">发生时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeepPitAlarmInfo(DeepPitAlarmInfoDto dto)
        {
            JObject mJObj = new JObject();

            if (dto.alarmExplain < 0 || dto.alarmExplain > 13)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确(报警类型不在范围内）。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
            MonitorType type = (MonitorType)dto.alarmExplain;
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.Add("deviceId", dto.deviceId.Replace(":", string.Empty));
            sp.Add("warnExplain", type.ToString());
            sp.Add("warnContent", dto.alarmContent);
            sp.Remove("recordNumber");
            sp.Remove("alarmExplain");
            sp.Remove("alarmContent");
            sp.NeetReturnValue();
            int result = await _warningService.doDeepPit(sp);
            if (result > 0)
            {
                mJObj.Add("code", "0");
                mJObj.Add("message", "上传成功。");
                return ResponseOutput.Ok(mJObj.ToString());
            }
            else if (result == -2)
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。(未找到该设备。)");
            }
            else
            {
                mJObj.Add("code", "1");
                mJObj.Add("message", "操作失败。");
            }
            return ResponseOutput.NotOk(mJObj.ToString());
        }


        /// <summary>
        /// 深基坑设备信息看板上传
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="uploadBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> DeppPitBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.uploadBoardUrl))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return ResponseOutput.NotOk(mJObj.ToString());
            }
            object dbparam = new
            {
                belongTo = string.Empty,
                recordNumber = dto.recordNumber,
                boardtype = "depppit",
                boardurl = dto.uploadBoardUrl,
                linkid = string.Empty
            };
            int result = await _aqtUploadService.doUpdateBoard(dbparam);
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
    }
}
