using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadHourXinwuquJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadHourXinwuquJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadHourXinwuquJob(ILogger<SmartUpLoadHourXinwuquJob> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService)
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
            DataSet ds = await _aqtUploadService.GetListsForXinwuqu();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
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
                            string funingurl = "";
                            if (jso.ContainsKey("funingurl"))
                            {
                                funingurl = jso.GetValue("funingurl").ToString();
                                jso.Remove("funingurl");
                            }
                            string url = jso.GetValue("siteuploadurl").ToString();
                            string account = jso.GetValue("uploadaccount").ToString();
                            string pwd = jso.GetValue("uploadpwd").ToString();
                            string api = jso.GetValue("post").ToString();
                            string realapi = api;
                            string dataxml = "";

                            if (api.Contains("Check/FreeToShoot"))
                            {
                                string DETAIL_STATUS = jso.GetValue("DETAIL_STATUS").ToString();
                                //随手拍信息
                                realapi = "insertInforOfSsp";
                                string remark = string.IsNullOrEmpty(jso.GetValue("remark").ToString()) ? "" : HttpUtility.HtmlDecode(jso.GetValue("remark").ToString());
                                string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                string INSTASHOT_ID = jso.GetValue("INSTASHOT_ID").ToString();
                                string INSTASHOT_DATE = jso.GetValue("createtime").ToString();
                                string PERSON = jso.GetValue("createuser").ToString();
                                string PERSON_ROLE = "";
                                string POSITION = "";
                                string NEIRONG = "";
                                string URL = "";
                                JArray jarray = null;
                                if (!string.IsNullOrEmpty(remark))
                                {
                                    jarray = new JArray("[" + remark + "]");
                                    JObject job = (JObject)jarray[0];
                                    NEIRONG = job.GetValue("remark").ToString();
                                }
                                if (DETAIL_STATUS.Equals("1"))
                                {
                                    //随手拍数据上传
                                    dataxml = string.Format(XML210, GONGCHENG_CODE, INSTASHOT_ID, INSTASHOT_DATE, PERSON, PERSON_ROLE, POSITION, NEIRONG);
                                    string xml = string.Format(XMLBase, realapi, dataxml);
                                    string response = UHttp.Post(url, xml, UHttp.CONTENT_TYPE_TEXT_XML);
                                }
                                //随手拍明细数据上传
                                realapi = "insertInforOfSspDetail";
                                dataxml = string.Format(XML211, GONGCHENG_CODE, INSTASHOT_ID, DETAIL_STATUS, NEIRONG, INSTASHOT_DATE, PERSON, PERSON_ROLE, URL);


                            }
                            else if (api.Contains("Fenceinterface/FenceAlarmInfo1"))
                            {
                                // 防护栏信息接口
                                realapi = "insertInforOfFhl";
                                string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                string DEVICEID = jso.GetValue("warnNumber").ToString();
                                string STATUS = "1";
                                string DEFECTWARNNUMBER = jso.GetValue("defectWarnNumber").ToString();
                                string DEFECTPOSITION = jso.GetValue("defectPosition").ToString();
                                string DEFECTDATE = jso.GetValue("defectDate").ToString();
                                string WARN_TYPE = "1";
                                string PERSON_ID = "";
                                dataxml = string.Format(XML28, GONGCHENG_CODE, DEVICEID, STATUS, DEFECTWARNNUMBER, DEFECTPOSITION, DEFECTDATE, WARN_TYPE, PERSON_ID);

                            }
                            else if (api.Contains("Fenceinterface/FenceAlarmInfo2"))
                            {
                                realapi = "insertInforOfFhl";
                                string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                string DEVICEID = jso.GetValue("warnNumber").ToString();
                                string STATUS = "2";
                                string DEFECTWARNNUMBER = jso.GetValue("defectWarnNumber").ToString();
                                string DEFECTPOSITION = jso.GetValue("DefectPosition").ToString();
                                string DEFECTDATE = jso.GetValue("recoveryDate").ToString();
                                string WARN_TYPE = "";
                                string PERSON_ID = "";
                                dataxml = string.Format(XML28, GONGCHENG_CODE, DEVICEID, STATUS, DEFECTWARNNUMBER, DEFECTPOSITION, DEFECTDATE, WARN_TYPE, PERSON_ID);
                            }
                            else if (api.Contains("Check/InspectionPointContent"))
                            {
                                realapi = "insertInforOfYdxj";
                                string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                string INSPECTIONID = jso.GetValue("inspectionId").ToString();
                                string SITE = "";
                                string BUILDING = "";
                                string FLOOR = "";
                                string INSPECTIONCONTENTID = jso.GetValue("inspectionContentId").ToString();
                                string CHECKPERSON = jso.GetValue("checkPerson").ToString();
                                string CHECKPERSONID = "";
                                string CHECKCONTENT = jso.GetValue("checkContent").ToString();
                                string INSPECTIONTIME = jso.GetValue("inspectionTime").ToString();
                                string URLS = jso.GetValue("urls").ToString();

                                dataxml = string.Format(XML29, GONGCHENG_CODE, INSPECTIONID, SITE, BUILDING, FLOOR, INSPECTIONCONTENTID, CHECKPERSON, CHECKPERSONID, CHECKCONTENT, INSPECTIONTIME, URLS);

                            }
                            else if (api.Contains("StereotacticBoard"))
                            {
                                //人员定位页面
                                realapi = "insertInforOfRydw";
                                string GONGCHENG_CODE = jso.GetValue("GONGCHENG_CODE").ToString();
                                string POSITION_URL = HttpUtility.HtmlEncode(jso.GetValue("stereotacticBoardUrl").ToString());
                                dataxml = string.Format(XML27, GONGCHENG_CODE, POSITION_URL);
                            }
                            try
                            {
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
                            catch (HttpRequestException ex)
                            {
                                _logger.LogError(api + ":" + ex.Message);
                                return;
                            }
                            catch (Exception ex)
                            {

                               _logger.LogError(api + ":" + ex.Message, true);
                               // return ResponseOutput.NotOk(api + ":" + ex.Message);
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

        /// <summary>
        /// 2.5 深基坑信息XML
        /// </summary>
        private static string XML25 = "<XML><GONGCHENG_CODE>{0}</GONGCHENG_CODE><DEVICE_ID>{1}</DEVICE_ID><MONITORTYPE>{2}</MONITORTYPE><WARNVALUE>{3}</WARNVALUE><ALARMVALUE>{4}</ALARMVALUE><VALUE>{5}</VALUE><WARNEXPLAIN>{6}</WARNEXPLAIN><WARNCONTENT>{7}</WARNCONTENT><COLLECTIONTIME>{8}</COLLECTIONTIME></XML>";

        /// <summary>
        /// 2.6 高支模信息XML
        /// </summary>
        private static string XML26 = "<XML><GONGCHENG_CODE>{0}</GONGCHENG_CODE><DEVICE_ID>{1}</DEVICE_ID><POWER>{2}</POWER><TEMPERATURE>{3}</TEMPERATURE><LOAD>{4}</LOAD><HORIZONTALANGLE>{5}</HORIZONTALANGLE><COORDINATE>{6}</COORDINATE><TRANSLATION>{7}</TRANSLATION><SETTLEMENT>{8}</SETTLEMENT><WARNEXPLAIN>{9}</WARNEXPLAIN><WARNCONTENT>{10}</WARNCONTENT><COLLECTIONTIME>{11}</COLLECTIONTIME></XML>";

        /// <summary>
        /// 2.7 人员定位页面XML
        /// </summary>
        private static string XML27 = "<XML><GONGCHENG_CODE>{0}</GONGCHENG_CODE><POSITION_URL>{1}</POSITION_URL></XML>";

        /// <summary>
        /// 2.8 防护栏信息XML
        /// </summary>
        private static string XML28 = "<XML><GONGCHENG_CODE>{0}</GONGCHENG_CODE><DEVICEID>{1}</DEVICEID><STATUS>{2}</STATUS><DEFECTWARNNUMBER>{3}</DEFECTWARNNUMBER><DEFECTPOSITION>{4}</DEFECTPOSITION><DEFECTDATE>{5}</DEFECTDATE><WARN_TYPE>{6}</WARN_TYPE><PERSON_ID>{7}</PERSON_ID></XML>";

        /// <summary>
        /// 2.9 移动巡检信息XML
        /// </summary>
        private static string XML29 = "<XML><GONGCHENG_CODE>{0}</GONGCHENG_CODE><INSPECTIONID>{1}</INSPECTIONID><SITE>{2}</SITE><BUILDING>{3}</BUILDING><FLOOR>{4}</FLOOR><INSPECTIONCONTENTID>{5}</INSPECTIONCONTENTID><CHECKPERSON>{6}</CHECKPERSON><CHECKPERSONID>{7}</CHECKPERSONID><CHECKCONTENT>{8}</CHECKCONTENT><INSPECTIONTIME>{9}</INSPECTIONTIME><URLS>{10}</URLS></XML>";

        /// <summary>
        /// 2.10 随手拍信息XML
        /// </summary>
        private static string XML210 = "<XML><INSTASHOT><GONGCHENG_CODE>{0}</GONGCHENG_CODE><INSTASHOT_ID>{1}</INSTASHOT_ID><INSTASHOT_DATE>{2}</INSTASHOT_DATE><PERSON>{3}</PERSON><PERSON_ROLE>{4}</PERSON_ROLE><POSITION>{5}</POSITION><NEIRONG>{6}</NEIRONG></INSTASHOT></XML>";

        /// <summary>
        /// 2.10.2 随手拍明细信息XML
        /// </summary>
        private static string XML211 = "<XML><INSTASHOT_DETAIL><GONGCHENG_CODE>{0}</GONGCHENG_CODE><INSTASHOT_ID>{1}</INSTASHOT_ID><DETAIL_STATUS>{2}</DETAIL_STATUS><DETAIL_NEIRONG>{3}</DETAIL_NEIRONG><DETAIL_DATE>{4}</DETAIL_DATE><PERSON>{5}</PERSON><PERSON_ROLE>{6}</PERSON_ROLE><DETAIL_URLS>{7}</DETAIL_URLS></INSTASHOT_DETAIL></XML>";

    }
}
