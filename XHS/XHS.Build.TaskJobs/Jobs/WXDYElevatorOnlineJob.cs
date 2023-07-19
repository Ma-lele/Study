using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.Employee;
using XHS.Build.Services.File;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class WXDYElevatorOnlineJob : JobBase, IJob
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WXDYElevatorOnlineJob> _logger;
        private readonly ISpecialEqpService _specialEqpService;
        private readonly IEmployeeService _employeeService;
        private readonly IFileService _fileService;
        private const string TIME_FORMAT = "yyyy-MM-dd HH:mm:ss ";
        private readonly IEmployeeCareerService _employeeCareerService;
        public WXDYElevatorOnlineJob(IConfiguration configuration, ILogger<WXDYElevatorOnlineJob> logger, ISpecialEqpService specialEqpService, IEmployeeService employeeService, IFileService fileService, IEmployeeCareerService employeeCareerService)
        {
            _configuration = configuration;
            _logger = logger;
            _specialEqpService = specialEqpService;
            _employeeService = employeeService;
            _fileService = fileService;
            _employeeCareerService = employeeCareerService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            string SUCCESS = "0000";
            string APPID = "sysfdas2fvdasf33dag";
            string ACCESS_TOKEN = await _specialEqpService.GetWXDYToken(APPID);
            if (string.IsNullOrEmpty(ACCESS_TOKEN))
            {
                return;
            }
            var ElevatorDt = await _specialEqpService.GetWXDYSpecialList("2","");
            if (ElevatorDt != null && ElevatorDt.Rows.Count > 0)
            {
                SortedDictionary<string, string> towerParam = null;
                StringBuilder sb = null;
                for (var i = 0; i < ElevatorDt.Rows.Count; i++)
                {
                    string run_status = "1";
                    var Rtd = await _specialEqpService.GetLastOneMinFirst(Convert.ToString(ElevatorDt.Rows[i]["secode"]));
                    if (Rtd == null)//最近的一分钟内未找到实时数据，则是已下线
                    {
                        run_status = "1";
                        Rtd = await _specialEqpService.GetLastOneFirst(Convert.ToString(ElevatorDt.Rows[i]["secode"]));
                        if (Rtd == null)
                        {
                            _logger.LogInformation("升降机" + Convert.ToString(ElevatorDt.Rows[i]["secode"]) + "设备未获取到实时数据");
                            continue;
                        }
                    }
                    else
                    {
                        run_status = "0";//在线的
                    }
                    //if (string.IsNullOrEmpty(Rtd.sedata))//必填项
                    //{
                    //    continue;
                    //}
                    //var sedata = JsonConvert.DeserializeObject<ElevatorRealDataInput>(HttpUtility.HtmlDecode(Rtd.sedata));
                    //if (string.IsNullOrEmpty(sedata.DriverCardNo))
                    //{
                    //    _logger.LogInformation("升降机" + Rtd.secode + "设备获取到人员身份证为空");
                    //    continue;
                    //}
                    if (ElevatorDt.Rows[i].IsNull("ID") || string.IsNullOrEmpty(ElevatorDt.Rows[i]["ID"].ToString()))
                    {
                        continue;
                    }
                    //员工信息
                    var employee = await _employeeService.QueryById(ElevatorDt.Rows[i]["ID"]);
                    if (employee == null)
                    {
                        _logger.LogInformation("升降机" + Rtd.secode + "设备" + ElevatorDt.Rows[i]["ID"] + "未获取到员工信息");
                        continue;
                    }
                    var Files = await _fileService.GetFileListByLindId(employee.ID);
                    if (!Files.Any())
                    {
                        _logger.LogInformation("升降机" + Rtd.secode + "设备" + ElevatorDt.Rows[i]["ID"] + "未获取到员工照片");
                        continue;
                    }
                    var Careers = await _employeeCareerService.Query(c => c.Papertype == "1");
                    if (!Careers.Any())
                    {
                        _logger.LogInformation("升降机" + Rtd.secode + "设备" + ElevatorDt.Rows[i]["ID"] + "未获取到证书信息");
                        continue;
                    }
                    towerParam = new SortedDictionary<string, string>();
                    sb = new StringBuilder(string.Empty);
                    towerParam["prj_id"] = Convert.ToString(ElevatorDt.Rows[i]["prj_id"]);
                    towerParam["prj_name"] = Convert.ToString(ElevatorDt.Rows[i]["prj_name"]);
                    towerParam["owner_name"] = Convert.ToString(ElevatorDt.Rows[i]["owner_name"]);
                    towerParam["device_type"] = "升降机设备";//Convert.ToString(ElevatorDt.Rows[i]["device_type"]);
                    towerParam["device_id"] = Convert.ToString(ElevatorDt.Rows[i]["secode"]);
                    towerParam["contract_record_code"] = Convert.ToString(ElevatorDt.Rows[i]["contract_record_code"]);
                    towerParam["id_card"] = ElevatorDt.Rows[i]["ID"].ToString();
                    towerParam["run_status"] = run_status;
                    towerParam["photo"] = ImgHelper.ImageToBase64(_fileService.GetImageUrl(Files[0]));
                    towerParam["name"] = employee.RealName;
                    towerParam["certificate_no"] = Careers[0].Papercode;
                    towerParam["certificate_datetime"] = Convert.ToDateTime(Careers[0].Enddate).ToString("yyyy-MM-dd HH:mm:ss");
                    towerParam["datetime"] = Rtd.updatedate.ToString(TIME_FORMAT);

                    foreach (var item in towerParam)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(item.Value)))
                        {
                            sb.Append(item.Value);
                        }
                    }
                    sb.Append(ACCESS_TOKEN);
                    string sign = UEncrypter.SHA256(sb.ToString());

                    string towerUrl = string.Format("http://{0}/rest/Elevator/addWorkData/{1}/{2}", _configuration.GetSection("WXDY").GetValue<string>("PushUrl"), APPID, sign);
                    string response = UHttp.Post(towerUrl, JsonConvert.SerializeObject(towerParam), UHttp.CONTENT_TYPE_JSON);
                    if (string.IsNullOrEmpty(response))
                    {
                        _logger.LogError("升降机设备人员信息：" + Rtd.secode + "URL:" + towerUrl + "请求远程错误");
                        continue;
                    }
                    JObject jo = JObject.Parse(response);
                    if (Convert.ToString(jo["flag"]) != SUCCESS)
                    {
                        _logger.LogError("升降机设备人员信息：" + towerParam["device_id"] + "URL:" + towerUrl + "请求返回：" + response);
                    }
                    else
                    {
                        _logger.LogInformation("升降机" + towerParam["device_id"] + "设备人员信息推送成功");
                    }
                }
            }
        }
    }
}
