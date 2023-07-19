using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadMinuteXinwuquJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadMinuteXinwuquJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadMinuteXinwuquJob(ILogger<SmartUpLoadMinuteXinwuquJob> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService)
        {
            _logger = logger;  
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("数据上传开始。", true);
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();
            int successcount = 0;
            int errcount = 0;
            //对接数据获取
            DataSet ds = await _aqtUploadService.GetListsForXinwuquMinute();
            string api = "";
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            try
                            {
                                DataRow dr = dt.Rows[j];
                                JObject jso = new JObject();
                                foreach (DataColumn column in dr.Table.Columns)
                                {
                                    if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.Decimal")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDouble());
                                    }
                                    else if (column.DataType.Equals(System.Type.GetType("System.DateTime")))
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToDateTime());
                                    }
                                    else
                                    {
                                        jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                                    }
                                }
                                string url = jso.GetValue("siteuploadurl").ToString();
                                string account = jso.GetValue("uploadaccount").ToString();
                                string pwd = jso.GetValue("uploadpwd").ToString();
                                api = jso.GetValue("post").ToString();
                                string realapi = api;
                                string dataxml = "";
                                if (api.Contains("UploadHistory"))      //卸料平台实时数据上传
                                {
                                    realapi = "insertPlat";
                                    if (jso.ContainsKey("rtdjson"))
                                    {
                                        string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                        string DEVICE_ID = jso.GetValue("deviceId").ToString();
                                        string DEVICE_NAME = jso.GetValue("unloadname").ToString();
                                        string OWNER_NAME = "";
                                        string DEVICE_TYPE = "";
                                        string RATED_WEIGHT = "";
                                        string WEIGHT = "";
                                        string BATTERY = "";
                                        string WEIGHT_SENSOR_STATUS = "0";
                                        string BATTERY_DEVICE_STATUS = "0";
                                        string IS_WEIGHT_ALARM = "0";
                                        string IS_BATTERY_ALARM = "0";
                                        string IS_DEVICE_FAILURE_ALARM = "0";
                                        var data1 = JObject.Parse(jso.GetValue("rtdjson").ToString());
                                        if (data1.ContainsKey("weight"))
                                        {
                                            WEIGHT = data1.GetValue("weight").ToString();
                                        }
                                        if (data1.ContainsKey("early_warning_weight"))
                                        {
                                            RATED_WEIGHT = data1.GetValue("early_warning_weight").ToString();
                                        }
                                        if (data1.ContainsKey("alarm_weight"))
                                        {
                                            double alarm_weight = data1.GetValue("alarm_weight").ToDouble();
                                            double weight = data1.GetValue("weight").ToDouble();
                                            if (weight >= alarm_weight)
                                            {
                                                IS_WEIGHT_ALARM = "1";
                                            }
                                        }
                                        if (data1.ContainsKey("electric_quantity"))
                                        {
                                            double quantity = data1.GetValue("electric_quantity").ToDouble();
                                            if (quantity < 10)
                                            {
                                                IS_BATTERY_ALARM = "1";
                                            }
                                            BATTERY = data1.GetValue("electric_quantity").ToString();
                                        }
                                        dataxml = string.Format(XML24, GONGCHENG_CODE, DEVICE_ID, DEVICE_NAME, OWNER_NAME, DEVICE_TYPE, RATED_WEIGHT, WEIGHT, BATTERY, WEIGHT_SENSOR_STATUS, BATTERY_DEVICE_STATUS, IS_WEIGHT_ALARM, IS_BATTERY_ALARM, IS_DEVICE_FAILURE_ALARM);

                                    }
                                }
                                else if (api.Contains("Craneinterface/UploadCraneHistory"))
                                {
                                    //分钟
                                    //塔机实时数据上传
                                    realapi = "insertTower";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                        string DEVICE_ID = jso.GetValue("deviceId").ToString();
                                        string DEVICE_NAME = jso.GetValue("name").ToString();
                                        string OWNER_NAME = "";
                                        string DEVICE_TYPE = jso.GetValue("model").ToString();
                                        string SYDJBH = "";
                                        string DRIVER_NAME = "";
                                        string DRIVER_ID = "";
                                        string LIMIT_WEIGHT = "";
                                        string LIFTING_WEIGHT = "0";
                                        string LIMIT_HEIGHT = "";
                                        string LIFTING_HEIGHT = "0";
                                        string ARM_LENGTH = "0";
                                        string RADIUS = "0";
                                        string RATED_TORQUE = "0";
                                        string TORQUE = "0";
                                        string TORQUE_PERCENTAGE = "0";
                                        string RATED_ROTATION = "0";
                                        string ROTATION = "0";
                                        string WIND_SPEED = "0";
                                        string DIP_ANGLE = "0";
                                        string WEIGHT_SENSOR_STATUS = "0";
                                        string TORQUE_SENSOR_STATUS = "0";
                                        string HEIGHT_SENSOR_STATUS = "0";
                                        string RADIUS_SENSOR_STATUS = "0";
                                        string ROTATION_SENSOR_STATUS = "0";
                                        string WIND_SPEED_SENSOR_STATUS = "0";
                                        string FACE_RECOGNITION_STATUS = "0";
                                        string IS_WEIGHT_ALARM = "0";
                                        string IS_TORQUE_ALARM = "0";
                                        string IS_HEIGHT_ALARM = "0";
                                        string IS_RADIUS_ALARM = "0";
                                        string IS_COLLISION_ALARM = "0";
                                        string IS_ROTATION_ALARM = "0";
                                        string IS_AREA_LIMIT_ALARM = "0";
                                        string IS_WIND_SPEED_ALARM = "0";
                                        string IS_DIP_ANGLE_ALARM = "0";
                                        string IS_AUTHORIZATION = "0";
                                        string TOP_LIMIT_ALARM = "0";
                                        string IS_POWER = "0";
                                        string DRIVER_IMAGE = "0";
                                        var data1 = JObject.Parse(jso.GetValue("sedata").ToString());
                                        if (data1.ContainsKey("SafeLoad"))
                                        {
                                            LIMIT_WEIGHT = data1.GetValue("SafeLoad").ToString();
                                        }
                                        if (data1.ContainsKey("Margin"))
                                        {
                                            RADIUS = data1.GetValue("Margin").ToString();
                                        }
                                        if (data1.ContainsKey("MomentPercent"))
                                        {
                                            TORQUE_PERCENTAGE = data1.GetValue("MomentPercent").ToString();
                                        }
                                        if (data1.ContainsKey("Rotation"))
                                        {
                                            ROTATION = data1.GetValue("Rotation").ToString();
                                        }
                                        if (data1.ContainsKey("Height"))
                                        {
                                            LIFTING_HEIGHT = data1.GetValue("Height").ToString();
                                        }
                                        if (data1.ContainsKey("WindSpeed"))
                                        {
                                            WIND_SPEED = data1.GetValue("WindSpeed").ToString();
                                        }
                                        if (data1.ContainsKey("Moment"))
                                        {
                                            TORQUE = data1.GetValue("Moment").ToString();
                                        }
                                        if (data1.ContainsKey("Weight"))
                                        {
                                            LIFTING_WEIGHT = data1.GetValue("Weight").ToString();
                                        }
                                        if (data1.ContainsKey("DriverCardNo"))
                                        {
                                            DRIVER_ID = data1.GetValue("DriverCardNo").ToString();
                                        }
                                        if (data1.ContainsKey("DriverName"))
                                        {
                                            DRIVER_NAME = data1.GetValue("DriverName").ToString();
                                        }

                                        var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(data1.GetValue("Alarm")));
                                        if (intList.Contains(1) || intList.Contains(2))//高度报警
                                        {
                                            HEIGHT_SENSOR_STATUS = "1";//高度传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            IS_HEIGHT_ALARM = "1";//高度报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            HEIGHT_SENSOR_STATUS = "0";
                                            IS_HEIGHT_ALARM = "0";
                                        }
                                        if (intList.Contains(4) || intList.Contains(8))//幅度
                                        {
                                            RADIUS_SENSOR_STATUS = "1";//幅度传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            IS_RADIUS_ALARM = "1";//幅度报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            RADIUS_SENSOR_STATUS = "0";
                                            IS_RADIUS_ALARM = "0";
                                        }
                                        if (intList.Contains(16) || intList.Contains(32))//幅度
                                        {
                                            ROTATION_SENSOR_STATUS = "1";//回转传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            IS_ROTATION_ALARM = "1";//回转报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            ROTATION_SENSOR_STATUS = "0";
                                            IS_ROTATION_ALARM = "0";
                                        }
                                        if (intList.Contains(64))//重量
                                        {
                                            WEIGHT_SENSOR_STATUS = "1";//重量传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            IS_WEIGHT_ALARM = "1";//重量报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            WEIGHT_SENSOR_STATUS = "0";
                                            IS_WEIGHT_ALARM = "0";
                                        }
                                        if (intList.Contains(128))
                                        {
                                            TORQUE_SENSOR_STATUS = "1";//力矩传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            IS_TORQUE_ALARM = "1";//力矩报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            TORQUE_SENSOR_STATUS = "0";
                                            IS_TORQUE_ALARM = "0";
                                        }
                                        if (intList.Contains(256))
                                        {
                                            WIND_SPEED_SENSOR_STATUS = "1";//风速传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            IS_WIND_SPEED_ALARM = "1";//风速报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            WIND_SPEED_SENSOR_STATUS = "0";
                                            IS_WIND_SPEED_ALARM = "0";
                                        }
                                        dataxml = string.Format(XML23, GONGCHENG_CODE, DEVICE_ID, DEVICE_NAME, OWNER_NAME, DEVICE_TYPE, SYDJBH,
                                            DRIVER_NAME, DRIVER_ID, LIMIT_WEIGHT, LIFTING_WEIGHT, LIMIT_HEIGHT, LIFTING_HEIGHT,
                                            ARM_LENGTH, RADIUS, RATED_TORQUE, TORQUE, TORQUE_PERCENTAGE, RATED_ROTATION, ROTATION, WIND_SPEED
                                            , DIP_ANGLE, WEIGHT_SENSOR_STATUS, TORQUE_SENSOR_STATUS, HEIGHT_SENSOR_STATUS, RADIUS_SENSOR_STATUS,
                                            ROTATION_SENSOR_STATUS, WIND_SPEED_SENSOR_STATUS, FACE_RECOGNITION_STATUS, IS_WEIGHT_ALARM
                                            , IS_TORQUE_ALARM, IS_HEIGHT_ALARM, IS_RADIUS_ALARM, IS_COLLISION_ALARM, IS_ROTATION_ALARM, IS_AREA_LIMIT_ALARM
                                            , IS_WIND_SPEED_ALARM, IS_DIP_ANGLE_ALARM, IS_AUTHORIZATION, TOP_LIMIT_ALARM, TOP_LIMIT_ALARM, DRIVER_IMAGE);
                                    }
                                }
                                else if (api.Contains("Hoistinterface/HoistHistory"))
                                {
                                    //分钟
                                    //施工升降机实时数据上传
                                    realapi = "insertSgsjj";
                                    if (jso.ContainsKey("sedata"))
                                    {
                                        string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                        string DEVICE_ID = jso.GetValue("deviceId").ToString();
                                        string DEVICE_NAME = jso.GetValue("name").ToString();
                                        string OWNER_NAME = "";
                                        string DEVICE_TYPE = "";
                                        string SYDJBH = "";
                                        string DRIVER_NAME = "";
                                        string DRIVER_ID = "";
                                        string RUN_STATUS = "";
                                        string WORK_START_TIME = "";
                                        string WORK_END_TIME = "";
                                        string START_HEIGHT = "";
                                        string END_HEIGHT = "";
                                        string START_FLOOR = "";
                                        string END_FLOOR = "";
                                        string WEIGHT = "";
                                        string SPEED = "";
                                        string WIND_SPEED = "";
                                        string DIP_ANGLE_X = "";
                                        string DIP_ANGLE_Y = "";
                                        string BATTERY = "";
                                        string BATTERY_DEVICE_STATUS = "0";
                                        string POWER_STATUS = "0";
                                        string WEIGHT_SENSOR_STATUS = "0";
                                        string DIP_ANGLE_SENSOR_STATUS = "0";
                                        string SPEED_SENSOR_STATUS = "0";
                                        string FACE_RECOGNITION_STATUS = "0";
                                        string SINGLE_DOOR_STATUS = "0";
                                        string DOUBLE_DOOR_STATUS = "0";
                                        string TOP_DOOR_STATUS = "0";
                                        string WEIGHT_ALARM = "0";
                                        string SPEED_ALARM = "0";
                                        string HEIGHT_ALARM = "0";
                                        string DIP_ANGLE_X_ALARM = "0";
                                        string DIP_ANGLE_Y_ALARM = "0";
                                        string SINGLE_DOOR_ALARM = "0";
                                        string DOUBLE_DOOR_ALARM = "0";
                                        string TOP_DOOR_ALARM = "0";
                                        string TOP_LIMIT_ALARM = "0";
                                        string BOTTOM_LIMIT_ALARM = "0";
                                        string ANTIDROP_ALARM = "0";
                                        string BATTERY_ALARM = "0";
                                        string AUTHORIZATION = "0";
                                        string POWER_ALARM = "0";
                                        string DRIVER_IMAGE = "";
                                        var data1 = JObject.Parse(jso.GetValue("sedata").ToString());
                                        if (data1.ContainsKey("Height"))
                                        {
                                            START_HEIGHT = data1.GetValue("Height").ToString();
                                        }
                                        if (data1.ContainsKey("Weight"))
                                        {
                                            WEIGHT = data1.GetValue("Weight").ToString();
                                        }
                                        if (data1.ContainsKey("Speed"))
                                        {
                                            SPEED = data1.GetValue("Speed").ToString();
                                        }
                                        if (data1.ContainsKey("DriverCardNo"))
                                        {
                                            DRIVER_ID = data1.GetValue("DriverCardNo").ToString();
                                        }
                                        if (data1.ContainsKey("DriverName"))
                                        {
                                            DRIVER_NAME = data1.GetValue("DriverName").ToString();
                                        }

                                        var intList = SpecialEqpHelp.getSpecErrList(Convert.ToInt32(data1.GetValue("Alarm")));
                                        if (intList.Contains(1))//倾角报警
                                        {
                                            DIP_ANGLE_SENSOR_STATUS = "1";//倾角传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            DIP_ANGLE_X_ALARM = "1";//倾角报警，0：正常,1：报警
                                            DIP_ANGLE_Y_ALARM = "1";//倾角报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            DIP_ANGLE_SENSOR_STATUS = "0";
                                            DIP_ANGLE_X_ALARM = "0";
                                            DIP_ANGLE_Y_ALARM = "0";
                                        }
                                        if (intList.Contains(2) || intList.Contains(8))//上限位报警
                                        {
                                            TOP_LIMIT_ALARM = "1";//上限位报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            TOP_LIMIT_ALARM = "0";
                                        }
                                        if (intList.Contains(4) || intList.Contains(16))//下限位报警
                                        {
                                            BOTTOM_LIMIT_ALARM = "1";//下限位报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            BOTTOM_LIMIT_ALARM = "0";
                                        }
                                        if (intList.Contains(32))//单门
                                        {
                                            SINGLE_DOOR_ALARM = "1";//单门报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            SINGLE_DOOR_ALARM = "0";
                                        }
                                        if (intList.Contains(64))//双门
                                        {
                                            DOUBLE_DOOR_ALARM = "1";//双门报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            DOUBLE_DOOR_ALARM = "0";
                                        }
                                        if (intList.Contains(128))//顶门
                                        {
                                            TOP_DOOR_ALARM = "1";//顶门报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            TOP_DOOR_ALARM = "0";
                                        }
                                        if (intList.Contains(512))//重量
                                        {
                                            WEIGHT_SENSOR_STATUS = "1";//重量传感器状态，0：正常,1：报警,2：离线,3：故障,4：违章,5：非正常断电
                                            WEIGHT_ALARM = "1";//重量报警，0：正常,1：报警
                                        }
                                        else
                                        {
                                            WEIGHT_SENSOR_STATUS = "0";
                                            WEIGHT_ALARM = "0";
                                        }
                                        dataxml = string.Format(XML22, GONGCHENG_CODE, DEVICE_ID, DEVICE_NAME, OWNER_NAME, DEVICE_TYPE, SYDJBH,
                                                DRIVER_NAME, DRIVER_ID, RUN_STATUS, WORK_START_TIME, WORK_END_TIME, START_HEIGHT,
                                                END_HEIGHT, START_FLOOR, END_FLOOR, WEIGHT, SPEED, WIND_SPEED, DIP_ANGLE_X, DIP_ANGLE_Y
                                                , BATTERY, BATTERY_DEVICE_STATUS, POWER_STATUS, WEIGHT_SENSOR_STATUS, DIP_ANGLE_SENSOR_STATUS,
                                                SPEED_SENSOR_STATUS, FACE_RECOGNITION_STATUS, SINGLE_DOOR_STATUS, DOUBLE_DOOR_STATUS
                                                , TOP_DOOR_STATUS, WEIGHT_ALARM, SPEED_ALARM, HEIGHT_ALARM, DIP_ANGLE_X_ALARM, DIP_ANGLE_Y_ALARM
                                                , SINGLE_DOOR_ALARM, DOUBLE_DOOR_ALARM, TOP_DOOR_ALARM, TOP_LIMIT_ALARM, BOTTOM_LIMIT_ALARM, ANTIDROP_ALARM
                                                , BATTERY_ALARM, AUTHORIZATION, POWER_ALARM, DRIVER_IMAGE);
                                    }
                                }
                                else if (api.Contains("DustInterface/UploadDustHistory"))
                                {
                                    //扬尘实时数据
                                    realapi = "insertZhgd";
                                    string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                    string MACHINE_ID = jso.GetValue("deviceId").ToString();
                                    string MONITOR_POINT = "";
                                    string STATUS = "在线";
                                    string PM25 = jso.GetValue("pm2dot5").ToString();
                                    string PM10 = jso.GetValue("pm10").ToString();
                                    string ZS = jso.GetValue("noise").ToString();
                                    string FX = jso.GetValue("windDirection").ToString();
                                    string FS = jso.GetValue("windSpeed").ToString();
                                    string WD = jso.GetValue("temperature").ToString();
                                    string SD = jso.GetValue("humidity").ToString();
                                    string QY = "";
                                    string TSP = "";
                                    dataxml = string.Format(XML21, MACHINE_ID, GONGCHENG_CODE, MONITOR_POINT, STATUS,
                                                PM25, PM10, ZS, FX, FS, WD, SD, QY, TSP);
                                }
                                if (string.IsNullOrEmpty(dataxml))
                                {
                                    continue;
                                }
                                string xml = string.Format(XMLBase, realapi, dataxml);
                                string response = UHttp.Post(url, xml, UHttp.CONTENT_TYPE_TEXT_XML);
                                string result = "";
                                if (!string.IsNullOrEmpty(response))
                                {
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(response);
                                    result = xmlDoc.InnerText;
                                    if (result == "000")
                                    {
                                        result = "操作成功。";
                                    }
                                }
                                else
                                {
                                    result = "操作失败。";
                                }
                                var LogEntity = new CityUploadOperateLog
                                {
                                    //Id=Guid.NewGuid().ToString(),
                                    url = url,
                                    api = realapi,
                                    account = account,
                                    param = xml,
                                    result = result,
                                    createtime = DateTime.Now
                                };
                                await _operateLogService.AddCityUploadApiLog(LogEntity);
                                if (string.IsNullOrEmpty(result))
                                {
                                    errcount++;
                                    break;
                                }
                                else
                                {
                                    if (result == "操作成功。")
                                    {
                                        if (!list.Contains(url + api))
                                        {
                                            await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                            list.Add(url + api);
                                        }
                                        successcount += successcount;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                               // return ResponseOutput.Ok(api + ":" + ex.Message);
                                 _logger.LogError(api + ":" + ex.Message, true);
                            }
                        }
                    }
                }
            }


            _logger.LogInformation("数据上传结束。", true);
        }

        /// <summary>
        /// 公共XML
        /// </summary>
        private static string XMLBase = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://service.framework.ccthanking.com/\">" +
            "<soapenv:Header/>" +
            "<soapenv:Body>" +
            "<ser:{0}>" +
            "<user>wndjs_is</user>" +
            "<password>wndjs_is123!!</password>" +
            "<xmlData><![CDATA[{1}" +
            "]]></xmlData>" +
             " </ser:{0}>" +
            "</soapenv:Body></soapenv:Envelope>";

        /// <summary>
        /// 2.1 扬尘在线监测信息XML
        /// </summary>
        private static string XML21 = "<XML><ZHGDPM><MACHINE_ID>{0}</MACHINE_ID><GONGCHENG_CODE>{1}</GONGCHENG_CODE><MONITOR_POINT>{2}</MONITOR_POINT><STATUS>{3}</STATUS><PM25>{4}</PM25><PM10>{5}</PM10><ZS>{6}</ZS><FX>{7}</FX><FS>{8}</FS><WD>{9}</WD><SD>{10}</SD><QY>{11}</QY><TSP>{12}</TSP></ZHGDPM></XML>";

        /// <summary>
        /// 2.2 施工升降机信息XML
        /// </summary>
        private static string XML22 = "<XML><SGSJJ><GONGCHENG_CODE>{0}</GONGCHENG_CODE><DEVICE_ID>{1}</DEVICE_ID><DEVICE_NAME>{2}</DEVICE_NAME><OWNER_NAME>{3}</OWNER_NAME><DEVICE_TYPE>{4}</DEVICE_TYPE><SYDJBH>{5}</SYDJBH><DRIVER_NAME>{6}</DRIVER_NAME><DRIVER_ID>{7}</DRIVER_ID><RUN_STATUS>{8}</RUN_STATUS><WORK_START_TIME>{9}</WORK_START_TIME><WORK_END_TIME>{10}</WORK_END_TIME><START_HEIGHT>{11}</START_HEIGHT><END_HEIGHT>{12}</END_HEIGHT><START_FLOOR>{13}</START_FLOOR><END_FLOOR>{14}</END_FLOOR><WEIGHT>{15}</WEIGHT><SPEED>{16}</SPEED><WIND_SPEED>{17}</WIND_SPEED><DIP_ANGLE_X>{18}</DIP_ANGLE_X><DIP_ANGLE_Y>{19}</DIP_ANGLE_Y><BATTERY>{20}</BATTERY><BATTERY_DEVICE_STATUS>{21}</BATTERY_DEVICE_STATUS><POWER_STATUS>{22}</POWER_STATUS><WEIGHT_SENSOR_STATUS>{23}</WEIGHT_SENSOR_STATUS><DIP_ANGLE_SENSOR_STATUS>{24}</DIP_ANGLE_SENSOR_STATUS><SPEED_SENSOR_STATUS>{25}</SPEED_SENSOR_STATUS><FACE_RECOGNITION_STATUS>{26}</FACE_RECOGNITION_STATUS><SINGLE_DOOR_STATUS>{27}</SINGLE_DOOR_STATUS><DOUBLE_DOOR_STATUS>{28}</DOUBLE_DOOR_STATUS><TOP_DOOR_STATUS>{29}</TOP_DOOR_STATUS><WEIGHT_ALARM>{30}</WEIGHT_ALARM><SPEED_ALARM>{31}</SPEED_ALARM><HEIGHT_ALARM>{32}</HEIGHT_ALARM><DIP_ANGLE_X_ALARM>{33}</DIP_ANGLE_X_ALARM><DIP_ANGLE_Y_ALARM>{34}</DIP_ANGLE_Y_ALARM><SINGLE_DOOR_ALARM>{35}</SINGLE_DOOR_ALARM><DOUBLE_DOOR_ALARM>{36}</DOUBLE_DOOR_ALARM><TOP_DOOR_ALARM>{37}</TOP_DOOR_ALARM><TOP_LIMIT_ALARM>{38}</TOP_LIMIT_ALARM><BOTTOM_LIMIT_ALARM>{39}</BOTTOM_LIMIT_ALARM><ANTIDROP_ALARM>{40}</ANTIDROP_ALARM><BATTERY_ALARM>{41}</BATTERY_ALARM><AUTHORIZATION>{42}</AUTHORIZATION><POWER_ALARM>{43}</POWER_ALARM><DRIVER_IMAGE>{44}</DRIVER_IMAGE></SGSJJ></XML>";

        /// <summary>
        /// 2.3 塔吊信息XML
        /// </summary>
        private static string XML23 = "<XML><TOWER><GONGCHENG_CODE>{0}</GONGCHENG_CODE><DEVICE_ID>{1}</DEVICE_ID><DEVICE_NAME>{2}</DEVICE_NAME><OWNER_NAME>{3}</OWNER_NAME><DEVICE_TYPE>{4}</DEVICE_TYPE><SYDJBH>{5}</SYDJBH><DRIVER_NAME>{6}</DRIVER_NAME><DRIVER_ID>{7}</DRIVER_ID><LIMIT_WEIGHT>{8}</LIMIT_WEIGHT><LIFTING_WEIGHT>{9}</LIFTING_WEIGHT><LIMIT_HEIGHT>{10}</LIMIT_HEIGHT><LIFTING_HEIGHT>{11}</LIFTING_HEIGHT><ARM_LENGTH>{12}</ARM_LENGTH><RADIUS>{13}</RADIUS><RATED_TORQUE>{14}</RATED_TORQUE><TORQUE>{15}</TORQUE><TORQUE_PERCENTAGE>{16}</TORQUE_PERCENTAGE><RATED_ROTATION>{17}</RATED_ROTATION><ROTATION>{18}</ROTATION><WIND_SPEED>{19}</WIND_SPEED><DIP_ANGLE>{20}</DIP_ANGLE><WEIGHT_SENSOR_STATUS>{21}</WEIGHT_SENSOR_STATUS><TORQUE_SENSOR_STATUS>{22}</TORQUE_SENSOR_STATUS><HEIGHT_SENSOR_STATUS>{23}</HEIGHT_SENSOR_STATUS><RADIUS_SENSOR_STATUS>{24}</RADIUS_SENSOR_STATUS><ROTATION_SENSOR_STATUS>{25}</ROTATION_SENSOR_STATUS><WIND_SPEED_SENSOR_STATUS>{26}</WIND_SPEED_SENSOR_STATUS><FACE_RECOGNITION_STATUS>{27}</FACE_RECOGNITION_STATUS><IS_WEIGHT_ALARM>{28}</IS_WEIGHT_ALARM><IS_TORQUE_ALARM>{29}</IS_TORQUE_ALARM><IS_HEIGHT_ALARM>{30}</IS_HEIGHT_ALARM><IS_RADIUS_ALARM>{31}</IS_RADIUS_ALARM><IS_COLLISION_ALARM>{32}</IS_COLLISION_ALARM><IS_ROTATION_ALARM>{33}</IS_ROTATION_ALARM><IS_AREA_LIMIT_ALARM>{34}</IS_AREA_LIMIT_ALARM><IS_WIND_SPEED_ALARM>{35}</IS_WIND_SPEED_ALARM><IS_DIP_ANGLE_ALARM>{36}</IS_DIP_ANGLE_ALARM><IS_AUTHORIZATION>{37}</IS_AUTHORIZATION><TOP_LIMIT_ALARM>{38}</TOP_LIMIT_ALARM><IS_POWER>{39}</IS_POWER><DRIVER_IMAGE>{40}</DRIVER_IMAGE></TOWER></XML>";

        /// <summary>
        /// 2.4 卸料平台信息XML
        /// </summary>
        private static string XML24 = "<XML><PLAT><GONGCHENG_CODE>{0}</GONGCHENG_CODE><DEVICE_ID>{1}</DEVICE_ID><DEVICE_NAME>{2}</DEVICE_NAME><OWNER_NAME>{3}</OWNER_NAME><DEVICE_TYPE>{4}</DEVICE_TYPE><RATED_WEIGHT>{5}</RATED_WEIGHT><WEIGHT>{6}</WEIGHT><BATTERY>{7}</BATTERY><WEIGHT_SENSOR_STATUS>{8}</WEIGHT_SENSOR_STATUS><BATTERY_DEVICE_STATUS>{9}</BATTERY_DEVICE_STATUS><IS_WEIGHT_ALARM>{10}</IS_WEIGHT_ALARM><IS_BATTERY_ALARM>{11}</IS_BATTERY_ALARM><IS_DEVICE_FAILURE_ALARM>{12}</IS_DEVICE_FAILURE_ALARM></PLAT></XML>";

    }
}
