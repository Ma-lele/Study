using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.AqtUpload;
using XHS.Build.Services.Elevator;
using XHS.Build.Services.OperateLogS;
using XHS.Build.Services.Site;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class DayunMinuteJob : JobBase, IJob
    {
        private readonly ILogger<DayunMinuteJob> _logger;
        private readonly IAqtUploadService _aqtUploadService;
        private readonly CityToken _cityToken;
        private readonly IOperateLogService _operateLogService;
        private readonly DayunToken _dayunToken;
        private readonly ISiteService _siteService;
        private readonly IElevatorService _elevatorService;

        public DayunMinuteJob(ILogger<DayunMinuteJob> logger, IElevatorService elevatorService, IOperateLogService operateLogService, CityToken cityToken, IHpSystemSetting systemSettingService, IAqtUploadService aqtUploadService, DayunToken dayunToken, ISiteService siteService)
        {
            _logger = logger;
            _aqtUploadService = aqtUploadService;
            _operateLogService = operateLogService;
            _cityToken = cityToken;
            _dayunToken = dayunToken;
            _siteService = siteService;
            _elevatorService = elevatorService;

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
                DateTime now = DateTime.Now;
                if(now.Hour == 1)
                {
                    return;
                }
                _logger.LogInformation("数据上传开始。", true);
           
               
                string uploadapi = string.Empty;
                string data = string.Empty;
                string url = "http://zh1.wxzjj.com:5678";
                JObject job = new JObject();
                JArray jar = new JArray();
                JArray jarCode = new JArray();
                JArray postjar = new JArray();
                JObject jobdate = new JObject();
                JObject jObject = new JObject();
                List<SugarParameter> param = new List<SugarParameter>();

                //获取需要拉取数据的项目列表
                DataTable postdt = await _siteService.GetDyUpdatedate(url);
                if (postdt.Rows.Count > 0)
                {
                    for (int j = 0; j < postdt.Rows.Count; j++)
                    {
                        DataRow dr = postdt.Rows[j];
                        jobdate.Add(dr["post"].ToString(), dr["uploadtime"].ToString());
                    }
                }
                //获取需要拉取数据的项目列表
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

                        }
                        jarCode.Add(jso);
                    }
                }

                string code = "";
                int groupId = 0;
                int siteId = 0;
                string st = "";
                int result = 0;
                int successcount = 0;
                TimeSpan ts;
                int page = 1;
                DateTime starttime;
                DateTime endtime;
                bool hasdata = true;
                foreach (JObject jobCode in jarCode)
                {
                    if (jobCode.ContainsKey("sitecode"))
                    {

                        code = jobCode.GetValue("sitecode").ToString();
                        groupId = jobCode.GetValue("GROUPID").ToInt();
                        siteId = jobCode.GetValue("SITEID").ToInt();
                        //获取工地扬尘数据
                        #region
                        uploadapi = "rest/ProjectInfo/getDustNoise";
                        if (jobdate.ContainsKey(uploadapi))
                        {
                            st = jobdate.GetValue(uploadapi).ToString();
                        }
                        else
                        {
                            st = now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        starttime = st.ToDateTime();
                        ts = now - st.ToDateTime();
                        if (ts.Days > 0)
                        {
                            endtime = starttime;
                            endtime = endtime.AddDays(1);
                            for (int i = 0; i < ts.Days; i++)
                            {
                                page = 1;
                                hasdata = true;
                                while (hasdata)
                                {
                                    jObject = new JObject();
                                    jar = new JArray();
                                    jObject.Add("contract_record_code", code);
                                    jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("page", page);
                                    jObject.Add("size", 100);
                                    data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                                    if (string.IsNullOrEmpty(data))
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                    page++;
                                    job = JObject.Parse(data);
                                    if (job.ContainsKey("rows"))
                                    {
                                        jar = JArray.Parse(job.GetValue("rows").ToString());
                                        if (jar.Count <= 0)
                                        {
                                            hasdata = false;
                                        }
                                        foreach (JObject item in jar)
                                        {
                                            param = new List<SugarParameter>();
                                            param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                            param.Add(new SugarParameter("@" + "SITEID", siteId));
                                            foreach (JProperty jProperty in item.Properties())
                                            {
                                                param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                            }
                                            result = await _siteService.doDySitePmData(param);

                                            successcount = successcount + result;
                                        }
                                    }
                                    else
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                }
                                starttime = endtime;
                                endtime = endtime.AddDays(1);
                            }
                        }
                        else
                        {
                            endtime = now;
                            page = 1;
                            hasdata = true;
                            while (hasdata)
                            {
                                jObject = new JObject();
                                jar = new JArray();
                                jObject.Add("contract_record_code", code);
                                jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                                jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                jObject.Add("page", page);
                                jObject.Add("size", 100);
                                data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                                page++;
                                if (string.IsNullOrEmpty(data))
                                {
                                    hasdata = false;
                                    break;
                                }
                                job = JObject.Parse(data);
                                if (job.ContainsKey("rows"))
                                {
                                    jar = JArray.Parse(job.GetValue("rows").ToString());
                                    if (jar.Count <= 0)
                                    {
                                        hasdata = false;
                                    }
                                    foreach (JObject item in jar)
                                    {
                                        param = new List<SugarParameter>();
                                        param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                        param.Add(new SugarParameter("@" + "SITEID", siteId));
                                        foreach (JProperty jProperty in item.Properties())
                                        {
                                            param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                        }
                                        result = await _siteService.doDySitePmData(param);

                                        successcount = successcount + result;
                                    }
                                }
                                else
                                {
                                    hasdata = false;
                                    break;
                                }
                            }
                        }

                        if (successcount > 0)
                        {
                            await _aqtUploadService.UpdateCityUploadDate(url, uploadapi, now);
                        }
                        #endregion

                        //获取获取工地塔吊数据
                        #region
                        successcount = 0;
                        uploadapi = "rest/ProjectInfo/getTowerData";

                        if (jobdate.ContainsKey(uploadapi))
                        {
                            st = jobdate.GetValue(uploadapi).ToString();
                        }
                        else
                        {
                            st = now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        starttime = st.ToDateTime();
                        ts = now - st.ToDateTime();
                        if (ts.Days > 0)
                        {
                            endtime = starttime;
                            endtime = endtime.AddDays(1);
                            for (int i = 0; i < ts.Days; i++)
                            {
                                page = 1;
                                hasdata = true;
                                while (hasdata)
                                {
                                    jObject = new JObject();
                                    jar = new JArray();
                                    jObject.Add("contract_record_code", code);
                                    jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("page", page);
                                    jObject.Add("size", 100);
                                    data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                                    page++;
                                    if (string.IsNullOrEmpty(data))
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                    job = JObject.Parse(data);
                                    if (job.ContainsKey("rows"))
                                    {
                                        jar = JArray.Parse(job.GetValue("rows").ToString());
                                        if (jar.Count <= 0)
                                        {
                                            hasdata = false;
                                        }
                                        foreach (JObject item in jar)
                                        {
                                            param = new List<SugarParameter>();
                                            param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                            param.Add(new SugarParameter("@" + "SITEID", siteId));
                                            foreach (JProperty jProperty in item.Properties())
                                            {
                                                param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                            }
                                            result = await _elevatorService.doDyCraneRtdInsert(param);
                                            successcount = successcount + result;
                                            SpecialEqpData seData = new SpecialEqpData();
                                            seData.Platform = "DayunGet";
                                            seData.Flag = 1;
                                            if (item.ContainsKey("device_id") && !string.IsNullOrEmpty(item.GetValue("device_id").ToString()))
                                            {
                                                seData.SeCode = item.GetValue("device_id").ToString();
                                            }
                                            if (item.ContainsKey("height") && !string.IsNullOrEmpty(item.GetValue("height").ToString()))
                                            {
                                                seData.Height = item.GetValue("height").ToDecimal();
                                            }
                                            if (item.ContainsKey("height") && !string.IsNullOrEmpty(item.GetValue("weight").ToString()))
                                            {
                                                seData.Weight = item.GetValue("weight").ToDecimal();
                                            }
                                            if (item.ContainsKey("magnitude") && !string.IsNullOrEmpty(item.GetValue("magnitude").ToString()))
                                            {
                                                seData.Margin = item.GetValue("magnitude").ToDouble();
                                            }
                                            if (item.ContainsKey("torque") && !string.IsNullOrEmpty(item.GetValue("torque").ToString()))
                                            {
                                                seData.Moment = item.GetValue("torque").ToDecimal();
                                            }
                                            if (item.ContainsKey("rotation") && !string.IsNullOrEmpty(item.GetValue("rotation").ToString()))
                                            {
                                                seData.Rotation = item.GetValue("rotation").ToDouble();
                                            }
                                            if (item.ContainsKey("wind_speed") && !string.IsNullOrEmpty(item.GetValue("wind_speed").ToString()))
                                            {
                                                seData.Wind = item.GetValue("wind_speed").ToInt();
                                            }
                                            if (item.ContainsKey("limit_weight") && !string.IsNullOrEmpty(item.GetValue("limit_weight").ToString()))
                                            {
                                                seData.SafeLoad = item.GetValue("limit_weight").ToDecimal();
                                            }
                                            if (item.ContainsKey("torque_percentage") && !string.IsNullOrEmpty(item.GetValue("torque_percentage").ToString()))
                                            {
                                                seData.MomentPercent = item.GetValue("torque_percentage").ToDecimal();
                                            }
                                            if (item.ContainsKey("id_card") && !string.IsNullOrEmpty(item.GetValue("id_card").ToString()))
                                            {
                                                seData.DriverCardNo = item.GetValue("id_card").ToString();
                                            }
                                            if (item.ContainsKey("datetime") && !string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                            {
                                                seData.UpdateTime = item.GetValue("datetime").ToDateTime();
                                                seData.CreateTime = item.GetValue("datetime").ToDateTime();
                                            }

                                            await _operateLogService.AddSpecialEqpDataLog(seData);

                                            successcount = successcount + result;
                                        }
                                    }
                                    else
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                }
                                starttime = endtime;
                                endtime = endtime.AddDays(1);
                            }
                        }
                        else
                        {
                            endtime = now;
                            page = 1;
                            hasdata = true;
                            while (hasdata)
                            {
                                jObject = new JObject();
                                jar = new JArray();
                                jObject.Add("contract_record_code", code);
                                jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                                jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                jObject.Add("page", page);
                                jObject.Add("size", 100);
                                data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                                page++;
                                if (string.IsNullOrEmpty(data))
                                {
                                    hasdata = false;
                                    break;
                                }
                                job = JObject.Parse(data);
                                if (job.ContainsKey("rows"))
                                {
                                    jar = JArray.Parse(job.GetValue("rows").ToString());
                                    if (jar.Count <= 0)
                                    {
                                        hasdata = false;
                                    }
                                    foreach (JObject item in jar)
                                    {
                                        param = new List<SugarParameter>();
                                        param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                        param.Add(new SugarParameter("@" + "SITEID", siteId));
                                        foreach (JProperty jProperty in item.Properties())
                                        {
                                            param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                        }
                                        result = await _elevatorService.doDyCraneRtdInsert(param);
                                        successcount = successcount + result;
                                        SpecialEqpData seData = new SpecialEqpData();
                                        seData.Platform = "DayunGet";
                                        seData.Flag = 1;
                                        if (item.ContainsKey("device_id") && !string.IsNullOrEmpty(item.GetValue("device_id").ToString()))
                                        {
                                            seData.SeCode = item.GetValue("device_id").ToString();
                                        }
                                        if (item.ContainsKey("height") && !string.IsNullOrEmpty(item.GetValue("height").ToString()))
                                        {
                                            seData.Height = item.GetValue("height").ToDecimal();
                                        }
                                        if (item.ContainsKey("height") && !string.IsNullOrEmpty(item.GetValue("weight").ToString()))
                                        {
                                            seData.Weight = item.GetValue("weight").ToDecimal();
                                        }
                                        if (item.ContainsKey("magnitude") && !string.IsNullOrEmpty(item.GetValue("magnitude").ToString()))
                                        {
                                            seData.Margin = item.GetValue("magnitude").ToDouble();
                                        }
                                        if (item.ContainsKey("torque") && !string.IsNullOrEmpty(item.GetValue("torque").ToString()))
                                        {
                                            seData.Moment = item.GetValue("torque").ToDecimal();
                                        }
                                        if (item.ContainsKey("rotation") && !string.IsNullOrEmpty(item.GetValue("rotation").ToString()))
                                        {
                                            seData.Rotation = item.GetValue("rotation").ToDouble();
                                        }
                                        if (item.ContainsKey("wind_speed") && !string.IsNullOrEmpty(item.GetValue("wind_speed").ToString()))
                                        {
                                            seData.Wind = item.GetValue("wind_speed").ToInt();
                                        }
                                        if (item.ContainsKey("limit_weight") && !string.IsNullOrEmpty(item.GetValue("limit_weight").ToString()))
                                        {
                                            seData.SafeLoad = item.GetValue("limit_weight").ToDecimal();
                                        }
                                        if (item.ContainsKey("torque_percentage") && !string.IsNullOrEmpty(item.GetValue("torque_percentage").ToString()))
                                        {
                                            seData.MomentPercent = item.GetValue("torque_percentage").ToDecimal();
                                        }
                                        if (item.ContainsKey("id_card") && !string.IsNullOrEmpty(item.GetValue("id_card").ToString()))
                                        {
                                            seData.DriverCardNo = item.GetValue("id_card").ToString();
                                        }
                                        if (item.ContainsKey("datetime") && !string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                        {
                                            seData.UpdateTime = item.GetValue("datetime").ToDateTime();
                                            seData.CreateTime = item.GetValue("datetime").ToDateTime();
                                        }

                                        await _operateLogService.AddSpecialEqpDataLog(seData);

                                        successcount = successcount + result;
                                    }
                                }
                                else
                                {
                                    hasdata = false;
                                    break;
                                }
                            }

                        }
                        if (successcount > 0)
                        {
                            await _aqtUploadService.UpdateCityUploadDate(url, uploadapi, now);
                        }

                        #endregion

                        //获取获取工地升降机数据
                        #region
                        successcount = 0;
                        uploadapi = "rest/ProjectInfo/getElevator";

                        if (jobdate.ContainsKey(uploadapi))
                        {
                            st = jobdate.GetValue(uploadapi).ToString();
                        }
                        else
                        {
                            st = now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        starttime = st.ToDateTime();
                        ts = now - st.ToDateTime();
                        if (ts.Days > 0)
                        {
                            endtime = starttime;
                            endtime = endtime.AddDays(1);
                            for (int i = 0; i < ts.Days; i++)
                            {
                                page = 1;
                                hasdata = true;
                                while (hasdata)
                                {
                                    jObject = new JObject();
                                    jar = new JArray();
                                    jObject.Add("contract_record_code", code);
                                    jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("page", page);
                                    jObject.Add("size", 100);
                                    data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                                    page++;
                                    if (string.IsNullOrEmpty(data))
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                    job = JObject.Parse(data);
                                    if (job.ContainsKey("rows"))
                                    {
                                        jar = JArray.Parse(job.GetValue("rows").ToString());
                                        if (jar.Count <= 0)
                                        {
                                            hasdata = false;
                                        }
                                        foreach (JObject item in jar)
                                        {
                                            param = new List<SugarParameter>();
                                            param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                            param.Add(new SugarParameter("@" + "SITEID", siteId));
                                            foreach (JProperty jProperty in item.Properties())
                                            {
                                                param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                            }
                                            result = await _elevatorService.doDyLiftRtdInsert(param);
                                            successcount = successcount + result;
                                            SpecialEqpData seData = new SpecialEqpData();
                                            seData.Platform = "DayunGet";
                                            seData.Flag = 2;
                                            if (!string.IsNullOrEmpty(item.GetValue("device_id").ToString()))
                                            {
                                                seData.SeCode = item.GetValue("device_id").ToString();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("height").ToString()))
                                            {
                                                seData.Height = item.GetValue("height").ToDecimal();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("weight").ToString()))
                                            {
                                                seData.Weight = item.GetValue("weight").ToDecimal();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                            {
                                                seData.DeviceTime = item.GetValue("datetime").ToString();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("speed").ToString()))
                                            {
                                                seData.Speed = item.GetValue("speed").ToDecimal();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("floor").ToString()))
                                            {
                                                seData.Floor = item.GetValue("floor").ToDecimal();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("id_card").ToString()))
                                            {
                                                seData.DriverCardNo = item.GetValue("id_card").ToString();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                            {
                                                seData.UpdateTime = item.GetValue("datetime").ToDateTime();
                                                seData.CreateTime = item.GetValue("datetime").ToDateTime();
                                            }
                                            await _operateLogService.AddSpecialEqpDataLog(seData);
                                        }
                                    }
                                    else
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                }
                                starttime = endtime;
                                endtime = endtime.AddDays(1);
                            }
                        }
                        else
                        {
                            endtime = now;
                            page = 1;
                            hasdata = true;
                            while (hasdata)
                            {
                                jObject = new JObject();
                                jar = new JArray();
                                jObject.Add("contract_record_code", code);
                                jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                                jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                jObject.Add("page", page);
                                jObject.Add("size", 100);
                                data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                                if (string.IsNullOrEmpty(data))
                                {
                                    hasdata = false;
                                    break;
                                }
                                page++;
                                job = JObject.Parse(data);
                                if (job.ContainsKey("rows"))
                                {
                                    jar = JArray.Parse(job.GetValue("rows").ToString());
                                    if (jar.Count <= 0)
                                    {
                                        hasdata = false;
                                    }
                                    foreach (JObject item in jar)
                                    {
                                        param = new List<SugarParameter>();
                                        param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                        param.Add(new SugarParameter("@" + "SITEID", siteId));
                                        foreach (JProperty jProperty in item.Properties())
                                        {
                                            param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                        }
                                        result = await _elevatorService.doDyLiftRtdInsert(param);
                                        successcount = successcount + result;
                                        SpecialEqpData seData = new SpecialEqpData();
                                        seData.Platform = "DayunGet";
                                        seData.Flag = 2;
                                        if (!string.IsNullOrEmpty(item.GetValue("device_id").ToString()))
                                        {
                                            seData.SeCode = item.GetValue("device_id").ToString();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("height").ToString()))
                                        {
                                            seData.Height = item.GetValue("height").ToDecimal();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("weight").ToString()))
                                        {
                                            seData.Weight = item.GetValue("weight").ToDecimal();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                        {
                                            seData.DeviceTime = item.GetValue("datetime").ToString();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("speed").ToString()))
                                        {
                                            seData.Speed = item.GetValue("speed").ToDecimal();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("floor").ToString()))
                                        {
                                            seData.Floor = item.GetValue("floor").ToDecimal();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("id_card").ToString()))
                                        {
                                            seData.DriverCardNo = item.GetValue("id_card").ToString();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                        {
                                            seData.UpdateTime = item.GetValue("datetime").ToDateTime();
                                            seData.CreateTime = item.GetValue("datetime").ToDateTime();
                                        }
                                        await _operateLogService.AddSpecialEqpDataLog(seData);
                                    }
                                }
                                else
                                {
                                    hasdata = false;
                                    break;
                                }
                            }

                        }
                        if (successcount > 0)
                        {
                            await _aqtUploadService.UpdateCityUploadDate(url, uploadapi, now);
                        }
                        #endregion

                        //获取获取工地卸料台数据
                        #region
                        successcount = 0;
                        uploadapi = "rest/ProjectInfo/getMaterial";
                        if (jobdate.ContainsKey(uploadapi))
                        {
                            st = jobdate.GetValue(uploadapi).ToString();
                        }
                        else
                        {
                            st = now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        ts = now - starttime;
                        if (ts.Days > 0)
                        {
                            endtime = starttime;
                            endtime = endtime.AddDays(1);
                            for (int i = 0; i < ts.Days; i++)
                            {
                                page = 1;
                                hasdata = true;
                                while (hasdata)
                                {
                                    jObject = new JObject();
                                    jar = new JArray();
                                    jObject.Add("contract_record_code", code);
                                    jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                                    jObject.Add("page", page);
                                    jObject.Add("size", 100);
                                    data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                                    page++;
                                    if (string.IsNullOrEmpty(data))
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                    job = JObject.Parse(data);
                                    if (job.ContainsKey("rows"))
                                    {
                                        jar = JArray.Parse(job.GetValue("rows").ToString());
                                        if (jar.Count <= 0)
                                        {
                                            hasdata = false;
                                        }
                                        foreach (JObject item in jar)
                                        {
                                            param = new List<SugarParameter>();
                                            param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                            param.Add(new SugarParameter("@" + "SITEID", siteId));
                                            foreach (JProperty jProperty in item.Properties())
                                            {
                                                param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                            }
                                            result = await _elevatorService.doDyUnloadRtdInsert(param);
                                            successcount = successcount + result;
                                            UnloadInput seData = new UnloadInput();
                                            if (!string.IsNullOrEmpty(item.GetValue("device_id").ToString()))
                                            {
                                                seData.unload_id = item.GetValue("device_id").ToString();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("limit_weight").ToString()))
                                            {
                                                seData.alarm_weight = float.Parse(item.GetValue("limit_weight").ToString()) * 1000;
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("weight").ToString()))
                                            {
                                                seData.weight = float.Parse(item.GetValue("weight").ToString()) * 1000;
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("battery").ToString()))
                                            {
                                                seData.electric_quantity = float.Parse(item.GetValue("battery").ToString());
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                            {
                                                seData.updatetime = item.GetValue("datetime").ToDateTime();
                                            }
                                            if (!string.IsNullOrEmpty(item.GetValue("is_weight_alarm").ToString()) && item.GetValue("is_weight_alarm").ToString() == "1")
                                            {
                                                seData.upstate = 2;
                                            }
                                            await _operateLogService.AddUploadDataLog(seData);
                                        }
                                    }
                                    else
                                    {
                                        hasdata = false;
                                        break;
                                    }
                                }
                                starttime = endtime;
                                endtime = endtime.AddDays(1);
                            }
                        }
                        else
                        {
                            endtime = now;
                            jObject = new JObject();
                            jar = new JArray();
                            jObject.Add("contract_record_code", code);
                            jObject.Add("st", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
                            jObject.Add("et", endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                            jObject.Add("page", page);
                            jObject.Add("size", 100);
                            data = _dayunToken.JsonRequest(url, uploadapi, jObject);
                            if (!string.IsNullOrEmpty(data))
                            {
                                job = JObject.Parse(data);
                                if (job.ContainsKey("rows"))
                                {
                                    jar = JArray.Parse(job.GetValue("rows").ToString());
                                    foreach (JObject item in jar)
                                    {
                                        param = new List<SugarParameter>();
                                        param.Add(new SugarParameter("@" + "GROUPID", groupId));
                                        param.Add(new SugarParameter("@" + "SITEID", siteId));
                                        foreach (JProperty jProperty in item.Properties())
                                        {
                                            param.Add(new SugarParameter("@" + jProperty.Name, jProperty.Value));
                                        }
                                        result = await _elevatorService.doDyUnloadRtdInsert(param);
                                        successcount = successcount + result;
                                        UnloadInput seData = new UnloadInput();
                                        if (!string.IsNullOrEmpty(item.GetValue("device_id").ToString()))
                                        {
                                            seData.unload_id = item.GetValue("device_id").ToString();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("limit_weight").ToString()))
                                        {
                                            seData.alarm_weight = float.Parse(item.GetValue("limit_weight").ToString()) * 1000;
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("weight").ToString()))
                                        {
                                            seData.weight = float.Parse(item.GetValue("weight").ToString()) * 1000;
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("battery").ToString()))
                                        {
                                            seData.electric_quantity = float.Parse(item.GetValue("battery").ToString());
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("datetime").ToString()))
                                        {
                                            seData.updatetime = item.GetValue("datetime").ToDateTime();
                                        }
                                        if (!string.IsNullOrEmpty(item.GetValue("is_weight_alarm").ToString()) && item.GetValue("is_weight_alarm").ToString() == "1")
                                        {
                                            seData.upstate = 2;
                                        }
                                        await _operateLogService.AddUploadDataLog(seData);
                                    }
                                }
                            }
                        }
                        if (successcount > 0)
                        {
                            await _aqtUploadService.UpdateCityUploadDate(url, uploadapi, now);
                        }
                        #endregion
                    }
                    else
                    {
                        continue;
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError("数据上传异常："+ex.Message, true);
            }


            _logger.LogInformation("数据上传结束。", true);
        }
    }
}
