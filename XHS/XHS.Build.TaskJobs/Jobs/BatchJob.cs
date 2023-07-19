using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Common.Util;
using XHS.Build.Common.Wechat;
using XHS.Build.Services.CityAqi;
using XHS.Build.Services.DailyJob;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.Fog;
using XHS.Build.Services.SmsQueue;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Services.Warning;
using XHS.Build.TaskJobs.Jobs;
using static XHS.Build.Common.Helps.HpFog;

namespace XHS.Build.TaskJobs
{
    public class BatchJob : JobBase, IJob
    {
        private readonly IWarningService _warningService;
        private readonly ILogger<BatchJob> _logger;
        private readonly IDeviceCNService _deviceCNService;
        private readonly IConfiguration _configuration;
        private readonly IDailyJobService _dailyJobService;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly ISystemSettingService _systemSettingService;
        private readonly ICityAqiService _cityAqiService;
        private readonly ISmsQueueService _smsQueueService;
        private readonly HpAliSMS _hpAliSMS;
        private const string DB_NAME = "XJ_Env";
        private const char SEPARATOR = ';';
        private const string URL_WARN = "http://{0}:9027/Services/WSTemplateMessage.svc";
        private const string SRT_ALERT = "调用了{0}的特种设备未安装提醒服务！[{1}]";


