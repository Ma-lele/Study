using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.OperateLogS;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class SmartUpLoadYcCarUndeveloped : JobBase, IJob
    {
        private readonly ILogger<SmartUpLoadYcCarUndeveloped> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly IOperateLogService _operateLogService;
        private readonly IBaseRepository<BaseEntity> _baseServices;
        public SmartUpLoadYcCarUndeveloped(ILogger<SmartUpLoadYcCarUndeveloped> logger, IOperateLogService operateLogService, IAqtUploadService aqtUploadService, IBaseRepository<BaseEntity> baseServices)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _baseServices = baseServices;

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

            string uploadtime = "";
            var data = await _baseServices.Db.Queryable<GCCityUploadDateEntity>().Where(it => it.uploadurl == "http://221.231.127.53:84/interface/" && it.post == "/yancheng/cheliang/clwcx").FirstAsync();
            if (data != null)
            {
                uploadtime = data.uploadtime.ToString();
            }
            string uploadurl = "http://221.231.127.53:84/interface/";   //射阳分组、建湖分组
            string GONGCHENG_CODE = "202103209248811061208095633|20211208_CLWCX_09593315";      //射阳工人文化宫用房建设
            GONGCHENG_CODE += ",202103209035678900630091358|20211216_CLWCX_10502418";// 建湖上冈现代科技园A区厂房工程
            GONGCHENG_CODE += ",202293209033622670314150317|20220314_CLWCX_15070119";// 盐城城投商务中心项目
            GONGCHENG_CODE += ",202283209817777770316111014|20220316_CLWCX_11133914";// 盐通高铁东台站站前广场及综合枢纽配套工程

            DataTable dt = await _aqtUploadService.GetListForCarUndeveloped(uploadtime, uploadurl);
            Tuisong(dt, GONGCHENG_CODE);


            uploadurl = "http://49.4.68.132:8094/api/";     //阜宁分组
            GONGCHENG_CODE = "202173209238874561207100032|20211207_CLWCX_10041114";    //阜宁冠城雍景
            GONGCHENG_CODE += ",202103209236639011209095819|20211209_CLWCX_10010914";   //阜宁栋梁名府
            dt = await _aqtUploadService.GetListForCarUndeveloped(uploadtime, uploadurl);
            Tuisong(dt, GONGCHENG_CODE);


            uploadurl = "http://221.231.127.53:84/interface/";      //新合盛
            GONGCHENG_CODE = "202103209035678900630091358|20211216_CLWCX_10502418";//盐城中海华樾花园一标段
            GONGCHENG_CODE += ",AJ320903120210160|20211029_CLWCX_14241419";//盐都区名望府三期项目
            dt = await _aqtUploadService.GetListForCarUndeveloped(uploadtime, uploadurl);
            Tuisong(dt, GONGCHENG_CODE);


            _logger.LogInformation("数据上传结束。", true);
        }
        public async void Tuisong(DataTable dt, string GONGCHENG_CODE)
        {
            if(dt == null || dt.Rows.Count <= 0)
            {
                return;
            }
            int successcount = 0;
            int errcount = 0;
            string url = "http://221.231.127.53:84/interface/";
            string Token = "IBGcPGQieY3m8518D1XIeVn65Zpm11GffuPR7oy/YDlIw5Xtx0SeMw==";
            string api = string.Empty;
            string realapi = string.Empty;
            string result = string.Empty;
            JArray jar = new JArray();
            JObject carwashjob = new JObject();
            CityUploadOperateLog LogEntity = new CityUploadOperateLog();
            DateTime now = DateTime.Now;
            List<string> list = new List<string>();

            #region
            string strWhere = string.Empty;
            foreach (string item in GONGCHENG_CODE.Split(','))
            {
                strWhere += " or GONGCHENG_CODE='" + item + "'";
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                strWhere = strWhere.Remove(0, 4);
            }
            var dtdata = dt.Select(strWhere);
            if (dtdata.Length > 0)
            {
                for (int j = 0; j < dtdata.Length; j++)
                {
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

                        api = jso.GetValue("post").ToString();
                        realapi = api;

                        //车辆未冲洗接口
                        if (api.Equals("/yancheng/cheliang/clwcx"))
                        {
                            realapi = "yancheng/cheliang/clwcx";
                            jso["xmbh"] = jso.GetValue("GONGCHENG_CODE").ToString().Split('|')[0];
                            jso["deviceNo"] = jso.GetValue("GONGCHENG_CODE").ToString().Split('|')[1];
                            carwashjob.Add("xmbh", jso.GetValue("xmbh").ToString());
                            carwashjob.Add("deviceNo", jso.GetValue("deviceNo").ToString());
                        }


                        jso.Remove("post");
                        jso.Remove("siteuploadurl");
                        jso.Remove("uploadaccount");
                        jso.Remove("uploadpwd");

                        if (api.Contains("/yancheng/cheliang/clwcx"))
                        {
                            jar = new JArray();
                            jar.Add(jso);
                            jso = new JObject();
                            jso.Add("Token", Token);
                            jso.Add("JSON", jar);
                        }
                        else
                        {
                            var data = jso;
                            jso = new JObject();
                            jso.Add("Token", Token);
                            jso.Add("JSON", data);
                        }
                        result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                        LogEntity = new CityUploadOperateLog
                        {
                            //Id=Guid.NewGuid().ToString(),
                            url = url,
                            api = api,
                            account = Token,
                            param = jso.ToString(),
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
                            int code = (int)mJObj["code"];
                            if (code == 0)
                            {
                                if (!list.Contains(url + api))
                                {
                                    await _aqtUploadService.UpdateCityUploadDate(url, api, now);
                                }
                                successcount += successcount;
                            }
                        }

                        if (carwashjob.ContainsKey("xmbh") && carwashjob.ContainsKey("deviceNo"))           //车辆未冲洗设备告警接口
                        {
                            realapi = "yancheng/cheliang/clwcxRegisterAlarm";
                            jar = new JArray();
                            jso = new JObject();
                            jso.Add("Token", Token);
                            carwashjob.Add("devstatus", "1");
                            jar.Add(carwashjob);
                            jso.Add("JSON", jar);
                            result = HttpNetRequest.POSTSendJsonRequest(url + realapi, JsonConvert.SerializeObject(jso), new Dictionary<string, string>() { });
                            carwashjob = new JObject();
                            LogEntity = new CityUploadOperateLog
                            {
                                //Id=Guid.NewGuid().ToString(),
                                url = url,
                                api = realapi,
                                account = Token,
                                param = jso.ToString(),
                                result = result,
                                createtime = DateTime.Now
                            };
                            await _operateLogService.AddCityUploadApiLog(LogEntity);
                           
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(api + ":" + ex.Message);
                    }
                }
            }



            #endregion

        }

    }
}
