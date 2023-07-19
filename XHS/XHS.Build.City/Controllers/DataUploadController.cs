using System;
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
using XHS.Build.Services.AqtUpload;

namespace XHS.Build.City.Controllers
{
    /// <summary>
    /// 《各级智慧监管平台数据上传标准V1.0》
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DataUploadController : ControllerBase
    {
        private readonly IUserAqtKeyToken _userToken;
        private readonly ConfigHelper _configHelper;
        private readonly AqtToken _aqtToken;
        private readonly IAqtUploadService _aqtUploadService;
        public DataUploadController(IConfiguration configuration, AqtToken aqtToken, IUserAqtKeyToken userToken, IAqtUploadService aqtUploadService)
        {
            _userToken = userToken;
            _configHelper = new ConfigHelper();
            _aqtToken = aqtToken;
            _aqtUploadService = aqtUploadService;
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
            data.Add("expireTime", (DateTime.Now.AddMinutes(20).ToUniversalTime().Ticks - 621355968000000000) /10000000);
            mJObj.Add("data", data);
            return mJObj.ToString();
        }

        /// <summary>
        /// 3.2 上传项目劳务单位基本信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="unifiedSocialCreditcode">统一社会信用代码</param>
        /// <param name="companyName">企业名称</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Companies")]
        public async Task<string> Companies(CompaniesDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.unifiedSocialCreditcode) || string.IsNullOrEmpty(dto.companyName) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "Companies";
            //mJObj.Add("recordNumber", recordNumber);
            //mJObj.Add("belongedTo", belongedTo);
            //mJObj.Add("unifiedSocialCreditcode", unifiedSocialCreditcode);
            //mJObj.Add("companyName", companyName);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.3 上传项目劳务人员基本信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="unifiedSocialCreditcode">统一社会信用代码</param>
        /// <param name="workerName">人员姓名</param>
        /// <param name="idNumber">身份证号</param>
        /// <param name="workType">工种类型</param>
        /// <param name="securityJob">安管职位 (项目经理、执行经理、安全人员)</param>
        /// <param name="isResign">是否离职（0否，1是）</param>
        /// <param name="entryDate">进场时间（2019-04-18）</param>
        /// <param name="exitDate">离职时间（2019-05-05）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Workers")]
        public async Task<string> Workers(WorkersDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.unifiedSocialCreditcode) || string.IsNullOrEmpty(dto.workerName) ||
                string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.idNumber) ||
                string.IsNullOrEmpty(dto.entryDate))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "Workers";

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.4 上传项目劳务人员安全教育信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="unifiedSocialCreditcode">统一社会信用代码</param>
        /// <param name="workerIdNumber">身份证号</param>
        /// <param name="educationContent">教育内容</param>
        /// <param name="educationType">教育类型(安全教育,入场教育,退场教育,技能培训,班前教育,VR安全教育,其它)</param>
        /// <param name="educationLocation">教育地点</param>
        /// <param name="educationDate">教育日期（2019-04-18）</param>
        /// <param name="educationDuration">教育时长(分钟)</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Workers/EducationInfos")]
        public async Task<string> EducationInfos(EducationInfosDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.unifiedSocialCreditcode) || string.IsNullOrEmpty(dto.workerIdNumber) ||
                string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.educationContent) 
                || string.IsNullOrEmpty(dto.educationType) || string.IsNullOrEmpty(dto.educationDate) || dto.educationDuration <= 0)
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "Workers/EducationInfos";
           
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.5 上传项目劳务人员奖惩信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="unifiedSocialCreditcode">统一社会信用代码</param>
        /// <param name="workerIdNumber">身份证号</param>
        /// <param name="eventContent">奖惩内容</param>
        /// <param name="eventType">奖惩类型（0奖励，1惩罚）</param>
        /// <param name="eventDate">奖惩日期（2019-04-18）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Workers/RewardPunishments")]
        public async Task<string> RewardPunishments(RewardPunishmentsDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.unifiedSocialCreditcode) || string.IsNullOrEmpty(dto.workerIdNumber) || 
                string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.eventDate))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "Workers/RewardPunishments";

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.6上传项目每小时场内人员进出数
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="recordDate">记录日期（2019-04-18）</param>
        /// <param name="recordHour">记录小时（0-23）</param>
        /// <param name="inPeople">进场人数</param>
        /// <param name="outPeople">出场人数</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Attendances")]
        public async Task<string> Attendances(AttendancesDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordDate) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "Attendances";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);

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
        /// 3.7上传浏览人员信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="peoplesBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/PeoplesBoard")]
        public async Task<string> PeoplesBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "peoples",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string api = "DeviceInfo/PeoplesBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.8 上传浏览指定人员基本信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="peopleBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/PeopleBoard")]
        public async Task<string> PeopleBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "people",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string api = "DeviceInfo/PeopleBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.9 上传浏览项目当前立体定位看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="stereotacticBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/StereotacticBoard")]
        public async Task<string> StereotacticBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DeviceInfo/StereotacticBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.10 上传项目当前场内人员不同工种数量
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="workTypeDic">键为工种代码,值为人数</param>
        /// <param name="recordDate">记录时间（2019-04-18）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Attendances/PeopleTypeCount")]
        public async Task<string> PeopleTypeCount(PeopleTypeCountDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.recordDate) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "Attendances/PeopleTypeCount";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.11 上传检查单信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="checkNumber">检查单编号</param>
        /// <param name="IsProvinStand">是否符合省标准，0:否 1:是</param>
        /// <param name="checkDate">检查时间（2019-07-07 10:22:22）</param>
        /// <param name="isNeedToRectify">是否需要整改</param>
        /// <param name="recommendFinishDate">建议整改完成时间（2019-07-07 10:22:22）</param>
        /// <param name="checkComment">检查备注</param>
        /// <param name="checkPeople">检查人姓名 多人用,隔开</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Check/Checklist")]
        public async Task<string> Checklist(ChecklistDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.checkNumber) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.checkPeople))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "Check/Checklist";
           
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.12 上传移动巡检点
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="checkPointId">巡检点id（唯一标识）</param>
        /// <param name="summary">巡检地点描述</param>
        /// <param name="building">楼栋号</param>
        /// <param name="floor">楼层号 默认0</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Check/CheckPoints")]
        public async Task<string> CheckPoints(CheckPointsDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.checkPointId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.summary))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "Check/CheckPoints";
            object dbparam = new
            {
                recordNumber = dto.recordNumber,
                checkPointId = dto.checkPointId,
                summary = dto.summary,
                building = dto.building,
                floor = dto.floor
            };
            _aqtUploadService.doUpdateCheckPoints(dbparam);

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.13 上传移动巡检信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="checkPointId">巡检点id（唯一标识）</param>
        /// <param name="checkPeople">检查人姓名</param>
        /// <param name="checkContent">巡检描述</param>
        /// <param name="picUrls">巡检照片 多张用,隔开</param>
        /// <param name="checkDate">巡检时间（2019-07-07 12:24:34）</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Check/MobileCheck")]
        public async Task<string> MobileCheck(MobileCheckDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.checkPointId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.checkPeople) || string.IsNullOrEmpty(dto.checkContent) || string.IsNullOrEmpty(dto.picUrls))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "Check/MobileCheck";
            object dbparam = new
            {
                recordNumber = dto.recordNumber,
                checkPointId = dto.checkPointId,
                checkPeople = dto.checkPeople,
                checkContent = dto.checkContent,
                picUrls = dto.picUrls,
                checkDate = dto.checkDate
            };
            _aqtUploadService.doUpdateMobileCheck(dbparam);

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.14 上传扬尘设备信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <param name="deviceName">设备名称</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DustInfo/DustDeviceInfo")]
        public async Task<string> DustDeviceInfo(DeviceInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.deviceId))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            string api = "DustInfo/DustDeviceInfo";
            object dbparam = new
            {
                recordNumber = dto.recordNumber,
                deviceId = dto.deviceId,
                deviceName = dto.deviceName
            };
            _aqtUploadService.doUpdateDustDeviceInfo(dbparam);

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.15 上传扬尘日监测数据
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <param name="temperature">温度</param>
        /// <param name="humidity">湿度</param>
        /// <param name="windSpeed">风速</param>
        /// <param name="windDirection">风向</param>
        /// <param name="pm2dot5">pm2.5</param>
        /// <param name="pm10">pm10</param>
        /// <param name="noise">噪音</param>
        /// <param name="moniterTime">监测日期，例如：2019-01-24</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DustInfo/DayDustInfo")]
        public async Task<string> DayDustInfo(DayDustInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DustInfo/DayDustInfo";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.16 上传扬尘信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="dustBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DustInfo/DustInfoBoard")]
        public async Task<string> DustInfoBoard(DustInfoBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.dustBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DustInfo/DustInfoBoard";
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "dustinfo",
                boardurl = dto.dustBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.17 上传视频监控点信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="videoId">视频地址的唯一id</param>
        /// <param name="type">视频类型（0：扬尘监控，1：塔吊监控，2：其它）</param>
        /// <param name="site">安装位置</param>
        /// <param name="url">视频跳转url</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/UploadVideo")]
        public async Task<string> UploadVideo(VideoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.videoId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.site) || string.IsNullOrEmpty(dto.url))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DeviceInfo/UploadVideo";

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.18 上传塔吊司机信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="propertyRightsRecordNo">设备信息号</param>
        /// <param name="PersonName">人员姓名</param>
        /// <param name="Sex">性别 0：男，1:女</param>
        /// <param name="Type">传固定值2（代表使用人员）</param>
        /// <param name="workTypeCode">工种(详见工种字典)</param>
        /// <param name="idCard">身份证号码</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DockingMachineryInfos/UploadSpecialOperationPersonnel")]
        public async Task<string> UploadSpecialOperationPersonnel(UploadSpecialOperationPersonnelDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.propertyRightsRecordNo) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || 
                string.IsNullOrEmpty(dto.PersonName) || string.IsNullOrEmpty(dto.idCard) || string.IsNullOrEmpty(dto.remark))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DockingMachineryInfos/UploadSpecialOperationPersonnel";

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.19 上传塔吊基本信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="machineryModel">机械型号</param>
        /// <param name="machineryType">机械类型：塔式起重机 = 0,施工升降机 = 1,货运施工升降机 = 2,桥式起重机 = 3,门式起重机 = 4</param>
        /// <param name="propertyRightsRecordNo">设备信息号</param>
        /// <param name="machineryCheckState">检测状态 未检测 = 0, 非我所检测 = 4, 检测中 = 5, 检测合格 = 6,检测不合格 = 7,复检中 = 17,复检合格 = 18,复检不合格 = 19</param>
        /// <param name="checkState">机械状态：未安装告知 = 0,安装告知审核中 = 1,安装告知审核通过 = 2,安装告知审核不通过 = 3,检测合格 = 6,办理使用登记审核中 = 8,办理使用登记未通过 = 9,办理使用登记通过 = 10,拆卸告知审核中 = 11,拆卸告知审核通过 = 12,拆卸告知审核不通过 = 13,使用登记注销审核中 = 14, 使用登记注销审核通过 = 15,使用登记注销审核不通过 = 16</param>
        /// <param name="reCheckReviewDate">检测合格日期</param>
        /// <param name="oem">生产厂家</param>
        ///  <param name="leaveTheFactoryNo">生产编号</param>
        ///  <param name="propertyEntCode">产权单位-社会统一信用代码</param>
        ///  <param name="propertyEntName">产权单位-名称</param>
        ///  <param name="userEntCode">使用单位-社会统一信用代码</param>
        ///  <param name="userEntName">使用单位-名称</param>
        ///  <param name="checkUrl">检测报告文件路径</param>
        ///  <param name="useRecordNo">使用登记证编号</param>
        ///  <param name="useRecordNoUrl">使用登记证文件全路径</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DockingMachineryInfos/UploadMachineryInfos")]
        public async Task<string> UploadMachineryInfos(UploadMachineryInfosDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.propertyRightsRecordNo) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo) || string.IsNullOrEmpty(dto.machineryModel)
                 || string.IsNullOrEmpty(dto.oem) || string.IsNullOrEmpty(dto.leaveTheFactoryNo) || string.IsNullOrEmpty(dto.propertyEntCode) || string.IsNullOrEmpty(dto.propertyEntName)
                || string.IsNullOrEmpty(dto.userEntCode) || string.IsNullOrEmpty(dto.userEntName))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DockingMachineryInfos/UploadMachineryInfos";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);

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
        /// 3.20 上传塔吊信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="craneBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeviceInfo/CraneBoard")]
        public async Task<string> CraneBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "crane",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string api = "DeviceInfo/CraneBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.23 上传升降机信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="hoistBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/HoistBoard")]
        public async Task<string> HoistBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "hoist",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string api = "DeviceInfo/HoistBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.24 上传卸料平台基本信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <param name="deviceName">设备名称</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/UploadDeviceInfo")]
        public async Task<string> UploadDeviceInfo(DeviceInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DeviceInfo/UploadDeviceInfo";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.25 上传卸料平台信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="uploadBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/UploadBoard")]
        public async Task<string> UploadBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "upload",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string api = "DeviceInfo/UploadBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.26 上传深基坑设备基本信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <param name="deviceName">设备名称</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/DeppPitDeviceInfo")]
        public async Task<string> DeppPitDeviceInfo(DeviceInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DeviceInfo/DeppPitDeviceInfo";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.27 上传深基坑设备信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deppPitBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/DeppPitBoard")]
        public async Task<string> DeppPitBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "depppit",
                boardurl = dto.uploadBoardUrl
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string api = "DeviceInfo/DeppPitBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.28 上传高支模设备基本信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <param name="deviceName">设备名称</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/HighFormworkDeviceInfo")]
        public async Task<string> HighFormworkDeviceInfo(DeviceInfoDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.deviceId) || string.IsNullOrEmpty(dto.recordNumber) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DeviceInfo/HighFormworkDeviceInfo";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.29 上传高支模设备信息看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="highFormworkBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/HighFormworkBoard")]
        public async Task<string> HighFormworkBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "highformwork",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string api = "DeviceInfo/HighFormworkBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
        /// 3.30 上传智慧监管整体看板
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="highFormworkBoardUrl">看板地址</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("DeviceInfo/SmartSupervisionBoard")]
        public async Task<string> SmartSupervisionBoard(UploadBoardDto dto)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(dto.uploadBoardUrl) || string.IsNullOrEmpty(dto.belongedTo))
            {
                mJObj.Add("code", "10001");
                mJObj.Add("message", "参数不正确。");
                return mJObj.ToString();
            }

            string api = "DeviceInfo/SmartSupervisionBoard";
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            object dbparam = new
            {
                belongTo = dto.belongedTo,
                recordNumber = "",
                boardtype = "site",
                boardurl = dto.uploadBoardUrl,
                linkid = ""
            };
            _aqtUploadService.doUpdateBoard(dbparam);
            string result = _aqtToken.JsonRequest(api, jsonString);
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
    }
}
