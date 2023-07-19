using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XHS.Build.Center.Auth;
using XHS.Build.Center.Jobs;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.DeviceBind;
using XHS.Build.Services.Server;
using XHS.Build.Services.TaskQz;
/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace XHS.Build.Center
{
    public class JobQuartzSite : JobBase, IJob
    {
        private readonly ITasksQzServices _tasksQzServices;
        private readonly IServerService _serverService;
        private readonly IDeviceBindService _deviceBindService;
        private readonly IMapper _mapper;
        private readonly ICache _cache;
        private readonly ILogger<JobQuartzSite> _logger;
        private readonly IConfiguration _configuration;
        private readonly NetToken _netToken;

        public JobQuartzSite(ITasksQzServices tasksQzServices, IServerService serverService, IDeviceBindService deviceBindService, IMapper mapper, ICache cache, ILogger<JobQuartzSite> logger, IConfiguration configuration, NetToken netToken)
        {
            _tasksQzServices = tasksQzServices;
            _serverService = serverService;
            _deviceBindService = deviceBindService;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
            _netToken = netToken;
        }
        public async Task Execute(IJobExecutionContext context)
        {

            //var param = context.MergedJobDataMap;
            // 可以直接获取 JobDetail 的值
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;

            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));

            // 也可以通过数据库配置，获取传递过来的参数
            //JobDataMap data = context.JobDetail.JobDataMap;
            //int jobId = data.GetInt("JobParam");

            //var model = await _tasksQzServices.QueryById(jobId);
            //if (model != null)
            //{
            //    model.RunTimes += 1;
            //    model.Remark += $"{executeLog}<br />";
            //    await _tasksQzServices.Update(model);
            //}

        }
        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("定时任务在跑的：" + DateTime.Now.ToString());
            var serverList = await _serverService.GetServerList();

            if (serverList.Any())
            {
                foreach (var server in serverList)
                {
                    try
                    {
                        DateTime dt = await _deviceBindService.GetSiteDatetime(server.domain);
                        //各个服务器认证
                        var token = _netToken.CenterToken(server.domain, server.netport.ToString());
                        //未获取到token，循环下一个服务器获取信息
                        if (string.IsNullOrEmpty(token))
                        {
                            _logger.LogDebug(server.domain + "获取Token失败");
                            continue;
                        }

                        string url = "http://" + server.domain;
                        if (server.netport > 0)
                        {
                            url += ":" + server.netport;
                        }
                        //center请求个服务器设备数据
                        var retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/Site", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });

                        if (!string.IsNullOrEmpty(retString))
                        {
                            var resObj = JsonConvert.DeserializeObject<DeviceResponse<List<SiteDeviceEntity>>>(retString);
                            if (resObj != null && resObj.Code != 0 && resObj.Code == 401)//有返回code,出错了或者未登录
                            {
                                var rtoken = _netToken.CenterRefresh(server.domain, server.netport.ToString(), token);
                                if (string.IsNullOrEmpty(rtoken))
                                {
                                    _logger.LogInformation("定时任务，请求设备信息刷新Token失败");
                                }
                                else
                                {
                                    retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/Site", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", null);
                                    if (!string.IsNullOrEmpty(retString))
                                    {
                                        resObj = JsonConvert.DeserializeObject<DeviceResponse<List<SiteDeviceEntity>>>(retString);
                                        if (resObj != null && resObj.success)
                                        {
                                            var dtolist = _mapper.Map<List<SiteDeviceTypeDto>>(resObj.data);

                                            List<DeviceBindInputDto> inputDtos = new List<DeviceBindInputDto>();
                                            dtolist.ForEach(item =>
                                            {
                                                foreach (PropertyInfo p in item.GetType().GetProperties())
                                                {
                                                    if (p.Name.ToLower() != "apiupdatedate" && p.Name.ToLower() != "operatedate")
                                                    {
                                                        inputDtos.Add(new DeviceBindInputDto() { Domain = server.domain, DeviceCode = p.GetValue(item) == null ? null : p.GetValue(item).ToString(), DeviceType = p.Name, UpdateDate = item.apiupdatedate == null ? item.operatedate : item.apiupdatedate });
                                                    }
                                                }
                                            });
                                            await _deviceBindService.AddOrEditDeviceBind(inputDtos);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("定时任务，请求设备信息刷新Token后获取数据：" + retString);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("定时任务，请求设备信息刷新Token后获取数据失败");
                                    }
                                }
                            }
                            else if (resObj != null && resObj.success)
                            {
                                var dtolist = _mapper.Map<List<SiteDeviceTypeDto>>(resObj.data);

                                List<DeviceBindInputDto> inputDtos = new List<DeviceBindInputDto>();
                                dtolist.ForEach(item =>
                                {
                                    foreach (PropertyInfo p in item.GetType().GetProperties())
                                    {
                                        if (p.Name.ToLower() != "apiupdatedate" && p.Name.ToLower() != "operatedate")
                                        {
                                            inputDtos.Add(new DeviceBindInputDto() { Domain = server.domain, DeviceCode = p.GetValue(item) == null ? null : p.GetValue(item).ToString(), DeviceType = p.Name, UpdateDate = item.apiupdatedate == null ? item.operatedate : item.apiupdatedate });
                                        }
                                    }
                                });
                                await _deviceBindService.AddOrEditDeviceBind(inputDtos);
                            }
                            else
                            {
                                _logger.LogInformation("定时任务，请求设备信息返回：" + retString);
                            }
                        }

                        continue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(server.domain + "发生错误。");
                        _logger.LogError(ex.ToString());
                        continue;
                    }

                }
            }
            if (jobid > 0)
            {
                var model = await _tasksQzServices.QueryById(jobid);
                if (model != null)
                {
                    //model.RunTimes += 1;
                    var separator = "<br>";
                    model.Remark =
                        $"【{DateTime.Now}】执行任务【Id：{context.JobDetail.Key.Name}，组别：{context.JobDetail.Key.Group}】【执行成功】{separator}";

                    await _tasksQzServices.Update(model);
                }
            }
        }

    }
}