        public BatchJob(ILogger<BatchJob> logger, IDeviceCNService deviceCNService, IConfiguration configuration, IDailyJobService dailyJobService, IHpSystemSetting hpSystemSetting, ISystemSettingService systemSettingService, ICityAqiService cityAqiService, ISmsQueueService smsQueueService, HpAliSMS hpAliSMS, IWarningService warningService)
        {
            _logger = logger;
            _deviceCNService = deviceCNService;
            _configuration = configuration;
            _dailyJobService = dailyJobService;
            _hpSystemSetting = hpSystemSetting;
            _systemSettingService = systemSettingService;
            _cityAqiService = cityAqiService;
            _smsQueueService = smsQueueService;
            _hpAliSMS = hpAliSMS;
            _warningService = warningService;

        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            //执行批处理
            try
            {
                int timeout = _configuration.GetSection("DbBackupTimeout").Get<int>();
                var res = await _deviceCNService.doBatch(timeout);
                if (res > 0)
                {
                    _logger.LogInformation("执行批处理成功,并生成 {0} 条数据!", res);
                }
                else
                {
                    _logger.LogInformation("已执行批处理!");
                }
            }
            catch (Exception)
            {

                throw;
            }
            

            //波浪推送
            BoSea();

            //调用特种设备未安装提醒服务
            //调用特种设备未安装提醒服务
            if (DateTime.Now.Hour.Equals(9) && DateTime.Now.Minute > 0 && DateTime.Now.Minute < 11)
            {
                try
                {
                    string domain = _configuration.GetSection("WcfDomain").Get<string>();

                    //string uri = "http://test.xhs-sz.com:9027/Services/WSTemplateMessage.svc";
                    object result = HpWcfInvoker.ExecuteMethd<IWSTemplateMessage>(string.Format(URL_WARN, domain), "sendSeAlert");
                    if (result != null && !Convert.ToInt32(result).Equals(0))
                    {//如果发生了发信才写日志
                        _logger.LogInformation(string.Format(SRT_ALERT, domain, result));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }

            //调用获取城市AQI
            try
            {
                InsertCityAqi();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace, true);
            }

            //调用获取城市实时天气
            try
            {
                UpdateCityWeather();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace, true);
            }

            //发送提醒
            _smsQueueService.SendSmsAll();

            ///* 调用发送微信短信报警服务 */
            _hpAliSMS.SendWarnAll(_configuration.GetSection("AppName").Get<string>());

            //同步钢丝绳
            //每天8点,钢丝绳损伤报警
            if (DateTime.Now.Hour.Equals(8))
            {
                _dailyJobService.CableDoSync();
            }


            //数据库备份
            try
            {
                _logger.LogInformation("数据库备份开始!", false);
                int dayDiff = _configuration.GetSection("DbBackupPeriod").Get<int>();//时间间隔（单位：天）
                                                                                     //清理数据库备份目录下过期文件
                string fullPath = Path.Combine(_configuration.GetSection("DbBackupPath").Get<string>(), DB_NAME);
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
                string LOG_CLEAR = "执行日志清理.";
                UFile.ClearExpiredFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../xjlog"),
                    _configuration.GetSection("LogSaveDay").Get<int>(), Const.FileEx.LOG_ALL, true, false);
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
                string TMP_CLEAR = "执行临时文件夹清理.";
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
                string codePath = _configuration.GetSection("TmpCodePath").Get<string>();
                string[] codePathArray = codePath.Split(SEPARATOR);

                foreach (string path in codePathArray)
                {
                    if (string.IsNullOrEmpty(path) || !UFile.IsExistDirectory(path))
                        continue;

                    try
                    {
                        string CODE_CLEAR = "执行验证码清理.";
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


            //自动雾炮
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

        }


        #region 波浪推送
        private static string getSignature(string timestamp, string nonce, string jdzch, string token, string data)
        {
            string str = timestamp + nonce + jdzch + token + data;      //拼一起
            char[] intArray = str.ToCharArray();                        //字符数组
            Array.Sort(intArray);                                       //排序
            str = string.Join(string.Empty, intArray);                  //再拼一起
            string result = UEncrypter.EncryptByMD5(str);               //MD5加密
            return result;
        }
        public async void BoSea()
        {
            if (!_configuration.GetSection("IsBolang").Get<bool>())
                return;

            bool hasError = false;
            try
            {
                string filename = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S024), "bolang", "device.csv.true");
                if (!UFile.IsExistFile(filename))
                    return;

                ///先把文件读好
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
                        string urlToken = string.Format("http://116.147.2.91:8088/api/Yc/GetToken?bacode={0}&username={1}&password={2}",
                            bacode, list[i][1], list[i][2]);
                        string result = UHttp.Post(urlToken, string.Empty);
                        JObject jobject = JObject.Parse(result);
                        if (string.IsNullOrEmpty(Convert.ToString(jobject["result"])) || !Convert.ToBoolean(jobject["result"]))
                        {
                            hasError = true;
                            _logger.LogInformation(string.Format("推送编号 {0} 的数据失败. {1}{2}", list[i][4], Environment.NewLine, result), true);
                            continue;
                        }
                        string AccessToken = Convert.ToString(jobject["data"]["AccessToken"]);


                        ///定义header
                        WebHeaderCollection header = new WebHeaderCollection();
                        string timestamp = Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                        string nonce = DateTime.Now.ToString("mmssfffffff");//随机数
                        string jdzch = list[i][3];//项目注册号
                        ///实时数据
                        DataRow dr = await _deviceCNService.getOneForSend(list[i][4]);
                        if (dr == null)
                            continue;
                        //拼xml
                        string data = string.Format("<sbbm>{0}</sbbm><PM25>{1}</PM25><PM10>{2}</PM10><Wd>{3}</Wd><Sd>{4}</Sd>" +
                            "<Fx>{5}</Fx><Fs>{6}</Fs><TSP>{7}</TSP><Noise>{8}</Noise><pressure>{9}</pressure>",
                            list[i][4], dr["pm2_5"], dr["pm2_5"], dr["pm10"], dr["temperature"],
                            dr["dampness"], dr["direction"], dr["speed"], dr["noise"], dr["atmos"]);

                        ///upload地址
                        string urlUpload = string.Format("http://116.147.2.91:8088/api/Yc/UploadData?bacode={0}&token={1}&data={2}",
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
                            hasError = true;
                            _logger.LogInformation(string.Format("推送编号 {0} 的数据失败. {1}{2}", list[i][4], Environment.NewLine, jobject.ToString()), true);
                        }
                    }
                    catch (Exception ex)
                    {
                        hasError = true;
                        _logger.LogInformation(ex.Message, true);
                    }
                }
                _logger.LogInformation(string.Format("共 {0} 条记录. 成功推送了 {1} 条.", list.Count, okCount));
            }
            catch (Exception ex)
            {
                hasError = true;
                _logger.LogInformation(ex.Message, true);
            }
            finally
            {
                if (!hasError)
                    _logger.LogInformation("顺利滴推死了博浪! v(^O^)v");
            }
        }
        #endregion

        public interface IWSTemplateMessage
        {
            /// <summary>
            /// 群发模板消息
            /// </summary>
            /// <param name="type">微信模板消息种类</param>
            /// <returns></returns>
            [OperationContract]
            int sendByType(WTemplateMessager.MType type);
            /// <summary>
            /// 单发模板消息
            /// </summary>
            /// <param name="WARNID">警告ID</param>
            /// <returns></returns>
            [OperationContract]
            int sendById(int WARNID);
            /// <summary>
            /// 特种设备未安装提醒
            /// </summary>
            /// <returns></returns>
            [OperationContract]
            int sendSeAlert();

        }

        

        #region 调用获取城市AQI
        /// <summary>
        /// 获取城市AQI（和风天气）
        /// </summary>
        private async void InsertCityAqi()
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
            string o3s = "";
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
                    o3s += jobjcityall["air_now_city"]["o3"] + ",";
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
                        o3s += jarysiteall[i]["o3"] + ",";
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
                o3s = o3s.Substring(0, o3s.Length - 1);
                qualitys = qualitys.Substring(0, qualitys.Length - 1);
                aqis = aqis.Substring(0, aqis.Length - 1);

                SgParams sp = new SgParams();
                sp.Add("areas", areas);
                sp.Add("positionnames", positionnames);
                sp.Add("pm25s", pm25s);
                sp.Add("o3s", o3s);
                sp.Add("pm10s", pm10s);
                sp.Add("qualitys", qualitys);
                sp.Add("aqis", aqis);
                sp.Add("billdate", billdate);
                int ret = await _cityAqiService.doInsert(sp);
                _logger.LogInformation("城市AQI插入数据库（" + ret + "）", false);
            }
        }
        #endregion


        #region 调用获取城市获取天气


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

                SgParams sp = new SgParams();
                sp.Add("area", citylist[k]);
                sp.Add("weather", weather);
                sp.Add("updatedate", updatedate);
                _cityAqiService.doWeatherUpdate(sp);
            }
        }
        #endregion

       

    }
}
