using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using XHS.Build.Services.Cable;
using XHS.Build.Services.Employee;
using XHS.Build.Services.EmployeeSite;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.DailyJob
{
    public class DailyJobService : BaseServices<BaseEntity>, IDailyJobService
    {
        private readonly IBaseRepository<BaseEntity> _baseRepository;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly ILogger<DailyJobService> _logger;
        private readonly IEmployeeSiteService _employeeSiteService;
        private readonly IEmployeeService _employeeService;
        private readonly ICableService _cableService;
        public DailyJobService(IBaseRepository<BaseEntity> baseRepository, IHpSystemSetting hpSystemSetting, ILogger<DailyJobService> logger, IEmployeeSiteService employeeSiteService, IEmployeeService employeeService, ICableService cableService)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
            _hpSystemSetting = hpSystemSetting;
            _logger = logger;
            _employeeSiteService = employeeSiteService;
            _employeeService = employeeService;
            _cableService = cableService;
        }

        public void CableDoSync()
        {
            //每天8点,钢丝绳损伤报警
            //if (DateTime.Now.Hour.Equals(8))
            //{
            try
            {
                DataSet ds = _cableService.GetList();
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string url = _hpSystemSetting.getSettingValue(Const.Setting.S121);
                    Dictionary<string, object> dicForm = new Dictionary<string, object>();
                    dicForm["grant_type"] = "password";
                    dicForm["username"] = _hpSystemSetting.getSettingValue(Const.Setting.S122);
                    dicForm["password"] = _hpSystemSetting.getSettingValue(Const.Setting.S123);
                    string form = JsonTransfer.dic2Form(dicForm);// JsonConvert.SerializeObject(dicForm);// JsonTransfer.dic2Form(dicForm);
                    string respToken = UHttp.Post(url + "token", form, UHttp.CONTENT_TYPE_FORM);
                    if (string.IsNullOrEmpty(respToken))
                    {
                        return;
                    }
                    JObject jToken = JObject.Parse(respToken);
                    string token = Convert.ToString(jToken["access_token"]);

                    WebHeaderCollection whc = new WebHeaderCollection();
                    whc.Add("Authorization", "bearer " + token);
                    Dictionary<string, object> dicParam = new Dictionary<string, object>();

                    Dictionary<string, DataRow> dicSensor = new Dictionary<string, DataRow>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dicSensor.Add(Convert.ToString(dt.Rows[i]["sensorid"]), dt.Rows[i]);
                    }
                    dicParam.Add("SensorId", dicSensor.Keys);
                    dicParam.Add("From", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00"));
                    dicParam.Add("To", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    string jParam = JsonConvert.SerializeObject(dicParam);

                    string response = UHttp.Post(url + "Alert", jParam, UHttp.CONTENT_TYPE_JSON, whc);
                    _logger.LogInformation(response);
                    List<Dictionary<string, string>> listSensor = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response);
                    int warnCount = 0;
                    foreach (Dictionary<string, string> item in listSensor)
                    {
                        if (string.IsNullOrEmpty(item["Type"]))
                            continue;
                        int warnid = _cableService.DoRiskLevelCheck(item["id"], Convert.ToInt32(item["Type"]));
                        if (warnid > 0)
                        {
                            _logger.LogInformation(string.Format("生成钢丝绳损伤报警: {0} .", warnid));
                            warnCount++;
                        }
                    }
                    _logger.LogInformation(string.Format("共生成钢丝绳损伤报警: {0} 件.", warnCount));

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message, true);
            }
            //}
        }

        /// <summary>
        /// 执行数据库备份
        /// </summary>
        /// <param name="fullPath">备份文件路径</param>
        /// <param name="dbName">数据库名</param>
        /// <param name="timeout">超时时间</param>
        public void Excute(string fullPath, string dbName, int timeout)
        {
            //目录不存在，则创建
            if (!UFile.IsExistDirectory(fullPath))
                UFile.CreateDirectory(fullPath);

            string fileName = Path.Combine(fullPath, DateTime.Now.ToString("yyyyMMdd") + Const.FileEx.BAK);
            //文件已存在，则返回
            if (UFile.IsExistFile(fileName))
                return;

            string sqlCommand = string.Format("BACKUP DATABASE {0} TO DISK='{1}'", dbName, fileName);
            _baseRepository.Db.Ado.ExecuteCommand(sqlCommand);
        }

        public void SparkcnDoSync()
        {
            string SQL = @"SELECT [SITEID]
                                          ,[GROUPID]
                                          ,[sitename]
                                          ,[siteshortname]
                                          ,[attendprojid]
                                          ,[attendprojtype]
                                          ,[attendprojidwuxi]
                                          ,ISNULL((SELECT DATEADD(HOUR,-1,MAX(updatedate)) FROM T_GC_EmployeeSite es WHERE s.SITEID = es.SITEID),'1990-01-01') updatedate
                                      FROM [T_GC_Site] s 
                                      WHERE status=0 AND [attendprojtype]=1 AND LEN([attendprojid])>0;
                                        SELECT DISTINCT [ID] FROM [T_GC_Employee]";
            /// <summary>
            /// 4.6 获取项目用工信息
            /// </summary>
            string XML46 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
              "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
              "<soap:Body><GetWorkerHmc xmlns=\"http://tempuri.org/\"><Citycode>{0}</Citycode><PrjNum>{1}</PrjNum><bTime>{2}</bTime><pass>{3}</pass>" +
              "</GetWorkerHmc></soap:Body></soap:Envelope>";

            /// <summary>
            /// 4.7 获取人员基本信息
            /// </summary>
            string XML47 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
               "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
               "<soap:Body><GetWorkers xmlns=\"http://tempuri.org/\"><Citycode>{0}</Citycode><PrjNum>{1}</PrjNum><Zjhm>{2}</Zjhm><pass>{3}</pass>" +
               "</GetWorkers></soap:Body></soap:Envelope>";
            List<string> _IDList = null;//身份证列表
            string POST_URL = _hpSystemSetting.getSettingValue(Const.Setting.S141);
            string PASS = _hpSystemSetting.getSettingValue(Const.Setting.S142);
            string CITYCODE = _hpSystemSetting.getSettingValue(Const.Setting.S143);
            if (string.IsNullOrEmpty(POST_URL) || string.IsNullOrEmpty(PASS) || string.IsNullOrEmpty(CITYCODE))
                return;

            // Connection connection = new Connection(System.Data.CommandType.Text);
            try
            {
                DataSet ds = _baseRepository.Db.Ado.GetDataSetAll(SQL);
                DataTable dtSite = ds.Tables[0];
                if (_IDList == null)
                {//刚启动没有身份证列表就初始化
                    DataTable dtID = ds.Tables[1];
                    _IDList = new List<string>();
                    for (int i = 0; i < dtID.Rows.Count; i++)
                    {
                        _IDList.Add(Convert.ToString(dtID.Rows[0]));
                    }
                }
                int count = 0;
                for (int i = 0; i < dtSite.Rows.Count; i++)
                {
                    //先拿项目下所有用工信息
                    string xml = string.Format(XML46, CITYCODE, dtSite.Rows[i]["attendprojid"], dtSite.Rows[i]["updatedate"], PASS);
                    string response = UHttp.Post(POST_URL, xml, UHttp.CONTENT_TYPE_TEXT_XML);
                    JObject jo = JObject.Parse(response);
                    if (Convert.ToInt32(jo["ResultState"]) != 1)
                    {
                        //失败了打印参数和错误
                        _logger.LogInformation(string.Format("{0} {1} {2} {3}", CITYCODE, dtSite.Rows[i]["attendprojid"],
                                dtSite.Rows[i]["updatedate"], PASS));
                        _logger.LogInformation(response, true);
                        continue;
                    }
                    JArray ja = JArray.FromObject(jo["data"]);
                    for (int j = 0; j < ja.Count; j++)
                    {
                        string ID = Convert.ToString(ja[j]["Zjhm"]);
                        GCEmployeeSiteEntity entity = new GCEmployeeSiteEntity();
                        entity.attendprojid = dtSite.Rows[i]["attendprojid"].ToString();
                        entity.socialcreditcode = dtSite.Rows[i]["Qyzzjgdm"].ToString();
                        entity.shiftname = dtSite.Rows[i]["Qyzzjgdm"].ToString();
                        entity.workertype = dtSite.Rows[i]["WorkerType"].ToString();
                        entity.position = dtSite.Rows[i]["Gwlb"].ToString();
                        entity.jobtype = dtSite.Rows[i]["TPType"].ToString();
                        entity.jobname = dtSite.Rows[i]["TPCodeName"].ToString();
                        entity.startdate = dtSite.Rows[i]["BeginDate"].ToDateTime();
                        entity.enddate = dtSite.Rows[i]["EndDate"].ToDateTime();
                        //更新员工监测点数据
                        //count += _employeeSiteService.doSiteRegist(new SugarParameter("@ID", ID),
                        //    new SugarParameter("@attendprojid", dtSite.Rows[i]["attendprojid"]),
                        //    //new SugarParameter("@SITEID", dtSite.Rows[i]["SITEID"]),
                        //    //new SugarParameter("@GROUPID", dtSite.Rows[i]["GROUPID"]),
                        //    new SugarParameter("@socialcreditcode", ja[j]["Qyzzjgdm"]),
                        //    new SugarParameter("@shiftname", ja[j]["Bzmc"]),
                        //    new SugarParameter("@workertype", ja[j]["WorkerType"]),
                        //    new SugarParameter("@position", ja[j]["Gwlb"]),
                        //    new SugarParameter("@jobtype", ja[j]["TPType"]),
                        //    new SugarParameter("@jobname", ja[j]["TPCodeName"]),
                        //    new SugarParameter("@startdate", ja[j]["BeginDate"]),
                        //    new SugarParameter("@enddate", ja[j]["EndDate"]));
                        count += _employeeSiteService.doSiteRegist(entity);
                        if (_IDList.Contains(ID))
                            continue;//如果已经有基本信息就跳过

                        //获取个人基本信息
                        xml = string.Format(XML47, CITYCODE, dtSite.Rows[i]["attendprojid"],
                                ID, PASS);
                        response = UHttp.Post(POST_URL, xml, UHttp.CONTENT_TYPE_TEXT_XML);
                        jo = JObject.Parse(response);
                        if (Convert.ToInt32(jo["ResultState"]) != 1)
                        {
                            //失败了打印参数和错误
                            _logger.LogInformation(string.Format("{0} {1} {2} {3}", CITYCODE,
                                dtSite.Rows[i]["attendprojid"], ID, PASS));
                            _logger.LogInformation(response, true);
                            continue;
                        }

                        JObject jaOne = JObject.FromObject(jo["data"][0]);
                        //注册个人基本信息
                        int ret = _employeeService.doRegist(
                            new SugarParameter("@ID", ID),
                            new SugarParameter("@GROUPID", dtSite.Rows[i]["GROUPID"]),
                            new SugarParameter("@realname", jaOne["WorkerName"]),
                            new SugarParameter("@mobile", jaOne["Lxdh"]),
                            new SugarParameter("@sex", jaOne["Xb"]),
                            new SugarParameter("@birthday", jaOne["Csrq"]),
                            new SugarParameter("@address", jaOne["Zz"]),
                            new SugarParameter("@ethnic", jaOne["Mz"]),
                            new SugarParameter("@idstartdate", jaOne["Sfzyxqrq"]),
                            new SugarParameter("@idenddate", jaOne["Sfzyxzrq"]),
                            new SugarParameter("@publisher", jaOne["SfzFzjg"]),
                            new SugarParameter("@province", jaOne["Province"]),
                            new SugarParameter("@city", jaOne["City"]),
                            new SugarParameter("@county", jaOne["County"]),
                            new SugarParameter("@image", Convert.ToString(jaOne["Image"])),
                            new SugarParameter("@jsonall", jaOne.Remove("Image") ? JsonConvert.SerializeObject(jaOne) : JsonConvert.SerializeObject(jaOne)),
                            new SugarParameter("operator", "sync"));
                        _IDList.Add(ID);//列表里加进去
                    }
                }
                _logger.LogInformation(string.Format("完成同步 {0} 件.", count));
            }
            catch (Exception ex)
            {
                string message = ex.Message + Environment.NewLine + ex.StackTrace;
                _logger.LogInformation(message, true);
            }
            finally
            {

            }
        }
    }
}
