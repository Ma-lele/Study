using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Njjy;
using XHS.Build.Services.Phase;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 阶段
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhaseController : ControllerBase
    {
        private readonly IPhaseService _phaseService;
        private readonly INjjyService _njjyService;
        private const string FILE_PATH = @"/{0}/{1}/{2}{3}{4}";
        public PhaseController(IPhaseService phaseService, INjjyService njjyService)
        {
            _phaseService = phaseService;
            _njjyService = njjyService;
        }

        /// <summary>
        /// 获取施工阶段
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getlist")]
        public async Task<IResponseOutput> GetList()
        {
            List<BnSitePhase> result = new List<BnSitePhase>();
            try
            {
                DataTable dt = await _phaseService.getAll();
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSitePhase bsp = new BnSitePhase();
                    bsp.PHASEID = Convert.ToInt32(dt.Rows[i]["PHASEID"]);
                    bsp.parentid = Convert.ToInt32(dt.Rows[i]["parentid"]);
                    bsp.phaseorder = Convert.ToInt32(dt.Rows[i]["phaseorder"]);
                    bsp.phasename = Convert.ToString(dt.Rows[i]["phasename"]);

                    result.Add(bsp);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取监测对象的施工阶段的修改履历
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="operatedate">最后更新时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getHisList")]
        public async Task<IResponseOutput> GetHisList(int SITEID, DateTime operatedate)
        {
            if (SITEID <= 0 || operatedate == null)
                return ResponseOutput.NotOk();

            List<BnSitePhaseHis> result = new List<BnSitePhaseHis>();
            try
            {
                DataTable dt = await _phaseService.getHisList(SITEID, operatedate);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.NotOk();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSitePhaseHis bsp = new BnSitePhaseHis();
                    bsp.PHASEHISID = Convert.ToUInt64(dt.Rows[i]["PHASEHISID"]);
                    bsp.PHASEID = Convert.ToInt32(dt.Rows[i]["PHASEID"]);
                    bsp.SITEID = Convert.ToInt32(dt.Rows[i]["SITEID"]);
                    bsp.photocount = Convert.ToInt32(dt.Rows[i]["photocount"]);
                    bsp.phasenamefrom = Convert.ToString(dt.Rows[i]["phasenamefrom"]);
                    bsp.phasenameto = Convert.ToString(dt.Rows[i]["phasenameto"]);
                    bsp.phasepercent = Convert.ToString(dt.Rows[i]["phasepercent"]);
                    bsp.phasedate = Convert.ToDateTime(dt.Rows[i]["phasedate"]);
                    bsp.username = Convert.ToString(dt.Rows[i]["operator"]);
                    bsp.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);

                    result.Add(bsp);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取监测对象的施工阶段的修改履历(njjy版)
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="operatedate">最后更新时间</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getNjjyHisList")]
        public async Task<IResponseOutput> GetNjjyHisList(int SITEID, DateTime operatedate)
        {
            if (SITEID <= 0 || operatedate == null)
                return ResponseOutput.NotOk();

            return ResponseOutput.Ok(await _njjyService.getPhaseHisList(SITEID, operatedate));
        }

        /// <summary>
        /// 获取监测对象的施工阶段的修改履历(建邺专用)
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getNjjyHisUpdated")]
        public async Task<IResponseOutput> GetNjjyHisUpdated(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(await _njjyService.getPhaseHisList(SITEID));
        }

        /// <summary>
        /// 获取照片列表
        /// </summary>
        /// <param name="PHASEHISID">进度历史ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPhotoList")]
        public async Task<IResponseOutput> GetPhotoList(long PHASEHISID)
        {
            if (PHASEHISID <= 0)
                return ResponseOutput.NotOk();

            List<string> result = new List<string>();
            try
            {
                DataTable dt = await _phaseService.getPhotoList(PHASEHISID);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.NotOk();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string filename = ""; //string.Format(FILE_PATH, dt.Rows[i]["path"], dt.Rows[i]["SITEID"], HpPhasePhoto.THUMB_PREFIX, dt.Rows[i]["SPPID"], dt.Rows[i]["ext"]);
                    result.Add(filename);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 更新监测点施工阶段
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="constructphase">施工阶段ID</param>
        /// <param name="phasepercent">施工进度百分比</param>
        /// <param name="phasepercent">施工进度日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("setSitePhase")]
        public async Task<IResponseOutput> SetSitePhase(int SITEID, int constructphase, int phasepercent, DateTime phasedate)
        {
            if (SITEID < 1 || constructphase < 1 || phasepercent < 0)
                return ResponseOutput.NotOk();
            var result = await _phaseService.doUpdate(SITEID, constructphase, phasepercent, phasedate);
            return result > 0 ? ResponseOutput.Ok() : ResponseOutput.NotOk("没有权限");
        }

        /// <summary>
        /// 获取监测对象的施工进度
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("GetSitePhaseList")]
        public async Task<IResponseOutput> GetSitePhaseList(int SITEID)
        {
            if (SITEID < 1) {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(await _phaseService.GetSitePhaseList(SITEID));
        }

        /// <summary>
        /// 更新监测对象的施工进度
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="PHASEIDS">进度ID</param>
        /// <param name="phasedates">更新时间</param>
        /// <param name="phasedatetype">更新类型</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("UpdateSitePhase")]
        public async Task<IResponseOutput> UpdateSitePhase(int SITEID, string PHASEIDS, string phasedates, int phasedatetype)
        {
            if (SITEID < 1)
            {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(await _phaseService.UpdateSitePhase(SITEID, PHASEIDS, phasedates, phasedatetype));
        }
    }
}