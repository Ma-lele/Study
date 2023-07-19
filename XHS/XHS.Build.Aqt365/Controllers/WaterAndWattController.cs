using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Aqt365;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Util;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Service.Video;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.Base;
using XHS.Build.Services.Cameras;
using XHS.Build.Services.ElecMeter;
using XHS.Build.Services.Inspection;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.Site;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Video;
using XHS.Build.Services.Water;
using XHS.Build.Services.WaterMeter;
using XHS.Build.Services.Event;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Services.CityAqi;
using XHS.Build.Services.Fog;
using static XHS.Build.Common.Helps.HpFog;
using XHS.Build.Services.Warning;
using XHS.Build.Services.DailyJob;
using System.Xml;

namespace xhs.build.Aqt365.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class WaterAndWattController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISystemSettingService _systemSettingService;
        private readonly IWaterMeterService _waterMeterService;
        private readonly IWaterService _waterService;
        private readonly IElecMeterService _elecMeterService;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IEventService _ieventService;
        private readonly SiteCityFuningToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly XHSRealnameToken _jwtToken;
        private readonly SiteCityXuweiToken _xuweiToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly ISiteService _siteService;
        private readonly IYsApiService _ysApiService;
        private readonly IVideoService _videoService;
        private readonly IBaseRepository<BaseEntity> _baseServices;
        private readonly IHkOpenApiService _hkOpenApiService;
        private readonly ICameraService _cameraService;
        private readonly IDeviceCNService _deviceCNService;
        private readonly ICityAqiService _cityAqiService;
        private readonly IFogJobService _fogJobService;
        private readonly IWarningService _warningService;
        private readonly IDailyJobService _dailyJobService;
        private readonly IEventService _eventService;


        public WaterAndWattController(IConfiguration configuration, ISystemSettingService systemSettingService, XHSRealnameToken jwtToken, IOperateLogService operateLogService, SiteCityFuningToken cityToken, IAqtUploadService aqtUploadService, IWaterMeterService waterMeterService, IElecMeterService elecMeterService, IWaterService waterService, IHpSystemSetting hpSystemSetting, SiteCityXuweiToken dayunToken, ISiteService siteService, SiteCityWuzhongToken cityWuzhongToken, IYsApiService ysApiService, IVideoService videoService, IBaseRepository<BaseEntity> baseServices, IHkOpenApiService hkOpenApiService, ICameraService cameraService, IEventService eventService, IDeviceCNService deviceCNService, ICityAqiService cityAqiService, IFogJobService fogJobService, IWarningService warningService, IDailyJobService dailyJobService)
        {
            _dailyJobService = dailyJobService;

            _ieventService = eventService;
            _systemSettingService = systemSettingService;
            _waterMeterService = waterMeterService;
            _waterService = waterService;
            _elecMeterService = elecMeterService;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
            _jwtToken = jwtToken;
            _xuweiToken = dayunToken;
            _hpSystemSetting = hpSystemSetting;
            _siteService = siteService;
            _configuration = configuration;
            _ysApiService = ysApiService;
            _videoService = videoService;
            _baseServices = baseServices;
            _hkOpenApiService = hkOpenApiService;
            _cameraService = cameraService;
            _deviceCNService = deviceCNService;
            _cityAqiService = cityAqiService;
            _fogJobService = fogJobService;
            _warningService = warningService;

        }

        [HttpPost]
        [Route("wuxifengxiafnenxi")]
        public async Task<IResponseOutput> wuxifengxiafnenxi()
        {
            string APPID = "sysfdas2fvdasf33dag";
            string url = _configuration.GetSection("WXDY").GetValue<string>("PushUrl");
            JObject tokenjob = new JObject();
            string ACCESS_TOKEN = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken");
            if (!string.IsNullOrEmpty(ACCESS_TOKEN))
            {
                tokenjob = JObject.Parse(ACCESS_TOKEN);
            }
            string token = tokenjob.GetValue("data").ToString();
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            string realapi = string.Empty;
            int successcount = 0;
            int errcount = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            SortedDictionary<string, object> jparam = new SortedDictionary<string, object>();
            StringBuilder sb = new StringBuilder(string.Empty);
            Dictionary<string, object> asciiDic = new Dictionary<string, object>();
            JObject jo;
            var uploadtime = await _baseServices.Db.Queryable<GCCityUploadDateEntity>().Where(it => it.uploadurl == url && it.post == "rest/AsEvent").Select(it => it.uploadtime).FirstAsync();
            //推送数据获取
            DataTable ds = await _ieventService.UpEvent(uploadtime.ToString());
            DataTable dt = ds;
            if (dt.Rows.Count > 0)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        DataRow dr = dt.Rows[j];
                        JObject jso = new JObject();
                        foreach (DataColumn column in dr.Table.Columns)
                        {
                            if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                            }
                            else if (column.DataType.Equals(System.Type.GetType("System.Decimal")))
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToDouble());
                            }
                            else if (column.DataType.Equals(System.Type.GetType("System.DateTime")))
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToDateTime());
                            }
                            else
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                            }
                        }
                        string api = jso.GetValue("post").ToString();
                        realapi = api;

                        if (api.Contains("AsEvent"))    //10.2	提交风险分析事件
                        {
                            realapi = "rest/AsEvent/save";
                            jso.Remove("SPID");
                        }
                        jso.Remove("post");
                        jso.Remove("uploadurl");
                        jso.Remove("uploadaccount");
                        jso.Remove("uploadpwd");
                        jso.Remove("funingurl");
                        jso.Remove("siteuploadurl");
                        jparam = new SortedDictionary<string, object>();
                        sb = new StringBuilder(string.Empty);
                        foreach (JProperty jProperty in jso.Properties())
                        {
                            jparam.Add(jProperty.Name, jProperty.Value);
                        }
                        asciiDic = new Dictionary<string, object>();
                        string[] arrKeys = jparam.Keys.ToArray();
                        Array.Sort(arrKeys, string.CompareOrdinal);
                        foreach (var key in arrKeys)
                        {
                            var value = jparam[key];
                            asciiDic.Add(key, value);
                        }
                        foreach (var item in asciiDic)
                        {
                            if (item.Key == "checkLists")
                            {
                                continue;
                            }
                            sb.Append(item.Value);
                        }
                        string sbdata = sb.ToString();
                        sb.Append(token);
                        string sign = UEncrypter.SHA256(sb.ToString());
                        string rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), realapi, APPID, sign);
                        string result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                        if (!string.IsNullOrEmpty(result))
                        {
                            jo = JObject.Parse(result);

                            if (Convert.ToString(jo["flag"]) == "0006" || Convert.ToString(jo["flag"]) == "0007" || Convert.ToString(jo["flag"]) == "0010")
                            {
                                ACCESS_TOKEN = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken?isForce=true");
                                if (!string.IsNullOrEmpty(ACCESS_TOKEN))
                                {
                                    tokenjob = JObject.Parse(ACCESS_TOKEN);
                                }
                                token = tokenjob.GetValue("data").ToString();
                                sbdata = sbdata + token;
                                sign = UEncrypter.SHA256(sbdata.ToString());
                                rtdUrl = string.Format("http://{0}/{1}/{2}/{3}", url, realapi, APPID, sign);

                                result = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                            }
                        }


                        var LogEntity = new CityUploadOperateLog
                        {
                            //Id=Guid.NewGuid().ToString(),
                            url = url,
                            api = realapi,
                            account = APPID,
                            param = jso.ToString(),
                            result = result,
                            createtime = DateTime.Now
                        };
                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                        if (string.IsNullOrEmpty(result))
                        {
                            errcount += errcount;
                        }
                        else
                        {
                            JObject mJObj = JObject.Parse(result);
                            string flag = mJObj.GetValue("flag").ToString();
                            if (flag == "0000")
                            {
                                if (!list.Contains(url + api))
                                {
                                    await _aqtUploadService.UpdateCityUploadDate(url, api, jso.GetValue("createdate").ToDateTime());
                                }
                                successcount += successcount;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // _logger.LogError(realapi + ":" + ex.Message);
                    }
                }
            }
            return ResponseOutput.Ok();
        }
        /// <summary>
        /// 清江浦区
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("qjpq")]
        public async Task<IResponseOutput> qjpq()
        {

            var LogEntity = new CityUploadOperateLog();
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //账号
            string aqtaccount = _hpSystemSetting.getSettingValue(Const.Setting.S178);
            //密码
            string aqtpassword = _hpSystemSetting.getSettingValue(Const.Setting.S179);
            if (string.IsNullOrEmpty(aqtaccount))
            {
                //  _logger.LogInformation("数据上传结束。未设置上传账号。", true);
                // return;
            }
            DataSet ds = await _aqtUploadService.GetListById();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {

                            DataRow dr = dt.Rows[j];
                            JObject jso = new JObject();
                            foreach (DataColumn column in dr.Table.Columns)
                            {
                                if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                                }
                                else if (column.DataType.Equals(System.Type.GetType("System.Decimal")))
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToDouble());
                                }
                                else if (column.DataType.Equals(System.Type.GetType("System.DateTime")))
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToDateTime());
                                }
                                else
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                                }
                            }
                            string url = jso.GetValue("uploadurl").ToString();
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            string api = jso.GetValue("post").ToString();
                            try
                            {
                                if (api.Contains("DustInfoBoard"))
                                {
                                    //扬尘信息看板参数替换
                                    string boardurl = jso.GetValue("uploadBoardUrl").ToString();
                                    jso.Remove("uploadBoardUrl");
                                    jso.Add("dustBoardUrl", boardurl);
                                }
                                else if (api.Contains("SmartSupervisionBoard"))
                                {
                                    //智慧监管整体看板
                                    string cityurl = _hpSystemSetting.getSettingValue(Const.Setting.S187);
                                    string username = _hpSystemSetting.getSettingValue(Const.Setting.S188);
                                    //_logger.LogError(api + "智慧监管整体看板url:" + cityurl);
                                    if (!string.IsNullOrEmpty(cityurl))
                                    {
                                        string nowtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        string data = HttpUtility.UrlEncode(UEncrypter.EncryptByRSA(username + ";" + nowtime, Const.Encryp.PUBLIC_KEY_OTHER));
                                        string boardurl = cityurl + "/#/SingleSignOn?type=0&data=" + data;
                                        jso.Remove("uploadBoardUrl");
                                        jso.Add("uploadBoardUrl", boardurl);
                                    }
                                }
                                else if (api.Contains("UploadSpecialOperationPersonnel"))
                                {
                                    if (jso.GetValue("workTypeCode").ToInt() >= 193)
                                    {
                                        jso["workTypeCode"] = 193;
                                    }
                                }
                                else if (api.Contains("/Board/insertBoard"))
                                {
                                    JObject datajob = new JObject();
                                    datajob.Add("appKey", "muj1F1568714076oby3342714HlZSe");
                                    datajob.Add("secret", "hd1568714076NVeejUu26775018gXudvbDzWF");
                                    jso.Remove("uploadurl");
                                    jso.Remove("uploadaccount");
                                    jso.Remove("uploadpwd");
                                    jso.Remove("siteuploadurl");
                                    jso.Remove("appKey");
                                    jso.Remove("secret");
                                    jso.Remove("post");
                                    datajob.Add("boardVo", jso);
                                    string retString = HttpNetRequest.POSTSendJsonRequest("http://124.70.9.139:8001/Board/insertBoard", JsonConvert.SerializeObject(datajob), new Dictionary<string, string>() { });
                                    LogEntity = new CityUploadOperateLog
                                    {
                                        //Id=Guid.NewGuid().ToString(),
                                        url = url,
                                        api = api,
                                        account = account,
                                        param = jso.ToString(),
                                        result = retString,
                                        createtime = DateTime.Now
                                    };
                                }
                                jso.Remove("post");
                                jso.Remove("uploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");

                                string result = string.Empty; //_cityToken.JsonRequest(api, JsonConvert.SerializeObject(jso), aqtaccount, aqtpassword);

                                LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = url,
                                    api = api,
                                    account = account,
                                    param = jso.ToString(),
                                    result = result,
                                    createtime = DateTime.Now
                                };
                                await _operateLogService.AddCityUploadApiLog(LogEntity);
                                if (string.IsNullOrEmpty(result))
                                {
                                    errcount += errcount;
                                }
                                else
                                {
                                    JObject Obj = JObject.Parse(result);
                                    int code = (int)Obj.GetValue("code");
                                    if (code == 0)
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                            list.Add(url + api);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                // _logger.LogError(api + ":" + ex.Message);
                                // return;
                            }
                            catch (Exception ex)
                            {
                                //     _logger.LogError(api + ":" + ex.Message);
                            }
                        }
                    }
                }
            }

            return ResponseOutput.Ok();
        }


        [HttpPost]
        [Route("yizhengjob")]
        public async Task<IResponseOutput> yizhengjob()
        {
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int successcount = 0;
            int errcount = 0;
            var LogEntity = new CityUploadOperateLog();
            string appKey = "lZA7s1570501854Hek1228839267xE6";
            string secret = "Bo1573035060QaYficN1283257";
            string uploaurl = string.Empty;
            string url = "http://49.4.11.116:8085/api/";
            string urlj = "http://49.4.68.132:8094/api/";
            string bordurl = "http://124.70.9.139:8001/Board/insertBoard";
            #region
            //实名制url
            //string realnameurl = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            //if (!string.IsNullOrEmpty(realnameurl))
            //{
            //    //对接数据获取
            //    DataTable siteajcodesdt = await _aqtUploadService.GetGroupSiteajcodes();
            //    if (siteajcodesdt.Rows.Count > 0)
            //    {
            //        for (int j = 0; j < siteajcodesdt.Rows.Count; j++)
            //        {
            //            DataRow dr = siteajcodesdt.Rows[j];
            //            JObject jso = new JObject();
            //            foreach (DataColumn column in dr.Table.Columns)
            //            {
            //                if (column.DataType.Equals(System.Type.GetType("System.Int32")))
            //                {
            //                    jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
            //                }
            //                else
            //                {
            //                    jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
            //                }
            //            }
            //            string uploadurl = jso.GetValue("uploadurl").ToString();
            //            string uploadaccount = jso.GetValue("uploadaccount").ToString();
            //            string uploadpwd = jso.GetValue("uploadpwd").ToString();
            //            string attenduserpsd = jso.GetValue("attenduserpsd").ToString();
            //            string recordNumber = jso.GetValue("siteajcodes").ToString();
            //            string belongto = jso.GetValue("belongto").ToString();
            //            if (string.IsNullOrEmpty(attenduserpsd))
            //            {
            //                break;
            //            }
            //            string account = attenduserpsd.Split("||".ToCharArray())[0];
            //            string pwd = attenduserpsd.Split("||".ToCharArray())[2];
            //            JObject jsoparam = new JObject();
            //            jsoparam.Add("recordNumber", recordNumber);
            //            //获取指定项目的人员基础信息
            //            string uploadapi = "Personal/PeopleInOutProInfo";
            //            string api = "construction-site-api/ordinary-employee-info";
            //            if (jsoparam.ContainsKey("currentDate"))
            //            {
            //                jsoparam.Remove("currentDate");
            //            }
            //            if (dic.ContainsKey(uploadurl + uploadapi))
            //            {
            //                string currentDate = "";
            //                dic.TryGetValue(uploadurl + uploadapi, out currentDate);
            //                jsoparam.Add("currentDate", currentDate);
            //            }
            //            else
            //            {
            //                DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
            //                if (citydt.Rows.Count > 0)
            //                {
            //                    DataRow citydr = citydt.Rows[0];
            //                    string currentDate = citydr["uploadtime"].ToString();
            //                    jsoparam.Add("currentDate", currentDate);
            //                    dic.Add(uploadurl, currentDate);
            //                }
            //            }
            //            string result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
            //            if (!string.IsNullOrEmpty(""))
            //            {
            //                try
            //                {
            //                    JObject job = JObject.Parse(result);
            //                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
            //                    {
            //                        JArray mJArray = new JArray();
            //                        mJArray = (JArray)job.GetValue("data");
            //                        for (int k = 0; k < mJArray.Count; k++)
            //                        {
            //                            JObject datajob = JObject.Parse(mJArray[k].ToString());
            //                            datajob.Add("importOrExitFlag", datajob.GetValue("isResign").ToInt());
            //                            datajob.Add("personName", datajob.GetValue("workerName"));
            //                            datajob.Add("idcard", datajob.GetValue("idNumber"));
            //                            datajob.Add("typeWork", datajob.GetValue("workType").ToInt());
            //                            datajob.Add("importTime", datajob.GetValue("entryDate"));
            //                            datajob.Add("exitTime", datajob.GetValue("exitDate"));
            //                            datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode"));
            //                            datajob.Add("userinfoid", datajob.GetValue("idNumber"));
            //                            datajob.Add("job", datajob.GetValue("securityJob"));
            //                            datajob.Remove("unifiedSocialCreditCode");
            //                            datajob.Remove("workerName");
            //                            datajob.Remove("idNumber");
            //                            datajob.Remove("workType");
            //                            datajob.Remove("entryDate");
            //                            datajob.Remove("exitDate");
            //                            datajob.Remove("isResign");
            //                            datajob.Remove("securityJob");
            //                            string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
            //                            LogEntity = new CityUploadOperateLog
            //                            {
            //                                url = uploadurl,
            //                                api = uploadapi,
            //                                account = uploadaccount,
            //                                param = mJArray[k].ToString(),
            //                                result = uploadresult,
            //                                createtime = DateTime.Now
            //                            };
            //                            await _operateLogService.AddCityUploadApiLog(LogEntity);
            //                            if (string.IsNullOrEmpty(uploadresult))
            //                            {
            //                                errcount += errcount;
            //                            }
            //                            else
            //                            {
            //                                if (!list.Contains(uploadurl + uploadapi))
            //                                {
            //                                    await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
            //                                    list.Add(uploadurl + uploadapi);
            //                                }
            //                                successcount += successcount;
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (HttpRequestException ex)
            //                {
            //                    // _logger.LogError(uploadapi + ":" + ex.Message);
            //                    // return;
            //                }
            //                catch (Exception ex)
            //                {
            //                    //_logger.LogError(uploadapi + ":" + ex.Message);
            //                }
            //            }
            //            //10.2、人员实时考勤数据上传
            //            uploadapi = "Personal/ManagerInOutSite";
            //            api = "construction-site-api/manager-in-out-site";
            //            if (jsoparam.ContainsKey("currentDate"))
            //            {
            //                jsoparam.Remove("currentDate");
            //            }
            //            if (dic.ContainsKey(uploadurl + uploadapi))
            //            {
            //                string currentDate = "";
            //                dic.TryGetValue(uploadurl + uploadapi, out currentDate);
            //                jsoparam.Add("currentDate", currentDate);
            //            }
            //            else
            //            {
            //                DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
            //                if (citydt.Rows.Count > 0)
            //                {
            //                    DataRow citydr = citydt.Rows[0];
            //                    string currentDate = citydr["uploadtime"].ToString();
            //                    jsoparam.Add("currentDate", currentDate);
            //                    dic.Add(uploadurl, currentDate);
            //                }
            //            }
            //            result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
            //            if (!string.IsNullOrEmpty(""))
            //            {
            //                try
            //                {
            //                    JObject job = JObject.Parse(result);
            //                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
            //                    {
            //                        JArray mJArray = new JArray();
            //                        mJArray = (JArray)job.GetValue("data");
            //                        for (int k = 0; k < mJArray.Count; k++)
            //                        {
            //                            JObject datajob = JObject.Parse(mJArray[k].ToString());
            //                            datajob.Add("userinfoid", datajob.GetValue("idcard"));
            //                            datajob.Add("belongedTo", belongto);
            //                            string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
            //                            LogEntity = new CityUploadOperateLog
            //                            {
            //                                url = uploadurl,
            //                                api = uploadapi,
            //                                account = uploadaccount,
            //                                param = mJArray[k].ToString(),
            //                                result = uploadresult,
            //                                createtime = DateTime.Now
            //                            };
            //                            await _operateLogService.AddCityUploadApiLog(LogEntity);
            //                            if (string.IsNullOrEmpty(uploadresult))
            //                            {
            //                                errcount += errcount;
            //                            }
            //                            else
            //                            {
            //                                if (!list.Contains(uploadurl + uploadapi))
            //                                {
            //                                    await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
            //                                    list.Add(uploadurl + uploadapi);
            //                                }
            //                                successcount += successcount;
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (HttpRequestException ex)
            //                {
            //                    // _logger.LogError(uploadapi + ":" + ex.Message);
            //                    // return;
            //                }
            //                catch (Exception ex)
            //                {
            //                    //_logger.LogError(uploadapi + ":" + ex.Message);
            //                }
            //            }
            //            //10.3、教育信息上传
            //            uploadapi = "Personal/PeopleSafeEduInfo";
            //            api = "construction-site-api/employee-education-info";
            //            if (jsoparam.ContainsKey("currentDate"))
            //            {
            //                jsoparam.Remove("currentDate");
            //            }
            //            if (dic.ContainsKey(uploadurl + uploadapi))
            //            {
            //                string currentDate = "";
            //                dic.TryGetValue(uploadurl + uploadapi, out currentDate);
            //                jsoparam.Add("currentDate", currentDate);
            //            }
            //            else
            //            {
            //                DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
            //                if (citydt.Rows.Count > 0)
            //                {
            //                    DataRow citydr = citydt.Rows[0];
            //                    string currentDate = citydr["uploadtime"].ToString();
            //                    jsoparam.Add("currentDate", currentDate);
            //                    dic.Add(uploadurl, currentDate);
            //                }
            //            }
            //            result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
            //            if (!string.IsNullOrEmpty(""))
            //            {
            //                try
            //                {
            //                    JObject job = JObject.Parse(result);
            //                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
            //                    {
            //                        JArray mJArray = new JArray();
            //                        mJArray = (JArray)job.GetValue("data");
            //                        for (int k = 0; k < mJArray.Count; k++)
            //                        {
            //                            JObject datajob = JObject.Parse(mJArray[k].ToString());
            //                            datajob.Add("idcard", datajob.GetValue("workerIdNumber"));
            //                            datajob.Add("typeWork", datajob.GetValue("workType").ToInt());
            //                            datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode"));
            //                            datajob.Add("educatType", datajob.GetValue("educationType"));
            //                            datajob.Add("educatContent", datajob.GetValue("educationContent"));
            //                            datajob.Add("educatDate", datajob.GetValue("educationDate"));
            //                            datajob.Add("educatAddress", "本工地" /*datajob.GetValue("educationLocation")*/);
            //                            datajob.Add("educationTime", datajob.GetValue("educationDuration"));
            //                            datajob.Add("userinfoid", datajob.GetValue("workerIdNumber"));
            //                            string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
            //                            LogEntity = new CityUploadOperateLog
            //                            {
            //                                url = uploadurl,
            //                                api = uploadapi,
            //                                account = uploadaccount,
            //                                param = mJArray[k].ToString(),
            //                                result = uploadresult,
            //                                createtime = DateTime.Now
            //                            };
            //                            await _operateLogService.AddCityUploadApiLog(LogEntity);
            //                            if (string.IsNullOrEmpty(uploadresult))
            //                            {
            //                                errcount += errcount;
            //                            }
            //                            else
            //                            {
            //                                if (!list.Contains(uploadurl + uploadapi))
            //                                {
            //                                    await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
            //                                    list.Add(uploadurl + uploadapi);
            //                                }
            //                                successcount += successcount;
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (HttpRequestException ex)
            //                {
            //                    // _logger.LogError(uploadapi + ":" + ex.Message);
            //                    // return;
            //                }
            //                catch (Exception ex)
            //                {
            //                    //_logger.LogError(uploadapi + ":" + ex.Message);
            //                }
            //            }
            //            //10.4、奖惩信息上传
            //            uploadapi = "Personal/PeopleRewardPunishInfo";
            //            api = "construction-site-api/employee-reward-punishments-info";
            //            if (jsoparam.ContainsKey("currentDate"))
            //            {
            //                jsoparam.Remove("currentDate");
            //            }
            //            if (dic.ContainsKey(uploadurl + uploadapi))
            //            {
            //                string currentDate = "";
            //                dic.TryGetValue(uploadurl + uploadapi, out currentDate);
            //                jsoparam.Add("currentDate", currentDate);
            //            }
            //            else
            //            {
            //                DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(uploadurl, uploadapi);
            //                if (citydt.Rows.Count > 0)
            //                {
            //                    DataRow citydr = citydt.Rows[0];
            //                    string currentDate = citydr["uploadtime"].ToString();
            //                    jsoparam.Add("currentDate", currentDate);
            //                    dic.Add(uploadurl, currentDate);
            //                }
            //            }
            //            result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
            //            if (!string.IsNullOrEmpty(result))
            //            {
            //                try
            //                {
            //                    JObject job = JObject.Parse(result);
            //                    if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
            //                    {
            //                        JArray mJArray = new JArray();
            //                        mJArray = (JArray)job.GetValue("data");
            //                        for (int k = 0; k < mJArray.Count; k++)
            //                        {
            //                            JObject datajob = JObject.Parse(mJArray[k].ToString());
            //                            datajob.Add("idcard", datajob.GetValue("workerIdNumber"));
            //                            datajob.Add("typeWork", datajob.GetValue("workType").ToInt());
            //                            datajob.Add("unitCode", datajob.GetValue("unifiedSocialCreditCode"));
            //                            datajob.Add("rewardType", datajob.GetValue("eventType"));
            //                            datajob.Add("rewardContent", datajob.GetValue("eventContent"));
            //                            datajob.Add("rewardDate", datajob.GetValue("eventDate"));
            //                            datajob.Add("userinfoid", datajob.GetValue("workerIdNumber"));
            //                            string uploadresult = UHttp.Post(urlj + uploadapi, JsonConvert.SerializeObject(datajob), UHttp.CONTENT_TYPE_JSON);
            //                            LogEntity = new CityUploadOperateLog
            //                            {
            //                                url = uploadurl,
            //                                api = uploadapi,
            //                                account = uploadaccount,
            //                                param = mJArray[k].ToString(),
            //                                result = uploadresult,
            //                                createtime = DateTime.Now
            //                            };
            //                            await _operateLogService.AddCityUploadApiLog(LogEntity);
            //                            if (string.IsNullOrEmpty(uploadresult))
            //                            {
            //                                errcount += errcount;
            //                            }
            //                            else
            //                            {
            //                                if (!list.Contains(uploadurl + uploadapi))
            //                                {
            //                                    await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
            //                                    list.Add(uploadurl + uploadapi);
            //                                }
            //                                successcount += successcount;
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (HttpRequestException ex)
            //                {
            //                    // _logger.LogError(uploadapi + ":" + ex.Message);
            //                    // return;
            //                }
            //                catch (Exception ex)
            //                {
            //                    //_logger.LogError(uploadapi + ":" + ex.Message);
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForYiZheng();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];
                            JObject jso = new JObject();
                            foreach (DataColumn column in dr.Table.Columns)
                            {
                                if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                                }
                                else if (column.DataType.Equals(System.Type.GetType("System.Decimal")))
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToDouble());
                                }
                                else if (column.DataType.Equals(System.Type.GetType("System.DateTime")))
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToDateTime());
                                }
                                else
                                {
                                    jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                                }
                            }
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            string api = jso.GetValue("post").ToString();
                            string realapi = api;
                            if (api.Contains("UploadCraneHistory"))      //卸料平台实时数据上传
                            {
                                realapi = "Craneinterface/UploadCraneHistory";
                                JObject job = JObject.Parse(jso.GetValue("sedata").ToString());
                                jso.Add("load", job.GetValue("Weight").ToDouble());
                                jso.Add("range", job.GetValue("Margin").ToDouble());
                                jso.Add("moment", job.GetValue("MomentPercent").ToDouble());
                                jso.Add("rotation", job.GetValue("Rotation").ToDouble());
                                jso.Add("height", job.GetValue("Height").ToDouble());
                                jso.Add("windSpeed", job.GetValue("WindSpeed").ToDouble());
                                //  jso.Add("dip", job.GetValue("").ToDouble());
                                // jso.Add("multiplyingPower", job.GetValue("").ToDouble());
                                jso.Remove("sedata");
                                jso.Remove("paramjson");
                            }//塔机实时
                            else if (api.Contains("CraneAlarmOn"))
                            {
                                realapi = "Craneinterface/CraneAlarmInfo";
                            }//塔吊报警
                            else if (api.Contains("HoistHistory"))
                            {
                                realapi = "Hoistinterface/HoistHistory";
                                JObject job = JObject.Parse(jso.GetValue("sedata").ToString());
                                jso.Add("load", job.GetValue("Weight").ToString());
                                jso.Add("avgSpeed", job.GetValue("Speed").ToDouble());
                                jso.Add("numberOfPeopleLoaded", jso.GetValue("NumOfPeople").ToInt());
                                jso.Remove("sedata");

                            }//升降机
                            else if (api.Contains("UploadHistory"))
                            {
                                realapi = "Uploadinterface/UploadHistory";
                                JObject job = JObject.Parse(jso.GetValue("rtdjson").ToString());
                                jso.Add("powerPercent", job.GetValue("electric_quantity").ToDouble());
                                jso.Add("weight", job.GetValue("weight").ToDouble());
                                jso.Add("weightBias", job.GetValue("bias").ToDouble());
                                jso.Add("warningWeight", job.GetValue("early_warning_weight").ToDouble());
                                jso.Add("AlarmWeight", job.GetValue("alarm_weight").ToDouble());
                                jso.Add("dataType", job.GetValue("upstate").ToInt());
                                jso.Remove("rtdjson");
                            }//卸料平台
                            else if (api.Contains("UploadDustInfo"))
                            {
                                realapi = "DustInterface/UploadDustHistory";
                            }//扬尘实时
                            else if (api.Contains("FenceAlarmOn"))
                            {
                                realapi = "Fenceinterface/FenceAlarmInfo";
                                jso.Add("warnNumber", jso.GetValue("deviceId"));
                                jso.Add("defectPosition", jso.GetValue("description"));
                                jso.Add("defectDate", jso.GetValue("time"));
                            }//临边防护缺失记录
                            else if (api.Contains("FenceAlarmOff"))
                            {
                                realapi = "Fenceinterface/FenceAlarmInfo";
                                jso.Add("warnNumber", jso.GetValue("deviceId"));
                                jso.Add("defectPosition", jso.GetValue("description"));
                                jso.Add("recoveryDate", jso.GetValue("time"));
                            }//临边防护恢复记录
                            else if (api.Contains("InspectContentInfo"))//检查单数据上传
                            {
                                realapi = "Check/InspectContentInfo";
                                if (jso.ContainsKey("checkContent"))
                                {
                                    string rectifyPerson = "";
                                    JArray jar = new JArray();
                                    JObject job = new JObject();
                                    JArray jarrayObj = new JArray();
                                    string checkContent = jso.GetValue("checkContent").ToString();
                                    JArray arr = JsonConvert.DeserializeObject<JArray>(checkContent);
                                    if (jso.ContainsKey("urls"))
                                    {
                                        string urls = jso.GetValue("urls").ToString();
                                        if (string.IsNullOrEmpty(urls) || urls == "[]")
                                        {
                                            jarrayObj.Add("无");
                                        }
                                        else
                                        {
                                            string[] urlarray = urls.Split(",".ToCharArray());
                                            for (int k = 0; k < urlarray.Length; k++)
                                            {
                                                jarrayObj.Add(urlarray[k]);
                                            }
                                        }
                                    }
                                    if (jso.ContainsKey("rectifyPerson"))
                                    {
                                        rectifyPerson = jso.GetValue("rectifyPerson").ToString();
                                        if (string.IsNullOrEmpty(rectifyPerson) || rectifyPerson == "[]")
                                        {
                                            rectifyPerson = "无";
                                        }
                                    }
                                    if (string.IsNullOrEmpty(checkContent) || checkContent == "[]")
                                    {
                                        job.Add("itemId", 1);
                                        job.Add("checkContent", "无");
                                        job.Add("rectifyPerson", rectifyPerson);
                                        job.Add("urls", jarrayObj);
                                        jar.Add(job);
                                    }
                                    else
                                    {
                                        for (int b = 0; b < arr.Count; b++)
                                        {
                                            job = new JObject();
                                            job.Add("itemId", b + 1);
                                            job.Add("rectifyPerson", rectifyPerson);
                                            var res = arr[b];
                                            job.Add("checkContent", res.Last.ToString());
                                            job.Add("urls", jarrayObj);
                                            jar.Add(job);
                                        }
                                    }
                                    jso.Add("checkLists", jar);
                                    jso.Add("idcard", jso.GetValue("idCard").ToString());
                                }
                                jso.Remove("idCard");
                                jso.Remove("checkContent");
                                jso.Remove("urls");
                            }//检查单数据上传
                            else if (api.Equals("Check/InspectionPoint"))
                            {
                                realapi = "Check/InspectionPoint";
                            }//巡检点
                            else if (api.Contains("RectifyContentInfo"))
                            {
                                realapi = "Check/RectifyContentInfo";
                                jso["checkNumber"] = "IS202107240916005345";

                                JArray jarrayObj = new JArray();
                                JObject job = new JObject();
                                JArray jar = new JArray();
                                if (jso.ContainsKey("urls"))
                                {
                                    string urls = jso.GetValue("urls").ToString();
                                    if (string.IsNullOrEmpty(urls) || urls == "[]")
                                    {
                                        jarrayObj.Add("无");
                                    }
                                    else
                                    {
                                        string[] urlarray = urls.Split(",".ToCharArray());
                                        for (int k = 0; k < urlarray.Length; k++)
                                        {
                                            jarrayObj.Add(urlarray[k]);
                                        }
                                    }
                                    if (jso.ContainsKey("rectifyRemark"))
                                    {
                                        string rectifyRemark = jso.GetValue("rectifyRemark").ToString();
                                        job.Add("itemId", 1);
                                        job.Add("rectifyRemark", rectifyRemark);
                                        job.Add("urls", jarrayObj);
                                        jar.Add(job);
                                    }
                                }
                                jso.Remove("rectifyContents");
                                jso.Add("rectifyContents", jar);
                            }//检查单整改完成数据上传
                            else if (api.Contains("InspectionPointContent"))
                            {
                                realapi = "Check/InspectionPointContent";
                                if (jso.ContainsKey("urls"))
                                {
                                    JArray jarrayObj = new JArray();
                                    string urls = jso.GetValue("urls").ToString();
                                    if (string.IsNullOrEmpty(urls) || urls == "[]")
                                    {
                                        jarrayObj.Add("无");
                                    }
                                    else
                                    {
                                        string[] urlarray = urls.Replace("[", "").Replace("]", "").Split(",".ToCharArray());
                                        for (int k = 0; k < urlarray.Length; k++)
                                        {
                                            jarrayObj.Add(urlarray[k]);
                                        }
                                    }
                                    jso.Remove("urls");
                                    jso.Add("urls", jarrayObj);
                                }
                            }//巡检内容数据上传
                            else if (api.Contains("FreeToShoot"))
                            {
                                realapi = "Check/FreeToShoot";
                                string urls = jso.GetValue("url").ToString();

                                JObject job = new JObject();
                                JArray jar = new JArray();
                                foreach (var item in urls.Split(';'))
                                {
                                    job = new JObject();
                                    job.Add("type", item.Split(',')[1]);
                                    job.Add("url", item.Split(',')[0]);
                                    jar.Add(job);
                                }
                                jso.Add("urls", jar);
                            }//随手拍数据上传
                            else if (api.Contains("HighFormworkDeviceInfo"))
                            {
                                realapi = "HighFormwork/HighFormworkHistory";
                            }//高支模数据上传
                            else if (api.Contains("UploadVideo"))
                            {
                                realapi = "Video/UploadVideos";
                            }//视频地址上传接口
                            else if (api.Contains("SmartSupervisionBoard"))
                            {
                                realapi = "Board/insertBoard";
                                jso.Add("url", jso.GetValue("uploadBoardUrl"));
                                jso.Remove("uploadBoardUrl");
                                var data = jso;
                                jso = new JObject();
                                jso.Add("boardVo", data);
                                jso.Add("appKey", appKey);
                                jso.Add("secret", secret);
                            }//数据看板上传
                            jso.Remove("post");
                            jso.Remove("siteuploadurl");
                            jso.Remove("uploadaccount");
                            jso.Remove("uploadpwd");
                            if (jso.ContainsKey("funingurl"))
                            {
                                jso.Remove("funingurl");
                            }
                            if (jso.ContainsKey("GONGCHENG_CODE"))
                            {
                                jso.Remove("GONGCHENG_CODE");
                            }
                            try
                            {
                                if (api.Contains("InspectContentInfo") || api.Contains("InspectionPoint") || api.Contains("RectifyContentInfo") || api.Contains("InspectionPointContent") || api.Contains("FreeToShoot") || api.Contains("UploadVideo"))
                                {
                                    uploaurl = urlj + realapi + "?appkey=" + appKey + "&secret=" + secret;
                                }
                                else if (api.Contains("SmartSupervisionBoard"))
                                {
                                    uploaurl = bordurl;
                                }
                                else
                                {
                                    uploaurl = url + realapi + "?appkey=" + appKey + "&secret=" + secret;
                                }
                                string result = UHttp.Post(uploaurl, JsonConvert.SerializeObject(jso), UHttp.CONTENT_TYPE_JSON);
                                LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = uploaurl,
                                    api = api,
                                    account = account,
                                    param = jso.ToString(),
                                    result = result,
                                    createtime = DateTime.Now
                                };
                                await _operateLogService.AddCityUploadApiLog(LogEntity);
                                if (string.IsNullOrEmpty(result))
                                {
                                    errcount += errcount;
                                }
                                else
                                {
                                    JObject mJObj = JObject.Parse(result);
                                    int code = (int)mJObj.GetValue("code");
                                    if (code == 200)
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                            list.Add(url + api);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                //_logger.LogError(api + ":" + ex.Message, true);
                            }
                        }
                    }
                }
            }

            return ResponseOutput.Ok();
        }

        [HttpPost]
        [Route("huarunce")]
        public async Task<IResponseOutput> huarunce()
        {

            string url = "https://scs.crland.com.cn/smart-api";
            string account = "hr1637146548748";
            string pwd = "7E437F55BEE72604592C4AAB0BB0F66D";
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string realapi = string.Empty;
            string method = string.Empty;
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListForHuarun();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count <= 0)
                    {
                        continue;
                    }
                    var dtdata = dt.Select("GONGCHENG_CODE='Huarun'");
                    if (dtdata.Length > 0)
                    {
                        for (int j = 0; j < dtdata.Length; j++)
                        {
                            try
                            {
                                DataRow dr = dtdata[j];
                                JObject jso = new JObject();
                                foreach (DataColumn column in dr.Table.Columns)
                                {
                                    if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.Decimal")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDouble());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.DateTime")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDateTime());
                                    }
                                    else
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                                    }
                                }

                                string api = jso.GetValue("post").ToString();
                                realapi = api;
                                if (api.Contains("UploadCraneHistory"))
                                {
                                    realapi = "/v1/tower-crane/saveRealtimeRecord";
                                    method = "tower-crane";
                                    jso.Add("hardwareId", jso.GetValue("deviceId"));
                                    jso.Add("datetime", jso.GetValue("moniterTime"));
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("sedata").ToString()))
                                        {
                                            JObject sedatajob = JObject.Parse(jso.GetValue("sedata").ToString());
                                            jso.Add("load", sedatajob.GetValue("Weight"));
                                            jso.Add("amplitude", sedatajob.GetValue("Margin"));
                                            jso.Add("torque", sedatajob.GetValue("Moment"));
                                            jso.Add("rotation", sedatajob.GetValue("Rotation"));
                                            jso.Add("height", sedatajob.GetValue("Height"));
                                            jso.Add("windScale", sedatajob.GetValue("Wind"));
                                            jso.Add("windSpeed", sedatajob.GetValue("WindSpeed"));
                                            jso.Add("dipAngle", 0);

                                            var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(sedatajob.GetValue("Alarm")));

                                            if (intList.Contains(512))
                                            {
                                                jso.Add("collisionAlarmStatus", true);
                                            }
                                            else
                                            {
                                                jso.Add("collisionAlarmStatus", false);
                                            }
                                        }
                                    }

                                    if (jso.ContainsKey("paramjson"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("paramjson").ToString()))
                                        {
                                            JObject paramjob = JObject.Parse(jso.GetValue("paramjson").ToString());
                                            jso.Add("ratedLoad", paramjob.GetValue("MaxWeight"));
                                            jso.Add("currentRatio", paramjob.GetValue("BeiLv"));
                                        }
                                    }
                                    jso.Remove("sedata");
                                    jso.Remove("paramjson");
                                }//塔吊实时
                                else if (api.Contains("CraneWorkData"))
                                {
                                    realapi = "/v1/tower-crane/saveCycleRecord";
                                    method = "tower-crane";
                                    jso.Add("datetime", jso.GetValue("updatedate").ToDateTime().ToString("yyyy-MM-dd"));
                                    if (jso.ContainsKey("workdata"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("workdata").ToString()))
                                        {
                                            JObject workjob = JObject.Parse(jso.GetValue("workdata").ToString());
                                            jso.Add("hardwareId", workjob.GetValue("SeCode"));
                                            jso.Add("startLoad", "");
                                            jso.Add("startRatio", "0");
                                            jso.Add("startAmplitude", workjob.GetValue("StartMargin"));
                                            jso.Add("startTorque", "0");
                                            jso.Add("startRotation", workjob.GetValue("StartRotation"));
                                            jso.Add("startHeight", workjob.GetValue("StartHeight"));
                                            jso.Add("startWindSpeed", "0");
                                            jso.Add("startDipAngle", "0");
                                            jso.Add("startWarningStatus", "");
                                            jso.Add("startAlarmStatus", "");
                                            jso.Add("startSensorStatus", "");
                                            jso.Add("startDatetime", Convert.ToDateTime(workjob.GetValue("StartTime")).ToString("yyyy-MM-dd HH:mm:ss"));
                                            jso.Add("endLoad", "0");
                                            jso.Add("endRatio", "0");
                                            jso.Add("endAmplitude", workjob.GetValue("EndMargin"));
                                            jso.Add("endTorque", "0");
                                            jso.Add("endRotation", workjob.GetValue("EndRotation"));
                                            jso.Add("endHeight", workjob.GetValue("EndHeight"));
                                            jso.Add("endWindSpeed", "0");
                                            jso.Add("endDipAngle", "0");
                                            jso.Add("endWarningStatus", "");
                                            jso.Add("endAlarmStatus", "");
                                            jso.Add("endSensorStatus", "");
                                            jso.Add("endDatetime", workjob.GetValue("EndTime").ToDateTime().ToString("yyyy-MM-dd HH:mm:ss"));
                                            jso.Add("maxTorque", workjob.GetValue("MaxMargin"));
                                        }
                                    }
                                    if (jso.ContainsKey("paramjson"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("paramjson").ToString()))
                                        {
                                            JObject paramjob = JObject.Parse(jso.GetValue("paramjson").ToString());
                                            jso.Add("ratio", paramjob.GetValue("BeiLv").ToString());
                                            jso.Add("maxTorqueLoad", "0");
                                            jso.Add("maxTorqueRatedLoad", "0");
                                            jso.Add("maxTorqueAmplitude", paramjob.GetValue("LiJvMaxMargin").ToString());
                                            jso.Add("maxWindSpeed", "0");

                                            var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(paramjob.GetValue("Alarm")));
                                            if (intList.Contains(256))
                                            {
                                                jso.Add("windSpeedAlarm", "1");
                                            }
                                            else
                                            {
                                                jso.Add("windSpeedAlarm", "0");
                                            }
                                        }
                                    }
                                    jso.Remove("recordNumber");
                                    jso.Remove("secode");
                                    jso.Remove("updatedate");
                                    jso.Remove("workdata");
                                }//塔吊循环记录数据
                                else if (api.Contains("HoistHistory"))
                                {
                                    realapi = "/v1/lift/saveRealtimeRecord";
                                    method = "lift";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        if (!string.IsNullOrEmpty(jso.GetValue("sedata").ToString()))
                                        {
                                            JObject sedatajob = JObject.Parse(jso.GetValue("sedata").ToString());
                                            jso.Add("hardwareId", sedatajob.GetValue("SeCode"));
                                            jso.Add("datetime", sedatajob.GetValue("DeviceTime").ToDateTime().ToString("yyyy-MM-dd HH:mm:dd"));
                                            jso.Add("load", (sedatajob.GetValue("Weight").ToDouble() * 1000).ToString());
                                            jso.Add("dipAngle", "");
                                            jso.Add("height", sedatajob.GetValue("Height").ToString());
                                            jso.Add("windScale", "");
                                            jso.Add("windSpeed", "");
                                        }
                                    }
                                    jso.Remove("sedata");
                                    jso.Remove("deviceId");
                                    jso.Remove("model");
                                    jso.Remove("name");
                                    jso.Remove("moniterTime");
                                    jso.Remove("projectInfoId");
                                    jso.Remove("recordNumber");
                                }//升降机实时
                                else if (api.Contains("UploadDustInfo"))
                                {
                                    realapi = "/v1/environment/saveRealtimeRecord";
                                    method = "environment";
                                    jso.Add("airHumidity", jso.GetValue("humidity"));
                                    jso.Add("airTemperature", jso.GetValue("temperature"));
                                    jso.Add("pm25", jso.GetValue("pm2dot5"));
                                }//扬尘实时
                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                if (jso.ContainsKey("funingurl"))
                                {
                                    jso.Remove("funingurl");
                                }
                                if (jso.ContainsKey("GONGCHENG_CODE"))
                                {
                                    jso.Remove("GONGCHENG_CODE");
                                }
                                if (jso.ContainsKey("rtdjson"))
                                {
                                    jso.Remove("rtdjson");
                                }
                                if (jso.ContainsKey("paramjson"))
                                {
                                    jso.Remove("paramjson");
                                }

                                long timestamp = DateTimeExtensions.ToTimestamp(DateTime.Now, true);
                                string signStr = pwd + "appId=" + account + "&version=" + "v1" + "&method=" + method + "&timestamp=" + timestamp + "&data=" + JsonConvert.SerializeObject(jso) + pwd;
                                string sign = UEncrypter.EncryptBySHA1(signStr).ToString().Replace("-", "").ToLower();
                                string URL = url + realapi + "?appId=" + account + "&version=" + "v1" + "&method=" + method + "&timestamp=" + timestamp + "&sign=" + sign;

                                string result = HttpNetRequest.POSTSendJsonRequest(URL, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });


                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = URL,
                                    api = api,
                                    account = account,
                                    param = jso.ToString(),
                                    result = result,
                                    createtime = DateTime.Now
                                };
                                await _operateLogService.AddCityUploadApiLog(LogEntity);
                                if (string.IsNullOrEmpty(result))
                                {
                                    errcount += errcount;
                                }
                                else
                                {
                                    JObject mJObj = JObject.Parse(result);
                                    string msg = mJObj.GetValue("msg").ToString();
                                    if (msg == "上报成功")
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate("120.195.199.66:5678", api, now);
                                            list.Add(url + api);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                // _logger.LogError(realapi + ":" + ex.Message);
                                // return;
                            }
                            catch (Exception ex)
                            {

                                //_logger.LogError(realapi + ":" + ex.Message, true);
                            }
                        }
                    }
                }
            }
            return ResponseOutput.Ok();
        }


        [HttpPost]
        [Route("yanchengshimignzhi")]
        public async Task<IResponseOutput> yanchengshimignzhi()
        {



            string str = string.Empty;
            XmlDocument xml = new XmlDocument();
            xml.Load(@"D:\新建文本文档.xml");
            XmlElement elem = xml.DocumentElement;
            foreach (XmlNode Student in elem.ChildNodes)
            {
                if (Student.Name == "ResponseContent")
                {
                    foreach (XmlNode item in Student.ChildNodes)
                    {
                        if (item.Name == "Document")
                        {
                            foreach (XmlNode item1 in item.ChildNodes)
                            {
                                {
                                    foreach (XmlNode item2 in item1.ChildNodes)
                                    {
                                        foreach (XmlNode item3 in item2.ChildNodes)
                                        {
                                            foreach (XmlNode item4 in item3.ChildNodes)
                                            {

                                                str += item4.Attributes[1].Value + ",";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }






            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            JArray jar = new JArray();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
            string api = string.Empty;
            string result = "";
            string Token = "IBGcPGQieY3m8518D1XIeVn65Zpm11GffuPR7oy/YDlIw5Xtx0SeMw==";
            string url = "http://221.231.127.53:84/interface";


            JObject job = new JObject();



            //实名制url
            string realnameurl = _hpSystemSetting.getSettingValue(Const.Setting.S172);
            if (!string.IsNullOrEmpty(realnameurl))
            {

                //对接数据获取
                DataTable siteajcodesdt = await _aqtUploadService.GetGroupSiteajcodes();
                if (siteajcodesdt.Rows.Count > 0)
                {
                    for (int j = 0; j < siteajcodesdt.Rows.Count; j++)
                    {
                        DataRow dr = siteajcodesdt.Rows[j];
                        JObject jso = new JObject();
                        foreach (DataColumn column in dr.Table.Columns)
                        {
                            if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                            }
                            else
                            {
                                jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                            }
                        }
                        string uploadurl = jso.GetValue("siteuploadurl").ToString();
                        string uploadaccount = jso.GetValue("uploadaccount").ToString();
                        string uploadpwd = jso.GetValue("uploadpwd").ToString();
                        string attenduserpsd = jso.GetValue("attenduserpsd").ToString();
                        string recordNumber = jso.GetValue("siteajcodes").ToString();
                        //string token = _cityToken.getSiteCityToken(uploadurl, uploadaccount, uploadpwd);
                        //if (string.IsNullOrEmpty(token))
                        //{
                        //    _logger.LogInformation("数据上传结束（对方鉴权获取失败）。", true);
                        //    return;
                        //}
                        if (string.IsNullOrEmpty(attenduserpsd) || string.IsNullOrEmpty(uploadurl))
                        {
                            continue;
                        }
                        if (!uploadurl.Equals("http://49.4.68.132:8094/api/"))
                        {
                            continue;
                        }
                        string account = attenduserpsd.Split("||".ToCharArray())[0];
                        string pwd = attenduserpsd.Split("||".ToCharArray())[2];
                        JObject jsoparam = new JObject();
                        jsoparam.Add("recordNumber", "AJ320923120210172,AJ320923120200050,AJ320923120210213,AJ320925120220005,AJ320981120210351,AJ320906120220001,AJ320981120210351");

                        //获取指定项目的人员基础信息
                        string uploadapi = "/yancheng/personal/peopleInOutProInfo";
                        string realapi = "/yancheng/personal/peopleInOutProInfo";
                        api = "construction-site-api/ordinary-employee-info";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }

                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {

                            job = JObject.Parse(result);
                            if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                            {
                                JArray mJArray = new JArray();
                                JObject jobject = new JObject();
                                mJArray = (JArray)job.GetValue("data");
                                for (int k = 0; k < mJArray.Count; k++)
                                {
                                    try
                                    {
                                        jobject = (JObject)mJArray[k];
                                        jobject.Add("importOrExitFlag", jobject.GetValue("isResign"));
                                        jobject.Add("personName", jobject.GetValue("workerName"));
                                        jobject.Add("idcard", jobject.GetValue("idNumber"));
                                        jobject.Add("typeWork", jobject.GetValue("workType"));
                                        jobject.Add("importTime", jobject.GetValue("entryDate"));
                                        jobject.Add("exitTime", jobject.GetValue("exitDate"));
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode"));
                                        jobject.Add("userinfoid", jobject.GetValue("idNumber"));
                                        string areastr = "{\"11\":\"北京\",\"12\":\"天津\",\"13\":\"河北\",\"14\":\"山西\",\"15\":\"内蒙古\",\"21\":\"辽宁\",\"22\":\"吉林\",\"23\":\"黑龙江\",\"31\":\"上海\",\"32\":\"江苏\",\"33\":\"浙江\",\"34\":\"安徽\",\"35\":\"福建\",\"36\":\"江西\",\"37\":\"山东\",\"41\":\"河南\",\"42\":\"湖北\",\"43\":\"湖南\",\"44\":\"广东\",\"45\":\"广西\",\"46\":\"海南\",\"50\":\"重庆\",\"51\":\"四川\",\"52\":\"贵州\",\"53\":\"云南\",\"54\":\"西藏\",\"61\":\"陕西\",\"62\":\"甘肃\",\"63\":\"青海\",\"64\":\"宁夏\",\"65\":\"新疆\",\"71\":\"台湾\",\"81\":\"香港\",\"82\":\"澳门\",\"91\":\"国外\"}";
                                        JObject areajob = JObject.Parse(areastr);
                                        string id = jobject.GetValue("idNumber").ToString().Substring(0, 2);
                                        jobject["nativeplace"] = areajob.GetValue(id).ToString();
                                        jobject["importTime"] = Convert.ToDateTime(jobject.GetValue("importTime")).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                                        jobject["exitTime"] = Convert.ToDateTime(jobject.GetValue("exitTime")).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                                        //jobject.Remove("unifiedSocialCreditCode");
                                        //jobject.Remove("belongedTo");
                                        //jobject.Remove("workerName");
                                        //jobject.Remove("idNumber");
                                        //jobject.Remove("workType");
                                        //jobject.Remove("securityJob");
                                        //jobject.Remove("entryDate");
                                        //jobject.Remove("exitDate");
                                        //jobject.Remove("isResign");
                                        //jobject.Remove("updateDate");
                                        //jobject.Remove("userinfoid");
                                        //jobject.Remove("cerNo");
                                        //jobject.Remove("cerUrl");
                                        var data = jobject;
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                // await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //   _logger.LogError(uploadapi + ":" + ex.Message);
                                    }
                                }

                            }


                        }

                        //教育信息上传
                        uploadapi = "/yancheng/personal/peopleSafeEduInfo";
                        realapi = "/yancheng/personal/peopleSafeEduInfo";
                        api = "construction-site-api/employee-education-info";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject jobject = (JObject)mJArray[k];
                                        jobject.Add("idcard", jobject.GetValue("workerIdNumber").ToString());
                                        jobject.Add("typeWork", jobject.GetValue("workType"));
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode"));
                                        jobject.Add("educatType", jobject.GetValue("educationContent"));
                                        jobject.Add("educatContent", jobject.GetValue("educatType"));
                                        jobject.Add("educatDate", jobject.GetValue("educationDate"));
                                        jobject.Add("educationTime", jobject.GetValue("educationDuration"));
                                        jobject.Add("educatAddress", jobject.GetValue("educationLocation"));
                                        jobject["educatAddress"] = "本工地";
                                        var data = jobject;
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                //   _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }

                        //获取人员实时考勤数据上传
                        uploadapi = "/yancheng/personal/managerInOutSite";
                        realapi = "/yancheng/personal/managerInOutSite";
                        api = "construction-site-api/manager-in-out-site";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject jobject = (JObject)mJArray[k];
                                        jobject.Add("userinfoid", jobject.GetValue("idcard").ToString());

                                        var data = jobject;
                                        if (data.GetValue("type").ToString() == "管理班组-1")
                                        {
                                            data["type"] = "安全人员";
                                        }
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                // await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                //   _logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }



                        //奖惩信息上传
                        uploadapi = "/yancheng/personal/peopleRewardPunishInfo";
                        realapi = "/yancheng/personal/peopleRewardPunishInfo";
                        api = "construction-site-api/employee-reward-punishments-info";
                        if (jsoparam.ContainsKey("currentDate"))
                        {
                            jsoparam.Remove("currentDate");
                        }
                        if (dic.ContainsKey(uploadurl + uploadapi))
                        {
                            string currentDate = "";
                            dic.TryGetValue(uploadurl + uploadapi, out currentDate);
                            jsoparam.Add("currentDate", currentDate);
                        }
                        else
                        {
                            DataTable citydt = await _aqtUploadService.GetLastCityUploadTime(url, uploadapi);
                            if (citydt.Rows.Count > 0)
                            {
                                DataRow citydr = citydt.Rows[0];
                                string currentDate = citydr["uploadtime"].ToString();
                                jsoparam.Add("currentDate", currentDate);
                                dic.Add(uploadurl + uploadapi, currentDate);
                            }
                            else
                            {
                                jsoparam.Add("currentDate", "");
                            }

                        }
                        result = _jwtToken.JsonRequest(realnameurl, account, pwd, api, JsonConvert.SerializeObject(jsoparam));
                        if (!string.IsNullOrEmpty(result))
                        {
                            try
                            {
                                job = JObject.Parse(result);
                                if (job.GetValue("data") != null && !string.IsNullOrEmpty(job.GetValue("data").ToString()))
                                {
                                    JArray mJArray = new JArray();
                                    mJArray = (JArray)job.GetValue("data");
                                    for (int k = 0; k < mJArray.Count; k++)
                                    {
                                        JObject jobject = (JObject)mJArray[k];
                                        jobject.Add("idcard", jobject.GetValue("workerIdNumber"));
                                        jobject.Add("typeWork", jobject.GetValue("workType"));
                                        jobject.Add("unitCode", jobject.GetValue("unifiedSocialCreditCode"));
                                        jobject.Add("rewardType", jobject.GetValue("eventType"));
                                        jobject.Add("rewardContent", jobject.GetValue("eventContent"));
                                        jobject.Add("rewardDate", jobject.GetValue("eventDate"));
                                        var data = jobject;
                                        jso = new JObject();
                                        jso.Add("Token", Token);
                                        jso.Add("JSON", data);
                                        string uploadresult = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                        LogEntity = new CityUploadOperateLog
                                        {
                                            url = url,
                                            api = uploadapi,
                                            account = uploadaccount,
                                            param = mJArray[k].ToString(),
                                            result = uploadresult,
                                            createtime = DateTime.Now
                                        };
                                        await _operateLogService.AddCityUploadApiLog(LogEntity);
                                        if (string.IsNullOrEmpty(uploadresult))
                                        {
                                            errcount++;
                                            break;
                                        }
                                        else
                                        {
                                            if (!list.Contains(uploadurl + uploadapi))
                                            {
                                                await _aqtUploadService.UpdateCityUploadDate(uploadurl, uploadapi, now);
                                                list.Add(uploadurl + uploadapi);
                                            }
                                            successcount += successcount;
                                        }
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                //_logger.LogError(uploadapi + ":" + ex.Message);
                            }
                        }
                    }
                }
            }
            return ResponseOutput.Ok();
        }

        [HttpPost]
        [Route("YnchengMintue")]
        public async Task<IResponseOutput> YnchengMintue()
        {
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            string Token = "IBGcPGQieY3m8518D1XIeVn65Zpm11GffuPR7oy/YDlIw5Xtx0SeMw==";

            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForYanchengMinute();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            try
                            {
                                DataRow dr = dt.Rows[j];
                                JObject jso = new JObject();
                                foreach (DataColumn column in dr.Table.Columns)
                                {
                                    if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.Decimal")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDouble());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.DateTime")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDateTime());
                                    }
                                    else
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                                    }
                                }
                                string funingurl = "";
                                if (jso.ContainsKey("funingurl"))
                                {
                                    funingurl = jso.GetValue("funingurl").ToString();
                                    jso.Remove("funingurl");
                                }
                                string url = jso.GetValue("siteuploadurl").ToString();
                                string api = jso.GetValue("post").ToString();
                                string realapi = api;


                                if (api.Contains("Craneinterface/UploadCraneHistory"))          //塔机实时数据上传
                                {
                                    realapi = "/yancheng/towerCraneMonitor/uploadCraneHistory";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        var data1 = JObject.Parse(jso.GetValue("sedata").ToString());
                                        if (data1.ContainsKey("SafeLoad"))
                                        {
                                            double SafeLoad = data1.GetValue("SafeLoad").ToDouble();
                                            jso.Add("load", SafeLoad);
                                        }
                                        else
                                        {
                                            jso.Add("load", 0);
                                        }
                                        if (data1.ContainsKey("Margin"))
                                        {
                                            double Margin = data1.GetValue("Margin").ToDouble();
                                            jso.Add("range", Margin);
                                        }
                                        else
                                        {
                                            jso.Add("range", 0);
                                        }
                                        if (data1.ContainsKey("MomentPercent"))
                                        {
                                            double MomentPercent = data1.GetValue("MomentPercent").ToDouble();
                                            jso.Add("moment", MomentPercent);
                                        }
                                        else
                                        {
                                            jso.Add("moment", 0);

                                        }
                                        if (data1.ContainsKey("Rotation"))
                                        {
                                            double Rotation = data1.GetValue("Rotation").ToDouble();
                                            jso.Add("rotation", Rotation);
                                        }
                                        else
                                        {
                                            jso.Add("rotation", 0);
                                        }
                                        if (data1.ContainsKey("Height"))
                                        {
                                            double Height = data1.GetValue("Height").ToDouble();
                                            jso.Add("height", Height);
                                        }
                                        else
                                        {
                                            jso.Add("height", 0);
                                        }
                                    }
                                }
                                else if (api.Contains("UploadHistory"))             //卸料平台实时数据上传
                                {
                                    realapi = "/yancheng/uploadinterface/uploadHistory";
                                    if (jso.ContainsKey("rtdjson"))
                                    {
                                        var data1 = JObject.Parse(jso.GetValue("rtdjson").ToString());
                                        if (data1.ContainsKey("weight"))
                                        {
                                            double weight = data1.GetValue("weight").ToDouble();
                                            jso.Add("weight", weight);
                                        }
                                        if (data1.ContainsKey("bias"))
                                        {
                                            double bias = data1.GetValue("bias").ToDouble();
                                            jso.Add("weightBias", bias);
                                        }
                                        if (data1.ContainsKey("early_warning_weight"))
                                        {
                                            double early_warning_weight = data1.GetValue("early_warning_weight").ToDouble();
                                            jso.Add("warningWeight", early_warning_weight);
                                        }
                                        if (data1.ContainsKey("alarm_weight"))
                                        {
                                            double alarm_weight = data1.GetValue("alarm_weight").ToDouble();
                                            jso.Add("alarmWeight", alarm_weight);
                                        }
                                        if (data1.ContainsKey("electric_quantity"))
                                        {
                                            double electric_quantity = data1.GetValue("electric_quantity").ToDouble();
                                            jso.Add("powerPercent", electric_quantity);
                                        }
                                        if (data1.ContainsKey("upstate"))
                                        {
                                            double upstate = data1.GetValue("upstate").ToDouble();
                                            jso.Add("dataType", upstate);
                                        }
                                        if (data1.ContainsKey("Id"))
                                        {
                                            jso.Add("eventId", data1.GetValue("Id").ToString());
                                        }
                                    }
                                }
                                else if (api.Contains("Hoistinterface/HoistHistory"))           //施工升降机实时数据上传
                                {
                                    realapi = "/yancheng/hoistinterface/hoistHistory";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        var data1 = JObject.Parse(jso.GetValue("sedata").ToString());
                                        if (data1.ContainsKey("SafeLoad"))
                                        {
                                            double SafeLoad = data1.GetValue("SafeLoad").ToDouble();
                                            jso.Add("load", SafeLoad);
                                        }
                                        if (data1.ContainsKey("Margin"))
                                        {
                                            double Margin = data1.GetValue("Margin").ToDouble();
                                            jso.Add("range", Margin);
                                        }
                                        if (data1.ContainsKey("MomentPercent"))
                                        {
                                            double MomentPercent = data1.GetValue("MomentPercent").ToDouble();
                                            jso.Add("moment", MomentPercent);
                                        }
                                        if (data1.ContainsKey("Rotation"))
                                        {
                                            double Rotation = data1.GetValue("Rotation").ToDouble();
                                            jso.Add("rotation", Rotation);
                                        }
                                        if (data1.ContainsKey("Height"))
                                        {
                                            double Height = data1.GetValue("Height").ToDouble();
                                            jso.Add("height", Height);
                                        }
                                    }
                                }
                                else  if(api.Contains("DeppPitHistory"))
                                {
                                    realapi = "/yancheng/deppPit/deppPitHistory";

                                    JArray jar = JArray.Parse(jso.GetValue("data").ToString());
                                    JObject job = JObject.Parse(jar[0].ToString());
                                    jso.Add("WatchPoint", job.GetValue("watchPoint"));
                                    jso.Add("WatchPointValue", job.GetValue("watchPointValue"));
                             

                                }
                                jso.Remove("post");
                                jso.Remove("siteuploadurl");
                                jso.Remove("uploadaccount");
                                jso.Remove("uploadpwd");
                                var data = jso;
                                jso = new JObject();
                                jso.Add("Token", Token);
                                
                                jso.Add("JSON", data);                                                                                                                 
                                string result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = url,
                                    api = api,
                                    account = Token,
                                    param = jso.ToString(),
                                    result = result,
                                    createtime = DateTime.Now
                                };
                                await _operateLogService.AddCityUploadApiLog(LogEntity);
                                if (string.IsNullOrEmpty(result))
                                {
                                    errcount++;
                                    break;
                                }
                                else
                                {
                                    JObject mJObj = JObject.Parse(result);
                                    int code = (int)mJObj["code"];
                                    if (code == 0)
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                               // _logger.LogError(ex.Message);
                            }


                        }
                    }
                }
            }
            return ResponseOutput.Ok();
        }

    }
}