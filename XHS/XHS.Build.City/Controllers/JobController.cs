using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XHS.Build.Common;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Common.Util;
using XHS.Build.Common.Wechat;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.CityAqi;
using XHS.Build.Services.DailyJob;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Warning;
using static XHS.Build.Common.Helps.HpFog;

namespace XHS.Build.Net.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    [NoOprationLog]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class JobController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JobController> _logger;
        private readonly IDeviceCNService _deviceCNService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IWarningService _warningService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly ICityAqiService _cityAqiService;
        private readonly HpAliSMS _hpAliSMS;
        private readonly ISmsQueueService _smsQueueService;
        private const string URL_WARN = "http://{0}:9027/Services/WSTemplateMessage.svc";
        private const string SRT_ALERT = "调用了{0}的特种设备未安装提醒服务！[{1}]";
        private readonly IDailyJobService _dailyJobService;
        private const string DB_NAME = "XJ_Env";
        private const string LOG_CLEAR = "执行日志清理.";
        private const string TMP_CLEAR = "执行临时文件夹清理.";
        private const string CODE_CLEAR = "执行验证码清理.";
        private const char SEPARATOR = ';';
        private readonly IBaseRepository<BaseEntity> _baseRepository;
        //private static string ACCESS_TOKEN = null;
        private readonly ICache _cache;
        private const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss ";
        private static long _Timestamp
        {
            get { return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000; }
        }
        public JobController(IConfiguration configuration, ILogger<JobController> logger, IDeviceCNService deviceCNService, IHpSystemSetting hpSystemSetting, IWarningService warningService, ISystemSettingService systemSettingService, ICityAqiService cityAqiService, ISmsQueueService smsQueueService, IDailyJobService dailyJobService, IBaseRepository<BaseEntity> baseRepository, ICache cache)
        {
            _configuration = configuration;
            _logger = logger;
            _deviceCNService = deviceCNService;
            _hpSystemSetting = hpSystemSetting;
            _warningService = warningService;
            _systemSettingService = systemSettingService;
            _cityAqiService = cityAqiService;
            _hpAliSMS = new HpAliSMS(hpSystemSetting);
            _smsQueueService = smsQueueService;
            _dailyJobService = dailyJobService;
            _baseRepository = baseRepository;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task OneMin()
        {
            #region WXDY
            if (_configuration.GetSection("WXDY").GetValue<bool>("WXDY"))
            {
                string SUCCESS = "0000";
                string APPID = "sysfdas2fvdasf33dag";

                string SQL = @"SELECT vdr.[GROUPID],vdr.[devicecode],vdr.[password],ddy.appid,ddy.secret,ddy.aes,
		            [lng],[lat],[tsp],[pm2_5],[pm10],[atmos],[direction],[noise],[dampness],[temperature],[speed],[updatetime],
	              ddy.[prj_id],ddy.[prj_name],ddy.[owner_name],ddy.[device_type],ddy.[contract_record_code],ddy.[monitor_point] 
                    FROM T_GC_DeviceDY ddy INNER JOIN vDeviceRtd vdr ON ddy.DEVICEID=vdr.DEVICEID 
                    AND DATEADD(SECOND,90,vdr.updatetime) > GETDATE() WHERE vdr.bdel=0 AND vdr.isself = 1 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0";
                string token_url = "http://{0}/rest/Token/get/{1}/";

                string TOKEN_URL_Format = string.Format(token_url, _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID);
                string ACCESS_TOKEN = _cache.Get<string>(APPID);
                if (string.IsNullOrEmpty(ACCESS_TOKEN))
                {
                    string pass = "{\"pass\":\"" + SpecialEqpHelp.GetPass() + "\"}";
                    string tokenresp = UHttp.Post(TOKEN_URL_Format, pass, UHttp.CONTENT_TYPE_JSON);
                    JObject jToken = JObject.Parse(tokenresp);
                    ACCESS_TOKEN = Convert.ToString(jToken["result"]["access_token"]);
                    _cache.Set(APPID, ACCESS_TOKEN, TimeSpan.FromHours(2));
                    if (Convert.ToString(jToken["flag"]) != SUCCESS)
                    {
                        _logger.LogInformation(tokenresp);
                        return;
                    }
                }
                DataTable dt = _baseRepository.Db.Ado.GetDataTable(SQL);
                SortedDictionary<string, string> jparam = null;
                StringBuilder sb = null;
                StringBuilder devicecodeall = new StringBuilder(string.Empty);
                //扬尘
                try
                {
                    int count = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        jparam = new SortedDictionary<string, string>();
                        sb = new StringBuilder(string.Empty);
                        jparam["prj_id"] = Convert.ToString(dt.Rows[i]["prj_id"]);
                        jparam["prj_name"] = Convert.ToString(dt.Rows[i]["prj_name"]);
                        jparam["owner_name"] = Convert.ToString(dt.Rows[i]["owner_name"]);
                        jparam["device_type"] = Convert.ToString(dt.Rows[i]["device_type"]);
                        jparam["device_id"] = Convert.ToString(dt.Rows[i]["devicecode"]);
                        jparam["contract_record_code"] = Convert.ToString(dt.Rows[i]["contract_record_code"]);
                        jparam["monitor_point"] = Convert.ToString(dt.Rows[i]["monitor_point"]);
                        jparam["pm25"] = Convert.ToString(dt.Rows[i]["pm2_5"]);
                        jparam["pm10"] = Convert.ToString(dt.Rows[i]["pm10"]);
                        jparam["noise"] = Convert.ToString(dt.Rows[i]["noise"]);
                        jparam["temperature"] = Convert.ToString(dt.Rows[i]["temperature"]);
                        jparam["humidity"] = Convert.ToString(dt.Rows[i]["dampness"]);
                        jparam["atmos"] = Convert.ToString(dt.Rows[i]["atmos"]);
                        jparam["wind_speed"] = Convert.ToString(dt.Rows[i]["speed"]);
                        jparam["wind_direction"] = Convert.ToString(dt.Rows[i]["direction"]);
                        jparam["tsp"] = Convert.ToString(dt.Rows[i]["tsp"]);
                        jparam["is_pm25_alarm"] = "0";
                        jparam["is_pm10_alarm"] = "0";
                        jparam["is_noise_alarm"] = "0";
                        jparam["is_tsp_alarm"] = "0";
                        jparam["datetime"] = Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString(TIME_FORMAT);

                        foreach (var item in jparam)
                        {
                            sb.Append(item.Value);
                        }
                        sb.Append(ACCESS_TOKEN);
                        string sign = UEncrypter.SHA256(sb.ToString());

                        string rtdUrl = string.Format("http://{0}/rest/DustNoise/addRealTimeData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                        string response = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(jparam), UHttp.CONTENT_TYPE_JSON);
                        JObject jo = JObject.Parse(response);
                        if (Convert.ToString(jo["flag"]) != SUCCESS)
                            _logger.LogError(response);
                        else
                        {
                            devicecodeall.AppendFormat("{0} ", jparam["device_id"]);
                            count++;
                        }

                    }
                    _logger.LogInformation(string.Format("成功推送了以下 {0} 台设备的数据:{1}.", count, devicecodeall));

                }
                catch (Exception ex)
                {
                    _logger.LogError("扬尘设备请求出错：" + ex.ToString());
                }

                //塔吊
                try
                {
                    SQL = @"select ddy.*,a.paramjson,a.secode,b.sedata,b.alarmstate,b.updatedate FROM T_GC_DeviceDY ddy inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID inner join (select * from ( select ROW_NUMBER() over(partition by secode order by updatedate desc) RowNum ,T_GC_SpecialEqpRtdData.* from T_GC_SpecialEqpRtdData where updatedate>'" + DateTime.Now.AddMinutes(-1).ToString() + "' ) as t1  where RowNum = 1) b on a.secode=b.secode where a.setype=1 and a.bdel=0 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0  ";
                    var towerDt = _baseRepository.Db.Ado.GetDataTable(SQL);
                    if (towerDt != null && towerDt.Rows.Count > 0)
                    {
                        SortedDictionary<string, string> towerParam = null;
                        sb = null;
                        for (var i = 0; i < towerDt.Rows.Count; i++)
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(towerDt.Rows[i]["sedata"])))//必填项
                            {
                                continue;
                            }
                            var sedata = JsonConvert.DeserializeObject<TowerCraneRealDataInput>(HttpUtility.HtmlDecode(Convert.ToString(towerDt.Rows[i]["sedata"])));
                            if (string.IsNullOrEmpty(Convert.ToString(towerDt.Rows[i]["paramjson"])))//必填项
                            {
                                continue;
                            }
                            var paramdata = JsonConvert.DeserializeObject<GCSpecialEqpParamjson>("{" + HttpUtility.HtmlDecode(Convert.ToString(towerDt.Rows[i]["paramjson"])));
                            towerParam = new SortedDictionary<string, string>();
                            sb = new StringBuilder(string.Empty);
                            towerParam["prj_id"] = Convert.ToString(towerDt.Rows[i]["prj_id"]);
                            towerParam["prj_name"] = Convert.ToString(towerDt.Rows[i]["prj_name"]);
                            towerParam["owner_name"] = Convert.ToString(towerDt.Rows[i]["owner_name"]);
                            towerParam["device_type"] = "塔吊";// Convert.ToString(towerDt.Rows[i]["device_type"]);
                            towerParam["device_id"] = Convert.ToString(towerDt.Rows[i]["secode"]);
                            towerParam["contract_record_code"] = Convert.ToString(towerDt.Rows[i]["contract_record_code"]);
                            if (!string.IsNullOrEmpty(sedata.DriverCardNo))
                            {
                                towerParam["id_card"] = sedata.DriverCardNo;
                            }
                            towerParam["limit_weight"] = sedata.SafeLoad == null ? "0" : sedata.SafeLoad.ToString();
                            towerParam["weight"] = sedata.Weight == null ? "0" : sedata.Weight.ToString();
                            towerParam["limit_height"] = paramdata.maxHeight;
                            towerParam["height"] = sedata.Height == null ? "0" : sedata.Height.ToString();
                            towerParam["limit_magnitude"] = paramdata.MaxMargin;//塔机工作臂长
                            towerParam["magnitude"] = sedata.Margin == null ? "0" : sedata.Margin.ToString();
                            towerParam["limit_torque"] = sedata.LiJvMaxMargin == null ? "0" : sedata.LiJvMaxMargin.ToString();//额定力矩
                            towerParam["torque"] = sedata.Moment == null ? "0" : sedata.Moment.ToString();//力矩
                            towerParam["torque_percentage"] = sedata.MomentPercent == null ? "0" : sedata.MomentPercent.ToString();//力矩百分比
                            towerParam["wind_speed"] = sedata.WindSpeed == null ? "0" : sedata.WindSpeed.ToString();
                            towerParam["rotation"] = sedata.Rotation == null ? "0" : sedata.Rotation.ToString();
                            var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(sedata.Alarm));
                            if (intList.Contains(1) || intList.Contains(2))//高度报警
                            {
                                towerParam["height_sensor_status"] = "1";//高度传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                towerParam["is_height_alarm"] = "1";//高度传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                            }
                            else
                            {
                                towerParam["height_sensor_status"] = "0";
                                towerParam["is_height_alarm"] = "0";
                            }
                            if (intList.Contains(4) || intList.Contains(8))//幅度
                            {
                                towerParam["magnitude_sensor_status"] = "1";//幅度传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                towerParam["is_magnitude_alarm"] = "1";//幅度传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                            }
                            else
                            {
                                towerParam["magnitude_sensor_status"] = "0";
                                towerParam["is_magnitude_alarm"] = "0";
                            }
                            if (intList.Contains(64))//重量
                            {
                                towerParam["weight_sensor_status"] = "1";//重量传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                towerParam["is_weight_alarm"] = "1";//重量传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                            }
                            else
                            {
                                towerParam["weight_sensor_status"] = "0";
                                towerParam["is_weight_alarm"] = "0";
                            }
                            if (intList.Contains(128))
                            {
                                towerParam["torque_sensor_status"] = "1";//力矩传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                towerParam["is_torque_alarm"] = "1";//力矩传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                            }
                            else
                            {
                                towerParam["torque_sensor_status"] = "0";
                                towerParam["is_torque_alarm"] = "0";
                            }
                            towerParam["datetime"] = Convert.ToDateTime(towerDt.Rows[i]["updatedate"]).ToString(TIME_FORMAT);

                            foreach (var item in towerParam)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                                {
                                    sb.Append(item.Value);
                                }
                            }
                            sb.Append(ACCESS_TOKEN);
                            string sign = UEncrypter.SHA256(sb.ToString());

                            string towerUrl = string.Format("http://{0}/rest/Tower/addRealTimeData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                            string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(towerParam), UHttp.CONTENT_TYPE_JSON);
                            JObject jo = JObject.Parse(response);
                            if (Convert.ToString(jo["flag"]) != SUCCESS)
                            {
                                _logger.LogError("塔吊设备：" + towerParam["device_id"] + "请求返回：" + response);
                            }
                            else
                            {
                                _logger.LogInformation(towerParam["device_id"] + "设备数据推送成功");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("塔吊设备请求出错：" + ex.ToString());
                }

                //升降机
                try
                {
                    SQL = @"select ddy.*,a.paramjson,a.secode,b.sedata,b.alarmstate,b.updatedate FROM T_GC_DeviceDY ddy inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID inner join (select * from ( select ROW_NUMBER() over(partition by secode order by updatedate desc) RowNum ,T_GC_SpecialEqpRtdData.* from T_GC_SpecialEqpRtdData where updatedate>'" + DateTime.Now.AddMinutes(-1).ToString() + "' ) as t1  where RowNum = 1) b on a.secode=b.secode where a.setype=2 and a.bdel=0 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0  ";
                    var ElevatorDt = _baseRepository.Db.Ado.GetDataTable(SQL);
                    if (ElevatorDt != null && ElevatorDt.Rows.Count > 0)
                    {
                        SortedDictionary<string, string> towerParam = null;
                        sb = null;
                        for (var i = 0; i < ElevatorDt.Rows.Count; i++)
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(ElevatorDt.Rows[i]["sedata"])))//必填项
                            {
                                continue;
                            }
                            var sedata = JsonConvert.DeserializeObject<ElevatorRealDataInput>(HttpUtility.HtmlDecode(Convert.ToString(ElevatorDt.Rows[i]["sedata"])));
                            if (string.IsNullOrEmpty(Convert.ToString(ElevatorDt.Rows[i]["paramjson"])))//必填项
                            {
                                continue;
                            }
                            var paramdata = JsonConvert.DeserializeObject<GCSpecialEqpParamjson>("{" + HttpUtility.HtmlDecode(Convert.ToString(ElevatorDt.Rows[i]["paramjson"])));
                            towerParam = new SortedDictionary<string, string>();
                            sb = new StringBuilder(string.Empty);
                            towerParam["prj_id"] = Convert.ToString(ElevatorDt.Rows[i]["prj_id"]);
                            towerParam["prj_name"] = Convert.ToString(ElevatorDt.Rows[i]["prj_name"]);
                            towerParam["owner_name"] = Convert.ToString(ElevatorDt.Rows[i]["owner_name"]);
                            towerParam["device_type"] = "升降机";// Convert.ToString(towerDt.Rows[i]["device_type"]);
                            towerParam["device_id"] = Convert.ToString(ElevatorDt.Rows[i]["secode"]);
                            towerParam["contract_record_code"] = Convert.ToString(ElevatorDt.Rows[i]["contract_record_code"]);
                            if (!string.IsNullOrEmpty(sedata.DriverCardNo))
                            {
                                towerParam["id_card"] = sedata.DriverCardNo;
                            }
                            towerParam["run_status"] = "0";
                            towerParam["weight"] = sedata.Weight == null ? "0" : sedata.Weight.ToString();
                            towerParam["height"] = sedata.Height == null ? "0" : sedata.Height.ToString();
                            towerParam["speed"] = sedata.Speed == null ? "0" : sedata.Speed.ToString();
                            towerParam["wind_speed"] = "0";
                            towerParam["dip_angle"] = "0";
                            var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(sedata.AlarmState));
                            if (intList.Contains(512))//重量
                            {
                                towerParam["weight_sensor_status"] = "1";//重量传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                towerParam["is_weight_alarm"] = "1";//重量传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                            }
                            else
                            {
                                towerParam["weight_sensor_status"] = "0";
                                towerParam["is_weight_alarm"] = "0";
                            }
                            if (intList.Contains(1))//倾角
                            {
                                towerParam["dip_angle_sensor_status"] = "1";
                                towerParam["is_dip_angle_alarm"] = "1";
                            }
                            else
                            {
                                towerParam["dip_angle_sensor_status"] = "0";
                                towerParam["is_dip_angle_alarm"] = "0";
                            }
                            //if (intList.Contains(1))//速度
                            //{
                            //    towerParam["speed_sensor_status"] = "1";
                            //    towerParam["is_ speed_alarm"] = "1";
                            //}
                            //else
                            //{
                            towerParam["speed_sensor_status"] = "0";
                            towerParam["is_ speed_alarm"] = "0";
                            //}
                            if (intList.Contains(2) || intList.Contains(4))
                            {
                                towerParam["is_top_limit_alarm"] = "1";
                                towerParam["is_bottom_limit_alarm"] = "1";
                            }
                            else
                            {
                                towerParam["is_top_limit_alarm"] = "0";
                                towerParam["is_bottom_limit_alarm"] = "0";
                            }

                            towerParam["datetime"] = Convert.ToDateTime(ElevatorDt.Rows[i]["updatedate"]).ToString(TIME_FORMAT);

                            foreach (var item in towerParam)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                                {
                                    sb.Append(item.Value);
                                }
                            }
                            sb.Append(ACCESS_TOKEN);
                            string sign = UEncrypter.SHA256(sb.ToString());

                            string towerUrl = string.Format("http://{0}/rest/Elevator/addRealTimeData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                            string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(towerParam), UHttp.CONTENT_TYPE_JSON);
                            JObject jo = JObject.Parse(response);
                            if (Convert.ToString(jo["flag"]) != SUCCESS)
                            {
                                _logger.LogError("升降机设备：" + towerParam["device_id"] + "请求返回：" + response);
                            }
                            else
                            {
                                _logger.LogInformation(towerParam["device_id"] + "设备数据推送成功");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("升降机设备请求出错：" + ex.ToString());
                }
            }
            #endregion

            #region fogkicker
            if (_configuration.GetValue<bool>("isfogkicker"))
            {
                string SQL = @"SELECT d.[SITEID]
                                          ,d.[devicecode]
                                          ,d.[fogkickline]
                                          ,d.[status]
                                          ,d.[checkintime]
                                          ,d.[checkouttime]
                                          ,d.[bdel]
	                                      ,dr.pm10
	                                      ,dr.updatetime
	                                      ,f.[fogcode]
                                          ,f.[fogname]
                                          ,f.[fogstatus]
                                          ,f.[switchno]
                                          ,f.[delay]
                                          ,f.[bwaterauto]
                                          ,f.[checkintime] fcheckintime
                                          ,f.[checkouttime] fcheckouttime
                                      FROM [T_GC_Device] d INNER JOIN T_GC_DeviceRtd dr 
                                      ON d.devicecode=dr.devicecode AND DATEADD(MINUTE,1,dr.updatetime) > GETDATE() AND d.bdel=0 AND d.[status]=1 AND d.fogkickline > 0 
                                      INNER JOIN T_GC_Fog f
                                      ON f.SITEID = d.SITEID AND f.fogstatus=0 AND f.bwaterauto=1
                                      WHERE dbo.fnGetSystemSettingValue('S148')=1
	                                    AND dr.pm10 >= d.fogkickline AND dr.pm10 < 2000";
                try
                {
                    DataTable dt = _baseRepository.Db.Ado.GetDataTable(SQL);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        try
                        {
                            BnCmd bnCmd = new BnCmd() { USERID = "0", fc = Convert.ToString(dt.Rows[i]["fogcode"]), sw = Convert.ToString(dt.Rows[i]["switchno"]), cmd = HpFog.CMD.ON, delay = Convert.ToString(dt.Rows[i]["delay"]), };
                            HpFog.SendCommand(bnCmd, _configuration.GetSection("BatchConsole").GetValue<string>("EquipServerIp"), _configuration.GetSection("BatchConsole").GetValue<int>("EquipServerPort"), _configuration.GetSection("BatchConsole").GetValue<string>("EquipPublicKey"));

                            _logger.LogInformation(string.Format("{0} 的PM10为 {1} 已超过雾炮连动阀值 {2},已发送开启指令给 {3}:{4}",
                                dt.Rows[i]["devicecode"], dt.Rows[i]["pm10"], dt.Rows[i]["fogkickline"], dt.Rows[i]["fogcode"], dt.Rows[i]["switchno"]));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            #endregion
        }

        /// <summary>
        /// 十分钟调用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task TenMin()
        {
            #region 批处理
            int timeout = _configuration.GetSection("BatchConsole").GetValue<int>("DbBackupTimeout");
            var res = await _deviceCNService.doBatch(timeout);
            if (res > 0)
            {
                _logger.LogInformation("执行批处理成功,并生成 {0} 条数据!", res);
            }
            else
            {
                _logger.LogInformation("已执行批处理!");
            }
            #endregion

            #region 博浪推送
            bool isbolang = _configuration.GetSection("BatchConsole").GetValue<bool>("IsBolang");
            if (isbolang)
            {
                string filename = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S024), "bolang", "device.csv.true");
                if (!string.IsNullOrEmpty(filename) && UFile.IsExistFile(filename))
                {
                    //先把文件读好
                    List<string[]> list = new List<string[]>();
                    using (var fs = System.IO.File.OpenRead(filename))
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');
                            list.Add(values);
                        }
                    }
                    int okCount = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            string bacode = list[i][0];
                            ///token地址
                            string urlToken = string.Format("http://122.97.130.226:8088/api/Yc/GetToken?bacode={0}&username={1}&password={2}",
                                bacode, list[i][1], list[i][2]);
                            string result = UHttp.Post(urlToken, string.Empty);
                            JObject jobject = JObject.Parse(result);
                            string AccessToken = Convert.ToString(jobject["data"]["AccessToken"]);

                            //定义header
                            WebHeaderCollection header = new WebHeaderCollection();
                            string timestamp = Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                            string nonce = DateTime.Now.ToString("mmssfffffff");//随机数
                            string jdzch = list[i][3];//项目注册号
                            //实时数据
                            DataRow dr = await _deviceCNService.getOneForSend(list[i][4]);
                            if (dr == null)
                                continue;
                            //拼xml
                            string data = string.Format("<sbbm>{0}</sbbm><PM25>{1}</PM25><PM10>{2}</PM10><Wd>{3}</Wd><Sd>{4}</Sd>" +
                                "<Fx>{5}</Fx><Fs>{6}</Fs><TSP>{7}</TSP><Noise>{8}</Noise><pressure>{9}</pressure>",
                                list[i][4], dr["pm2_5"], dr["pm2_5"], dr["pm10"], dr["temperature"],
                                dr["dampness"], dr["direction"], dr["speed"], dr["noise"], dr["atmos"]);

                            //upload地址
                            string urlUpload = string.Format("http://122.97.130.226:8088/api/Yc/UploadData?bacode={0}&token={1}&data={2}",
                                bacode, AccessToken, data);
                            string signature = getSignature(timestamp, nonce, jdzch, AccessToken, data);
                            header["jdzch"] = jdzch;
                            header["timestamp"] = timestamp;
                            header["nonce"] = nonce;
                            header["signature"] = signature.ToUpper();//转大写
                            result = UHttp.Post(urlUpload, string.Empty, header);
                            jobject = JObject.Parse(result);
                            if (string.IsNullOrEmpty(Convert.ToString(jobject["ErrorCode"])) && Convert.ToBoolean(jobject["result"]))
                            {
                                okCount++;
                                _logger.LogInformation(string.Format("推送了编号 {0} 的数据.", list[i][4]));
                            }
                            else
                            {
                                _logger.LogInformation(string.Format("推送编号 {0} 的数据失败. {1}{2}", list[i][4], Environment.NewLine, jobject.ToString()));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(ex.Message);
                        }
                    }
                }
            }
            #endregion

            #region 调用特种设备未安装提醒服务 每天早上9点？
            if (DateTime.Now.Hour.Equals(9) && DateTime.Now.Minute > 0 && DateTime.Now.Minute < 11)
            {
                try
                {
                    string domain = _configuration.GetSection("BatchConsole").GetValue<string>("WcfDomain");
                    object result = HpWcfInvoker.ExecuteMethod<IWSTemplateMessage>(string.Format(URL_WARN, domain), "sendSeAlert");

                    if (result != null && !Convert.ToInt32(result).Equals(0))
                    {
                        //如果发生了发信才写日志
                        //_logParam.Set("operation", string.Format(SRT_ALERT, domain, result));  没有操作  先注释了
                        _logger.LogInformation(string.Format(SRT_ALERT, domain, result), false);
                    }
                }
                catch (Exception ex)
                {
                    var message = ex.Message + Environment.NewLine + ex.StackTrace;
                    _logger.LogInformation(message);
                }
            }
            #endregion

            #region 自动启动雾泡
            try
            {
                DataTable dt = await _warningService.getSendCmdList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        BnCmd bnCmd = new BnCmd() { USERID = "0", fc = Convert.ToString(dt.Rows[i]["fogcode"]), sw = Convert.ToString(dt.Rows[i]["switchno"]), cmd = HpFog.CMD.ON, delay = Convert.ToString(dt.Rows[i]["delay"]), };
                        HpFog.SendCommand(bnCmd, _configuration.GetSection("BatchConsole").GetValue<string>("EquipServerIp"), _configuration.GetSection("BatchConsole").GetValue<int>("EquipServerPort"), _configuration.GetSection("BatchConsole").GetValue<string>("EquipPublicKey"));
                        await _warningService.doUpdateCmd(Convert.ToInt32(dt.Rows[i]["WARNID"]));
                        _logger.LogInformation(string.Format("根据报警ID {0} ,发送开启雾泡指令.{1}:{2}", dt.Rows[i]["WARNID"], dt.Rows[i]["fogcode"], dt.Rows[i]["switchno"]));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message);
            }
            #endregion

            #region 调用获取城市AQI
            try
            {
                InsertCityAqi();
            }
            catch (Exception ex)
            {
                var message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message);
            }

            #endregion

            #region 调用获取城市实时天气
            try
            {
                UpdateCityWeather();
            }
            catch (Exception ex)
            {
                var message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message, true);
            }
            #endregion

            //发送提醒
            _smsQueueService.SendSmsAll();

            /* 调用发送微信短信报警服务 */
            _hpAliSMS.SendWarnAll("");

            #region 苏州住建
            if (_configuration.GetSection("SuZhouZhuJian").GetValue<bool>("SuZhouZhuJian"))
            {
                int SUCCESS = 200;                                       //成功状态
                string APPKEY = "d9390eac-0488-4517-b37c-a1d5e072c19a";             //appkey
                string APPSECRET = "17104541-f4b2-4172-b9b9-a73ce851627f";          //秘钥
                string SQL = "SELECT TOP 1000 devicecode,dr.SITEID,tsp,pm2_5,pm10,atmos," +
                      "direction,noise,dampness,temperature,speed,dbo.fnGetWindSpeedStr(speed) windlevel,updatetime " +
                      "FROM T_GC_DeviceRtd dr  INNER JOIN T_GC_Site s ON dr.SITEID = s.SITEID AND s.bpush = 1 " +
                      " WHERE updatetime > DATEADD(MINUTE,-{0},GETDATE()) AND s.GROUPID IN ({1})";
                int EXPTIME = 300000;
                string URL = "http://221.224.132.158:8081/smart-site/rest/hj/uploadhj";
                string ERROR_MESSAGE = "设备 {0} 的数据推送失败: {1}";

                try
                {
                    string sql = string.Format(SQL, _configuration.GetSection("SuZhouZhuJian").GetValue<int>("Interval"), _configuration.GetSection("SuZhouZhuJian").GetValue<string>("GroupIds"));
                    DataTable dt = _baseRepository.Db.Ado.GetDataTable(sql);
                    int count = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        try
                        {
                            long timestamp = _Timestamp;
                            string signature = string.Format("{0}&{1}&{2}&{3}", APPKEY, APPSECRET, timestamp, EXPTIME);
                            string token = UEncrypter.HmacSHA1(signature, APPSECRET);

                            WebHeaderCollection wheader = new WebHeaderCollection();
                            wheader["app_Key"] = APPKEY;
                            wheader["access_token"] = token;
                            wheader["time_stamp"] = timestamp.ToString();
                            string data = "\"MN\":\"{0}\",\"DateTime\":\"{1}\",\"TSP\":\"{2}\",\"PM25\":\"{3}\",\"PM10\":\"{4}\",\"temperature\":\"{5}\"," +
                                "\"humidity\":\"{6}\",\"atmos\":\"{7}\",\"windspeed\":\"{8}\",\"winddirection\":\"{9}\",\"leq\":\"{10}\",\"windLevel\":\"{11}\"," +
                                "\"atmospheric\":\"\",\"date\":\"\",\"time\":\"\"";
                            data = "params={" + string.Format(data, dr["devicecode"], Convert.ToDateTime(dr["updatetime"]).ToString("yyyMMddHHmmss"),
                                dr["tsp"], dr["pm2_5"], dr["pm10"], dr["temperature"], dr["dampness"], dr["atmos"], dr["speed"],
                                dr["direction"], dr["noise"], dr["windlevel"]) + "}";
                            _logger.LogInformation(data);
                            string response = UHttp.Post(URL, data, UHttp.CONTENT_TYPE_FORM, wheader);
                            JObject jo = JObject.Parse(response);
                            if (Convert.ToInt32(jo["code"]) == SUCCESS)
                                count++;
                            else
                                _logger.LogInformation(string.Format(ERROR_MESSAGE, dr["devicecode"], jo["msg"]), true);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(string.Format(ERROR_MESSAGE, dr["devicecode"], ex.Message), true);
                        }
                    }

                    _logger.LogInformation(string.Format("数据推送结束. {0} / {1}", count, dt.Rows.Count));

                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message, true);
                }
                finally
                {
                }
            }

            #endregion

            #region SND 推送 10分钟
            if (_configuration.GetSection("SND").GetValue<bool>("SND"))
            {
                try
                {
                    //WriteLog("智能公示牌节目单下发");
                    DataTable dt = _deviceCNService.getRtdList();
                    if (dt.Rows.Count <= 0)
                        return;
                    JArray ja = new JArray();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        long time = (Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000;
                        JObject pm25 = new JObject()
                {
                   { "indexId", "4101001" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["pm2_5"]) },
                };
                        JObject pm10 = new JObject()
                {
                   { "indexId", "4101002" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["pm10"]) },
                };
                        JObject atmos = new JObject()
                {
                   { "indexId", "4101011" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["atmos"]) },
                };
                        JObject speed = new JObject()
                {
                   { "indexId", "4101012" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["speed"]) },
                };
                        JObject direction = new JObject()
                {
                   { "indexId", "4101012" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["direction"]) },
                };
                        JObject temperature = new JObject()
                {
                   { "indexId", "4101014" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["temperature"]) },
                };
                        JObject dampness = new JObject()
                {
                   { "indexId", "4101015" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["dampness"]) },
                };
                        JObject tsp = new JObject()
                {
                   { "indexId", "4101091" },
                    { "time", time },
                    { "value", Convert.ToString(dt.Rows[i]["tsp"]) },
                };
                        JArray data = new JArray();
                        data.Add(pm25);
                        data.Add(pm10);
                        data.Add(atmos);
                        data.Add(speed);
                        data.Add(direction);
                        data.Add(temperature);
                        data.Add(dampness);
                        data.Add(tsp);
                        JObject device = new JObject();
                        device.Add("mn", Convert.ToString(dt.Rows[i]["devicecode"]));
                        device.Add("data", data);
                        ja.Add(device);
                    }
                    WClient wc = new WClient(_configuration.GetSection("SND").GetValue<string>("URL"), WClient.CONTENT_TYPE_FORM);
                    string param = string.Format("key={0}&data=", _configuration.GetSection("SND").GetValue<string>("AppKey")) + ja.ToString();
                    string response = wc.PostData(param);
                    _logger.LogInformation(response);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message, true);
                }
            }
            #endregion



        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task OneHour()
        {
            #region MyRegion
            try
            {
                //每天1点
                if (DateTime.Now.Hour.Equals(1))
                {
                    //数据库备份
                    try
                    {
                        _logger.LogInformation("数据库备份开始!", false);
                        int dayDiff = 30;//时间间隔（单位：天）
                                         //清理数据库备份目录下过期文件
                        string fullPath = Path.Combine(_configuration.GetSection("BatchConsole").GetValue<string>("DbBackupPath"), DB_NAME);
                        UFile.ClearExpiredFile(fullPath, dayDiff, Const.FileEx.BAK_ALL, true, false);
                        //执行备份
                        _dailyJobService.Excute(fullPath, DB_NAME, _configuration.GetSection("BatchConsole").GetValue<int>("DbBackupTimeout"));
                        _logger.LogInformation("数据库备份完成!", false);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message + Environment.NewLine + ex.StackTrace;
                        _logger.LogInformation(message, true);
                    }

                    //清理日志
                    try
                    {
                        UFile.ClearExpiredFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../xjlog"),
                            _configuration.GetSection("BatchConsole").GetValue<int>("LogSaveDay"), Const.FileEx.LOG_ALL, true, false);
                        _logger.LogInformation(LOG_CLEAR, false);

                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message + Environment.NewLine + ex.StackTrace;
                        _logger.LogInformation(message, true);
                    }

                    //清理临时文件夹内3日文件
                    try
                    {
                        string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
                        UFile.ClearExpiredFile(tmpPath, 3, Const.FileEx.ALL, true, false);
                        _logger.LogInformation(TMP_CLEAR, false);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message + Environment.NewLine + ex.StackTrace;
                        _logger.LogInformation(message, true);
                    }

                    //清理验证码
                    try
                    {
                        string codePath = _configuration.GetSection("BatchConsole").GetValue<string>("TmpCodePath");
                        string[] codePathArray = codePath.Split(SEPARATOR);

                        foreach (string path in codePathArray)
                        {
                            if (string.IsNullOrEmpty(path) || !UFile.IsExistDirectory(path))
                                continue;

                            try
                            {
                                //默认保留3天
                                UFile.ClearExpiredFile(path, 3, Const.FileEx.ALL, true, false);
                                _logger.LogInformation(CODE_CLEAR, false);
                            }
                            catch (Exception ex)
                            {
                                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                                _logger.LogInformation(message, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message, true);
                    }
                }

                //同步工地员工信息
                _dailyJobService.SparkcnDoSync();

                //同步钢丝绳
                _dailyJobService.CableDoSync();
            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message, true);
            }
            #endregion
        }


        /// <summary>
        /// timespan+nonc+ jdzch +token+ data,这几个字符串拼接的。
        /// 然后1、将字符串中字符按升序排序  
        /// 2、使用MD5加密（32位）  
        /// 3、把二进制转化为大写的十六进制  然后出来就是加密后字符串
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="jdzch">项目注册号</param>
        /// <param name="token">token</param>
        /// <param name="data">xml数据</param>
        /// <returns></returns>
        private static string getSignature(string timestamp, string nonce, string jdzch, string token, string data)
        {
            string str = timestamp + nonce + jdzch + token + data;      //拼一起
            char[] intArray = str.ToCharArray();                        //字符数组
            Array.Sort(intArray);                                       //排序
            str = string.Join(string.Empty, intArray);                  //再拼一起
            string result = UEncrypter.EncryptByMD5(str);               //MD5加密
            return result;
        }

        /// <summary>
        /// 获取城市AQI（和风天气）
        /// </summary>
        private void InsertCityAqi()
        {
            string citys = "";
            string key = "";
            string domain = "";

            //获取设定值
            DataTable dt = _systemSettingService.getValue("S035,S036,S077");
            if (dt.Rows.Count <= 0)
            {
                return;
            }
            else
            {
                citys = dt.Rows[0]["S036"].ToString();
                key = dt.Rows[0]["S035"].ToString();
                domain = dt.Rows[0]["S077"].ToString();

                if (String.IsNullOrEmpty(citys) || String.IsNullOrEmpty(key))
                {
                    return;
                }
            }

            string[] citylist = citys.Split(',');
            string result = ""; string positionnames = "";
            string pm25s = "";
            string pm10s = "";
            string qualitys = "";
            string aqis = "";
            string areas = "";
            string pubtime = "";
            DateTime billdate = DateTime.Parse("1900-01-01 00:00:00");
            JObject jobj = new JObject();
            JObject jobjcityall = new JObject();
            JObject obj = new JObject();
            JArray jarysiteall = new JArray();

            _logger.LogInformation("城市AQI取得开始（" + citys + "）", false);

            //循环多个城市
            for (int k = 0; k < citylist.Length; k++)
            {
                result = HpCityAqi.GetCityAqi(domain, citylist[k], key);
                jobj = new JObject();
                jobjcityall = new JObject();
                jobj = JObject.Parse(result);
                jobjcityall = JObject.Parse(jobj["HeWeather6"][0].ToString());

                //城市均值
                obj = new JObject();

                pubtime = jobjcityall["air_now_city"]["pub_time"].ToString();
                DateTime point = DateTime.Parse(pubtime);

                //只使用上二小时或者上一小时或者当前小时的数据
                if (point.Date == DateTime.Now.AddHours(-2).Date && point.Hour == DateTime.Now.AddHours(-2).Hour
                    || point.Date == DateTime.Now.AddHours(-1).Date && point.Hour == DateTime.Now.AddHours(-1).Hour
                    || point.Date == DateTime.Now.Date && point.Hour == DateTime.Now.Hour)
                {
                    areas += jobjcityall["basic"]["location"] + ",";
                    positionnames += ",";
                    pm25s += jobjcityall["air_now_city"]["pm25"] + ",";
                    pm10s += jobjcityall["air_now_city"]["pm10"] + ",";
                    qualitys += jobjcityall["air_now_city"]["qlty"] + ",";
                    aqis += jobjcityall["air_now_city"]["aqi"] + ",";
                    billdate = point;
                }

                //循环城市各个监测点
                jarysiteall = new JArray();
                jarysiteall = JArray.Parse(jobjcityall["air_now_station"].ToString());

                for (int i = 0; i < jarysiteall.Count; i++)
                {

                    pubtime = jarysiteall[i]["pub_time"].ToString();
                    point = DateTime.Parse(pubtime);

                    //只使用上二小时或者上一小时当前小时的数据
                    if (point.Date == DateTime.Now.AddHours(-2).Date && point.Hour == DateTime.Now.AddHours(-2).Hour
                        || point.Date == DateTime.Now.AddHours(-1).Date && point.Hour == DateTime.Now.AddHours(-1).Hour
                        || point.Date == DateTime.Now.Date && point.Hour == DateTime.Now.Hour)
                    {
                        areas += jobjcityall["basic"]["location"] + ",";
                        positionnames += jarysiteall[i]["air_sta"] + ",";
                        pm25s += jarysiteall[i]["pm25"] + ",";
                        pm10s += jarysiteall[i]["pm10"] + ",";
                        qualitys += jarysiteall[i]["qlty"] + ",";
                        aqis += jarysiteall[i]["aqi"] + ",";
                        billdate = point;
                    }
                }
            }

            //如果存在有效数据，插入数据库
            if (areas.Length > 0)
            {
                areas = areas.Substring(0, areas.Length - 1);
                positionnames = positionnames.Substring(0, positionnames.Length - 1);
                pm25s = pm25s.Substring(0, pm25s.Length - 1);
                pm10s = pm10s.Substring(0, pm10s.Length - 1);
                qualitys = qualitys.Substring(0, qualitys.Length - 1);
                aqis = aqis.Substring(0, aqis.Length - 1);

                int ret = _cityAqiService.doInsert(new DBParams("@areas", areas), new DBParams("@positionnames", positionnames), new DBParams("@pm25s", pm25s), new DBParams("@pm10s", pm10s), new DBParams("@qualitys", qualitys), new DBParams("@aqis", aqis), new DBParams("@billdate", billdate));
                _logger.LogInformation("城市AQI插入数据库（" + ret + "）");
            }
        }

        private void UpdateCityWeather()
        {
            string citys = "";
            string key = "";
            string domain = "";

            //获取设定值
            DataTable dt = _systemSettingService.getValue("S035,S036,S077");
            if (dt.Rows.Count <= 0)
            {
                return;
            }
            else
            {
                citys = dt.Rows[0]["S036"].ToString();
                key = dt.Rows[0]["S035"].ToString();
                domain = dt.Rows[0]["S077"].ToString();

                if (String.IsNullOrEmpty(citys) || String.IsNullOrEmpty(key))
                {
                    return;
                }
            }

            string[] citylist = citys.Split(',');
            string result = "";
            JObject jobj = new JObject();
            JObject jresult = new JObject();

            //只在每个小时的0分~15分之间获取实况天气，保证每个城市每小时只调用一次接口
            if (DateTime.Now.Minute > 15)
            {
                return;
            }

            _logger.LogInformation("城市是实况天气取得开始（" + citys + "）", false);



            //循环多个城市
            for (int k = 0; k < citylist.Length; k++)
            {
                result = HpCityAqi.GetCityWeather(domain, citylist[k], key);
                jobj = new JObject();
                jobj = JObject.Parse(result);
                jresult = JObject.Parse(jobj["HeWeather6"][0].ToString());

                if (jresult["status"].ToString().ToUpper() != "OK")
                {
                    return;
                }

                string weather = jresult["now"].ToString();
                string updatedate = jresult["update"]["loc"].ToString();

                _cityAqiService.doWeatherUpdate(new DBParams("@area", citylist[k]), new DBParams("@weather", weather), new DBParams("@updatedate", updatedate));

            }

        }


    }
}
