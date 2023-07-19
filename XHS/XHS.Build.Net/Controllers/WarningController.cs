using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Invade;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Warning;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 报警
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class WarningController : ControllerBase
    {
        private readonly IWarningService _warningService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly HpAliSMS _hpAliSMS;
        private readonly IInvadeService _invadeService;
        public WarningController(IWarningService warningService, IHpSystemSetting hpSystemSetting, IInvadeService invadeService)
        {
            _warningService = warningService;
            _hpSystemSetting = hpSystemSetting;
            _hpAliSMS = new HpAliSMS(hpSystemSetting);
            _invadeService = invadeService;
        }

        /// <summary>
        /// 安全帽未佩戴
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Helmet(WarnHelmetInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.projid) || string.IsNullOrEmpty(input.location) || input.createtime == null || string.IsNullOrEmpty(input.imgurl) || input.thumblist == null || input.thumblist.Length == 0)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }

            int retDB = await _warningService.doHelmet(new SugarParameter("@projid", input.projid), new SugarParameter("@location", input.location), new SugarParameter("@createtime", input.createtime), new SugarParameter("@jsonall", JsonConvert.SerializeObject(input)));
            if (retDB > 0)
            {
                _hpAliSMS.SendWarnAIById(retDB, "");
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 陌生人
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Stranger(WarnStrangerInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.thumblist == null || jsonData.thumblist.Length == 0)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }

            int retDB = await _warningService.doStranger(new SugarParameter("@projid", jsonData.projid), new SugarParameter("@createtime", jsonData.createtime), new SugarParameter("@jsonall", JsonConvert.SerializeObject(jsonData)));
            if (retDB > 0)
            {
                _hpAliSMS.SendWarnAIById(retDB, "");
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }
        /// <summary>
        /// 车道识别
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Trespasser(WarnTrespasserInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.thumblist == null || jsonData.thumblist.Length == 0 || jsonData.createtime == null)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            int retDB = await _warningService.doTrespasser(new SugarParameter("@projid", jsonData.projid), new SugarParameter("@createtime", jsonData.createtime), new SugarParameter("@jsonall", JsonConvert.SerializeObject(jsonData)));
            if (retDB > 0)
            {
                _hpAliSMS.SendWarnAIById(retDB, "");
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 火警
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Fire(WarnFireInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.temperature < 0 || jsonData.createtime == null || jsonData.thumblist == null || jsonData.thumblist.Length == 0)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            int retDB = await _warningService.doFire(new SugarParameter("@projid", jsonData.projid), new SugarParameter("@temperature", jsonData.temperature), new SugarParameter("@createtime", jsonData.createtime), new SugarParameter("@jsonall", JsonConvert.SerializeObject(jsonData)));
            if (retDB > 0)
            {
                _hpAliSMS.SendWarnAIById(retDB, "");
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 烟雾
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> smoke(SmokeInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.createtime == null || jsonData.thumblist == null || jsonData.thumblist.Length == 0)
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            int retDB = await _warningService.doSmoke(new SugarParameter("@projid", jsonData.projid), new SugarParameter("@location", jsonData.location), new SugarParameter("@createtime", jsonData.createtime), new SugarParameter("@jsonall", JsonConvert.SerializeObject(jsonData)));
            if (retDB > 0)
            {
                _hpAliSMS.SendWarnAIById(retDB, "");
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }


        /// <summary>
        /// 5.5	升降机人数超载
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Overload(WarnOverloadInput jsonData)
        {
            if (jsonData == null || jsonData.createtime == null || string.IsNullOrEmpty(jsonData.liftovercode) || jsonData.numofpeople < 0 || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.thumblist == null || jsonData.thumblist.Length == 0 || string.IsNullOrEmpty(jsonData.projid))
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            int retDB = await _warningService.doOverload(new SugarParameter("@projid", jsonData.projid), new SugarParameter("@numofpeople", jsonData.numofpeople), new SugarParameter("@liftovercode", jsonData.liftovercode), new SugarParameter("@createtime", jsonData.createtime), new SugarParameter("@jsonall", JsonConvert.SerializeObject(jsonData)));
            if (retDB > 0)
            {
                _hpAliSMS.SendWarnAIById(retDB, "");
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 区域入侵
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Invade(InvadeWarnInsertInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.invadecode))
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            var ret = await _invadeService.SPWarnInsertForInvade(input);
            if (ret > 0)
            {
                return ResponseOutput.Ok(ret);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(ret), ret);
            }
        }

        /// <summary>
        /// 反光衣
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> ReflectiveVest(WarnReflectiveVestInput jsonData)
        {
            if (jsonData == null || string.IsNullOrEmpty(jsonData.projid) || string.IsNullOrEmpty(jsonData.imgurl) || jsonData.createtime == null || string.IsNullOrEmpty(jsonData.thumburl))
            {
                return ResponseOutput.NotOk("参数错误", -10001);
            }
            int retDB = await _warningService.doVest(new SugarParameter("@projid", jsonData.projid), new SugarParameter("@createtime", jsonData.createtime), new SugarParameter("@jsonall", JsonConvert.SerializeObject(jsonData)));
            if (retDB > 0)
            {
                _hpAliSMS.SendWarnAIById(retDB, "");
                return ResponseOutput.Ok(1);
            }
            else
            {
                return ResponseOutput.NotOk(HpMessage.getMessage(retDB), retDB);
            }
        }

        /// <summary>
        /// 获取某日报警数
        /// </summary>
        /// <param name="siteid">监测点ID</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetWarnCountList(int siteid, DateTime date)
        {           
            DataTable dt = await _warningService.GetWarnListForTaihu(siteid, date, date);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取某天报警数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="type">种类(0：全部,1:扬尘离线报警,2:扬尘超标报警,3:车辆冲洗报警,4:特种设备报警,5:临边围挡报警,61:安全帽未佩戴,62:陌生人进场识别,63:人车分流识别,64:火警,65:区域闯入,66:黄土裸露,67:密闭运输,68:反光衣)</param>
        /// <param name="date">日期（yyyy-MM-dd）</param>
        /// <returns>预警信息数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListType(int SITEID, int type, DateTime date)
        {
            try
            {
                DateTime startdate = date.Date;
                DateTime enddate = date.AddDays(1).Date;
                DataTable dt = await _warningService.getListByType(SITEID, type, startdate, enddate);
                JArray ja = new JArray();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JObject jo = new JObject();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        jo.Add(dt.Columns[j].ColumnName.ToString(), Convert.ToString(dt.Rows[i][j]));
                    }
                    //3:车辆冲洗报警
                    if (type == 3)
                    {
                        JObject jos = JObject.Parse(jo.GetValue("paramjson").ToString());
                        jo.Add("imgurl", jos.GetValue("img"));
                        jo.Add("thumblist", "");
                        jo.Add("video", jos.GetValue("video"));
                    }
                    else if (type >= 61&& type <= 68)
                    {
                        JObject jos = JObject.Parse(jo.GetValue("jsonall").ToString());
                        jo.Add("imgurl", jos.GetValue("imgurl"));
                        jo.Add("thumblist", jos.GetValue("thumblist"));
                        jo.Add("video", "");
                    }
                    else
                    {
                        jo.Add("imgurl", "");
                        jo.Add("thumblist","");
                        jo.Add("video", "");
                    }

                    jo.Remove("paramjson");
                    jo.Remove("jsonall");
                    ja.Add(jo);
                }
                return ResponseOutput.Ok(ja);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }

        }

        /// <summary>
        /// 获取设备某天报警数据
        /// </summary>
        /// <param name="devicecode">设备编号</param>
        /// <param name="type">种类(0：全部,1:扬尘离线报警,2:扬尘超标报警,3:车辆冲洗报警,4:特种设备报警,5:临边围挡报警,61:安全帽未佩戴,62:陌生人进场识别,63:人车分流识别,64:火警,65:区域闯入,66:黄土裸露,67:密闭运输,68:反光衣)</param>
        /// <param name="date">日期（yyyy-MM-dd）</param>
        /// <returns>预警信息数据集</returns>
        [HttpGet]
        public async Task<IResponseOutput> GetListByDeviceCode(string devicecode, int type, DateTime date)
        {
            try
            {
                DateTime startdate = date.Date;
                DateTime enddate = date.AddDays(1).Date;
                DataTable dt = await _warningService.getListByDevicecode(devicecode, type, startdate, enddate);
                JArray ja = new JArray();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JObject jo = new JObject();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        jo.Add(dt.Columns[j].ColumnName.ToString(), Convert.ToString(dt.Rows[i][j]));
                    }
                    //3:车辆冲洗报警
                    if (type == 3)
                    {
                        JObject jos = JObject.Parse(jo.GetValue("paramjson").ToString());
                        jo.Add("imgurl", jos.GetValue("img"));
                        jo.Add("thumblist", "");
                        jo.Add("video", jos.GetValue("video"));
                    }
                    else if (type >= 61 && type <= 68)
                    {
                        JObject jos = JObject.Parse(jo.GetValue("jsonall").ToString());
                        jo.Add("imgurl", jos.GetValue("imgurl"));
                        jo.Add("thumblist", jos.GetValue("thumblist"));
                        jo.Add("video", "");
                    }
                    else
                    {
                        jo.Add("imgurl", "");
                        jo.Add("thumblist", "");
                        jo.Add("video", "");
                    }

                    jo.Remove("paramjson");
                    jo.Remove("jsonall");
                    ja.Add(jo);
                }
                return ResponseOutput.Ok(ja);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }

        }
    }
}
