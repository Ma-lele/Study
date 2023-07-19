using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Dust;
using XHS.Build.Services.SpecialEqpDoc;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.Model.Models;
using Newtonsoft.Json.Linq;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 特种设备相关接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpecialEqpController : ControllerBase
    {
        private readonly IDustService _dustService;
        private readonly ISpecialEqpDocService _specialEqpDocService;
        private readonly IHpSpecialEqpDoc _hpSpecialEqpDoc;
        private readonly IHpSystemSetting _hpSystemSetting;
        public SpecialEqpController(IHpSystemSetting hpSystemSetting, IDustService dustService, ISpecialEqpDocService specialEqpDocService, IHpSpecialEqpDoc hpSpecialEqpDoc)
        {
            _dustService = dustService;
            _hpSystemSetting = hpSystemSetting;
            _specialEqpDocService = specialEqpDocService;
            _hpSpecialEqpDoc = hpSpecialEqpDoc;
        }
        /// <summary>
        ///  获取特设列表
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getspecialeqplist")]
        public async Task<IResponseOutput> GetSpecialEqpList(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk("请填写正确的参数");


            return ResponseOutput.Ok(await _dustService.getListForSite(SITEID));

        }
        /// <summary>
        ///  获取特设文件列表
        /// </summary>
        /// <param name="SEID">特设ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getSpecialEqpDocList")]
        public async Task<IResponseOutput> GetSpecialEqpDocList(int SEID)
        {
            if (SEID <= 0)
                return ResponseOutput.NotOk("请填写正确的参数");

            List<BnSpecialEqpDoc> result = new List<BnSpecialEqpDoc>();
            try
            {
                DataSet ds = await _specialEqpDocService.getList(SEID);
                if (ds == null || ds.Tables[0].Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    BnSpecialEqpDoc bsed = new BnSpecialEqpDoc();
                    bsed.filetype = Convert.ToInt32(ds.Tables[0].Rows[i]["filetype"]);
                    bsed.SEDOCID = Convert.ToString(ds.Tables[0].Rows[i]["SEDOCID"]);
                    bsed.filename = Convert.ToString(ds.Tables[0].Rows[i]["filename"]);
                    bsed.fileex = Convert.ToString(ds.Tables[0].Rows[i]["fileex"]);
                    bsed.updatedate = Convert.ToDateTime(ds.Tables[0].Rows[i]["updatedate"]);

                    result.Add(bsed);
                }

            }
            catch (Exception ex)
            {
                ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(result);
        }
        /// <summary>
        /// 删除特设文件
        /// </summary>
        /// <param name="SEDOCID">特设文件ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("deleteDoc")]
        public IResponseOutput DeleteDoc(string SEDOCID)
        {
            if (string.IsNullOrEmpty(SEDOCID))
                return ResponseOutput.NotOk("请选择需要删除的文件");

            int result = 0;
            try
            {
                result = _hpSpecialEqpDoc.doDelete(SEDOCID);
                return ResponseOutput.Ok(result);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ResponseOutput.NotOk("删除文件发生错误");
            }
            
        }
        /// <summary>
        /// 获取建邺特设列表
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getNjjySiteSpec")]
        public IResponseOutput GetNjjySiteSpec(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk("请选择需要查看的站点");

            try
            {
                DataSet ds = _specialEqpDocService.getNjjySiteSpec(SITEID);
                
                return ResponseOutput.Ok(ds);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ResponseOutput.NotOk("查看站点内容出错");
            }
        }

        /// <summary>
        /// 获取特设最新实时数据
        /// </summary>
        /// <param name="SEID">特设ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getSpecialEqpRdtOne")]
        public IResponseOutput GetSpecialEqpRdtOne(int SEID)
        {
            if (SEID <= 0)
                return ResponseOutput.NotOk("请选择需要查看信息"); ;

            string result = null;
            try
            {
                DataSet ds = _specialEqpDocService.getRealOne(SEID);
                
                return ResponseOutput.Ok(ds);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return ResponseOutput.NotOk("查看站点内容出错");
            }
        }

        /// <summary>
        ///  获取升降机历史数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="date">日期</param>
        /// <param name="sn">设备号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getelevatorhisdata")]
        public async Task<IResponseOutput> GetElevatorHisData(int pageIndex, DateTime date, string sn, int pageSize = 20)
        {
            DateTime startTime = date.Date;
            DateTime endTime = date.AddDays(1).Date;
            List<SpecialEqpData> retString = await _specialEqpDocService.GetListDataHis(sn, startTime, endTime, pageIndex, pageSize);
            JObject jso = new JObject();
            jso.Add("rows", JArray.FromObject(retString));
            return ResponseOutput.Ok(jso);

        }

        /// <summary>
        ///  获取塔吊历史数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="date">日期</param>
        /// <param name="sn">设备号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getowercranehisdata")]
        public async Task<IResponseOutput> GetTowerCraneHisData(int pageIndex, DateTime date, string sn, int pageSize = 20)
        {
            DateTime startTime = date.Date;
            DateTime endTime = date.AddDays(1).Date;
            List<SpecialEqpData> retString = await _specialEqpDocService.GetListDataHis(sn, startTime, endTime, pageIndex, pageSize);
            JObject jso = new JObject();
            jso.Add("rows", JArray.FromObject(retString));
            return ResponseOutput.Ok(jso);

        }
    }
}