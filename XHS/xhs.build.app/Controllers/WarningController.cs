using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Site;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.Warning;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarningController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IWarningService _warningService;
        private readonly ISiteService _siteService;
        private readonly IHpWarningProof _hpWarningProof;
        private readonly ISmsQueueService _smsQueueService;
        /// <summary>
        /// 
        /// </summary>
        public WarningController(IWarningService warningService, ISiteService siteService, IUser user, IHpWarningProof hpWarningProof, ISmsQueueService smsQueueService)
        {
            _warningService = warningService;
            _siteService = siteService;
            _hpWarningProof = hpWarningProof;
            _user = user;
            _smsQueueService = smsQueueService;
        }
        /// <summary>
        /// 检索预警
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        [HttpGet]
        [Route("getlisttype")]
        public async Task<IResponseOutput> GetListType(int SITEID, int type, DateTime startdate, DateTime enddate)
        {
            try
            {
                DataTable dt = await _warningService.getListType(SITEID, type, startdate, enddate);
                return ResponseOutput.Ok(dt);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }

        }

        /// <summary>
        /// 获取预警统计
        /// </summary>
        /// <param name="timetype">周期.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsumlist")]
        public async Task<IResponseOutput> GetSumList(int timetype)
        {
            if (timetype <= 0)
                return ResponseOutput.NotOk("请填写正确的参数");

            string result = null;
            try
            {
                DataTable dt = null;
                switch (timetype)
                {
                    case 1:
                        dt = await _warningService.getSumList(DateTime.Now, DateTime.Now);
                        break;
                    case 2:
                        dt = await _warningService.getSumList(DateTime.Now.AddDays(-6), DateTime.Now);
                        break;
                    case 3:
                        dt = await _warningService.getSumList(DateTime.Now.AddMonths(-1).AddDays(1), DateTime.Now);
                        break;
                    case 4:
                        dt = await _warningService.getSumList(DateTime.Now.AddMonths(-3).AddDays(1), DateTime.Now);
                        break;

                    default:
                        break;
                }
                return ResponseOutput.Ok(dt);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }
        /// <summary>
        /// 获取重要告警
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        [HttpGet]
        [Route("getlistprime")]
        public async Task<IResponseOutput> GetListPrime(int SITEID, DateTime startdate, DateTime enddate)
        {
            string result = null;
            try
            {
                DataTable dt = await _warningService.getListPrime(SITEID, startdate, enddate);

                return ResponseOutput.Ok(dt);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }

        }
        /// <summary>
        /// 获取该监测对象的负责人
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("gettagetlist")]
        public async Task<IResponseOutput> GetTagetList(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk("请填写正确的参数");

            string result = null;
            try
            {
                DataTable dt = await _siteService.getAsignListForApp(SITEID);

                return ResponseOutput.Ok(dt);

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 上传任务相关文件
        /// </summary>
        /// <param name="SITEID">SITEID</param>
        /// <param name="WPID	">WPID</param>
        [HttpPost]
        [Route("file")]
        public async Task<IResponseOutput> UploadProff(WarnFileInput input)
        {
            if (input == null || input.SITEID.Equals(0) || input.WPID.Equals(0) || string.IsNullOrEmpty(input.filename)
                || string.IsNullOrEmpty(input.fileString))
                return ResponseOutput.NotOk<int>("请填写正确的参数", 0);
            //string filename = file.FileName;
            //long filesize = file.Length / 1024;
            //Stream fs = file.OpenReadStream();
            //byte[] bt = new byte[file.Length];
            //fs.Read(bt, 0, bt.Length);
            //string fileString = Convert.ToBase64String(bt);
            int result = 0;
            try
            {
                result = _hpWarningProof.doUpload(input.SITEID, input.WPID, input.bsolved, input.filename, input.fileString, input.filesize);
                return result <= 0 ? ResponseOutput.NotOk<int>("文件上传失败", 0) : ResponseOutput.Ok(result);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>(ex.Message, 0);
            }
        }

        /// <summary>
        /// 分页获取预警列表
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="normalai"></param>
        /// <param name="warntype"></param>
        /// <param name="warnstatus"></param>
        /// <param name="wpcode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("handle")]
        public async Task<IResponseOutput> GetWarningHandleList(DateTime startdate, DateTime enddate, int normalai, int warntype, int warnstatus, string wpcode, int pageIndex = 1, int pageSize = 20)
        {
            var page = new PageOutput() { Total = 0, List = new DataTable() };
            var list = await _warningService.GetWarnListByCondition(startdate, enddate, normalai, warntype, warnstatus, wpcode, pageIndex, pageSize);
            if (list != null && list.Rows.Count > 0)
            {
                page.Total = Convert.ToInt32(list.Rows[0]["totalcount"]);
                page.List = list;
            }
            return ResponseOutput.Ok(page);
        }

        /// <summary>
        /// 预警问题添加批示
        /// </summary>
        /// <param name="WARNID">预警ID</param>
        /// <param name="type">类型</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Route("remarkAdd")]
        public async Task<IResponseOutput> RemarkAdd([FromForm] long WARNID, [FromForm] int type, [FromForm] string remark)
        {
            if (WARNID < 1 || type < 1 || string.IsNullOrEmpty(remark))
                return ResponseOutput.NotOk("请填写完整信息", 0);

            int result = 0;
            try
            {
                object param = new { WARNID = WARNID, type = type, USERID = _user.Id, remark = WebUtility.UrlDecode(remark) };
                result = await _warningService.doNjjyWarnAddRemark(param);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message, 0);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 预警问题解决
        /// </summary>
        /// <param name="WARNID">预警ID</param>
        /// <param name="type">类型</param>
        /// <param name="solvedremark">解决日志</param>
        /// <param name="ismulti">0:单条处理  1：批量处理</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("solve")]
        public async Task<IResponseOutput> Solve([FromForm] long WARNID, [FromForm] int type, [FromForm] string solvedremark, [FromForm] int ismulti)
        {
            if (WARNID < 1 || type < 1 || string.IsNullOrEmpty(solvedremark))
                return ResponseOutput.NotOk("请填写完整信息");

            string result = null;
            try
            {
                object param = new { WARNID = WARNID, type = type, solveduser = _user.Name, solvedremark = WebUtility.UrlDecode(solvedremark), ismulti = ismulti };
                DataTable dt = await _warningService.doNjjyWarnSolve(param);

                return ResponseOutput.Ok(dt);

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 结束预警问题
        /// </summary>
        /// <param name="ROUNDID">预警ID</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("finish")]
        public async Task<IResponseOutput> Finish([FromForm] long WARNID, [FromForm] int type, [FromForm] string remark)
        {
            if (WARNID < 1)
                return ResponseOutput.NotOk("请填写完整信息");

            int result = 0;
            try
            {
                object param = new { WARNID = WARNID, type = type, username = _user.Name, remark = string.IsNullOrEmpty(remark) ? "" : WebUtility.UrlDecode(remark) };
                result = await _warningService.doNjjyWarnFinish(param);
                return ResponseOutput.Ok(result);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 设置短信提醒
        /// </summary>
        /// <param name="input">GROUPID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("setSmsQueue")]
        public async Task<IResponseOutput> SetSmsQueue(WarnSMSQueueInput input)
        {
            if (input == null || input.SITEID < 1 || input.WPID < 1 || string.IsNullOrEmpty(input.phonenumber) || string.IsNullOrEmpty(input.username)
                  || string.IsNullOrEmpty(input.sitename) || string.IsNullOrEmpty(input.wpcode))
            {
                return ResponseOutput.NotOk("请填写完整信息");
            }

            int result = 0;
            try
            {
                Dictionary<string, string> jsonparam = new Dictionary<string, string>();
                var param1 = new SugarParameter("@GROUPID", _user.GroupId);
                var param2 = new SugarParameter("@SITEID", input.SITEID);
                var param3 = new SugarParameter("@linkid", input.WPID);
                var param4 = new SugarParameter("@sqtype", 2);
                var param5 = new SugarParameter("@phonenumber", input.phonenumber);
                var param6 = new SugarParameter("@username", input.username);
                var param7 = new SugarParameter("@templatecode", HpAliSMS.TPL_WARN_UPDATE);
                var param9 = new SugarParameter("@uuids", input.uuids);
                jsonparam.Add("sitename", input.sitename);
                jsonparam.Add("wpcode", input.wpcode);
                jsonparam.Add("warnstatus", input.warnstatus);
                var param8 = new SugarParameter("@jsonparam", JsonConvert.SerializeObject(jsonparam));
                result = await _smsQueueService.doInsert(param1, param2, param3, param4, param5, param6, param7, param8, param9);
                return result > 0 ? ResponseOutput.Ok(result) : ResponseOutput.NotOk("");
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }


        /// <summary>
        /// 预警信息
        /// </summary>
        /// <param name="WARNID">WARNID</param>
        /// <param name="type">type</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getWarnDetail")]
        public async Task<IResponseOutput> GetWarnDetail(int WARNID, int type)
        {
            if (WARNID <= 0 || type <= 0)
                return ResponseOutput.NotOk("请填写完整信息");

            try
            {
                DataSet ds = await _warningService.getNjjyWarnDetail(WARNID, type);

                return ResponseOutput.Ok(ds);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 获取一个工地AI的30天和1年的统计件数
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getSiteAiCount")]
        public async Task<IResponseOutput> GetSiteAiCount(int SITEID, int type)
        {
            if (SITEID <= 0 || type <= 0)
                return ResponseOutput.NotOk("请填写完整信息");

            try
            {
                DataSet ds = await _warningService.getSiteAiCount(SITEID, type);

                return ResponseOutput.Ok(ds);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 获取一个工地指定日的统计件数
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警,46:升降机超员,65:区域闯入,66:黄土裸露,67:密闭运输,68:反光衣</param>
        /// <param name="billdate">指定日</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getSiteAiDayCount")]
        public async Task<IResponseOutput> GetSiteAiDayCount(int SITEID, int type, DateTime billdate)
        {
            if (SITEID <= 0 || type <= 0)
                return ResponseOutput.NotOk("请填写完整信息");

            try
            {
                DataTable dt = await _warningService.getSiteAiDayCount(SITEID, type, billdate);

                return ResponseOutput.Ok(dt);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 获取工地列表
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警,65:区域闯入,66:黄土裸露,67:密闭运输,68:反光衣</param>
        /// <param name="billdate">指定日</param>
        /// <param name="ENDWARNID">本页最后一条数据的WARNID（用于手机分页）</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">一页件数</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getSiteAiList")]
        public async Task<IResponseOutput> GetSiteAiList(int SITEID, int type, DateTime billdate, int ENDWARNID, int pageindex, int pagesize)
        {
            if (SITEID <= 0 || type <= 0 || ENDWARNID < 0 || pageindex <= 0 || pagesize <= 0)
                return ResponseOutput.NotOk("请填写完整信息");

            try
            {
                DataSet ds = await _warningService.getSiteAiList(SITEID, type, billdate, ENDWARNID, pageindex, pagesize);

                return ResponseOutput.Ok(ds);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
        }

        /// <summary>
        /// 首页 新增统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("warningindexcount")]
        public async Task<IResponseOutput> GetWarning()
        {
            DataTable dt = await _warningService.spWarnCountForAppTop(DateTime.Now);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 告警统计（每日，按设备类型分）
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="deviceType">1 扬尘，2 摄像头，3 塔吊，4 升降机，5 卸料平台，6 深基坑，7 高支模，8 临边围挡（旧）,9 实名制</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/[action]")]
        public async Task<IResponseOutput> WarningTypeDaily(int SITEID,DateTime startTime, DateTime endTime, int deviceType)
        {
            int days = (endTime - startTime).TotalDays.ObjToInt();
            if (days <= 0 || days > 365)
            {
                return ResponseOutput.NotOk("日期超限");
            }

            var result = await _warningService.spV2WarnTypeDaily(
                startTime.ToString("yyyy-MM-dd")
                , endTime.ToString("yyyy-MM-dd")
                , SITEID
                , deviceType
                );

            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 通用告警-实时告警数据列表
        /// </summary>
        /// <param name="type">1 扬尘，2 摄像头，3 塔吊，4 升降机，5 卸料平台，6 深基坑，7 高支模，8 临边围挡（旧）,9 实名制</param>
        /// <param name="keyword">设备编号、名称、所属区域</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/[action]")]
        public async Task<IResponseOutput> WarnLiveList(int SITEID,int type, string keyword, DateTime startTime, DateTime endTime,
            int pageIndex = 1, int pageSize = 20)
        {
            var result = await _warningService.spV2WarnLiveList(type, SITEID, keyword, startTime, endTime, pageSize, pageIndex);
            return ResponseOutput.Ok(result);
        }
    }
}