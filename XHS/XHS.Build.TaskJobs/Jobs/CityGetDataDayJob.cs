using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    /// <summary>
    /// 省对接接口数据获取（3号文档）
    /// </summary>
    /// <returns></returns>
    public class CityGetDataDayJob : JobBase, IJob
    {
        private readonly ILogger<CityGetDataDayJob> _logger; 
        private readonly IAqtUploadService _aqtUploadService;
        private readonly AqtToken _aqtToken;
        private readonly ISiteService _siteService;
        public CityGetDataDayJob(ILogger<CityGetDataDayJob> logger, ISiteService siteService, IHpSystemSetting hpSystemSetting, AqtToken aqtToken, IAqtUploadService aqtUploadService)
        {
            _logger = logger;  
            _aqtUploadService = aqtUploadService;
            _siteService = siteService;
            _aqtToken = aqtToken;
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
            string now = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            DateTime endTime = now.ToDateTime();
            DateTime beginTime = now.ToDateTime().AddDays(-1);
            JObject jresult;
            JObject jdata;
            JArray jardata;
            List<SugarParameter> param = new List<SugarParameter>();
            Dictionary<string,string> dic = new Dictionary<string, string>();
            DataSet ds = await _aqtUploadService.GetCityBelongtoList();
            int resultcount = 0;
            string api = "";
            string result;
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
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
                            string belongedTo = jso.GetValue("belongto").ToString();


                            // 3.1 安监通在建项目数据
                            bool hasdata = true;
                            int page = 1;
                            while (hasdata)
                            {
                                // 3.11 获取项目危大工程基本信息
                                api = "ProjectInfoStatistics/GetProjectInfoStatisList";
                                keyValues = new Dictionary<string, object>();
                                keyValues.Add("superOrganName", belongedTo);
                                keyValues.Add("page", page);
                                keyValues.Add("size", 50);
                                keyValues.Add("projectState", 1);
                                result = _aqtToken.UrlRequest(api, keyValues, "http://58.213.147.234:8000/api/");
                                jresult = JObject.Parse(result);
                                if (string.IsNullOrEmpty(result))
                                {
                                    hasdata = false;
                                    break;
                                }
                                page++;
                                if (jresult.ContainsKey("code") && jresult.GetValue("code").ToString() == "0")
                                {

                                    int k = 1;
                                    jardata = (JArray)jresult.GetValue("data");
                                    if (jardata.Count < 50)
                                    {
                                        hasdata = false;
                                    }
                                    if (jardata.Count > 0)
                                    {
                                        foreach (JObject jobCode in jardata)
                                        {
                                            param = new List<SugarParameter>();
                                            param.Add(new SugarParameter("@id", jobCode.GetValue("id").ToString()));
                                            param.Add(new SugarParameter("@belongedTo", belongedTo));
                                            param.Add(new SugarParameter("@recordNumber", jobCode.GetValue("recordNumber").ToString()));
                                            param.Add(new SugarParameter("@remark", jobCode.GetValue("remark").ToString()));
                                            param.Add(new SugarParameter("@superOrganName", jobCode.GetValue("superOrganName").ToString()));
                                            param.Add(new SugarParameter("@projectName", jobCode.GetValue("projectName").ToString()));
                                            param.Add(new SugarParameter("@projectAddress", jobCode.GetValue("projectAddress").ToString()));
                                            param.Add(new SugarParameter("@sgdw", jobCode.GetValue("sgdw").ToString()));
                                            param.Add(new SugarParameter("@jsdw", jobCode.GetValue("jsdw").ToString()));
                                            param.Add(new SugarParameter("@jldw", jobCode.GetValue("jldw").ToString()));
                                            param.Add(new SugarParameter("@sgdwEntCode", jobCode.GetValue("sgdwEntCode").ToString()));
                                            param.Add(new SugarParameter("@jsdwEntCode", jobCode.GetValue("jsdwEntCode").ToString()));
                                            param.Add(new SugarParameter("@jldwEntCode", jobCode.GetValue("jldwEntCode").ToString()));
                                            param.Add(new SugarParameter("@projectPrice", jobCode.GetValue("projectPrice").ToString()));
                                            param.Add(new SugarParameter("@sumProjectPrice", jobCode.GetValue("sumProjectPrice").ToString()));
                                            param.Add(new SugarParameter("@projectAcreage", jobCode.GetValue("projectAcreage").ToString()));
                                            param.Add(new SugarParameter("@projectStartDateTimne", jobCode.GetValue("projectStartDateTimne").ToDateTime()));
                                            param.Add(new SugarParameter("@projectEndDateTimne", jobCode.GetValue("projectEndDateTimne").ToDateTime()));
                                            param.Add(new SugarParameter("@projectCategory", jobCode.GetValue("projectCategory").ToString()));
                                            param.Add(new SugarParameter("@projectTarget", jobCode.GetValue("projectTarget").ToString()));
                                            param.Add(new SugarParameter("@factCompletionDate", jobCode.GetValue("factCompletionDate").ToDateTime()));
                                            param.Add(new SugarParameter("@recordDate", jobCode.GetValue("recordDate").ToDateTime()));
                                            param.Add(new SugarParameter("@projectState", jobCode.GetValue("projectState").ToString()));
                                            param.Add(new SugarParameter("@yiChouChaCount", jobCode.GetValue("yiChouChaCount").ToString()));
                                            param.Add(new SugarParameter("@jiHuaChouChaCount", jobCode.GetValue("jiHuaChouChaCount").ToString()));
                                            param.Add(new SugarParameter("@personCount", jobCode.GetValue("personCount").ToString()));
                                            param.Add(new SugarParameter("@supervisionDepartmentId", jobCode.GetValue("supervisionDepartmentId").ToString()));
                                            param.Add(new SugarParameter("@supervisionDepartmentName", jobCode.GetValue("supervisionDepartmentName").ToString()));
                                            param.Add(new SugarParameter("@belongsDepartments", jobCode.GetValue("belongsDepartments").ToString()));
                                            param.Add(new SugarParameter("@itemNumber", jobCode.GetValue("itemNumber").ToString()));
                                            param.Add(new SugarParameter("@projectHierarchy", jobCode.GetValue("projectHierarchy").ToString()));
                                            param.Add(new SugarParameter("@shiGongXuekeNo", jobCode.GetValue("shiGongXuekeNo").ToString()));
                                            param.Add(new SugarParameter("@latitudeCoordinate", jobCode.GetValue("latitudeCoordinate").ToString()));
                                            param.Add(new SugarParameter("@longitudeCoordinate", jobCode.GetValue("longitudeCoordinate").ToString()));
                                            param.Add(new SugarParameter("@proBigCategory", jobCode.GetValue("proBigCategory").ToString()));
                                            resultcount = await _aqtUploadService.doAqtProjectInsert(param);
                                            k++;
                                            if (resultcount <= 0)
                                            {
                                                //写日志
                                                // _logger.LogInformation("项目危大工程数据更新失败。机构编码(" + belongedTo + ") 安监备案号(" + jobCode.GetValue("recordNumber").ToString() + ")", true);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //写日志
                                    //_logger.LogInformation("项目危大工程数据更新失败。机构编码(" + belongedTo + ")" + jresult.GetValue("message").ToString(), true);
                                }
                            }

                            // 3.3 获取项目自查数
                            api = "ProjectInformation/GetProjectSelfInspectionCount";
                            keyValues = new Dictionary<string, object>();
                            keyValues.Add("beginTime", beginTime);
                            keyValues.Add("endTime", endTime);
                            keyValues.Add("belongedTo", belongedTo);
                            result = _aqtToken.UrlRequest(api, keyValues);
                            jresult = JObject.Parse(result);
                            if (jresult.ContainsKey("code") && jresult.GetValue("code").ToString() == "0")
                            {
                                jdata = (JObject)jresult.GetValue("data");
                                param = new List<SugarParameter>();
                                param.Add(new SugarParameter("@belongto", belongedTo));
                                param.Add(new SugarParameter("@selfinspcount", jdata.GetValue("count").ToInt()));
                                param.Add(new SugarParameter("@updatedate", beginTime));
                                resultcount = await _aqtUploadService.doAqtSelfInspectCountInsert(param);
                                if (resultcount <= 0)
                                {
                                    //写日志
                                    _logger.LogInformation("项目自查数数据更新失败。机构编码(" + belongedTo + ")", true);
                                }

                            }
                            else
                            {
                                //写日志
                                _logger.LogInformation("项目自查数数据更新失败。机构编码(" + belongedTo + ")" + jresult.GetValue("message").ToString(), true);
                            }

                            // 3.6 获取项目月评总数
                            api = "ProjectInformation/GetMonthReviewCount";
                            keyValues = new Dictionary<string, object>();
                            keyValues.Add("beginTime", beginTime);
                            keyValues.Add("endTime", endTime);
                            keyValues.Add("belongedTo", belongedTo);
                            result = _aqtToken.UrlRequest(api, keyValues);
                            jresult = JObject.Parse(result);
                            if (jresult.ContainsKey("code") && jresult.GetValue("code").ToString() == "0")
                            {
                                jdata = (JObject)jresult.GetValue("data");
                                param = new List<SugarParameter>();
                                param.Add(new SugarParameter("@belongto", belongedTo));
                                param.Add(new SugarParameter("@monthreviewcount", jdata.GetValue("count").ToInt()));
                                param.Add(new SugarParameter("@updatedate", beginTime));
                                resultcount = await _aqtUploadService.doAqtMonthReviewCountInsert(param);
                                if (resultcount <= 0)
                                {
                                    //写日志
                                    _logger.LogInformation("项目月评总数数据更新失败。机构编码(" + belongedTo + ")", true);
                                }

                            }
                            else
                            {
                                //写日志
                                _logger.LogInformation("项目月评总数数据更新失败。机构编码(" + belongedTo + ")" + jresult.GetValue("message").ToString(), true);
                            }

                            // 3.7 获取项目每月自评状态及结果
                            api = "ProjectInformation/GetMonthReviewResults";
                            keyValues = new Dictionary<string, object>();
                            keyValues.Add("year", beginTime.Year);
                            keyValues.Add("month", beginTime.Month);
                            keyValues.Add("belongedTo", belongedTo);
                            result = _aqtToken.UrlRequest(api, keyValues);
                            jresult = JObject.Parse(result);
                            int sumscore = 0;
                            if (jresult.ContainsKey("code") && jresult.GetValue("code").ToString() == "0")
                            {
                                jardata = (JArray)jresult.GetValue("data");
                                if (jardata.Count > 0)
                                {
                                    foreach (JObject jobCode in jardata)
                                    {
                                        if (!string.IsNullOrEmpty(jobCode.GetValue("sumscore").ToString()))
                                        {
                                            sumscore = jobCode.GetValue("sumscore").ToInt();
                                        }
                                        param = new List<SugarParameter>();
                                        param.Add(new SugarParameter("@belongto", belongedTo));
                                        param.Add(new SugarParameter("@siteajcode", jobCode.GetValue("recordNumber").ToString()));
                                        param.Add(new SugarParameter("@mrstate", jobCode.GetValue("state").ToString()));
                                        param.Add(new SugarParameter("@sumscore", sumscore));
                                        param.Add(new SugarParameter("@updatedate", beginTime));
                                        resultcount = await _aqtUploadService.doAqtMonthReviewResultInsert(param);
                                        if (resultcount <= 0)
                                        {
                                            //写日志
                                            _logger.LogInformation("项目每月自评状态及结果数据更新失败。机构编码(" + belongedTo + ") 安监备案号(" + jobCode.GetValue("recordNumber").ToString() + ")", true);
                                        }
                                    }
                                }


                            }
                            else
                            {
                                //写日志
                                _logger.LogInformation("项目每月自评状态及结果数据更新失败。机构编码(" + belongedTo + ")" + jresult.GetValue("message").ToString(), true);
                            }

                            // 3.8 获取项目安标考评结果
                            api = "ProjectInformation/GetSafetyStandardResults";
                            keyValues = new Dictionary<string, object>();
                            keyValues.Add("belongedTo", belongedTo);
                            keyValues.Add("pageIndex", 1);
                            keyValues.Add("pageSize", 10000);
                            result = _aqtToken.UrlRequest(api, keyValues);
                            jresult = JObject.Parse(result);
                            if (jresult.ContainsKey("code") && jresult.GetValue("code").ToString() == "0")
                            {
                                jardata = (JArray)jresult.GetValue("data");
                                if (jardata.Count > 0)
                                {
                                    foreach (JObject jobCode in jardata)
                                    {
                                        param = new List<SugarParameter>();
                                        param.Add(new SugarParameter("@belongto", belongedTo));
                                        param.Add(new SugarParameter("@siteajcode", jobCode.GetValue("recordNumber").ToString()));
                                        param.Add(new SugarParameter("@ssrstate", jobCode.GetValue("state").ToString()));
                                        param.Add(new SugarParameter("@ssrresult", jobCode.GetValue("result").ToString()));
                                        resultcount = await _aqtUploadService.doAqtSafetyStandardResultInsert(param);
                                        if (resultcount <= 0)
                                        {
                                            //写日志
                                            _logger.LogInformation("项目安标考评结果数据更新失败。机构编码(" + belongedTo + ") 安监备案号(" + jobCode.GetValue("recordNumber").ToString() + ")", true);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                //写日志
                                _logger.LogInformation("项目安标考评结果数据更新失败。机构编码(" + belongedTo + ")" + jresult.GetValue("message").ToString(), true);
                                
                            }

                            // 3.1 获取超危项目
                            hasdata = true;
                            page = 1;
                            while (hasdata)
                            {
                                // 3.11 获取项目危大工程基本信息
                                api = "DockingSuperScaleDanger/GetSuperScaleDangerList";
                                keyValues = new Dictionary<string, object>();
                                keyValues.Add("belongedTo", belongedTo);
                                keyValues.Add("pageIndex", page);
                                keyValues.Add("pageSize", 50);
                                result = _aqtToken.UrlRequest(api, keyValues);
                                jresult = JObject.Parse(result);
                                if (string.IsNullOrEmpty(result))
                                {
                                    hasdata = false;
                                    break;
                                }
                                page++;
                                if (jresult.ContainsKey("code") && jresult.GetValue("code").ToString() == "0")
                                {

                                    int k = 1;
                                    jardata = (JArray)jresult.GetValue("data");
                                    if (jardata.Count < 50)
                                    {
                                        hasdata = false;
                                    }
                                    if (jardata.Count > 0)
                                    {
                                        foreach (JObject jobCode in jardata)
                                        {
                                            param = new List<SugarParameter>();
                                            param.Add(new SugarParameter("@SDID", belongedTo + ((page - 1) * 50 + k).ToString()));
                                            param.Add(new SugarParameter("@belongto", belongedTo));
                                            param.Add(new SugarParameter("@siteajcode", jobCode.GetValue("recordNumber").ToString()));
                                            param.Add(new SugarParameter("@masterclass", jobCode.GetValue("masterClass").ToString()));
                                            param.Add(new SugarParameter("@ziliaomingcheng", jobCode.GetValue("ziLiaoMingCheng").ToString()));
                                            param.Add(new SugarParameter("@begintime", jobCode.GetValue("beginTime").ToDateTime()));
                                            param.Add(new SugarParameter("@endtime", jobCode.GetValue("endtime").ToDateTime()));
                                            param.Add(new SugarParameter("@remark", jobCode.GetValue("remark").ToString()));
                                            param.Add(new SugarParameter("@dangerstatus", jobCode.GetValue("status").ToString()));
                                            param.Add(new SugarParameter("@infomation", jobCode.GetValue("infoMation").ToString()));
                                            param.Add(new SugarParameter("@files", jobCode.GetValue("files").ToString()));
                                            param.Add(new SugarParameter("@dangeruuid", jobCode.GetValue("uuid").ToString()));
                                            resultcount = await _siteService.doSiteDangerInsert(param);
                                            k++;
                                            if (resultcount <= 0)
                                            {
                                                //写日志
                                                _logger.LogInformation("项目危大工程数据更新失败。机构编码(" + belongedTo + ") 安监备案号(" + jobCode.GetValue("recordNumber").ToString() + ")", true);
                                                hasdata = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            else
                            {
                                //写日志
                                _logger.LogInformation("项目危大工程数据更新失败。机构编码(" + belongedTo + ")" + jresult.GetValue("message").ToString(), true);
                                    hasdata = false;
                                    break;
                            }
                        }
                    }
                    }
                }
            }


            _logger.LogInformation("数据处理结束。", true);
        }
    }
}
