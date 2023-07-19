using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Employee;
using XHS.Build.Services.File;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class WXDYJob : JobBase, IJob
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WXDYJob> _logger;
        private readonly IBaseRepository<BaseEntity> _baseRepository;
        private const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss ";
        private readonly ISpecialEqpService _specialEqpService;
        private readonly IEmployeeService _employeeService;
        private readonly IFileService _fileService;
        private readonly ICache _cache;
        private readonly IEmployeeCareerService _employeeCareerService;
        public WXDYJob(IConfiguration configuration, ILogger<WXDYJob> logger, IBaseRepository<BaseEntity> baseRepository, ISpecialEqpService specialEqpService, IEmployeeService employeeService, IFileService fileService, ICache cache, IEmployeeCareerService employeeCareerService)
        {
            _configuration = configuration;
            _logger = logger;
            _baseRepository = baseRepository;
            _specialEqpService = specialEqpService;
            _cache = cache;
            _employeeService = employeeService;
            _fileService = fileService;
            _employeeCareerService = employeeCareerService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            string SUCCESS = "0000";
            string APPID = "sysfdas2fvdasf33dag";
            JObject tokenjob = new JObject();
            Dictionary<string, object> asciiDic = new Dictionary<string, object>();
            string[] arrKeys;
            string token = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken");
            if (!string.IsNullOrEmpty(token))
            {
                tokenjob = JObject.Parse(token);
            }
            string ACCESS_TOKEN = tokenjob.GetValue("data").ToString();
            if (string.IsNullOrEmpty(ACCESS_TOKEN))
            {
                _logger.LogInformation("未获取到大运Token，推送结束。");
                return;
            }

            string SQL = @"SELECT vdr.[GROUPID],vdr.[devicecode],vdr.[password],ddy.appid,ddy.secret,ddy.aes,
		            [lng],[lat],[tsp],[pm2_5],[pm10],[atmos],[direction],[noise],[dampness],[temperature],[speed],[updatetime],
	              ddy.[prj_id],ddy.[prj_name],ddy.[owner_name],ddy.[device_type],ddy.[contract_record_code],ddy.[monitor_point] 
                    FROM T_GC_DeviceDY ddy INNER JOIN vDeviceRtd vdr ON ddy.DEVICEID=vdr.DEVICEID 
                    AND DATEADD(SECOND,90,vdr.updatetime) > GETDATE() WHERE vdr.bdel=0 AND vdr.isself = 1 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0";
            DataTable dt = _baseRepository.Db.Ado.GetDataTable(SQL);
            SortedDictionary<string, string> jparam = null;
            StringBuilder sb = null;
            StringBuilder devicecodeall = new StringBuilder(string.Empty);
            //扬尘
            try
            {
                int count = 0;
                //跳过扬尘
                //for (int i = 0; i < dt.Rows.Count; i++)
                for (int i = 0; i < 0; i++)
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

                    asciiDic = new Dictionary<string, object>();
                    arrKeys = jparam.Keys.ToArray();
                    Array.Sort(arrKeys, string.CompareOrdinal);
                    foreach (var key in arrKeys)
                    {
                        var value = jparam[key];
                        asciiDic.Add(key, value);
                    }
                    foreach (var item in asciiDic)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                        {
                            sb.Append(item.Value);
                        }
                    }
                    sb.Append(ACCESS_TOKEN);
                    string sign = UEncrypter.SHA256(sb.ToString());

                    string rtdUrl = string.Format("http://{0}/rest/DustNoise/addRealTimeData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                    string response = UHttp.Post(rtdUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                    JObject jo = JObject.Parse(response);
                    if (Convert.ToString(jo["flag"]) == "0006" || Convert.ToString(jo["flag"]) == "0007" || Convert.ToString(jo["flag"]) == "0010")
                    {
                        token = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken?isForce=true");

                        if (!string.IsNullOrEmpty(token))
                        {
                            tokenjob = JObject.Parse(token);
                        }
                        ACCESS_TOKEN = tokenjob.GetValue("data").ToString();
                        if (string.IsNullOrEmpty(ACCESS_TOKEN))
                        {
                            _logger.LogInformation("扬尘推送未获取到大运Token，推送结束。");
                            _logger.LogInformation(string.Format("成功推送了以下 {0} 台扬尘设备的数据:{1}.", count, devicecodeall));
                            return;
                        }
                    }
                    if (Convert.ToString(jo["flag"]) != SUCCESS)
                        _logger.LogError(DateTime.Now.ToString() + "扬尘设备" + jparam["device_id"] + "URL:" + rtdUrl + response);
                    else
                    {
                        devicecodeall.AppendFormat("{0} ", jparam["device_id"]);
                        count++;
                    }

                }
                //_logger.LogInformation(string.Format("成功推送了以下 {0} 台扬尘设备的数据:{1}.", count, devicecodeall));
            }
            catch (Exception ex)
            {
                _logger.LogError("扬尘设备请求出错：" + ex.ToString());
            }
            //塔吊注册

            SQL = @"select ddy.*,a.paramjson,a.secode,a.setype,a.sestatus,a.remark,a.operatedate
                    FROM T_GC_DeviceDY ddy INNER JOIN vDeviceRtd vdr ON ddy.DEVICEID=vdr.DEVICEID AND vdr.sitearea <> 47 AND vdr.GROUPID <> 14 
                    inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  
                    inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID And len(a.remark ) >0 AND a.setype = 1 AND DATEADD(SECOND,90,a.operatedate) > GETDATE()";
            var towerDt = _baseRepository.Db.Ado.GetDataTable(SQL);
            devicecodeall = new StringBuilder(string.Empty);
            if (towerDt != null && towerDt.Rows.Count > 0)
            {
                try
                {
                    SortedDictionary<string, string> towerParam = null;
                    int count = 0;
                    sb = null;
                    for (var i = 0; i < towerDt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(towerDt.Rows[i]["paramjson"])))//必填项
                        {
                            continue;
                        }
                        var paramdata = JsonConvert.DeserializeObject<GCSpecialEqpParamjson>(HttpUtility.HtmlDecode(Convert.ToString(towerDt.Rows[i]["paramjson"])));
                        towerParam = new SortedDictionary<string, string>();
                        sb = new StringBuilder(string.Empty);
                        towerParam["device_id"] = Convert.ToString(towerDt.Rows[i]["secode"]);
                        towerParam["contract_record_code"] = Convert.ToString(towerDt.Rows[i]["contract_record_code"]);
                        towerParam["usageNo"] = Convert.ToString(towerDt.Rows[i]["remark"]);
                        towerParam["propertyNo"] = Convert.ToString(towerDt.Rows[i]["remark"]);
                        towerParam["MaxWeight"] = paramdata.MaxWeight;//最大起重量
                        towerParam["MaxMarginWeight"] = paramdata.MaxMarginWeight;//最大幅度起重量
                        towerParam["Qzbc"] = paramdata.MaxMargin;//塔机工作臂长
                        towerParam["datetime"] = Convert.ToDateTime(towerDt.Rows[i]["operatedate"]).ToString(TIME_FORMAT);

                        asciiDic = new Dictionary<string, object>();
                        arrKeys = towerParam.Keys.ToArray();
                        Array.Sort(arrKeys, string.CompareOrdinal);
                        foreach (var key in arrKeys)
                        {
                            var value = towerParam[key];
                            asciiDic.Add(key, value);
                        }
                        foreach (var item in asciiDic)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                            {
                                sb.Append(item.Value);
                            }
                        }
                        sb.Append(ACCESS_TOKEN);
                        string sign = UEncrypter.SHA256(sb.ToString());

                        string towerUrl = string.Format("http://{0}/rest/Tower/Info/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                        string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                        JObject jo = JObject.Parse(response);
                        if (Convert.ToString(jo["flag"]) == "0006" || Convert.ToString(jo["flag"]) == "0007" || Convert.ToString(jo["flag"]) == "0010")
                        {
                            token = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken?isForce=true");

                            if (!string.IsNullOrEmpty(token))
                            {
                                tokenjob = JObject.Parse(token);
                            }
                            ACCESS_TOKEN = tokenjob.GetValue("data").ToString();
                            if (string.IsNullOrEmpty(ACCESS_TOKEN))
                            {
                                _logger.LogInformation("塔吊注册推送未获取到大运Token，推送结束。");
                                _logger.LogInformation(string.Format("成功推送了以下 {0} 台塔吊注册数据:{1}.", count, devicecodeall));
                                return;
                            }
                        }
                        if (Convert.ToString(jo["flag"]) != SUCCESS)
                        {
                            //  _logger.LogError("塔吊设备：" + towerParam["device_id"] + "URL:" + towerUrl + "请求返回：" + Environment.NewLine + response);
                        }
                        else
                        {
                            devicecodeall.AppendFormat("{0} ", towerParam["device_id"]);
                            count++;
                        }
                    }
                    // _logger.LogInformation(string.Format("成功推送了以下 {0} 台塔吊设备的数据:{1}.", count, devicecodeall));
                }
                catch (Exception ex)
                {
                    //_logger.LogError("塔吊设备请求出错：" + ex.ToString());
                }
            }
            //塔吊

            SQL = @"select ddy.*,a.paramjson,a.secode,a.sestatus,a.remark,b.sedata,b.alarmstate,b.updatedate FROM T_GC_DeviceDY ddy inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID inner join (select * from ( select ROW_NUMBER() over(partition by secode order by updatedate desc) RowNum ,T_GC_SpecialEqpRtdData.* from T_GC_SpecialEqpRtdData where updatedate>'" + DateTime.Now.AddMinutes(-1).ToString() + "' ) as t1  where RowNum = 1) b on a.secode=b.secode where a.setype=1 and a.bdel=0 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0  ";
            towerDt = _baseRepository.Db.Ado.GetDataTable(SQL);
            devicecodeall = new StringBuilder(string.Empty);
            if (towerDt != null && towerDt.Rows.Count > 0)
            {
                try
                {
                    SortedDictionary<string, string> towerParam = null;
                    int count = 0;
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
                        var paramdata = JsonConvert.DeserializeObject<GCSpecialEqpParamjson>(HttpUtility.HtmlDecode(Convert.ToString(towerDt.Rows[i]["paramjson"])));
                        towerParam = new SortedDictionary<string, string>();
                        sb = new StringBuilder(string.Empty);
                        towerParam["prj_id"] = Convert.ToString(towerDt.Rows[i]["prj_id"]);
                        towerParam["prj_name"] = Convert.ToString(towerDt.Rows[i]["prj_name"]);
                        towerParam["owner_name"] = Convert.ToString(towerDt.Rows[i]["owner_name"]);
                        towerParam["device_type"] = "塔吊";// Convert.ToString(towerDt.Rows[i]["device_type"]);
                        towerParam["device_id"] = Convert.ToString(towerDt.Rows[i]["secode"]);
                        towerParam["contract_record_code"] = Convert.ToString(towerDt.Rows[i]["contract_record_code"]);
                        towerParam["usageNo"] = Convert.ToString(towerDt.Rows[i]["remark"]);
                        towerParam["propertyNo"] = Convert.ToString(towerDt.Rows[i]["remark"]);
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

                        asciiDic = new Dictionary<string, object>();
                        arrKeys = towerParam.Keys.ToArray();
                        Array.Sort(arrKeys, string.CompareOrdinal);
                        foreach (var key in arrKeys)
                        {
                            var value = towerParam[key];
                            asciiDic.Add(key, value);
                        }
                        foreach (var item in asciiDic)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                            {
                                sb.Append(item.Value);
                            }
                        }
                        sb.Append(ACCESS_TOKEN);
                        string sign = UEncrypter.SHA256(sb.ToString());

                        string towerUrl = string.Format("http://{0}/rest/Tower/addRealTimeData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                        string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                        JObject jo = JObject.Parse(response);
                        if (Convert.ToString(jo["flag"]) == "0006" || Convert.ToString(jo["flag"]) == "0007" || Convert.ToString(jo["flag"]) == "0010")
                        {
                            token = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken?isForce=true");

                            if (!string.IsNullOrEmpty(token))
                            {
                                tokenjob = JObject.Parse(token);
                            }
                            ACCESS_TOKEN = tokenjob.GetValue("data").ToString();
                            if (string.IsNullOrEmpty(ACCESS_TOKEN))
                            {
                                _logger.LogInformation("塔吊推送未获取到大运Token，推送结束。");
                                _logger.LogInformation(string.Format("成功推送了以下 {0} 台塔吊设备的数据:{1}.", count, devicecodeall));
                                return;
                            }
                        }
                        if (Convert.ToString(jo["flag"]) != SUCCESS)
                        {
                            _logger.LogError("塔吊设备：" + towerParam["device_id"] + "URL:" + towerUrl + "请求返回：" + Environment.NewLine + response);
                        }
                        else
                        {
                            devicecodeall.AppendFormat("{0} ", towerParam["device_id"]);
                            count++;
                        }
                    }
                    _logger.LogInformation(string.Format("成功推送了以下 {0} 台塔吊设备的数据:{1}.", count, devicecodeall));
                }
                catch (Exception ex)
                {
                    _logger.LogError("塔吊设备请求出错：" + ex.ToString());
                }
            }

            //升降机注册

            SQL = @"select ddy.*,a.sename,a.paramjson,a.secode,a.setype,a.sestatus,a.remark,a.operatedate
                    FROM T_GC_DeviceDY ddy INNER JOIN vDeviceRtd vdr ON ddy.DEVICEID=vdr.DEVICEID AND vdr.sitearea <> 47 AND vdr.GROUPID <> 14 
                    inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  
                    inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID And len(a.remark ) >0 AND a.setype = 2 AND DATEADD(SECOND,90,a.operatedate) > GETDATE()";
            var ElevatorDt = _baseRepository.Db.Ado.GetDataTable(SQL);
            devicecodeall = new StringBuilder(string.Empty);
            if (ElevatorDt != null && ElevatorDt.Rows.Count > 0)
            {
                try
                {
                    SortedDictionary<string, string> towerParam = null;
                    int count = 0;
                    sb = null;
                    for (var i = 0; i < ElevatorDt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(ElevatorDt.Rows[i]["paramjson"])))//必填项
                        {
                            continue;
                        }
                        var paramdata = JsonConvert.DeserializeObject<GCSpecialEqpParamjson>(HttpUtility.HtmlDecode(Convert.ToString(ElevatorDt.Rows[i]["paramjson"])));
                        towerParam = new SortedDictionary<string, string>();
                        sb = new StringBuilder(string.Empty);
                        towerParam["device_id"] = Convert.ToString(ElevatorDt.Rows[i]["secode"]);
                        towerParam["contract_record_code"] = Convert.ToString(ElevatorDt.Rows[i]["contract_record_code"]);
                        towerParam["usageNo"] = Convert.ToString(ElevatorDt.Rows[i]["remark"]);
                        towerParam["propertyNo"] = Convert.ToString(ElevatorDt.Rows[i]["remark"]);
                        towerParam["MaxPerson"] = paramdata.MaxPerson;//最大人数
                        towerParam["MaxWeight"] = paramdata.MaxWeight;//最大重量，单位kg
                        string local_cage = "M";
                        if (Convert.ToString(ElevatorDt.Rows[i]["sename"]).Contains("左"))
                        {
                            local_cage = "L";
                        }
                        else if (Convert.ToString(ElevatorDt.Rows[i]["sename"]).Contains("右"))
                        {
                            local_cage = "R";
                        }
                        towerParam["local_cage"] = local_cage;//笼位
                        towerParam["datetime"] = Convert.ToDateTime(ElevatorDt.Rows[i]["operatedate"]).ToString(TIME_FORMAT);

                        asciiDic = new Dictionary<string, object>();
                        arrKeys = towerParam.Keys.ToArray();
                        Array.Sort(arrKeys, string.CompareOrdinal);
                        foreach (var key in arrKeys)
                        {
                            var value = towerParam[key];
                            asciiDic.Add(key, value);
                        }
                        foreach (var item in asciiDic)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                            {
                                sb.Append(item.Value);
                            }
                        }
                        sb.Append(ACCESS_TOKEN);
                        string sign = UEncrypter.SHA256(sb.ToString());

                        string towerUrl = string.Format("http://{0}/rest/Elevator/Info/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                        string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                        JObject jo = JObject.Parse(response);
                        if (Convert.ToString(jo["flag"]) == "0006" || Convert.ToString(jo["flag"]) == "0007" || Convert.ToString(jo["flag"]) == "0010")
                        {
                            token = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken?isForce=true");

                            if (!string.IsNullOrEmpty(token))
                            {
                                tokenjob = JObject.Parse(token);
                            }
                            ACCESS_TOKEN = tokenjob.GetValue("data").ToString();
                            if (string.IsNullOrEmpty(ACCESS_TOKEN))
                            {
                                _logger.LogInformation("升降机推送未获取到大运Token，推送结束。");
                                _logger.LogInformation(string.Format("成功推送了以下 {0} 台升降机设备的数据:{1}.", count, devicecodeall));
                                return;
                            }
                        }
                        if (Convert.ToString(jo["flag"]) != SUCCESS)
                        {
                            _logger.LogError("升降机设备：" + towerParam["device_id"] + "URL:" + towerUrl + "请求返回：" + Environment.NewLine + response);
                        }
                        else
                        {
                            //_logger.LogInformation("升降机" + towerParam["device_id"] + "设备数据推送成功");
                            devicecodeall.AppendFormat("{0} ", towerParam["device_id"]);
                            count++;
                        }
                    }
                    _logger.LogInformation(string.Format("成功推送了以下 {0} 台升降机设备的数据:{1}.", count, devicecodeall));
                }
                catch (Exception ex)
                {
                    _logger.LogError("升降机设备请求出错：" + ex.ToString());
                }

            }

            //升降机

            SQL = @"select ddy.*,a.paramjson,a.secode,a.sestatus,a.remark,b.sedata,b.alarmstate,b.updatedate FROM T_GC_DeviceDY ddy inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID inner join (select * from ( select ROW_NUMBER() over(partition by secode order by updatedate desc) RowNum ,T_GC_SpecialEqpRtdData.* from T_GC_SpecialEqpRtdData where updatedate>'" + DateTime.Now.AddMinutes(-1).ToString() + "' ) as t1  where RowNum = 1) b on a.secode=b.secode where a.setype=2 and a.bdel=0 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0  ";
            ElevatorDt = _baseRepository.Db.Ado.GetDataTable(SQL);
            devicecodeall = new StringBuilder(string.Empty);
            if (ElevatorDt != null && ElevatorDt.Rows.Count > 0)
            {
                try
                {
                    SortedDictionary<string, string> towerParam = null;
                    int count = 0;
                    sb = null;
                    for (var i = 0; i < ElevatorDt.Rows.Count; i++)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(ElevatorDt.Rows[i]["sedata"])))//必填项
                        {
                            continue;
                        }
                        var sedata = JsonConvert.DeserializeObject<ElevatorRealDataInput>(HttpUtility.HtmlDecode(Convert.ToString(ElevatorDt.Rows[i]["sedata"])));
                        towerParam = new SortedDictionary<string, string>();
                        sb = new StringBuilder(string.Empty);
                        towerParam["prj_id"] = Convert.ToString(ElevatorDt.Rows[i]["prj_id"]);
                        towerParam["prj_name"] = Convert.ToString(ElevatorDt.Rows[i]["prj_name"]);
                        towerParam["owner_name"] = Convert.ToString(ElevatorDt.Rows[i]["owner_name"]);
                        towerParam["device_type"] = "升降机";// Convert.ToString(towerDt.Rows[i]["device_type"]);
                        towerParam["device_id"] = Convert.ToString(ElevatorDt.Rows[i]["secode"]);
                        towerParam["contract_record_code"] = Convert.ToString(ElevatorDt.Rows[i]["contract_record_code"]);
                        towerParam["usageNo"] = Convert.ToString(ElevatorDt.Rows[i]["remark"]);
                        towerParam["propertyNo"] = Convert.ToString(ElevatorDt.Rows[i]["remark"]);
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
                        towerParam["is_speed_alarm"] = "0";
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

                        asciiDic = new Dictionary<string, object>();
                        arrKeys = towerParam.Keys.ToArray();
                        Array.Sort(arrKeys, string.CompareOrdinal);
                        foreach (var key in arrKeys)
                        {
                            var value = towerParam[key];
                            asciiDic.Add(key, value);
                        }
                        foreach (var item in asciiDic)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                            {
                                sb.Append(item.Value);
                            }
                        }
                        sb.Append(ACCESS_TOKEN);
                        string sign = UEncrypter.SHA256(sb.ToString());

                        string towerUrl = string.Format("http://{0}/rest/Elevator/addRealTimeData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                        string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(asciiDic), UHttp.CONTENT_TYPE_JSON);
                        JObject jo = JObject.Parse(response);
                        if (Convert.ToString(jo["flag"]) == "0006" || Convert.ToString(jo["flag"]) == "0007" || Convert.ToString(jo["flag"]) == "0010")
                        {
                            token = HttpNetRequest.HttpGet("http://wx.admin.xhs-sz.com:9035/api/specialeqp/getwxdytoken?isForce=true");

                            if (!string.IsNullOrEmpty(token))
                            {
                                tokenjob = JObject.Parse(token);
                            }
                            ACCESS_TOKEN = tokenjob.GetValue("data").ToString();
                            if (string.IsNullOrEmpty(ACCESS_TOKEN))
                            {
                                _logger.LogInformation("升降机推送未获取到大运Token，推送结束。");
                                _logger.LogInformation(string.Format("成功推送了以下 {0} 台升降机设备的数据:{1}.", count, devicecodeall));
                                return;
                            }
                        }
                        if (Convert.ToString(jo["flag"]) != SUCCESS)
                        {
                            _logger.LogError("升降机设备：" + towerParam["device_id"] + "URL:" + towerUrl + "请求返回：" + Environment.NewLine + response);
                        }
                        else
                        {
                            //_logger.LogInformation("升降机" + towerParam["device_id"] + "设备数据推送成功");
                            devicecodeall.AppendFormat("{0} ", towerParam["device_id"]);
                            count++;
                        }
                    }
                    _logger.LogInformation(string.Format("成功推送了以下 {0} 台升降机设备的数据:{1}.", count, devicecodeall));
                }
                catch (Exception ex)
                {
                    _logger.LogError("升降机设备请求出错：" + ex.ToString());
                }

            }


        }
    }
}
