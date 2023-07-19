using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using XHS.Build.City.Dtos;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Configs;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Cameras;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Video;

namespace XHS.Build.City.Controllers
{
    /// <summary>
    /// 《各级智慧监管平台数据抓取标准V1.0》
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DataGetController : ControllerBase
    {
  
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IUserAqtKeyToken _userToken;
        private readonly ConfigHelper _configHelper;
        private readonly AqtToken _aqtToken;
        private readonly ICameraService _cameraService;
        private readonly IVideoService _videoService;
        public DataGetController(IConfiguration configuration, IVideoService videoService, ICameraService cameraService, AqtToken aqtToken, IUserAqtKeyToken userToken,IHpSystemSetting hpSystemSetting)
        {
            _hpSystemSetting = hpSystemSetting;
            _userToken = userToken;
            _configHelper = new ConfigHelper();
            _aqtToken = aqtToken;
            _cameraService = cameraService;
            _videoService = videoService;
        }

        /// <summary>
        /// 3.1 鉴权接口
        /// </summary>
        /// <param name="account">account值</param>
        /// <param name="password">password值（MD5加密，32位大写）</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("getToken")]
        public async Task<string> getToken(AccountDto dto)
        {
            string jwtStr = string.Empty;
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.account) || string.IsNullOrEmpty(dto.password))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string dAppId = dto.account;//UEncrypter.DecryptByRSA16(key, MyConfig.Webconfig.PrivateKey);
            string dSecret = dto.password;//UEncrypter.DecryptByRSA16(secret, MyConfig.Webconfig.PrivateKey);

            var keyList = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
            if (keyList == null)
            {
                mJObj.Add("code", "10003");
                mJObj.Add("message", "appKey不存在。");
                return mJObj.ToString();
            }
            var keySecret = keyList.Items.FirstOrDefault(k => k.Key == dAppId && k.Secret == dSecret);
            if (keySecret == null || string.IsNullOrEmpty(keySecret.Key) || string.IsNullOrEmpty(keySecret.Secret))
            {
                mJObj.Add("code", "10004");
                mJObj.Add("message", "appkey和appSecret不匹配。");
                return mJObj.ToString();
            }

