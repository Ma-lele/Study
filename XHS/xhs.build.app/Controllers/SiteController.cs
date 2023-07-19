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
using XHS.Build.Common.Util;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Cameras;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.Dust;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Unload;
using XHS.Build.Services.Warning;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 监测站点
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SiteController : ControllerBase
    {
        private readonly IDeviceCNService _deviceCNService;
        private readonly ISiteService _siteService;
        private readonly IWarningService _warningService;
        private readonly ICameraService _cameraService;
        private readonly IDustService _dustService;
        private readonly IUnloadService _unloadService;
        private readonly IHpSystemSetting _hpSystemSetting;
        public SiteController(ISiteService siteService, IHpSystemSetting hpSystemSetting, IDeviceCNService deviceCNService, IWarningService warningService, ICameraService cameraService, IDustService dustService, IUnloadService unloadService)
        {
            _siteService = siteService;
            _deviceCNService = deviceCNService;
            _warningService = warningService;
            _cameraService = cameraService;
            _dustService = dustService;
            _unloadService = unloadService;
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 获取用户所属组下的监测对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getList")]
        public async Task<IResponseOutput> GetList()
        {
            DataTable dt = await _siteService.getListForMobile();
            string mapType = _hpSystemSetting.getSettingValue(Const.Setting.S144);
            if(mapType == "1")
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        double sitelat = (double) dr["sitelat"];
                        double sitelng = (double) dr["sitelng"];
                        if (sitelat > 0)
                        {
                            GPSPoint point= ConvertGPS.WGS84_to_BD09(sitelat, sitelng);
                            dr["sitelat"] = point.GetLat().ToString();
                            dr["sitelng"] = point.GetLng().ToString();
                        }
                    }
                }
            }
           
            return ResponseOutput.Ok(dt);
        }


        /// <summary>
        /// 获取用户所属组下的工地进度统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getphasecount")]
        public async Task<IResponseOutput> GetPhaseCount()
        {
            return ResponseOutput.Ok(await _siteService.getPhaseCount());
        }

        /// <summary>
        /// 获取用户所属组下的子监测对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getChildListForMobile")]
        public async Task<IResponseOutput> GetChildListForMobile()
        {
            DataTable dt = await _siteService.getChildListForMobile();
            string mapType = _hpSystemSetting.getSettingValue(Const.Setting.S144);
            if (mapType == "1")
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        double sitelat = (double)dr["sitelat"];
                        double sitelng = (double)dr["sitelng"];
                        if (sitelat > 0)
                        {
                            GPSPoint point = ConvertGPS.WGS84_to_BD09(sitelat, sitelng);
                            dr["sitelat"] = point.GetLat().ToString();
                            dr["sitelng"] = point.GetLng().ToString();
                        }
                    }
                }
            }
            return ResponseOutput.Ok(dt);
        }

        /// <summary>
        /// 获取监测对象的子监测对象
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getChildListById")]
        public async Task<IResponseOutput> GetChildListById(int SITEID)
        {
            return ResponseOutput.Ok(await _siteService.getChildListById(SITEID));
        }
        /// <summary>
        /// 获取监测点信息
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getOne")]
        public async Task<IResponseOutput> GetOne(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();
            return ResponseOutput.Ok(await _siteService.getOneApp(SITEID));
        }

        /// <summary>
        /// 获取监测点设备绑定情况
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getMenu")]
        public async Task<IResponseOutput> GetMenu(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();
            return ResponseOutput.Ok(await _siteService.getMenu(SITEID));
        }

        /// <summary>
        /// 获取监测对象24小时内的主要污染数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get24HoursData")]
        public async Task<IResponseOutput> Get24HoursData(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            List<BnSite24Hours> result = new List<BnSite24Hours>();
            try
            {
                DataTable dt = await _siteService.get24HoursData(SITEID);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSite24Hours bsh = new BnSite24Hours();
                    bsh.SITEID = UDataRow.ToInt(dt.Rows[i], "SITEID");
                    bsh.datatime = Convert.ToString(dt.Rows[i]["datatime"]);
                    bsh.tsp = UDataRow.ToDecimal(dt.Rows[i], "tsp");
                    bsh.pm2_5 = UDataRow.ToDecimal(dt.Rows[i], "pm2_5");
                    bsh.pm10 = UDataRow.ToDecimal(dt.Rows[i], "pm10");
                    bsh.noise = UDataRow.ToDecimal(dt.Rows[i], "noise");
                    bsh.tspmax = UDataRow.ToDecimal(dt.Rows[i], "tspmax");
                    bsh.pm2_5max = UDataRow.ToDecimal(dt.Rows[i], "pm2_5max");
                    bsh.pm10max = UDataRow.ToDecimal(dt.Rows[i], "pm10max");
                    bsh.noisemax = UDataRow.ToDecimal(dt.Rows[i], "noisemax");

                    result.Add(bsh);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 获取监测对象近60分钟内的主要污染数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get60MinutesData")]
        public async Task<IResponseOutput> Get60MinutesData(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk("请选择站点信息");

            List<BnSite24Hours> result = new List<BnSite24Hours>();
            try
            {
                DataTable dt = await _siteService.get60MinutesData(SITEID);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSite24Hours bsh = new BnSite24Hours();
                    bsh.SITEID = UDataRow.ToInt(dt.Rows[i], "SITEID");
                    bsh.datatime = Convert.ToString(dt.Rows[i]["datatime"]);
                    bsh.tsp = UDataRow.ToDecimal(dt.Rows[i], "tsp");
                    bsh.pm2_5 = UDataRow.ToDecimal(dt.Rows[i], "pm2_5");
                    bsh.pm10 = UDataRow.ToDecimal(dt.Rows[i], "pm10");
                    bsh.noise = UDataRow.ToDecimal(dt.Rows[i], "noise");
                    bsh.tspmax = UDataRow.ToDecimal(dt.Rows[i], "tspmax");
                    bsh.pm2_5max = UDataRow.ToDecimal(dt.Rows[i], "pm2_5max");
                    bsh.pm10max = UDataRow.ToDecimal(dt.Rows[i], "pm10max");
                    bsh.noisemax = UDataRow.ToDecimal(dt.Rows[i], "noisemax");

                    result.Add(bsh);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="startdate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getDataHistories")]
        public async Task<IResponseOutput> GetDataHistories(int SITEID, DateTime startdate)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk("请选择站点");

            List<BnSite24Hours> result = new List<BnSite24Hours>();
            try
            {
                DataSet ds = await _deviceCNService.getSiteDataHis(new { SITEID = SITEID, datatype = 1, startdate = startdate, enddate = startdate.AddHours(1), pageIndex = 1, pageSize = 100 });
                DataTable dt = ds.Tables[0];
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnSite24Hours bsh = new BnSite24Hours();
                    bsh.SITEID = UDataRow.ToInt(dt.Rows[i], "SITEID");
                    bsh.datatime = Convert.ToString(dt.Rows[i]["datatime"]);
                    bsh.tsp = UDataRow.ToDecimal(dt.Rows[i], "tsp");
                    bsh.pm2_5 = UDataRow.ToDecimal(dt.Rows[i], "pm2_5");
                    bsh.pm10 = UDataRow.ToDecimal(dt.Rows[i], "pm10");
                    bsh.noise = UDataRow.ToDecimal(dt.Rows[i], "noise");
                    bsh.dampness = UDataRow.ToDecimal(dt.Rows[i], "dampness");
                    bsh.temperature = UDataRow.ToDecimal(dt.Rows[i], "temperature");
                    bsh.atmos = UDataRow.ToDecimal(dt.Rows[i], "atmos");
                    bsh.speed = UDataRow.ToDecimal(dt.Rows[i], "speed");
                    bsh.direction = Convert.ToString(dt.Rows[i]["direction"]);

                    result.Add(bsh);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 根据设备种类获取监测对象
        /// </summary>
        /// <param name="linktype"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getListByDeviceType")]
        public async Task<IResponseOutput> GetListByDeviceType(int linktype)
        {
            if (linktype <= 0)
                return ResponseOutput.NotOk();

            return ResponseOutput.Ok(await _siteService.getListByDeviceType(linktype));
        }

        /// <summary>
        /// 根据功能ID获取监测对象
        /// </summary>
        /// <param name="MENUID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getListByMenuId")]
        public async Task<IResponseOutput> GetListByMenuId(string MENUID)
        {
           return ResponseOutput.Ok(await _siteService.getListByMenuId(MENUID));
        }


        ///<summary>
        /// 
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getTodayWarnList")]
        public async Task<IResponseOutput> GetTodayWarnList(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            return ResponseOutput.Ok(await _warningService.getTodayList(SITEID));
        }

        ///<summary>
        /// 
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getWeekWarnList")]
        public async Task<IResponseOutput> GetWeekWarnList(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            return ResponseOutput.Ok(await _warningService.getListApp(SITEID, DateTime.Now.AddDays(-6), DateTime.Now));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WARNID"></param>
        /// <param name="warnstatus"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("setWarnStatus")]
        public async Task<IResponseOutput> GetWarnStatus(int WARNID, int warnstatus)
        {
            if (WARNID <= 0 || warnstatus < 0)
                return ResponseOutput.NotOk();
            return ResponseOutput.Ok(await _warningService.doChangeStatus(WARNID, warnstatus));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getSiteUser")]
        public async Task<IResponseOutput> GetSiteUser(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();
            return ResponseOutput.Ok(await _siteService.getAsignListForApp(SITEID));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getCameraList")]
        public async Task<IResponseOutput> GetCameraList(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            List<BnCamera> result = new List<BnCamera>();
            try
            {
                DataTable dt = await _cameraService.getCameraList(SITEID);
                if (dt == null || dt.Rows.Count.Equals(0))
                    return ResponseOutput.Ok(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BnCamera bc = new BnCamera();
                    bc.CAMERAID = Convert.ToInt32(dt.Rows[i]["CAMERAID"]);
                    bc.cameraname = Convert.ToString(dt.Rows[i]["cameraname"]);
                    bc.cameracode = Convert.ToString(dt.Rows[i]["cameracode"]);
                    bc.channel = Convert.ToInt32(dt.Rows[i]["channel"]);
                    bc.cameraparam = Convert.ToString(dt.Rows[i]["cameraparam"]);
                    bc.cameratype = Convert.ToInt32(dt.Rows[i]["cameratype"]);

                    result.Add(bc);
                }

            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatedate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getFlyList")]
        public async Task<IResponseOutput> GetFlyList(DateTime operatedate)
        {
            if (operatedate == null)
                return ResponseOutput.NotOk();
            return ResponseOutput.Ok(await _siteService.getFlyList(operatedate));
        }

        /// <summary>
        /// 五方人员
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("fiveparts")]
        public async Task<IResponseOutput> GetListFivePart(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请选择正确的站点");
            }
            return ResponseOutput.Ok(await _dustService.GetListFivePart(SITEID));
        }

        /// <summary>
        /// 五方人员更新
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="USERIDS"></param>
        /// <param name="parttypes"></param>
        /// <param name="usernames"></param>
        /// <param name="companys"></param>
        /// <param name="mobiles"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("setFivePart")]
        public async Task<IResponseOutput> SetFivePart([FromForm]int SITEID, [FromForm] string USERIDS, [FromForm] string parttypes, [FromForm] string usernames, [FromForm] string companys, [FromForm] string mobiles, [FromForm] string username)
        {
            if (SITEID <= 0 || string.IsNullOrEmpty(USERIDS) || string.IsNullOrEmpty(parttypes) || string.IsNullOrEmpty(usernames)
                || string.IsNullOrEmpty(mobiles))
                return ResponseOutput.NotOk();

            int result = 0;
            try
            {
                result = await _dustService.doRegistFivePart(new { SITEID = SITEID, USERIDS = USERIDS, parttypes = parttypes, usernames = usernames, companys = companys, mobiles = mobiles, @operator= username });
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk(ex.Message);
            }
            return ResponseOutput.Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getDocList")]
        public async Task<IResponseOutput> GetDocList(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请选择正确的站点");
            }
            DataSet ds=await _siteService.getSiteDocList(SITEID);
            if (ds != null)
            {
                return ResponseOutput.Ok(ds.Tables[0]);
            }
            else
            {
                return ResponseOutput.NotOk("未查找到数据");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SITEDOCID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("deleteDoc")]
        public async Task<IResponseOutput> DeleteDoc(string SITEDOCID)
        {
            return ResponseOutput.NotOk("功能开发中...",0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getTruckList")]
        public async Task<IResponseOutput> GetTruckList(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请选择正确的站点");
            }

            return ResponseOutput.Ok(await _siteService.getTruckList(SITEID));
        }

        /// <summary>
        /// 获取卸料平台信息
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getUnloadList")]
        public async Task<IResponseOutput> GetUnloadList(int SITEID)
        {
            if (SITEID <= 0)
            {
                return ResponseOutput.NotOk("请选择正确的站点");
            }

            return ResponseOutput.Ok(await _unloadService.getListForSite(SITEID));
        }

        /// <summary>
        /// 获取臭氧最近信息 getSiteO3Latest
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getSiteO3Latest")]

        public async Task<IResponseOutput> GetSiteO3Latest(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            return ResponseOutput.Ok(await _siteService.getSiteO3Latest(SITEID));
        }

        /// <summary>
        /// 获取臭氧图表信息getSiteO3Chart
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getSiteO3Chart")]

        public async Task<IResponseOutput> GetSiteO3Chart(int SITEID)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            return ResponseOutput.Ok(await _siteService.getSiteO3Chart(SITEID));
        }

        /// <summary>
        /// 获取监测点臭氧历史数据getSiteO3His
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <param name="datatype">1：分钟 2：小时 3：日均</param>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>结果集</returns>
        [HttpGet]
        [Route("getSiteO3His")]
        public async Task<IResponseOutput> GetSiteO3His(int SITEID, int datatype, DateTime startdate, DateTime enddate, int pageIndex=1, int pageSize=20)
        {
            if (SITEID <= 0)
                return ResponseOutput.NotOk();

            return ResponseOutput.Ok(await _siteService.getSiteO3His(SITEID, datatype, startdate, enddate, pageIndex, pageSize));
        }

    }
}