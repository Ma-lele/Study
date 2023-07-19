using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Aqt365.Controllers
{
    /// <summary>
    /// 项目数据动态考核
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]

    public class SiteController : ControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly IDeviceCNService _deviceCNService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IUserAqtKeyToken _userToken;
        private readonly IUserAqtKey _userAqtKey;
        private readonly IBaseRepository<GCBoardEntity> _baseRepository;
        private readonly XHSRealnameToken _jwtToken;
        public SiteController(IUserAqtKeyToken userToken, XHSRealnameToken jwtToken,IBaseRepository<GCBoardEntity> baseRepository, IUserAqtKey userAqtKey, IHpSystemSetting hpSystemSetting, ISiteService siteService, IDeviceCNService deviceCNService)
        {
            _hpSystemSetting = hpSystemSetting;
            _siteService = siteService;
            _deviceCNService = deviceCNService;
            _userToken = userToken;
            _userAqtKey = userAqtKey;
            _baseRepository = baseRepository;
            _jwtToken = jwtToken;
        }

        /// <summary>
        /// 获得对接网址
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="secret"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<JObject> GetSiteConnectURL(string appkey, string secret,string recordNumber,string belongedTo)
        {
            JObject mJObj = new JObject();
            string siteid = "";
            string attenduserpsd = "";
            if (string.IsNullOrEmpty(appkey) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("siteconnecturl", "");
                mJObj.Add("token", "");
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
            DataRow dr = await _siteService.getOneByRecordNumber(recordNumber);
            if(dr != null)
            {
                string drkey = dr["appkey"].ToString();
                string drsecret = dr["secret"].ToString();
                siteid = dr["SITEID"].ToString();
                attenduserpsd = dr["attenduserpsd"].ToString();
                if (drkey != appkey || drsecret != secret)
                {
                    mJObj.Add("siteconnecturl", "");
                    mJObj.Add("token", "");
                    mJObj.Add("msg", "项目AppKey或Secret不正确。");
                    return mJObj;
                }
            }
            else
            {
                mJObj.Add("siteconnecturl", "");
                mJObj.Add("token", "");
                mJObj.Add("msg", "未查询到该安全监督备案号对应的项目。");
                return mJObj;
            }
            //给jwt提供携带的数据
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("recordNumber", recordNumber);
            keyValuePairs.Add("siteid", siteid);

            var token = _userToken.Create(new[]
           {
                new Claim(AppKeyClaimAttributes.Attenduserpsd, attenduserpsd),
                new Claim(AppKeyClaimAttributes.SiteId, siteid),
                new Claim(AppKeyClaimAttributes.Appkey, appkey),
                new Claim(AppKeyClaimAttributes.RecordNumber, recordNumber)
            });
         //   TnToken tnToken = _tokenHelper.CreateToken(keyValuePairs);
          
            string url =   new StringBuilder()
                 .Append(HttpContext.Request.Scheme)
                 .Append("://")
                 .Append(HttpContext.Request.Host)
                 .Append(HttpContext.Request.PathBase)
                 .Append(HttpContext.Request.Path).ToString();
            url = url.Substring(0, url.LastIndexOf("/")+1);
            mJObj.Add("siteconnecturl", url);
            mJObj.Add("token", token);
            return mJObj;
        }


        /// <summary>
        /// 获得当前在场人员数量
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetSiteCurrentWorkInfo(string token, string recordNumber, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("currentworkcount", "0");
                mJObj.Add("workdata", "");
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
            JArray myJsonArray = new JArray();
            mJObj.Add("currentworkcount", 0);
            mJObj.Add("workdata", myJsonArray);
            string attenduserpsd = _userAqtKey.Attenduserpsd;
            string url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            if(string.IsNullOrEmpty(url))
            {
                return mJObj;
            }
            if (string.IsNullOrEmpty(attenduserpsd))
            {
                return mJObj;
            }
            string account = attenduserpsd.Split("||")[0];
            string pwd = attenduserpsd.Split("||")[1];
            JObject jso = new JObject();
            jso.Add("recordNumber", recordNumber);
            // string url = "https://www.xhssmz.com:9021/api/realname/";
            //string account = "xuzhou";
           // string pwd = "eD27Ege*sw45";
            string api = "construction-site-api/site-current-employee-info";
            string result = _jwtToken.JsonRequest(url, account, pwd, api, JsonConvert.SerializeObject(jso));
            if (!string.IsNullOrEmpty(result))
            {
                JObject job = JObject.Parse(result);
                if (job.GetValue("data")!= null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                {
                    mJObj = new JObject();
                    mJObj = (JObject)job.GetValue("data");
                }
                else
                {
                    JObject mJObj1 = new JObject();
                    mJObj1.Add("workTypeCode", 10);
                    mJObj1.Add("workcount", 0);
                    myJsonArray.Add(mJObj1);
                }

            }
            else
            {
                JObject mJObj1 = new JObject();
                mJObj1.Add("workTypeCode", 10);
                mJObj1.Add("workcount", 0);
                myJsonArray.Add(mJObj1);
            }           
            return mJObj;
        }

        /// <summary>
        /// 获得项目当前时间安管人员在岗信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetSiteCurrentManagerInfo(string token, string recordNumber, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("managercount", "0");
                mJObj.Add("managerInfos", "");
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
            JArray myJsonArray = new JArray();
            mJObj.Add("managercount", 0);
            mJObj.Add("managerInfos", myJsonArray);
            string attenduserpsd = _userAqtKey.Attenduserpsd;
            string url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            if (string.IsNullOrEmpty(url))
            {
                return mJObj;
            }
            if (string.IsNullOrEmpty(attenduserpsd))
            {
                return mJObj;
            }
            string account = attenduserpsd.Split("||")[0];
            string pwd = attenduserpsd.Split("||")[1];
            JObject jso = new JObject();
            jso.Add("recordNumber", recordNumber);
            // string url = "https://www.xhssmz.com:9021/api/realname/";
            //string account = "xuzhou";
            // string pwd = "eD27Ege*sw45";
            string api = "construction-site-api/site-current-employee-manager-info";
            string result = _jwtToken.JsonRequest(url, account, pwd, api, JsonConvert.SerializeObject(jso));
            if (!string.IsNullOrEmpty(result))
            {
                JObject job = JObject.Parse(result);
                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                {
                    mJObj = new JObject();
                    mJObj = (JObject)job.GetValue("data");
                }
                else
                {
                    JObject mJObj1 = new JObject();
                    mJObj1.Add("name", "");
                    mJObj1.Add("idcard", "");
                    myJsonArray.Add(mJObj1);
                }

            }
            else
            {
                JObject mJObj1 = new JObject();
                mJObj1.Add("name", "");
                mJObj1.Add("idcard", "");
                myJsonArray.Add(mJObj1);
            }
            return mJObj;
        }

        /// <summary>
        /// 获得当前时间扬尘噪声监测数据
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetSiteCurrentDustInfo(string token, string recordNumber, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
           
           DataTable dt = await _deviceCNService.getSiteRtdApi(_userAqtKey.SiteId);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mJObj.Add("pm2dot5", dr["pm2_5"].ToDouble());
                mJObj.Add("pm10", dr["pm10"].ToDouble());
                mJObj.Add("noise", dr["noise"].ToDouble());
                mJObj.Add("upload", dr["upload"].ToString());
                return mJObj;
            }
            mJObj.Add("msg", "未查到扬尘噪声监测数据。");
            return mJObj;

        }

        /// <summary>
        /// 获得一段时间内的临边防护报警次数
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetSiteEdgeAlarm(string token, string recordNumber, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("alarmcount", 0);
                mJObj.Add("edgelength", 0);
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
        
           
            DataRow dr = await _siteService.getSitefenceCount(_userAqtKey.SiteId);
            if (dr != null)
            {
                mJObj.Add("alarmcount", dr["alarmcount"].ToInt());
                mJObj.Add("edgelength", dr["edgelength"].ToDouble());
                return mJObj;
            }
            mJObj.Add("msg", "未查到临边防护报警数据。");
            return mJObj;
        }

        /// <summary>
        /// 获得塔吊的预警、报警次数
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetSiteCraneAlarm(string token, string recordNumber, string belongedTo, string deviceId)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo) || string.IsNullOrEmpty(deviceId))
            {
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }

            DataRow dr = await _siteService.getSiteCraneAlarmCount(_userAqtKey.SiteId, deviceId);
            int cranestatus = 0;
            if (dr != null)
            {
                mJObj.Add("warningcount", int.Parse(dr["warningcount"].ToString()));
                mJObj.Add("alarmcount", int.Parse(dr["alarmcount"].ToString()));
                if (!string.IsNullOrEmpty(dr["cranestatus"].ToString()))
                {
                    cranestatus = int.Parse(dr["cranestatus"].ToString());
                }
                mJObj.Add("cranestatus", cranestatus);
                return mJObj;
            }
            mJObj.Add("msg", "未查到塔吊报警数据。");
            return mJObj;

        }

        /// <summary>
        /// 获得施工升降机的预警、报警次数
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetSiteElevatorAlarm(string token, string recordNumber, string belongedTo, string deviceId)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo) || string.IsNullOrEmpty(deviceId))
            {
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
          
            DataRow dr = await _siteService.getSiteElevatorAlarmCount(_userAqtKey.SiteId, deviceId);
            int elevatorstatus = 0;
            if (dr != null)
            {
                mJObj.Add("warningcount", int.Parse(dr["warningcount"].ToString()));
                mJObj.Add("alarmcount", int.Parse(dr["alarmcount"].ToString()));
                if (!string.IsNullOrEmpty(dr["elevatorstatus"].ToString()))
                {
                    elevatorstatus = int.Parse(dr["elevatorstatus"].ToString());
                }
                mJObj.Add("elevatorstatus", elevatorstatus);
                return mJObj;
            }
            mJObj.Add("msg", "未查到施工升降机警数据。");
            return mJObj;
        }

        /// <summary>
        /// 获得卸料平台的预警、报警次数
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber">监督备案号</param>
        /// <param name="belongedTo">所属机构编号</param>
        /// <param name="deviceId">设备唯一id</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetSiteUploadAlarm(string token, string recordNumber, string belongedTo, string deviceId)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo) || string.IsNullOrEmpty(deviceId))
            {
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
           
            DataRow dr = await _siteService.getSiteUploadAlarmCount(_userAqtKey.SiteId, deviceId);
            int uploadstatus = 0;
            if (dr != null)
            {
                mJObj.Add("warningcount", int.Parse(dr["warningcount"].ToString()));
                mJObj.Add("alarmcount", int.Parse(dr["alarmcount"].ToString()));
                if (!string.IsNullOrEmpty(dr["uploadstatus"].ToString()))
                {
                    uploadstatus = int.Parse(dr["uploadstatus"].ToString());
                }
                mJObj.Add("uploadstatus", uploadstatus);
                return mJObj;
            }
            mJObj.Add("msg", "未查到卸料平台报警数据。");
            return mJObj;
        }

        /// <summary>
        /// 获得智慧工地集成平台URL跳转地址的有效性
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber"></param>
        /// <param name="belongedTo"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetInSiteURL(string token, string recordNumber, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("siteurl", "");
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
            
            var s034 = _hpSystemSetting.getSettingValue(Const.Setting.S034);
            if (string.IsNullOrEmpty(s034))
            {
                mJObj.Add("siteurl", "");
                return mJObj;
            }
            mJObj.Add("siteurl", s034);
            return mJObj;
        }

        /// <summary>
        /// 查询所有看板URL地址的有效性
        /// </summary>
        /// <param name="token"></param>
        /// <param name="recordNumber"></param>
        /// <param name="belongedTo"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<JObject> GetBoardURL(string token, string recordNumber, string belongedTo)
        {
            JObject mJObj = new JObject();
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(recordNumber) || string.IsNullOrEmpty(belongedTo))
            {
                mJObj.Add("msg", "参数不正确。");
                return mJObj;
            }
            var s034 = _hpSystemSetting.getSettingValue(Const.Setting.S034);
            List<GCAqtBoardEntity>  boardlist =   await _baseRepository.Db.Queryable<GCAqtBoardEntity>().ToListAsync();
            long ts = DateTimeExtensions.ToTimestamp(DateTime.Now);
            for (int k = 0; k < boardlist.Count; k++)
            {

                GCAqtBoardEntity entity = boardlist[k];
                string boardname = entity.boardname;
                JObject param = new JObject();
                param.Add("id", recordNumber);
                param.Add("type", entity.boardtype);
                param.Add("tm", ts);
                byte[] byteArray = Encoding.GetEncoding("utf-8").GetBytes(JsonConvert.SerializeObject(param));
                string data = HttpUtility.UrlEncode(Convert.ToBase64String(byteArray));

                string url = "http://"+s034 + "/handler/HLogin.ashx?action=board&data=" + data;
                mJObj.Add(boardname, url);
            }

            return mJObj;
        }

        }
}
