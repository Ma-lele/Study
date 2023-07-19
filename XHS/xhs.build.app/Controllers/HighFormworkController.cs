using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.HighFormwork;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 高支模
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HighFormworkController : ControllerBase
    {
        private readonly IHighFormworkService _highFormworkService;
        private readonly IUser _user;
        public HighFormworkController(IHighFormworkService highFormworkService,IUser user)
        {
            _highFormworkService = highFormworkService;
            _user = user;
        }

        /// <summary>
        /// 获取项目区域，设备信息
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsitehighformworklist")]
        public async Task<IResponseOutput> GetSiteHighFormworkList(int SITEID)
        {
            DataSet ds = await _highFormworkService.GetSiteHighFormworkList(_user.GroupId,SITEID);
            return ResponseOutput.Ok(ds);
        }

        /// <summary>
        /// 获取告警统计
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsitehighformworkwarncount")]
        public async Task<IResponseOutput> GetSiteHighFormworkWarnCount(int SITEID)
        {
            DataSet ds = await _highFormworkService.GetSiteHighFormworkWarnCount(SITEID);
            return ResponseOutput.Ok(ds);
        }

        /// <summary>
        /// 获取告警记录
        /// </summary>
        /// <param name="hfwcode">设备编号</param>
        /// <param name="date">日期(yyyy-mm-dd)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsitehighformworkwarnlist")]
        public async Task<IResponseOutput> GetSiteHighFormworkWarnList(string hfwcode,DateTime date)
        {
            DataTable dt = await _highFormworkService.GetSiteHighFormworkWarnHis(hfwcode, date, date);
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取项目高支模图表信息app
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="HFWAID"></param>
        /// <param name="hfwcode"></param>
        /// <param name="spotcode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsitehighformworkappchart")]
        public async Task<IResponseOutput> GetSiteHighFormworkAppChart(int SITEID, int HFWAID, string hfwcode, string spotcode)
        {
            DataSet ds = await _highFormworkService.GetSiteHighFormworkAppChart(SITEID, HFWAID, hfwcode, spotcode);
            return ResponseOutput.Ok(ds);
        }

        /// <summary>
        /// 获取项目高支模历史数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="HFWAID"></param>
        /// <param name="hfwcode"></param>
        /// <param name="spotcode"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getsitehighformworkhisdata")]
        public async Task<IResponseOutput> GetSiteHighFormworkHisData(int SITEID, int HFWAID, string hfwcode, string spotcode, DateTime startdate, DateTime enddate)
        {
            DataTable dt = await _highFormworkService.GetSiteHighFormworkHisData(SITEID, HFWAID, hfwcode, spotcode, startdate, enddate);
            return ResponseOutput.Ok(dt);
        }
    }
}