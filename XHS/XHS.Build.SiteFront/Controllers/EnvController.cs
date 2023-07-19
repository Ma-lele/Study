using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.Env;
using XHS.Build.Services.Group;
using XHS.Build.Services.Site;
using XHS.Build.SiteFront.Attributes;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 环境监测
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    [Permission]
    public class EnvController : ControllerBase
    {
        private readonly IEnvService _envService;
        private readonly ISiteService _siteService;
        private readonly IUser _user;
        private readonly IGroupService _groupService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="envService"></param>
        public EnvController(IEnvService envService, ISiteService siteService, IGroupService groupService, IUser user)
        {
            _envService = envService;
            _siteService = siteService;
            _groupService = groupService;
            _user = user;
        }

        /// <summary>
        /// 获取子监测点扬尘设备数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetPmSiteSubList()
        {
            DataTable data = await _siteService.getChildListById(int.Parse(_user.SiteId));
           
            return ResponseOutput.Ok(data);

        }
        /// <summary>
        /// 获取超标预警值
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetWarnLine()
        {
            DataTable data = await _groupService.getWarnline();

            return ResponseOutput.Ok(data);

        }


        /// <summary>
        /// 获取监测点最新扬尘实时数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetPmSiteRtdData(int PARENTSITEID,int SITEID = 0)
        {
            if(SITEID == 0 || SITEID == int.Parse(_user.SiteId))
            {
                SITEID = int.Parse(_user.SiteId);
            }
            else
            {
                if(PARENTSITEID != int.Parse(_user.SiteId))
                {
                    return ResponseOutput.NotOk("参数不正确。");
                }
            }

            DataTable data = await _envService.GetPmSiteRtdData(SITEID);
            //if(data != null && data.Rows.Count > 0)
            //{
            //    return ResponseOutput.Ok(data.Rows[0]);
            //}
            //else
            //{
               
            //}
            return ResponseOutput.Ok(data);

        }

        /// <summary>
        /// 获取监测点扬尘实时，小时，日均数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetPmSiteChart(int PARENTSITEID,int SITEID = 0)
        {
            if (SITEID == 0 || SITEID == int.Parse(_user.SiteId))
            {
                SITEID = int.Parse(_user.SiteId);
            }
            else
            {
                if (PARENTSITEID != int.Parse(_user.SiteId))
                {
                    return ResponseOutput.NotOk("参数不正确。");
                }
            }
            DataSet data = await _envService.GetPmSiteChart(SITEID);
            
            return ResponseOutput.Ok(data);

        }


        /// <summary>
        /// 获取监测点超标告警统计
        /// </summary>
        /// <param name="PARENTSITEID"></param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetPmSiteDayWarnCount(int PARENTSITEID,DateTime startdate, DateTime enddate, int SITEID = 0)
        {
            if (SITEID == 0 || SITEID == int.Parse(_user.SiteId))
            {
                SITEID = int.Parse(_user.SiteId);
            }
            else
            {
                if (PARENTSITEID != int.Parse(_user.SiteId))
                {
                    return ResponseOutput.NotOk("参数不正确。");
                }
            }
            DataTable data = await _envService.GetPmSiteDayWarnCount(SITEID, startdate, enddate);

            return ResponseOutput.Ok(data);

        }


        /// <summary>
        /// 获取监测点离线柱状图
        /// </summary>
        /// <param name="PARENTSITEID"></param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetPmOnlineBarChart(int PARENTSITEID,DateTime startdate, DateTime enddate, int SITEID = 0)
        {
            if (SITEID == 0 || SITEID == int.Parse(_user.SiteId))
            {
                SITEID = int.Parse(_user.SiteId);
            }
            else
            {
                if (PARENTSITEID != int.Parse(_user.SiteId))
                {
                    return ResponseOutput.NotOk("参数不正确。");
                }
            }
            DataTable data = await _envService.GetPmOnlineBarChart(SITEID, startdate, enddate);

            return ResponseOutput.Ok(data);

        }

        /// <summary>
        /// 获取监测点扬尘历史数据
        /// </summary>
        /// <param name="PARENTSITEID"></param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <param name="datatype">1：分钟 2：小时 3：日均</param>
        /// <param name="pageindex">页数</param>
        /// <param name="pagesize">每页记录数</param>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetSiteDataHis(int PARENTSITEID, DateTime startdate, DateTime enddate, int datatype, int pageindex=1, int pagesize=10, int SITEID = 0)
        {
            if (SITEID == 0 || SITEID == int.Parse(_user.SiteId))
            {
                SITEID = int.Parse(_user.SiteId);
            }
            else
            {
                if (PARENTSITEID != int.Parse(_user.SiteId))
                {
                    return ResponseOutput.NotOk("参数不正确。");
                }
            }
            DataTable data = await _envService.GetSiteDataHis(SITEID, startdate, enddate, datatype, pageindex, pagesize);

            return ResponseOutput.Ok(data);

        }
        /// <summary>
        /// 获取监测点臭氧图表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetSiteO3Chart()
        {
            DataSet data = await _envService.GetSiteO3Chart(int.Parse(_user.SiteId));

            return ResponseOutput.Ok(data);

        }

        /// <summary>
        /// 获取监测点臭氧历史数据
        /// </summary>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <param name="datatype">--1：分钟 2：小时 3：日均 4：8小时滑动平均 5：日评价</param>
        /// <param name="pageindex">页数</param>
        /// <param name="pagesize">每页记录数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T021/[action]")]
        public async Task<IResponseOutput> GetSiteO3His(DateTime startdate, DateTime enddate, int datatype, int pageindex = 1, int pagesize = 10)
        {
            DataTable data = await _envService.GetSiteO3His(int.Parse(_user.SiteId), startdate, enddate, datatype, pageindex, pagesize);

            return ResponseOutput.Ok(data);

        }
    }
}
