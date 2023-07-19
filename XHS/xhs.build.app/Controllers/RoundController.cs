using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Round;
using XHS.Build.Services.SmsQueue;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 移动巡检
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoundController : ControllerBase
    {
        private readonly IRoundService _roundService;
        private readonly IHpRoundProof _hpRoundProof;
        private readonly ISmsQueueService _smsQueueService;
        private readonly IUser _user;
        public RoundController(IRoundService roundService, IHpRoundProof hpRoundProof, ISmsQueueService smsQueueService, IUser user)
        {
            _roundService = roundService;
            _hpRoundProof = hpRoundProof;
            _smsQueueService = smsQueueService;
            _user = user;
        }

        /// <summary>
        /// 移动巡检 
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="SITEID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getroundchart")]
        public async Task<IResponseOutput> GetRoundChart(int SITEID, int type)
        {
            if (SITEID < 0)
            {
                return ResponseOutput.NotOk("请输入正确的参数");
            }
            return ResponseOutput.Ok(await _roundService.GetChartData(type.ToString(), SITEID));
        }

        /// <summary>
        /// 巡查插入
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="PHASEID">阶段ID</param>
        /// <param name="fellow">检查人员</param>
        /// <param name="solveduserid">解决用户ID</param>
        /// <param name="RTHID">问题类型历史ID</param>
        /// <param name="limit">解决时限,单位小时</param>
        /// <param name="roundtype">问题ID</param>
        /// <param name="createtime">日期</param>
        /// <param name="remark">备注</param>
        /// <param name="roundlevel">巡查等级</param>
        /// <returns>巡查单号</returns>
        [HttpPost]
        [Route("create")]
        public async Task<IResponseOutput> Create([FromForm] int SITEID, [FromForm] int PHASEID, [FromForm] string fellow, [FromForm] int solveduserid, [FromForm] int limit, [FromForm] int RTHID, [FromForm] string roundtype, [FromForm] DateTime createtime, [FromForm] string remark, [FromForm] string roundlevel)
        {
            if (SITEID < 1 || solveduserid < 0  || PHASEID < 1)
                return ResponseOutput.NotOk("请输入正确的参数");

            try
            {
                int status = 1;
                if (string.IsNullOrEmpty(roundtype) || roundtype == "0")
                {
                    status = 5;
                }
                object param = new { USERID = _user.Id, SITEID = SITEID, solveduserid = solveduserid, limit = limit, RTHID= RTHID, roundtype = roundtype, fellow = fellow,status = status, roundlevel = string.IsNullOrEmpty(roundlevel) ? "" : roundlevel, remark = remark, PHASEID = PHASEID };

                DataTable dt = await _roundService.doInsert(param);
                return ResponseOutput.Ok(dt);
            }
            catch (Exception ex)
            {
                return ResponseOutput.Ok("添加巡查记录失败");
            }

        }


        /// <summary>
        /// 删除巡检
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="ROUNDID">巡检ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IResponseOutput> Delete(int SITEID, int ROUNDID)
        {
            if (SITEID < 1 || ROUNDID < 1)
                return ResponseOutput.NotOk("请输入正确的参数");

            int result = 0;
            try
            {
                result = await _roundService.doDelete(ROUNDID);
                if (result > 0)
                {
                    //result = HpRoundProof.doDeleteAll(GROUPID, SITEID, ROUNDID);
                }
            }
            catch (Exception ex)
            {
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 巡查问题添加批示
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Route("remarkadd")]
        public async Task<IResponseOutput> RemarkAdd([FromForm] long ROUNDID, [FromForm] string remark)
        {
            if (ROUNDID < 1 || string.IsNullOrEmpty(remark))
                return ResponseOutput.NotOk<int>("请填写处理信息");

            int result = 0;
            try
            {
                result = await _roundService.doRemarkAdd(ROUNDID, remark);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>("信息提交发生错误");
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 巡查问题解决
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <param name="solveduser">解决用户名</param>
        /// <param name="solvedremark">解决日志</param>
        /// <returns></returns>
        [HttpPost]
        [Route("solve")]
        public async Task<IResponseOutput> Solve([FromForm] long ROUNDID, [FromForm] string solvedremark)
        {
            if (ROUNDID < 1 || string.IsNullOrEmpty(solvedremark))
                return ResponseOutput.NotOk<int>("请填写处理信息");

            int result = 0;
            try
            {
                result = await _roundService.doSolve(ROUNDID, solvedremark);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>("信息提交发生错误");
            }
            return ResponseOutput.Ok<int>(result);
        }

        /// <summary>
        /// 修改监测点问题项
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="rtcontent">问题项</param>
        /// <returns></returns>
        [HttpPost]
        [Route("doRoundTypeInsert")]
        public async Task<IResponseOutput> DoRoundTypeInsert([FromForm] int SITEID, [FromForm] string rtcontent)
        {
            if (SITEID < 1)
                return ResponseOutput.NotOk<int>("请填写处理信息");

            int result = 0;
            try
            {
                result = await _roundService.DoRoundTypeInsert(SITEID, rtcontent);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>("信息提交发生错误");
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 结束巡查问题
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("finish")]
        public async Task<IResponseOutput> Finish([FromForm] long ROUNDID, [FromForm] string remark)
        {
            if (ROUNDID < 1)
                return ResponseOutput.NotOk<int>("请填写处理信息");

            int result = 0;
            try
            {
                result = await _roundService.doFinish(ROUNDID, remark);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>("信息提交发生错误");
            }
            return ResponseOutput.Ok(result);
        }
        /// <summary>
        /// 检索巡查
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="updatedate">最后更新时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getListByDate")]
        public async Task<IResponseOutput> GetListByDate(int SITEID, DateTime updatedate)
        {
            if (SITEID <= 0 || updatedate == null)
                return ResponseOutput.NotOk("请输入正确的参数");

            DataTable dt = await _roundService.getListByDate(SITEID, updatedate);
            return ResponseOutput.Ok(dt);

        }

        /// <summary>
        /// 检索问题类型
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="RTHID">问题类型历史ID（-1时，最新有效）</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getRoundType")]
        public async Task<IResponseOutput> GetRoundType(int SITEID, int RTHID)
        {
            if (SITEID <= 0 )
                return ResponseOutput.NotOk("请输入正确的参数");

            DataTable dt = await _roundService.GetRoundType(SITEID, RTHID);
            return ResponseOutput.Ok(dt);

        }

        /// <summary>
        /// 根据用户检索巡查
        /// </summary>
        /// <param name="updatedate">最后更新时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getListByUser")]
        public async Task<IResponseOutput> GetListByUser(DateTime updatedate)
        {
            if (updatedate == null)
                return ResponseOutput.NotOk("请输入正确的参数");

            DataTable dt = await _roundService.getListByUser(updatedate);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 移动巡检 移动端统计列表
        /// </summary>
        /// <param name="datetype">0:全部，1：天，2：周，3：月，4：3月</param>
        /// <param name="orderby">0:综合排序，1：待整改，2：检查单，3：整改次数，4：累计问题</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMobileCountList")]
        public async Task<IResponseOutput> GetMobileCountList(int datetype = 0, int orderby = 0)
        {
            return ResponseOutput.Ok(await _roundService.GetMobileListCount(datetype, orderby));
        }

        /// <summary>
        /// 上传任务相关文件
        /// </summary>
        [HttpPost]
        [Route("uploadProff")]
        public async Task<IResponseOutput> UploadProff(RoundFileInput input)
        {
            if (input == null || input.SITEID.Equals(0) || string.IsNullOrEmpty(input.filename)
                || string.IsNullOrEmpty(input.fileString))
                return ResponseOutput.NotOk<int>("请填写正确的参数", 0);

            int result = 0;
            try
            {
                result = _hpRoundProof.doUpload(_user.GroupId, input.SITEID, input.ROUNDID, input.filename, input.fileString, input.filesize);
                return result <= 0 ? ResponseOutput.NotOk<int>("文件上传失败", 0) : ResponseOutput.Ok(result);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk<int>(ex.Message, 0);
            }
        }

        /// <summary>
        /// 设置短信提醒
        /// </summary>
        /// <param name="GROUPID">GROUPID</param>
        /// <param name="SITEID">SITEID</param>
        /// <param name="ROUNDID">巡检</param>
        /// <param name="phonenumber">电话号码,逗号分隔</param>
        /// <param name="username">用户名,逗号分隔</param>
        /// <param name="sitename">监测点名称</param>
        /// <param name="roundcode">巡检单号</param>
        /// <param name="roundstatus">巡检状态(新建时为空)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("setSmsQueue")]
        public async Task<IResponseOutput> SetSmsQueue([FromForm] int SITEID, [FromForm] ulong ROUNDID, [FromForm] string phonenumber, [FromForm] string username, [FromForm] string sitename, [FromForm] string roundcode, [FromForm] string roundstatus, [FromForm] string uuids = "")
        {
            if (SITEID < 1 || ROUNDID < 1 || string.IsNullOrEmpty(phonenumber) || string.IsNullOrEmpty(username)
      || string.IsNullOrEmpty(sitename) || string.IsNullOrEmpty(roundcode))
            {
                return ResponseOutput.NotOk("请输入正确的参数");
            }

            int result = 0;
            try
            {
                Dictionary<string, string> jsonparam = new Dictionary<string, string>();
                var param1 = new SugarParameter("@GROUPID", _user.GroupId);
                var param2 = new SugarParameter("@SITEID", SITEID);
                var param3 = new SugarParameter("@linkid", ROUNDID);
                var param4 = new SugarParameter("@sqtype", 1);
                var param5 = new SugarParameter("@phonenumber", phonenumber);
                var param6 = new SugarParameter("@username", username);
                var param7 = new SugarParameter("@templatecode", "");
               // var param9 = new SugarParameter("@uuid", uuids);
                if (string.IsNullOrEmpty(roundstatus))
                {
                    param7.Value = HpAliSMS.TPL_ROUND_NEW;
                }
                else
                {
                    param7.Value = HpAliSMS.TPL_ROUND_UPDATE;
                    jsonparam.Add("roundstatus", roundstatus);
                }
                jsonparam.Add("sitename", sitename);
                jsonparam.Add("roundcode", roundcode);
                var param8 = new SugarParameter("@jsonparam", JsonConvert.SerializeObject(jsonparam));

                result = await _smsQueueService.doInsert(param1, param2, param3, param4, param5, param6, param7, param8);
               // result = await _smsQueueService.doInsert(param1, param2, param3, param4, param5, param6, param7, param8, param9);
                if (result > 0)//即时触发提醒
                    _smsQueueService.SendSmsAll();
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 巡查图片列表
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getProffList")]
        public async Task<IResponseOutput> GetProffList(ulong ROUNDID)
        {
            if (ROUNDID <= 0)
                return ResponseOutput.NotOk("请输入正确的参数");

            List<BnProof> result = new List<BnProof>();
            try
            {
                DataTable dt = _hpRoundProof.getList(ROUNDID);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnProof bp = new BnProof();
                    bp.ROUNDID = Convert.ToUInt64(dt.Rows[i]["ROUNDID"]);
                    bp.PROOFID = Convert.ToString(dt.Rows[i]["PROOFID"]);
                    bp.filename = Convert.ToString(dt.Rows[i]["filename"]);
                    bp.filesize = Convert.ToInt32(dt.Rows[i]["filesize"]);
                    bp.bsolved = Convert.ToInt16(dt.Rows[i]["bsolved"]);
                    bp.createtime = Convert.ToDateTime(dt.Rows[i]["createtime"]);
                    result.Add(bp);
                }

            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ResponseOutput.NotOk("查询失败");
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 移动巡检，待办工单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("roundorder")]
        public async Task<IResponseOutput> GetRoundOrderList(int siteid, int roundtype, int status, int datetype, int page, int size)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (size < 1)
            {
                size = 1;
            }
            
            return ResponseOutput.Ok(await _roundService.GetOrderList(siteid, roundtype, status, datetype, page, size));
        }

        /// <summary>
        /// 获取站点下的待办
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="roundtype"></param>
        /// <param name="status"></param>
        /// <param name="datetype"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("roundordersite")]
        public async Task<IResponseOutput> GetRoundOrderSiteList(int siteid, int roundtype, int status, int datetype, int page, int size)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (size < 1)
            {
                size = 1;
            }
            return ResponseOutput.Ok(await _roundService.GetOrderListWithoutRole(siteid, roundtype, status, datetype, page, size));
        }
    }
}