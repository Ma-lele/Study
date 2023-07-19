using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class DayunDayJob : JobBase, IJob
    {
        private readonly ILogger<DayunDayJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly CityToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly DayunToken _dayunToken;
        private readonly ISiteService _siteService;

        public DayunDayJob(ILogger<DayunDayJob> logger, IOperateLogService operateLogService, CityToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService, DayunToken dayunToken, ISiteService siteService)
        {
            _logger = logger;  
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
            _dayunToken = dayunToken;
            _siteService = siteService;

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

            string uploadapi = string.Empty;
            string data = string.Empty;
            string url = "http://zh1.wxzjj.com:5678";
            JObject job = new JObject();
            JArray jar = new JArray();
            JArray jarCode = new JArray();
            JObject jObject = new JObject();
            List<SugarParameter> param = new List<SugarParameter>();


            //根据视频ID获取rstp流
            uploadapi = "rest/ProjectInfo/getVideoUrl";
            jObject = new JObject();
            jObject.Add("id", "fd732026878245b588f727cc9db9b07f");
            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            jar = JArray.Parse(data);


            //获取工地扬尘数据
            #region
            uploadapi = "rest/ProjectInfo/getDustNoise";
            jObject = new JObject();
            jar = new JArray();
            jObject.Add("contract_record_code", "3202052019022801A01000");
            jObject.Add("st", "2021-06-20 13:00:00");
            jObject.Add("et", "2021-06-21 13:00:00");
            jObject.Add("page", 1);
            jObject.Add("size", 100);
            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            job = JObject.Parse(data);
            jar = JArray.Parse(job.GetValue("rows").ToString());
            //foreach (JObject item in jar)
            //{
            //    param = new List<SugarParameter>();
            //    param.Add(new SugarParameter("@" + "contract_record_code", code));
            //    foreach (JProperty jProperty in item.Properties())
            //    {
            //        param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
            //    }
            //}
            #endregion
            //获取获取工地卸料台数据
            #region
            uploadapi = "rest/ProjectInfo/getMaterial";
            jObject = new JObject();
            jObject.Add("contract_record_code", "3202052019110101A01000");
            jObject.Add("st", "2021-06-10 13:00:00");
            jObject.Add("et", "2021-06-11 13:00:00");
            jObject.Add("page", 1);
            jObject.Add("size", 100);
            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            job = JObject.Parse(data);
            jar = JArray.Parse(job.GetValue("rows").ToString());
            //foreach (JObject item in jar)
            //{
            //    param = new List<SugarParameter>();
            //    param.Add(new SugarParameter("@" + "contract_record_code", code));
            //    foreach (JProperty jProperty in item.Properties())
            //    {
            //        param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
            //    }
            //}
            #endregion



            //获取获取工地升降机数据
            #region
            uploadapi = "rest/ProjectInfo/getElevator";
            jObject = new JObject();
            jar = new JArray();
            jObject.Add("contract_record_code", "3202052019110101A01000");
            jObject.Add("st", "2021-06-10 13:00:00");
            jObject.Add("et", "2021-06-11 13:00:00");
            jObject.Add("page", 1);
            jObject.Add("size", 100);
            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            job = JObject.Parse(data);
            jar = JArray.Parse(job.GetValue("rows").ToString());
            //foreach (JObject item in jar)
            //{
            //    param = new List<SugarParameter>();
            //    param.Add(new SugarParameter("@" + "contract_record_code", code));
            //    foreach (JProperty jProperty in item.Properties())
            //    {
            //        param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));

            //    }
            //}
            #endregion




            //获取获取工地塔吊数据
            #region
            uploadapi = "rest/ProjectInfo/getTowerData";
            jObject = new JObject();
            jar = new JArray();
            jObject.Add("contract_record_code", "3202052019110101A01000");
            jObject.Add("st", "2021-06-20 13:00:00");
            jObject.Add("et", "2021-06-21 13:00:00");
            jObject.Add("page", 1);
            jObject.Add("size", 100);
            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            job = JObject.Parse(data);
            jar = JArray.Parse(job.GetValue("rows").ToString());

            //foreach (JObject item in jar)
            //{
            //    param = new List<SugarParameter>();
            //    param.Add(new SugarParameter("@" + "contract_record_code", "3202812020011702A01000"));
            //    foreach (JProperty jProperty in item.Properties())
            //    {
            //        param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
            //    }
            //    await _siteService.doDySiteLiftData(param);
            //}

            #endregion


            DataTable dt = await _siteService.GetDySiteList();
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
                        jarCode.Add(jso);

                    }
                }
            }

            //获取拥有权限的合同备案清单列表
            #region
            //jObject = new JObject();
            //uploadapi = "rest/ProjectInfo/getProjectList";



            //获取拥有权限的合同备案清单列表
            //data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            //jar = JArray.Parse(data);
            #endregion
            foreach (JObject jobCode in jarCode)
            {
                string code = jobCode.GetValue("sitecode").ToString();
                //获取工地基本信息
                #region
                uploadapi = "rest/ProjectInfo/getProject";
                jObject = new JObject();
                job = new JObject();
                jObject.Add("contract_record_code", code);
                data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                job = JObject.Parse(data);
                param = new List<SugarParameter>();
                param.Add(new SugarParameter("@" + "contract_record_code", code));
                foreach (JProperty jProperty in job.Properties())
                {
                    if (jProperty.Name == "longitude" || jProperty.Name == "latitude")
                    {
                        jProperty.Value = Convert.ToSingle(jProperty.Value.ToString());
                    }
                    param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                }
                //插入数据库
                var result = await _siteService.doDySiteUpdate(param);
                #endregion












                //根据视频ID获取rstp流
                #region
                uploadapi = "rest/ProjectInfo/getVideoUrl";
                jObject = new JObject();
                jObject.Add("id", "fd732026878245b588f727cc9db9b07f");
                data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                jar = JArray.Parse(data);

                foreach (JObject item in jar)
                {
                    param = new List<SugarParameter>();
                    param.Add(new SugarParameter("@" + "contract_record_code", code));
                    foreach (JProperty jProperty in item.Properties())
                    {
                        param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                    }
                }
                #endregion


                //视频列表
                #region
               
                param = new List<SugarParameter>();
                string ids = "";
                string deviceNames = "";
                string statuss = "";
                string datetimes = "";
                int i = 0;
                foreach (JObject item in jar)
                {
                    if (i == 0)
                    {
                        ids = item.GetValue("id").ToString();
                        deviceNames = item.GetValue("deviceName").ToString();
                        statuss = item.GetValue("status").ToString();
                        datetimes = item.GetValue("datetime").ToString();
                    }
                    else
                    {
                        ids = ids + "," + item.GetValue("id").ToString();
                        deviceNames = deviceNames + "," + item.GetValue("deviceName").ToString();
                        statuss = statuss + "," + item.GetValue("status").ToString();
                        datetimes = datetimes + "," + item.GetValue("datetime").ToString();
                    }
                    i++;

                }
                param.Add(new SugarParameter("@" + "GROUPID", 17));
                param.Add(new SugarParameter("@" + "SITEID", 108));
                param.Add(new SugarParameter("@ids", ids));
                param.Add(new SugarParameter("@deviceNames", deviceNames));
                param.Add(new SugarParameter("@statuss", statuss));
                param.Add(new SugarParameter("@datetimes", datetimes));
               // int result = await _siteService.doDyCameraInsert(param);
                #endregion
            }


            //获取拥有权限的合同备案清单列表
            jObject = new JObject();
            jar = new JArray();
            uploadapi = "rest/ProjectInfo/getProjectList";
            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            jar = JArray.Parse(data);


            //获取工地基本信息
            uploadapi = "rest/ProjectInfo/getProject";
            jObject = new JObject();
            job = new JObject();
            jObject.Add("contract_record_code", "3202012019110401A02000");
            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
            job = JObject.Parse(data);


            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
