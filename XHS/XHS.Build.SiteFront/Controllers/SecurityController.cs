using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Security;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 移动巡检
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ISecurityService _securityService;
        private readonly IHpSystemSetting _hpSystemSetting;

        public SecurityController(IUser user, ISecurityService securityService, IHpSystemSetting hpSystemSetting)
        {
            _user = user;
            _securityService = securityService;
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 移动巡检-左上角统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T053/[action]")]
        public async Task<IResponseOutput> Total()
        {
            var result = await _securityService.spV2SecurityStats(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 移动巡检-巡更点信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T053/[action]")]
        public async Task<IResponseOutput> Point()
        {
            var result = await _securityService.spV2SecurityPoint(_user.SiteId.ToInt());
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 巡更次数统计
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T053/[action]")]
        public async Task<IResponseOutput> Line(DateTime startDate,DateTime endDate)
        {
            int days = (endDate - startDate).TotalDays.ToInt();
            if (days <= 0 || days > 365)
            {
                return ResponseOutput.NotOk("日期超限");
            }

            var result = await _securityService.spV2SecurityHisCount(_user.SiteId.ToInt(), startDate, endDate);
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 历史记录
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="SECURITYID">点位ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T053/[action]")]
        public async Task<IResponseOutput> History(string keyword, DateTime startDate, DateTime endDate,
            int SECURITYID = -1, int pageIndex = 1, int pageSize = 20)
        {
            int days = (endDate - startDate).TotalDays.ToInt();
            if (days <= 0 || days > 365)
            {
                return ResponseOutput.NotOk("日期超限");
            }

            var result = await _securityService.spV2SecurityHisList(
                _user.SiteId.ToInt(), keyword, startDate, endDate, SECURITYID, pageIndex, pageSize
                );
            return ResponseOutput.Ok(result);
        }


        /// <summary>
        /// 巡检图片
        /// </summary>
        /// <param name="SCHISID">巡检记录id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T053/[action]")]
        public async Task<IResponseOutput> Image(int SCHISID)
        {
            var result = await _securityService.spV2SecurityHisImage(_user.SiteId.ToInt(), SCHISID);
            var list = result.ToDataList<FileEntity>();
            string S034 = _hpSystemSetting.getSettingValue("S034");
            foreach (var f in list)
            {
                f.path = "http://" + S034 + "/resourse/" + f.GROUPID + "/" + f.filetype + "/" + f.SITEID + "/" + f.linkid + "/" + f.FILEID + "." + f.filename.Split('.')[1];
                f.tmbpath = "http://" + S034 + "/resourse/" + f.GROUPID + "/" + f.filetype + "/" + f.SITEID + "/" + f.linkid + "/tmb_" + f.FILEID + "." + f.filename.Split('.')[1];
            }

            return ResponseOutput.Ok(list);
        }
    }
}