            var token = _userToken.Create(new[]
            {
                new Claim(KeyClaimAttributes.Key, keySecret.Key),
                new Claim(KeyClaimAttributes.GroupId, keySecret.GroupId==null?"":keySecret.GroupId),
                new Claim(KeyClaimAttributes.Name, keySecret.Name)
            });
            mJObj.Add("code", "200");
            mJObj.Add("message", "操作成功！");
            JObject data = new JObject();
            data.Add("accessToken", token);
            data.Add("expireTime", (DateTime.Now.AddMinutes(20).ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            mJObj.Add("data", data);
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取特种人员信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="highFormworkBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("PersonnelInformation/GetSiteCurrentManagerInfo")]
        public async Task<string> GetSiteCurrentManagerInfo(string idCard)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(idCard))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "PersonnelInformation/GetSiteCurrentManagerInfo";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("idCard", idCard);
            string result = _aqtToken.UrlRequest(api,keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取当前机构下的项目自查数
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间(非必传，传null为截至到当前时间)</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("ProjectInformation/GetProjectSelfInspectionCount")]
        public async Task<string> GetProjectSelfInspectionCount(DateTime beginTime, DateTime? endTime, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "ProjectInformation/GetProjectSelfInspectionCount";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("beginTime", beginTime);
            keyValues.Add("endTime", endTime);
            keyValues.Add("belongedTo", belongedTo);
            string result = _aqtToken.UrlRequest(api,keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取项目自查基本信息
        /// </summary>
        /// <param name="pageIndex">支持分页抓取 页码(从1开始)</param>
        /// <param name="pageSize">每页抓取数据条数</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("ProjectInformation/GetProjectSelfInspection")]
        public async Task<string> GetProjectSelfInspection(int pageIndex, int pageSize, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "ProjectInformation/GetProjectSelfInspection";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("pageIndex", pageIndex);
            keyValues.Add("pageSize", pageSize);
            keyValues.Add("belongedTo", belongedTo);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取项目自查内容看板
        /// </summary>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="recordNumber">安全监督备案号</param>
        /// <param name="checkFormNumber">检查单编号</param>
        /// <param name="type">检查类型（省标/项目内部/JGJ59/扬尘）检查</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("ProjectInformation/GetSelfInspectionContent")]
        public async Task<string> GetSelfInspectionContent(string belongedTo, string recordNumber, string checkFormNumber, string type)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "ProjectInformation/GetSelfInspectionContent";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("belongedTo", belongedTo);
            keyValues.Add("recordNumber", recordNumber);
            keyValues.Add("checkFormNumber", checkFormNumber);
            keyValues.Add("type", type);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取项目月评总数
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("ProjectInformation/GetMonthReviewCount")]
        public async Task<string> GetMonthReviewCount(DateTime beginTime, DateTime endTime, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "ProjectInformation/GetMonthReviewCount";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("beginTime", beginTime);
            keyValues.Add("endTime", endTime);
            keyValues.Add("belongedTo", belongedTo);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取项目每月自评状态及结果
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月份（传0获取全年数据）</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("ProjectInformation/GetMonthReviewResults")]
        public async Task<string> GetMonthReviewResults(int year, int month, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "ProjectInformation/GetMonthReviewResults";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("year", year);
            keyValues.Add("month", month);
            keyValues.Add("belongedTo", belongedTo);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取项目安标考评结果
        /// </summary>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="pageIndex">支持分页抓取 页码(从1开始)</param>
        /// <param name="pageSize">每页抓取数据条数</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("ProjectInformation/GetSafetyStandardResults")]
        public async Task<string> GetSafetyStandardResults( string belongedTo,int pageIndex, int pageSize)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "ProjectInformation/GetSafetyStandardResults";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("belongedTo", belongedTo);
            keyValues.Add("pageIndex", pageIndex);
            keyValues.Add("pageSize", pageSize);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }


        /// <summary>
        /// 获取机械设备基本信息
        /// </summary>
        /// <param name="pageIndex">支持分页抓取 页码(从1开始)</param>
        /// <param name="pageSize">每页抓取数据条数</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("DockingMachineryInfos/GetMachineryInfos")]
        public async Task<string> GetMachineryInfos( int pageIndex, int pageSize, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "DockingMachineryInfos/GetMachineryInfos";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("belongedTo", belongedTo);
            keyValues.Add("pageIndex", pageIndex);
            keyValues.Add("pageSize", pageSize);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取机械设备信息看板
        /// </summary>
        /// <param name="propertyRightsRecordNo">设备信息号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("DockingMachineryInfos/ViewMachineryInfo")]
        public async Task<string> ViewMachineryInfo(string propertyRightsRecordNo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(propertyRightsRecordNo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "DockingMachineryInfos/ViewMachineryInfo";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("propertyRightsRecordNo", propertyRightsRecordNo);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }


        /// <summary>
        /// 获取项目危大工程基本信息
        /// </summary>
        /// <param name="pageIndex">支持分页抓取 页码(从1开始)</param>
        /// <param name="pageSize">每页抓取数据条数</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("DockingSuperScaleDanger/GetSuperScaleDangerList")]
        public async Task<string> GetSuperScaleDangerList( int pageIndex, int pageSize, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "DockingSuperScaleDanger/GetSuperScaleDangerList";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("belongedTo", belongedTo);
            keyValues.Add("pageIndex", pageIndex);
            keyValues.Add("pageSize", pageSize);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取项目超规模危大工程资料看板
        /// </summary>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="recordNumber">安全监督备案号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("DockingMachineryInfos/ViewScreenSuperScaleDanger")]
        public async Task<string> ViewScreenSuperScaleDanger(string belongedTo, string recordNumber)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "DockingMachineryInfos/ViewScreenSuperScaleDanger";
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("belongedTo", belongedTo);
            keyValues.Add("recordNumber", recordNumber);
            string result = _aqtToken.UrlRequest(api, keyValues);
            if (string.IsNullOrEmpty(result))
            {
                mJObj = new JObject();
                mJObj.Add("code", "10005");
                mJObj.Add("message", "接口调用异常。");
            }
            else
            {
                return result;
            }
            return mJObj.ToString();
        }

        /// <summary>
        /// 获取视频实时流地址
        /// </summary>
        /// <param name="videoId">视频地址的唯一id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("getVideoRealUrl")]
        public async Task<string> getVideoRealUrl(string videoId)
        {
            JObject mJObj = new JObject();
            JObject mJObj1 = new JObject();
            if (string.IsNullOrEmpty(videoId))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string[] videoinfo = videoId.Split("-");
            videoId = videoinfo[0];
            int channel = 1;
            if (videoinfo.Length > 1)
            {
                channel = videoId[1];
            }
            List<VSiteCamera> list= await _cameraService.GetCameraInfoByCameracode(videoId);
            if (list.Count > 0)
            {
                VSiteCamera camera = list[0];
                BnCamera bc = new BnCamera();
                bc.cameracode = camera.cameracode;
                bc.channel = channel;
                bc.cameratype = camera.cameratype;
                if (camera.cameratype == 1 || camera.cameratype == 16 || camera.cameratype == 18)
                {
                    bc.action = "hls";
                }
                else
                {
                    mJObj.Add("code", "10005");
                    mJObj.Add("message", "暂不支持该视频。");
                }
                BnCameraResult<BnPlaybackURL> result = _videoService.GetRealurl(bc);
                if (result.code == "0")
                {
                    if (string.IsNullOrEmpty(result.url))
                    {
                        mJObj.Add("code", "10005");
                        mJObj.Add("message", "未获取到流地址。");
                    }
                    else
                    {
                        mJObj.Add("code", "0");
                        mJObj.Add("message", "获取成功。");
                        mJObj1.Add("url", result.url);
                        mJObj1.Add("sitename", camera.siteshortname);
                        mJObj1.Add("videoname", camera.cameraname);
                        mJObj.Add("data", mJObj1);
                    }
                }
                
            }
            else
            {
                mJObj.Add("code", "10005");
                mJObj.Add("message", "未查到该视频。");
            }

            return mJObj.ToString();
        }
    }
}
