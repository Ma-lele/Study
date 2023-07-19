using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Services.Event;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.Analyst.Web
{
    public class PersonnelResultsJob : JobBase, IJob
    {
        private readonly ILogger<PersonnelResultsJob> _logger;
        private readonly XHSRealnameToken _jwtToken;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IEventService _eventService;

        public PersonnelResultsJob(ILogger<PersonnelResultsJob> logger, XHSRealnameToken jwtToken, IHpSystemSetting hpSystemSetting, IEventService eventService)
        {
            _logger = logger;
            _jwtToken = jwtToken;
            _hpSystemSetting = hpSystemSetting;
            _eventService = eventService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            try
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                string url;
                string user;
                string pwd;
                string city;
                int year = dt.Year;
                int month = dt.Month-1;
                if(month == 0)
                {
                    month = 12;
                    year = year - 1;
                }
                string date = dt.ToShortDateString();
                url = _hpSystemSetting.getSettingValue(Const.Setting.S172);
                user = _hpSystemSetting.getSettingValue(Const.Setting.S176);
                pwd = _hpSystemSetting.getSettingValue(Const.Setting.S177);
                city = _hpSystemSetting.getSettingValue(Const.Setting.S175);
                //农名工工资预警信息
                JObject job1 = new JObject();
                job1.Add("city", city);
                job1.Add("warningYear", year);
                job1.Add("warningMonth", month);
                //农名工工资预警信息
                var data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/pay-warning-list", JsonConvert.SerializeObject(job1));
                if (!string.IsNullOrEmpty(data1))
                {
                    JObject result = JObject.Parse(data1);
                    JArray jarr = (JArray)result.GetValue("data");
                    string eventtype = "RY05";//工资代发
                    string eventtype2 = "RY06";//施工总承包进度款转入
                    List<SugarParameter> param;
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        JObject jo = (JObject)jarr[i];
                        param = new List<SugarParameter>();
                        param.Add(new SugarParameter("@recordNumber", jo.GetValue("safetySupervisionRecordNumber").ToString()));
                        param.Add(new SugarParameter("@eventtype", eventtype));
                        param.Add(new SugarParameter("@collectionday", year + "-" + String.Format("{0:D2}", month)));
                        param.Add(new SugarParameter("@totalcount", jo.GetValue("attendancePersonnelCount").ToString()));
                        param.Add(new SugarParameter("@realcount", jo.GetValue("agentPaymentCount").ToString()));
                        param.Add(new SugarParameter("@result", jo.GetValue("agentPaymentRatio").ToString()));
                        await _eventService.PersonnelResultInsert(param);
                        if (jo.GetValue("yesNoLotsMoneyThisMonth").ToString().Contains("无"))
                        {
                            param = new List<SugarParameter>();
                            param.Add(new SugarParameter("@recordNumber", jo.GetValue("safetySupervisionRecordNumber").ToString()));
                            param.Add(new SugarParameter("@eventtype", eventtype2));
                            param.Add(new SugarParameter("@collectionday", year + "-" + String.Format("{0:D2}", month)));
                            param.Add(new SugarParameter("@totalcount", 0));
                            param.Add(new SugarParameter("@realcount", 0));
                            param.Add(new SugarParameter("@result", 0));
                            await _eventService.PersonnelResultInsert(param);
                        }
                        else
                        {
                            param = new List<SugarParameter>();
                            param.Add(new SugarParameter("@recordNumber", jo.GetValue("safetySupervisionRecordNumber").ToString()));
                            param.Add(new SugarParameter("@eventtype", eventtype2));
                            param.Add(new SugarParameter("@collectionday",  year + "-" + String.Format("{0:D2}", month)));
                            param.Add(new SugarParameter("@totalcount", 0));
                            param.Add(new SugarParameter("@realcount", 0));
                            param.Add(new SugarParameter("@result", 1));
                            await _eventService.PersonnelResultInsert(param);
                        }

                    }

                }

                //管理人员到岗率信息            
                data1 = _jwtToken.JsonRequest(url, user, pwd, "construction-site-api-app-service-new/attendance-rate-list", JsonConvert.SerializeObject(job1));
                if (!string.IsNullOrEmpty(data1))
                {
                    JObject result = JObject.Parse(data1);
                    JArray jarr = (JArray)result.GetValue("data");
                    string eventtype = "RY01";//综合到岗率
                    string eventtype2 = "RY04";//绿码率
                    List<SugarParameter> param;
                    for (int i = 0; i < jarr.Count; i++)
                    {
                        JObject jo = (JObject)jarr[i];
                        param = new List<SugarParameter>();
                        param.Add(new SugarParameter("@recordNumber", jo.GetValue("safetySupervisionRecordNumber").ToString()));
                        param.Add(new SugarParameter("@eventtype", eventtype));
                        param.Add(new SugarParameter("@collectionday", year + "-" + String.Format("{0:D2}", month)));
                        param.Add(new SugarParameter("@totalcount", jo.GetValue("supervisoryAverageRatio").ToString()));
                        param.Add(new SugarParameter("@realcount", jo.GetValue("projectComanyOnDutyRatio").ToString()));
                        param.Add(new SugarParameter("@result", jo.GetValue("conAverageRatio").ToString()));
                        await _eventService.PersonnelResultInsert(param);

                        param = new List<SugarParameter>();
                        param.Add(new SugarParameter("@recordNumber", jo.GetValue("safetySupervisionRecordNumber").ToString()));
                        param.Add(new SugarParameter("@eventtype", eventtype2));
                        param.Add(new SugarParameter("@collectionday", year + "-" + String.Format("{0:D2}", month)));
                        param.Add(new SugarParameter("@totalcount", 0));
                        param.Add(new SugarParameter("@realcount", 0));
                        param.Add(new SugarParameter("@result", jo.GetValue("safetyStateNotGoodRatio").ToString()));
                        await _eventService.PersonnelResultInsert(param);


                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
