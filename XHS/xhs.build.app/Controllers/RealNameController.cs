using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.RealName;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using XHS.Build.Services.EmployeeSite;
using System;
using System.Data;
using XHS.Build.Common.Util;
using Util;
using XHS.Build.Services.Group;
using XHS.Build.Common.Auth;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 请求网络数据
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RealNameController : ControllerBase
    {
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IQunYaoRealNameService _qunYaoRealNameService;
        private readonly IEmployeeSiteService _employeeSiteService;
        private readonly IGroupService _groupService;
        private readonly IUser _user;
        public RealNameController(IHpSystemSetting hpSystemSetting, IGroupService groupService, IQunYaoRealNameService qunYaoRealNameService, IEmployeeSiteService employeeSiteService, IUser user)
        {
            _hpSystemSetting = hpSystemSetting;
            _qunYaoRealNameService = qunYaoRealNameService;
            _employeeSiteService = employeeSiteService;
            _groupService = groupService;
            _user = user;
        }

        public class RequestParam
        {
            //用户uuid（多用户逗号分隔）
            public string ProjectCode { get; set; }
            public string StartTime { get; set; } = "";
            public string EndTime { get; set; }
        }
        /// <summary>
        /// 获取工地当日人员实时考勤信息
        /// </summary>
        /// <param name="type">实名制类型(1：群耀  2：大运  4：都驰  )</param>
        /// <param name="itemId">项目编号</param>
        /// <param name="stTm">开始时间（yyyy-MM-dd）</param>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetInAndOutByPeopleAndDay(int type, string itemId, string stTm, int SITEID =0)
        {
            var retString = "";
            switch (type)
            {
                case 1:
                    //1：群耀
                    retString = await _qunYaoRealNameService.GetInAndOutByPeopleAndDay(stTm, itemId);
                    if (string.IsNullOrEmpty(retString))
                    {
                        return ResponseOutput.NotOk("返回内容为空");
                    }
                    var retObj = JsonConvert.DeserializeObject<RealNameDto>(retString);
                    if (retObj.ResultState == "1")
                    {
                        return ResponseOutput.Ok(retObj.Data);
                    }
                    else
                    {
                        return ResponseOutput.NotOk(retObj.resultMessage);
                    }

                case 2:
                    //2：大运
                    var s118 = _hpSystemSetting.getSettingValue(Const.Setting.S118);
                    if (string.IsNullOrEmpty(s118))
                    {
                        return ResponseOutput.NotOk("未获取到请求地址信息");
                    }
                    retString = HttpNetRequest.HttpGet(s118 + "queryInAndOutByPeopleAndDay?stTm=" + stTm + "&itemId=" + itemId);
                    if (string.IsNullOrEmpty(retString))
                    {
                        return ResponseOutput.NotOk("返回内容为空");
                    }
                    JObject mJObj = new JObject();
                    mJObj = JObject.Parse(retString);
                    if (mJObj["resultMessage"].ToString() == "success")//成功
                    {
                        JArray myJsonArray;
                        myJsonArray = (JArray)mJObj.GetValue("data");
                        List<BnRealName> list = new List<BnRealName>();
                        foreach (var item in myJsonArray)
                        {
                            BnRealName bnRealName = new BnRealName();
                            string InSj = (string)((JObject)item).GetValue("inTm");
                            string OutSj = (string)((JObject)item).GetValue("outTm");
                            if (!string.IsNullOrEmpty(InSj))
                            {
                                InSj = InSj.Substring(InSj.IndexOf(" ") + 1);
                            }
                            if (!string.IsNullOrEmpty(OutSj))
                            {
                                OutSj = OutSj.Substring(OutSj.IndexOf(" ") + 1);
                            }
                            bnRealName.InSj = InSj;
                            bnRealName.OutSj = OutSj;
                            bnRealName.Zjhm = (string)((JObject)item).GetValue("certNo");
                            bnRealName.WorkerName = (string)((JObject)item).GetValue("userName");
                            bnRealName.UserType = (string)((JObject)item).GetValue("userType");
                            list.Add(bnRealName);
                        }
                        return ResponseOutput.Ok(list);
                    }
                    else
                    {
                        return ResponseOutput.NotOk((string)mJObj.GetValue("resultMessage"));
                    }
                case 4:
                    //4：都驰
                    DateTime starttime =  DateTime.ParseExact(stTm, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
                    //if (SITEID == 0)
                    //{
                    //    SITEID = 29;
                   // }
                    DataTable dt = await _employeeSiteService.GetAttendPerson(SITEID, starttime);
                    return ResponseOutput.Ok(dt);
                case 5:
                    //5：新合盛实名制
                    string Url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
                    if (string.IsNullOrEmpty(Url))
                    {
                        return ResponseOutput.NotOk("实名制接口地址未配置。");
                    }
                    string keysecret = await _groupService.GetAttendUserPsd(_user.GroupId);
                    if (string.IsNullOrEmpty(keysecret))
                    {
                        return ResponseOutput.NotOk("实名制接口SECRET未配置。");
                    }

                    string centertoken = "";
                    string tokenapi = "authenticate/token-site";
                    string jpushapi = "construction-site-api/employee-attendance";
                    var key = keysecret.Split("||")[0];
                    var SECRET = keysecret.Split("||")[1];
                    string postStr = "{\"userName\":\"" + key + "\",\"password\":\"" + SECRET + "\"}";
                    retString = UhttpClient.PostJson(Url + tokenapi, postStr);
                    if (!string.IsNullOrEmpty(retString))
                    {
                        mJObj = new JObject();
                        mJObj = JObject.Parse(retString);
                        centertoken = (string) mJObj.GetValue("accessToken");
                        if (string.IsNullOrEmpty(centertoken))
                        {
                            return ResponseOutput.NotOk("接口认证失败。");
                        }

                    }
                    //RequestParam param = new RequestParam();
                    //param.ProjectCode = itemId;
                    //param.StartTime = stTm + " 00:00:00";
                    //param.EndTime = stTm + " 23:59:59";
                    Dictionary<string, object> dictparam = new Dictionary<string, object>()
                    {
                         { "ProjectCode", itemId },
                         { "StartTime", stTm + " 00:00:00" },
                         { "EndTime", stTm + " 23:59:59" }
                     };
                    retString = UhttpClient.Get(Url + jpushapi, dictparam, new Dictionary<string, string>() { { "Authorization", "Bearer " + centertoken } });
                    if (!string.IsNullOrEmpty(retString))
                    {
                        mJObj = new JObject();
                        mJObj = JObject.Parse(retString);
                        if (mJObj["resultStatus"].ToBool())//成功
                        {
                            JArray myJsonArray;
                            myJsonArray = (JArray)mJObj.GetValue("data");
                            List<BnRealName> list = new List<BnRealName>();
                            foreach (var item in myJsonArray)
                            {
                                BnRealName bnRealName = new BnRealName();
                                string InSj = (string)((JObject)item).GetValue("entryDate");
                                string OutSj = (string)((JObject)item).GetValue("outDate");
                                if (!string.IsNullOrEmpty(InSj))
                                {
                                    InSj = InSj.Substring(InSj.IndexOf(" ") + 1);
                                }
                                if (!string.IsNullOrEmpty(OutSj))
                                {
                                    OutSj = OutSj.Substring(OutSj.IndexOf(" ") + 1);
                                }
                                bnRealName.InSj = InSj;
                                bnRealName.OutSj = OutSj;
                                bnRealName.Zjhm = (string)((JObject)item).GetValue("idNumber");
                                bnRealName.WorkerName = (string)((JObject)item).GetValue("employeeName");
                                bnRealName.UserType = (string)((JObject)item).GetValue("workType");
                                list.Add(bnRealName);
                            }
                            return ResponseOutput.Ok(list);
                        }
                        else
                        {
                            return ResponseOutput.NotOk((string)mJObj.GetValue("msg"));
                        }
                    }
                    return ResponseOutput.NotOk("未取到数据。");
                default:
                    return ResponseOutput.NotOk("该类型实名制还未对接，敬请期待。");
            }



        }

    }
}
