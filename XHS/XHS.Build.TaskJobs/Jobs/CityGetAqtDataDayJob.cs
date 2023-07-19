using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Services.AqtUpload;
using XHS.Build.TaskJobs.Jobs;
using Newtonsoft.Json;
namespace XHS.Build.TaskJobs
{
    /// <summary>
    /// 省对接接口数据获取（3号文档）
    /// </summary>
    /// <returns></returns>
    public class CityGetAqtDataDayJob : JobBase, IJob
    {
        private readonly ILogger<CityGetAqtDataDayJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;

        public CityGetAqtDataDayJob(ILogger<CityGetAqtDataDayJob> logger, IAqtUploadService aqtUploadService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("数据处理开始。", true);

            string account = "17352335191";
            string password = "1966AEA81A053A0537E01D7A6B73D4EE";
            JObject tokenjob = new JObject();
            tokenjob.Add("account", account);
            tokenjob.Add("password", password);
            JObject token = new JObject();
            List<SugarParameter> param = new List<SugarParameter>();

            //取Token
            string tokenstr = HttpNetRequest.POSTSendJsonRequest("http://58.213.147.234:8000/api/GetToken", JsonConvert.SerializeObject(tokenjob), null);

            if (tokenstr != null && tokenstr != "")
            {
                token = JObject.Parse(tokenstr);
            }
            else
            {
                return;
            }

            //取安监号,GROUPID,SITEID
            DataTable dt = await _aqtUploadService.GetAqtInspectSiteList();
            if (dt.Rows.Count > 0)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    JObject data = new JObject();
                    DataRow dr = dt.Rows[j];
                    JObject jso = new JObject();
                    foreach (DataColumn column in dr.Table.Columns)
                    {
                        if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                        {
                            jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                        }
                        else
                        {
                            jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                        }
                    }
                    string siteajcode = jso.GetValue("siteajcode").ToString();


                    Dictionary<string, object> keyValues = new Dictionary<string, object>();
                    keyValues.Add("searchType", 2);
                    keyValues.Add("pageIndex", 1);
                    keyValues.Add("pageSize", 200);
                    keyValues.Add("recordNumber", siteajcode);

                    string datastr = HttpNetRequest.SendRequest("http://58.213.147.234:8000/api/DailyInspectTotal/GetDailyInspectCheckList", keyValues, "GET", new Dictionary<string, string>() { { "accessToken", token["data"]["accessToken"].ToString().Replace("bearer ", "") } });

                    if (datastr != null && datastr != "")
                    {
                        data = JObject.Parse(datastr);
                    }
                    else
                    {
                        continue;
                    }

                    int GROUPID = (int)jso.GetValue("GROUPID");
                    int SITEID = (int)jso.GetValue("SITEID");

                    foreach (JObject jobdata in data.GetValue("data"))
                    {
                        if (!siteajcode.Equals(jobdata.GetValue("recordNumber").ToString()))
                        {
                            //过滤不一致项目数据
                            continue;
                        }
                        param = new List<SugarParameter>();
                        param.Add(new SugarParameter("@GROUPID", GROUPID));
                        param.Add(new SugarParameter("@SITEID", SITEID));
                        foreach (JProperty jProperty in jobdata.Properties())
                        {
                            if (jProperty.Name == "id")
                            {
                                param.Add(new SugarParameter("@AQTINSPID", jProperty.Value));
                            }
                            else
                            {
                                param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                            }
                        }
                        //插入数据库
                        var result = await _aqtUploadService.doAqtInspectSave(param);
                    }
                }
            }


            //取区属编号、备案号、通知书编号、同名字段
            DataTable dt1 = await _aqtUploadService.GetAqtInspectNameListForUpdate();
            if (dt1.Rows.Count > 0)
            {
                for (int j = 0; j < dt1.Rows.Count; j++)
                {
                    string normMainName = "";
                    JObject data = new JObject();
                    DataRow dr = dt1.Rows[j];
                    JObject jso = new JObject();
                    foreach (DataColumn column in dr.Table.Columns)
                    {
                        if (column.DataType.Equals(System.Type.GetType("System.Int32")))
                        {
                            jso.Add(column.ColumnName, dr[column.ColumnName].ToInt());
                        }
                        else
                        {
                            jso.Add(column.ColumnName, dr[column.ColumnName].ToString());
                        }
                    }
                    string belongedTo = jso.GetValue("belongedTo").ToString();
                    string recordNumber = jso.GetValue("recordNumber").ToString();
                    string tzsno = jso.GetValue("tzsno").ToString();
                    string checkFormNumber = jso.GetValue("checkFormNumber").ToString();

                    Dictionary<string, object> keyValues = new Dictionary<string, object>();
                    keyValues.Add("belongedTo", belongedTo);
                    keyValues.Add("recordNumber", recordNumber);
                    keyValues.Add("tzsno", tzsno);
                    keyValues.Add("checkFormNumber", checkFormNumber);

                    //取数据
                    string strdata = HttpNetRequest.SendRequest("http://58.213.147.234:8000/api/FieldInspection/GetDailyInspectDetail", keyValues, "GET", new Dictionary<string, string>() { { "accessToken", token["data"]["accessToken"].ToString().Replace("bearer ", "") } });

                    if (strdata != null && strdata != "")
                    {
                        data = JObject.Parse(strdata);
                    }
                    else
                    {
                        continue;
                    }

                    foreach (JObject jobdata in data["data"]["normLanInspectionList"])
                    {
                        if (jobdata["normMainName"].ToString() == "")
                        {
                            normMainName += "其他::";
                        }
                        else
                        {
                            normMainName += jobdata.GetValue("normMainName").ToString() + "::";
                        }
                    }


                    if (normMainName.Length > 0)
                    {
                        normMainName = normMainName.Substring(0, normMainName.Length - 2);
                        param = new List<SugarParameter>();
                        param.Add(new SugarParameter("@AQTINSPID", jso.GetValue("AQTINSPID").ToString()));
                        param.Add(new SugarParameter("@normMainNames", normMainName));

                        //更新数据库
                        var result = await _aqtUploadService.doAqtInspectNameUpdate(param);
                    }
                }
            }
            _logger.LogInformation("数据处理结束。", true);
        }
    }
}
