using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadGuangliandaJob : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadGuangliandaJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        public SmartUpLoadGuangliandaJob(ILogger<SmartUpLoadGuangliandaJob> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService)
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
            string result = "";
            string Token = "";
            string url = "http://zl.glodon.com:8900/api/v1/events";
            string ajcode = "[{\"siteajcode\":\"AJ320707120210146\",\"UploadDustInfo\":\"eyJhbGciOiJIUzI1NiIsImlhdCI6MTYzMjA0NzM2NSwiZXhwIjoxOTQ3NDA3MzY1LjB9.eyJ2IjoiMSIsImkiOiI3NjE3Y2QwYS0xOTM0LTExZWMtYjU1OC0wMjQyYWMxMjAwMDUiLCJ0IjoiR2xvZG9uIiwiaiI6InN0ZDEiLCJzIjoxNjMxOTgwODAwLjAsImsiOiJ7XCJtXCI6IFwiRW52aXJvbm1lbnRcIiwgXCJvXCI6IFwiOTEzMjA1MDUzMjEyNDI2NzZXXCJ9In0.wNY1x01oEGxdiNLFJMPskFTMeR-uwQktnmr760ojEtE\",\"FenceAlarmInfo\":\"eyJhbGciOiJIUzI1NiIsImlhdCI6MTYzMjI5MTA4OCwiZXhwIjoxOTQ3NjUxMDg4LjB9.eyJ2IjoiMSIsImkiOiJlY2FmODllMi0xYjZiLTExZWMtOTMzMS0wMjQyYWMxMjAwMDUiLCJ0IjoiR2xvZG9uIiwiaiI6InN0ZDEiLCJzIjoxNjMyMjQwMDAwLjAsImsiOiJ7XCJtXCI6IFwiR3VhcmRGZW5jZVwiLCBcIm9cIjogXCI5MTMyMDUwNTMyMTI0MjY3NldcIn0ifQ.OuybudC67FtWvF7fcIBVqaHAgYrq-3c6jLfOJLkGk4U\",\"GuangliandaCraneOn\":\"eyJhbGciOiJIUzI1NiIsImlhdCI6MTYzMjA0NzMzOSwiZXhwIjoxOTQ3NDA3MzM5LjB9.eyJ2IjoiMSIsImkiOiI2Njg0ZWY4MC0xOTM0LTExZWMtYjBjMC0wMjQyYWMxMjAwMDUiLCJ0IjoiR2xvZG9uIiwiaiI6InN0ZDEiLCJzIjoxNjMxOTgwODAwLjAsImsiOiJ7XCJtXCI6IFwiQ3JhbmVUb3dlclwiLCBcIm9cIjogXCI5MTMyMDUwNTMyMTI0MjY3NldcIn0ifQ.d-4iFAoq1vK1JNBdamNtRn02vXGMbadL4yf4mbd3R2Y\"},{\"siteajcode\":\"AJ320707120210110\",\"UploadDustInfo\":\"eyJhbGciOiJIUzI1NiIsImlhdCI6MTYzMjI3ODQ3NiwiZXhwIjoxOTQ3NjM4NDc2LjB9.eyJ2IjoiMSIsImkiOiI4ZWU0MTk2Yy0xYjRlLTExZWMtOGQyMi0wMjQyYWMxMjAwMDUiLCJ0IjoiR2xvZG9uIiwiaiI6InN0ZDEiLCJzIjoxNjMyMjQwMDAwLjAsImsiOiJ7XCJtXCI6IFwiRW52aXJvbm1lbnRcIiwgXCJvXCI6IFwiOTEzMjA1MDUzMjEyNDI2NzZXXCJ9In0.XFhDSYPWSHECFzyi9fyQ9sz215UpexNTGmEPAToWPP8\",\"FenceAlarmInfo\":\"eyJhbGciOiJIUzI1NiIsImlhdCI6MTYzMjI5MTIxMCwiZXhwIjoxOTQ3NjUxMjEwLjB9.eyJ2IjoiMSIsImkiOiIzNTM5OWVjOC0xYjZjLTExZWMtYmE4ZS0wMjQyYWMxMjAwMDUiLCJ0IjoiR2xvZG9uIiwiaiI6InN0ZDEiLCJzIjoxNjMyMjQwMDAwLjAsImsiOiJ7XCJtXCI6IFwiR3VhcmRGZW5jZVwiLCBcIm9cIjogXCI5MTMyMDUwNTMyMTI0MjY3NldcIn0ifQ.cKi7hBffT4oSawNH_qRaPo0KujdqKQxoBUeARJ4ds0I\",\"GuangliandaCraneOn\":\"eyJhbGciOiJIUzI1NiIsImlhdCI6MTYzMjI3ODQ4OSwiZXhwIjoxOTQ3NjM4NDg5LjB9.eyJ2IjoiMSIsImkiOiI5NmM0YmQ1OC0xYjRlLTExZWMtYjQ1MC0wMjQyYWMxMjAwMDUiLCJ0IjoiR2xvZG9uIiwiaiI6InN0ZDEiLCJzIjoxNjMyMjQwMDAwLjAsImsiOiJ7XCJtXCI6IFwiQ3JhbmVUb3dlclwiLCBcIm9cIjogXCI5MTMyMDUwNTMyMTI0MjY3NldcIn0ifQ.rqkZMRpXYwcX2Emp-WS7uCba7gVjDtcb8uuRz6MjqoY\"}]";
            JArray ajcodejar = JArray.Parse(ajcode);

            DataSet ds = await _aqtUploadService.GetListForGuanglianda();
            if (ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    if (dt.Rows.Count <= 0)
                    {
                        continue;
                    }
                    foreach (JObject codejob in ajcodejar)
                    {
                        string codestr = codejob.GetValue("siteajcode").ToString();
                        var dtdata = dt.Select("('AJ320707120210110' = recordNumber and post<> '/GuangliandaCraneOn' and post <> '/CraneWorkData') or('AJ320707120210110' <> recordNumber and recordNumber='" + codestr + "')");
                        if (dtdata.Length > 0)
                        {
                            for (int j = 0; j < dtdata.Length; j++)
                            {
                                JArray evejar = new JArray();
                                JObject evejob = new JObject();
                                JObject evejob1 = new JObject();
                                try
                                {
                                    DataRow dr = dtdata[j];
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
                                    
                                    string account = jso.GetValue("uploadaccount").ToString();
                                    string pwd = jso.GetValue("uploadpwd").ToString();
                                    string api = jso.GetValue("post").ToString();

                                    if (api.Contains("UploadDustInfo"))          //广联达环境监测数据推送
                                    {
                                        Token = codejob.GetValue("UploadDustInfo").ToString();
                                        jso.Add("pm25", jso.GetValue("pm2dot5").ToDouble() * 1000);
                                        jso["pm10"] = jso.GetValue("pm10").ToDouble() * 1000;
                                        jso["noise"] = jso.GetValue("noise").ToDouble();
                                        jso["windSpeed"] = jso.GetValue("windSpeed").ToDouble();
                                        jso.Add("temp", jso.GetValue("temperature").ToDouble());
                                        jso["humidity"] = jso.GetValue("humidity").ToDouble();
                                        jso["windDirection"] = jso.GetValue("windDirectionvalue").ToDouble();
                                        if (!string.IsNullOrEmpty(jso.GetValue("windDirection").ToString()))
                                        {
                                            if (jso.GetValue("windDirection").ToString() == "东北偏北风")
                                            {
                                                jso.Add("windDirection2", 1);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "东北风")
                                            {
                                                jso.Add("windDirection2", 2);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "东北偏东风")
                                            {
                                                jso.Add("windDirection2", 3);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "东风")
                                            {
                                                jso.Add("windDirection2", 4);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "东南偏东风")
                                            {
                                                jso.Add("windDirection2", 5);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "东南风")
                                            {
                                                jso.Add("windDirection2", 6);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "东南偏南风")
                                            {
                                                jso.Add("windDirection2", 7);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "南风")
                                            {
                                                jso.Add("windDirection2", 8);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "西南偏南风")
                                            {
                                                jso.Add("windDirection2", 9);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "西南风")
                                            {
                                                jso.Add("windDirection2", 10);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "西南偏西风")
                                            {
                                                jso.Add("windDirection2", 11);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "西风")
                                            {
                                                jso.Add("windDirection2", 12);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "西北偏西风")
                                            {
                                                jso.Add("windDirection2", 13);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "西北风")
                                            {
                                                jso.Add("windDirection2", 14);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "西北偏北")
                                            {
                                                jso.Add("windDirection2", 15);
                                            }
                                            else if (jso.GetValue("windDirection").ToString() == "北风")
                                            {
                                                jso.Add("windDirection2", 16);
                                            }
                                        }
                                        jso.Add("tsp", jso.GetValue("rtTSP").ToDouble() * 1000);
                                        evejob.Add("uuid", jso.GetValue("deviceId").ToString());
                                        evejob.Add("time", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                                        evejob.Add("interval", 60);
                                        jso.Remove("recordNumber");
                                        jso.Remove("belongedTo");
                                        jso.Remove("deviceId");
                                        jso.Remove("temperature");
                                        jso.Remove("pm2dot5");
                                        jso.Remove("upload");
                                        jso.Remove("windDirectionvalue");
                                        jso.Remove("rtTSP");
                                        evejob.Add("current", jso);
                                        evejob1.Add("nickname", "扬尘设备");
                                        evejob.Add("metadata", evejob1);
                                        evejar.Add(evejob);
                                    }
                                    else if (api.Contains("FenceAlarmInfo1"))
                                    {
                                        Token = codejob.GetValue("FenceAlarmInfo").ToString();
                                        JObject portStatus = new JObject();
                                        Random rd = new Random();
                                        if (rd.Next() % 2 == 0)
                                        {
                                            portStatus.Add("port1", 2);
                                            portStatus.Add("port2", 1);
                                        }
                                        else
                                        {
                                            portStatus.Add("port1", 1);
                                            portStatus.Add("port2", 2);
                                        }
                                        evejob1.Add("portStatus", portStatus);
                                        evejob1.Add("proximity", 1);
                                        evejob1.Add("antiCross", 1);
                                        evejob.Add("uuid", jso.GetValue("warnNumber").ToString());
                                        evejob.Add("time", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                                        evejob.Add("interval", 3600);
                                        evejob.Add("current", evejob1);
                                        evejob1 = new JObject();
                                        evejob1.Add("org_name", "新合盛");
                                        evejob1.Add("nickname", jso.GetValue("defectPosition").ToString());
                                        evejob1.Add("type", "临边设备");
                                        evejob.Add("metadata", evejob1);
                                        evejar.Add(evejob);
                                    }
                                    else if (api.Contains("FenceAlarmInfo2"))
                                    {
                                        Token = codejob.GetValue("FenceAlarmInfo").ToString();
                                        evejob.Add("uuid", jso.GetValue("warnNumber").ToString());
                                        evejob.Add("time", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                                        evejob.Add("interval", 3600);
                                        JObject portStatus = new JObject();
                                        portStatus.Add("port1", 1);
                                        portStatus.Add("port2", 1);
                                        evejob1.Add("portStatus", portStatus);
                                        evejob1.Add("proximity", 1);
                                        evejob1.Add("antiCross", 1);
                                        evejob.Add("current", evejob1);
                                        evejob1 = new JObject();
                                        evejob1.Add("org_name", "新合盛");
                                        evejob1.Add("nickname", jso.GetValue("DefectPosition").ToString());
                                        evejob1.Add("type", "临边设备");
                                        evejob.Add("metadata", evejob1);
                                        evejar.Add(evejob);
                                    }
                                    else if (api.Contains("GuangliandaCraneOn"))
                                    {
                                        Token = codejob.GetValue("GuangliandaCraneOn").ToString();
                                        evejob.Add("uuid", jso.GetValue("deviceId").ToString());
                                        evejob.Add("time", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                                        evejob.Add("interval", 60);



                                        //evejob1.Add("alarmID", jso.GetValue("WARNID").ToString());
                                        string sedatastr = jso.GetValue("sedata").ToString();
                                        if (!string.IsNullOrEmpty(sedatastr))
                                        {
                                            JObject sedatajob = JObject.Parse(sedatastr);


                                            evejob1.Add("alarmFbnZone", 0);
                                            evejob1.Add("alarmHwFault", 0);
                                            evejob1.Add("alarmIncline", 0);
                                            evejob1.Add("alarmLimit", 0);
                                            evejob1.Add("alarmHookLimit", 0);
                                            evejob1.Add("alarmTroLimit", 0);
                                            evejob1.Add("alarmRotaLimit", 0);
                                            evejob1.Add("alarmLoad", 0);
                                            evejob1.Add("alarmObCollision", 0);
                                            evejob1.Add("alarmTorque", 0);
                                            evejob1.Add("alarmTrCollision", 0);
                                            evejob1.Add("alarmWindspeed", 0);
                                            evejob1.Add("isAlarm", false);

                                            string typestr = "";
                                            int Alarm = (sedatajob.GetValue("Alarm").ToInt());
                                            while (Alarm > 0)
                                            {
                                                int type = (int)Math.Floor(Math.Log(Alarm) / Math.Log(2));
                                                typestr += type.ToString() + ',';
                                                Alarm = (int)Math.Floor(Alarm - Math.Pow(2, type));
                                            }
                                            typestr = typestr.Trim(',');
                                            if (typestr.Contains('1') || typestr.Contains('0'))
                                            {
                                                evejob1["alarmHookLimit"] = 2;
                                                evejob1["isAlarm"] = true;
                                            }
                                            else if (typestr.Contains('2') || typestr.Contains('3'))
                                            {
                                                evejob1["alarmTroLimit"] = 2;
                                                evejob1["isAlarm"] = true;
                                            }
                                            else if (typestr.Contains('4') || typestr.Contains('5'))
                                            {
                                                evejob1["alarmRotaLimit"] = 2;
                                                evejob1["isAlarm"] = true;
                                            }
                                            else if (typestr.Contains('6'))
                                            {
                                                evejob1["alarmLoad"] = 2;
                                                evejob1["isAlarm"] = true;
                                            }
                                            else if (typestr.Contains('7'))
                                            {
                                                evejob1["alarmTorque"] = 2;
                                                evejob1["isAlarm"] = true;
                                            }
                                            else if (typestr.Contains('8'))
                                            {
                                                evejob1["alarmWindspeed"] = 2;
                                                evejob1["isAlarm"] = true;
                                            }


                                            evejob1.Add("amplitude", sedatajob.GetValue("Margin").ToDouble());

                                            evejob1.Add("driverAuthStatus", "通过");
                                            evejob1.Add("rotation", sedatajob.GetValue("Rotation").ToDouble());
                                            evejob1.Add("safeLoad", sedatajob.GetValue("SafeLoad").ToDouble());
                                            evejob1.Add("time", Convert.ToDateTime(sedatajob.GetValue("UpdateTime")).ToString("yyyyMMddHHmmssfff"));

                                            evejob1.Add("torqueRatio", sedatajob.GetValue("MomentPercent").ToDouble());
                                            evejob1.Add("weight", sedatajob.GetValue("Weight").ToDouble());
                                            evejob1.Add("driverID", sedatajob.GetValue("DriverId").ToString());
                                            evejob1.Add("driverName", sedatajob.GetValue("DriverName").ToString());
                                            evejob1.Add("driverIDCard", sedatajob.GetValue("DriverCardNo").ToString());
                                        }
                                        evejob1.Add("driverAuthPhoto", jso.GetValue("boardbaseurl").ToString());

                                        if (Convert.ToDateTime(jso.GetValue("updatedate").ToString()) > Convert.ToDateTime("1970-01-01"))
                                        {
                                            evejob1.Add("driverAuthTime", Convert.ToDateTime(jso.GetValue("updatedate")).ToString("yyyyMMddHHmmssfff"));
                                        }
                                        evejob.Add("current", evejob1);
                                        evejob1 = new JObject();
                                        evejob1.Add("nickname", jso.GetValue("sename").ToString());
                                        evejob1.Add("deviceId", jso.GetValue("deviceId").ToString());
                                        evejob1.Add("model", jso.GetValue("machineryModel").ToString());
                                        evejob1.Add("recordNumber", jso.GetValue("propertyno").ToString());

                                        string paramjsonstr = jso.GetValue("paramjson").ToString();
                                        if (!string.IsNullOrEmpty(paramjsonstr))
                                        {
                                            JObject paramjsonobj = JObject.Parse(paramjsonstr);
                                            evejob1.Add("sHookLimit", paramjsonobj.GetValue("Tsgd").ToDouble());
                                            evejob1.Add("nTroLimit", paramjsonobj.GetValue("MinMargin").ToDouble());
                                            evejob1.Add("fTroLimit", paramjsonobj.GetValue("MaxMargin").ToDouble());
                                            evejob1.Add("sRotaLimit", paramjsonobj.GetValue("LeftRotation").ToDouble());
                                            evejob1.Add("eRotaLimit", paramjsonobj.GetValue("RightRotation").ToDouble());
                                            //evejob1.Add("wTrCollision", paramjsonobj.GetValue("").ToDouble());
                                            //evejob1.Add("aTrCollision", paramjsonobj.GetValue("").ToDouble());
                                            //evejob1.Add("wHeight", paramjsonobj.GetValue("").ToDouble());
                                            evejob1.Add("aHeight", paramjsonobj.GetValue("MaxHeight").ToDouble());
                                            //evejob1.Add("wWeight", paramjsonobj.GetValue("").ToDouble());
                                            evejob1.Add("aWeight", paramjsonobj.GetValue("MaxWeight").ToDouble());
                                            //evejob1.Add("wTorque", paramjsonobj.GetValue("").ToDouble());
                                            evejob1.Add("aTorque", paramjsonobj.GetValue("LiJvMaxMargin").ToDouble());
                                            //evejob1.Add("wWindspeed", paramjsonobj.GetValue("").ToDouble());
                                            //evejob1.Add("aWindspeed", paramjsonobj.GetValue("").ToDouble());
                                            //evejob1.Add("wAngle", paramjsonobj.GetValue("").ToDouble());
                                            //evejob1.Add("aAngle", paramjsonobj.GetValue("").ToDouble());
                                        }


                                        evejob.Add("metadata", evejob1);
                                        evejar.Add(evejob);

                                    }
                                    else if (api.Contains("CraneWorkData"))
                                    {
                                        Token = codejob.GetValue("GuangliandaCraneOn").ToString();
                                        evejob.Add("uuid", jso.GetValue("secode").ToString());
                                        evejob.Add("time", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                                        evejob.Add("interval", 3600);
                                        string workdatastr = jso.GetValue("workdata").ToString();
                                        if (!string.IsNullOrEmpty(workdatastr))
                                        {
                                            JObject workdajob = JObject.Parse(workdatastr);
                                            evejob1.Add("upTime", Convert.ToDateTime(jso.GetValue("updatedate")).ToString("yyyyMMddHHmmssfff"));
                                            evejob1.Add("upHeight", workdajob.GetValue("StartHeight").ToDouble());
                                            evejob1.Add("upAmplitude", workdajob.GetValue("StartMargin").ToDouble());
                                            evejob1.Add("upRotation", workdajob.GetValue("StartRotation").ToDouble());
                                            evejob1.Add("downTime", Convert.ToDateTime(workdajob.GetValue("EndTime")).ToString("yyyyMMddHHmmssfff"));
                                            evejob1.Add("downHeight", workdajob.GetValue("EndHeight").ToDouble());
                                            evejob1.Add("downAmplitude", workdajob.GetValue("EndMargin").ToDouble());
                                            evejob1.Add("downRotation", workdajob.GetValue("EndRotation").ToDouble());
                                            evejob1.Add("weight", workdajob.GetValue("MaxWeight").ToDouble());
                                            //evejob1.Add("maxLoadRatio", workdajob.GetValue("").ToString());
                                            //evejob1.Add("isViolation", workdajob.GetValue("").ToString());
                                            //evejob1.Add("violationType", workdajob.GetValue("").ToString());
                                            evejob1.Add("driverID", workdajob.GetValue("DriverCardNo").ToString());
                                            evejob1.Add("driverName", workdajob.GetValue("DriverName").ToString());
                                            evejob1.Add("driverIDCard", workdajob.GetValue("DriverCardNo").ToString());
                                            evejob1.Add("driverAuthStatus", "通过");

                                            
                                            if (Convert.ToDateTime(jso.GetValue("driverAuthTime").ToString()) > Convert.ToDateTime("1970-01-01"))
                                            {
                                                evejob1.Add("driverAuthTime", Convert.ToDateTime(jso.GetValue("driverAuthTime")).ToString("yyyyMMddHHmmssfff"));
                                            }
                                            evejob1.Add("driverAuthPhoto", jso.GetValue("boardbaseurl").ToString());
                                        }
                                        JObject job = new JObject();
                                        job.Add("record", evejob1);
                                        evejob.Add("current", job);
                                        evejar.Add(evejob);
                                    }

                                    jso.Remove("post");
                                    jso.Remove("siteuploadurl");
                                    jso.Remove("uploadaccount");
                                    jso.Remove("uploadpwd");
                                    if (jso.ContainsKey("funingurl"))
                                    {
                                        jso.Remove();
                                    }

                                    result = HttpNetRequest.POSTSendJsonRequest(url, JsonConvert.SerializeObject(evejar), new Dictionary<string, string>() { { "Authorization", "Bearer " + Token } });

                                    var LogEntity = new CityUploadOperateLog
                                    {
                                        //Id=Guid.NewGuid().ToString(),
                                        url = url,
                                        api = api,
                                        account = Token,
                                        param = evejar.ToString(),
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
                                        JObject mJObj = JObject.Parse(result);
                                        int code = (int)mJObj["returnCode"];
                                        if (code == 0)
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
                                    _logger.LogError(Token + ":" + ex.Message);
                                    return;
                                }
                                catch (Exception ex)
                                {

                                    _logger.LogError(Token + ":" + ex.Message);
                                }
                            }
                        }
                    }
                }
            }

            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
