using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Report;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 报告
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private const string FORMAT_DATE = "yyyy-MM-dd";
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// 获取距离截止日期最近一周的夜间噪声数据
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="enddate">一周的截止日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getNoiseNightReport")]
        public async Task<IResponseOutput> GetNoiseNightReport(int SITEID, DateTime enddate)
        {
            if (SITEID <= 0 || enddate == null)
                return ResponseOutput.NotOk();

            List<BnPolluteValue> result = new List<BnPolluteValue>();
            DateTime startdate = enddate.AddDays(-6);

            try
            {
                DataTable dt = await _reportService.getNoiseNight(SITEID, startdate.ToString(FORMAT_DATE), enddate.ToString(FORMAT_DATE));
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnPolluteValue bpv = new BnPolluteValue();
                    bpv.datatime = UDataRow.ToDateTime(dt.Rows[i], "datatime");
                    bpv.value = UDataRow.ToDecimal(dt.Rows[i], "noisenight");

                    result.Add(bpv);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取距离截止日期最近一周的噪声分布数据
        /// </summary>
        /// <param name="enddate">一周的截止日期</param>
        /// <param name="type">数据周期种类</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getNoiseDistributionReport")]
        public async Task<IResponseOutput> GetNoiseDistributionReport( DateTime enddate, short type)
        {
            if (enddate == null || type <= 0)
                return ResponseOutput.NotOk();

            List<List<BnNoiseDist>> result = new List<List<BnNoiseDist>>();
            DateTime startdate = DateTime.Now;
            if (type.Equals(1))
            {
                startdate = enddate.AddDays(-6);                // 一周的
            }
            else if (type.Equals(2))
            {
                startdate = enddate.AddDays(1 - enddate.Day);
                enddate = startdate.AddMonths(1).AddDays(-1);   // 一个月的
            }
            else
                return ResponseOutput.NotOk();


            try
            {
                DataSet ds = await _reportService.getNoiseDistribution(startdate.ToString(FORMAT_DATE), enddate.ToString(FORMAT_DATE));
                if (ds == null || ds.Tables.Count < 3)
                    return ResponseOutput.Ok(result);

                //全天
                List<BnNoiseDist> all = new List<BnNoiseDist>();
                DataTable dta = ds.Tables[0];
                for (int i = 0; i < dta.Rows.Count; i++)
                {
                    BnNoiseDist bpv = new BnNoiseDist();
                    bpv.SITEID = UDataRow.ToInt(dta.Rows[i], "SITEID");
                    bpv.sitename = Convert.ToString(dta.Rows[i]["sitename"]);
                    bpv.siteshortname = Convert.ToString(dta.Rows[i]["siteshortname"]);
                    bpv.percentok = UDataRow.ToDecimal(dta.Rows[i], "percentok");
                    bpv.percentng = UDataRow.ToDecimal(dta.Rows[i], "percentng");
                    bpv.type = 0;//0:全天;1:白天;2:夜间

                    all.Add(bpv);
                }

                //白天
                List<BnNoiseDist> day = new List<BnNoiseDist>();
                DataTable dtd = ds.Tables[1];
                for (int i = 0; i < dtd.Rows.Count; i++)
                {
                    BnNoiseDist bpv = new BnNoiseDist();
                    bpv.SITEID = UDataRow.ToInt(dtd.Rows[i], "SITEID");
                    bpv.sitename = Convert.ToString(dtd.Rows[i]["sitename"]);
                    bpv.siteshortname = Convert.ToString(dtd.Rows[i]["siteshortname"]);
                    bpv.percentok = UDataRow.ToDecimal(dtd.Rows[i], "percentok");
                    bpv.percentng = UDataRow.ToDecimal(dtd.Rows[i], "percentng");
                    bpv.type = 1;//0:全天;1:白天;2:夜间

                    day.Add(bpv);
                }

                //夜间
                List<BnNoiseDist> night = new List<BnNoiseDist>();
                DataTable dtn = ds.Tables[2];
                for (int i = 0; i < dtn.Rows.Count; i++)
                {
                    BnNoiseDist bpv = new BnNoiseDist();
                    bpv.SITEID = UDataRow.ToInt(dtn.Rows[i], "SITEID");
                    bpv.sitename = Convert.ToString(dtn.Rows[i]["sitename"]);
                    bpv.siteshortname = Convert.ToString(dtn.Rows[i]["siteshortname"]);
                    bpv.percentok = UDataRow.ToDecimal(dtn.Rows[i], "percentok");
                    bpv.percentng = UDataRow.ToDecimal(dtn.Rows[i], "percentng");
                    bpv.type = 2;//0:全天;1:白天;2:夜间

                    night.Add(bpv);
                }
                result.Add(all);
                result.Add(day);
                result.Add(night);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 噪声周报
        /// </summary>
        /// <param name="startdate">开始日期</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getNoiseWeek")]
        public async Task<IResponseOutput> GetNoiseWeek(DateTime startdate)
        {
            return ResponseOutput.Ok(await _reportService.getNoiseWeek(startdate));
        }
        /// <summary>
        /// PM10/PM2.5平均浓度月报
        /// </summary>
        /// <param name="yearmonth">年月 例：2017-01</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPmMonth")]
        public async Task<IResponseOutput> GetPmMonth(string yearmonth)
        {
            if (string.IsNullOrEmpty(yearmonth))
            {
                return ResponseOutput.NotOk();
            }

            List<BnPmMonth> result = new List<BnPmMonth>();

            try
            {
                DataTable dt = await _reportService.getPmMonth(yearmonth, 0);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(result);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnPmMonth bpm = new BnPmMonth();
                    bpm.SITEID = UDataRow.ToInt(dt.Rows[i], "SITEID");
                    bpm.sitename = Convert.ToString(dt.Rows[i]["sitename"]);
                    bpm.siteshortname = Convert.ToString(dt.Rows[i]["siteshortname"]);
                    bpm.sitetype = UDataRow.ToInt(dt.Rows[i], "sitetype"); ;//0:全天;1:白天;2:夜间
                    bpm.monthpm10 = UDataRow.ToDecimal(dt.Rows[i], "monthpm10");
                    bpm.monthpm2_5 = UDataRow.ToDecimal(dt.Rows[i], "monthpm2_5");

                    result.Add(bpm);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取App简报数据
        /// </summary>   
        /// <returns></returns>
        [HttpGet]
        [Route("getAppSimpleReport")]
        public async Task<IResponseOutput> GetAppSimpleReport()
        {
            return ResponseOutput.Ok(await _reportService.getAppSimpleReport());
        }

        /// <summary>
        /// 获取监测点颗粒物分布
        /// </summary>   
        /// <returns></returns>
        [HttpGet]
        [Route("getSitePmDistribution")]
        public async Task<IResponseOutput> GetSitePmDistribution(int SITEID, DateTime startdate, DateTime enddate)
        {
            return ResponseOutput.Ok(await _reportService.getSitePmDistribution(SITEID, startdate, enddate));
        }

        /// <summary>
        /// 获取距离截止日期最近一周的PM10/PM2.5分布数据
        /// </summary>
        /// <param name="enddate">一周的截止日期</param>
        /// <param name="type">数据周期种类</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getPmDistributionReport")]
        public async Task<IResponseOutput> GetPmDistributionReport( DateTime enddate, short type)
        {
            if ( enddate == null || type <= 0)
                return ResponseOutput.NotOk();

            List<List<BnPmDist>> result = new List<List<BnPmDist>>();
            DateTime startdate = DateTime.Now;
            if (type.Equals(1))
            {
                startdate = enddate.AddDays(-6);                // 一周的
            }
            else if (type.Equals(2))
            {
                startdate = enddate.AddMonths(-1).AddDays(1);   // 一个月的
            }
            else
                return ResponseOutput.NotOk();

            try
            {
                DataSet ds = await _reportService.getPmDistribution(startdate.ToString(FORMAT_DATE), enddate.ToString(FORMAT_DATE));
                if (ds == null || ds.Tables.Count < 2)
                    return ResponseOutput.Ok(result);

                //Pm10
                List<BnPmDist> pm10s = new List<BnPmDist>();
                DataTable dta = ds.Tables[0];
                for (int i = 0; i < dta.Rows.Count; i++)
                {
                    BnPmDist bpd = new BnPmDist();
                    bpd.SITEID = UDataRow.ToInt(dta.Rows[i], "SITEID");
                    bpd.sitename = Convert.ToString(dta.Rows[i]["sitename"]);
                    bpd.siteshortname = Convert.ToString(dta.Rows[i]["siteshortname"]);
                    bpd.lv1 = UDataRow.ToInt(dta.Rows[i], "lv1");
                    bpd.lv2 = UDataRow.ToInt(dta.Rows[i], "lv2");
                    bpd.lv3 = UDataRow.ToInt(dta.Rows[i], "lv3");
                    bpd.lv4 = UDataRow.ToInt(dta.Rows[i], "lv4");
                    bpd.lv5 = UDataRow.ToInt(dta.Rows[i], "lv5");
                    bpd.lv6 = UDataRow.ToInt(dta.Rows[i], "lv6");
                    bpd.type = 0;//0:Pm10;1:Pm2.5

                    pm10s.Add(bpd);
                }

                //Pm2.5
                List<BnPmDist> pm2_5s = new List<BnPmDist>();
                DataTable dtn = ds.Tables[1];
                for (int i = 0; i < dtn.Rows.Count; i++)
                {
                    BnPmDist bpd = new BnPmDist();
                    bpd.SITEID = UDataRow.ToInt(dtn.Rows[i], "SITEID");
                    bpd.sitename = Convert.ToString(dtn.Rows[i]["sitename"]);
                    bpd.siteshortname = Convert.ToString(dtn.Rows[i]["siteshortname"]);
                    bpd.lv1 = UDataRow.ToInt(dtn.Rows[i], "lv1");
                    bpd.lv2 = UDataRow.ToInt(dtn.Rows[i], "lv2");
                    bpd.lv3 = UDataRow.ToInt(dtn.Rows[i], "lv3");
                    bpd.lv4 = UDataRow.ToInt(dtn.Rows[i], "lv4");
                    bpd.lv5 = UDataRow.ToInt(dtn.Rows[i], "lv5");
                    bpd.lv6 = UDataRow.ToInt(dtn.Rows[i], "lv6");
                    bpd.type = 1;//0:Pm10;1:Pm2.5

                    pm2_5s.Add(bpd);
                }
                result.Add(pm10s);
                result.Add(pm2_5s);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk();
            }
            return ResponseOutput.Ok(result);
        }
    }
}