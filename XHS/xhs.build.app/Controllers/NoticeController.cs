using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Repository.SystemSetting;
using XHS.Build.Services.Notice;
using XHS.Build.Services.SystemSetting;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NoticeController : ControllerBase
    {
        private readonly INoticeService _noticeService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IHpSystemSetting _hpSystemSetting;
        public NoticeController(INoticeService noticeService, ISystemSettingService systemSettingService, ISystemSettingRepository systemSettingRepository, IHpSystemSetting hpSystemSetting)
        {
            _noticeService = noticeService;
            _systemSettingService = systemSettingService;
            _systemSettingRepository = systemSettingRepository;
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 获取公告
        /// </summary>
        /// <param name="operatedate">操作时间</param>
        [HttpGet]
        [Route("getList")]
        public async Task<IResponseOutput> GetList(DateTime operatedate)
        {
            if (operatedate == null)
                return ResponseOutput.NotOk();

            List<BnNotice> result = new List<BnNotice>();

            try
            {
                DataTable dt = await _noticeService.getListByUser(operatedate);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnNotice bn = new BnNotice();
                    bn.NOTICEID = Convert.ToInt32(dt.Rows[i]["NOTICEID"]);
                    bn.GROUPID = Convert.ToInt32(dt.Rows[i]["GROUPID"]);
                    bn.title = Convert.ToString(dt.Rows[i]["title"]);
                    bn.category = Convert.ToString(dt.Rows[i]["category"]);
                    //bn.content = Convert.ToString(dt.Rows[i]["content"]);
                    bn.startdate = Convert.ToDateTime(dt.Rows[i]["startdate"]);
                    bn.enddate = UDataRow.ToDateTime(dt.Rows[i], "enddate", Convert.ToDateTime("2099-12-31"));
                    bn.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);

                    result.Add(bn);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 通过分组获取公告列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getListByGroup")]
        public async Task<IResponseOutput> GetListByGroup()
        {
            return ResponseOutput.Ok(await _noticeService.getList());
        }

        /// <summary>
        /// 发送污染超标公告
        /// </summary>
        /// <param name="bnp">污染数据公告Bean</param>
        /// <returns></returns>
        [HttpGet]
        [Route("send")]
        public async Task<IResponseOutput> Send(BnNoticePollute bnp)
        {
            if (bnp == null)
                return ResponseOutput.NotOk();

            int result = 0;
            try
            {
                //先取发送对象列表
                DataTable dt = await _noticeService.getUserBySite(bnp.GROUPID, bnp.SITEID);
                string USERIDS = string.Empty;

                foreach (DataRow dr in dt.Rows)
                {
                    USERIDS += dr["USERID"] + Const.Symbol.COMMA;
                }
                if (!string.IsNullOrEmpty(USERIDS))
                {
                    USERIDS = USERIDS.Substring(0, USERIDS.Length - 1);
                }
                //拼内容
                string content = string.Format(_hpSystemSetting.getSettingValue(Const.Setting.S029)
                    , bnp.datatime
                    , bnp.datatype
                    , bnp.pm2_5
                    , bnp.pm10
                    , bnp.tsp
                    , bnp.noise);

                result = await _noticeService.doInsert(new { GROUPID = bnp.GROUPID, title = "监测值超标警告", content = content, category = "超标警告", startdate = DateTime.Now.ToString("yyyy-MM-dd"), @operator = bnp.username, USERIDS = USERIDS });
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message); ;
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取单个公告及图像资源
        /// </summary>
        /// <param name="NOTICEID">公告ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getOne")]
        public async Task<IResponseOutput> GetOne(int NOTICEID)
        {
            if (NOTICEID <= 0)
                return ResponseOutput.NotOk();

            DataSet ds = await _noticeService.getOne(NOTICEID);
            DataTable dt = await _noticeService.getFileList(NOTICEID);

            return ResponseOutput.Ok(new DataTableHelp().UniteDataTable(ds.Tables[0], dt, ""));

        }

        /// <summary>
        /// 获取单个公告及图像资源
        /// </summary>
        /// <param name="WEBNOTICEID">公告ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWebNoticeOne")]
        public async Task<IResponseOutput> GetWebNoticeOne(int WEBNOTICEID)
        {
            if (WEBNOTICEID <= 0)
                return ResponseOutput.NotOk("请选择需要查看的数据");

            return ResponseOutput.Ok(await _noticeService.getWebNoticeOne(WEBNOTICEID));

        }
        /// <summary>
        /// 通过分组获取有效公告列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetWebNoticeActiveListByGroup")]
        public async Task<IResponseOutput> GetWebNoticeActiveListByGroup()
        {
            return ResponseOutput.Ok(await _noticeService.getWebNoticeActiveListByGroup());
        }
    }
}