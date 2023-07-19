using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Center.Auth;
using XHS.Build.Center.Jobs;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.DeviceBind;
using XHS.Build.Services.Server;
using XHS.Build.Services.TaskQz;
/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace XHS.Build.Center
{
    public class JobQuartzSpecial : JobBase, IJob
    {
        private readonly ITasksQzServices _tasksQzServices;
        private readonly IServerService _serverService;
        private readonly IDeviceBindService _deviceBindService;
        private readonly IMapper _mapper;
        private readonly ICache _cache;
        private readonly ILogger<JobQuartzSpecial> _logger;
        private readonly IConfiguration _configuration;
        private readonly NetToken _netToken;
        public JobQuartzSpecial(ITasksQzServices tasksQzServices, IServerService serverService, IDeviceBindService deviceBindService, IMapper mapper, ICache cache, ILogger<JobQuartzSpecial> logger, IConfiguration configuration, NetToken netToken)
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

        }
        public async Task Run(IJobExecutionContext context, int jobid)
        {
            _logger.LogInformation("Special定时任务在跑的：" + DateTime.Now.ToString());
            var serverList = await _serverService.GetServerList();

            if (serverList.Any())
            {
                foreach (var server in serverList)
                {
                    try
                    {
                        DateTime dt = await _deviceBindService.GetBindDatetime(server.domain, "special");
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
                        var retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/Special", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });

                        if (!string.IsNullOrEmpty(retString))
                        {
                            var resObj = JsonConvert.DeserializeObject<DeviceResponse<List<SpecialEntity>>>(retString);
                            if (resObj != null && resObj.Code != 0 && resObj.Code == 401)//有返回code,出错了或者未登录
                            {
                                var rtoken = _netToken.CenterRefresh(server.domain, server.netport.ToString(), token);
                                if (string.IsNullOrEmpty(rtoken))
                                {
                                    _logger.LogInformation("定时任务，请求特种设备信息刷新Token失败");
                                }
                                else
                                {
                                    retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/Special", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + rtoken } });
                                    if (!string.IsNullOrEmpty(retString))
                                    {
                                        resObj = JsonConvert.DeserializeObject<DeviceResponse<List<SpecialEntity>>>(retString);
                                        if (resObj != null && resObj.success)
                                        {
                                            var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                            dtolist.ForEach(item =>
                                            {
                                                item.Domain = server.domain;
                                                item.DeviceType = "special";
                                            });
                                            await _deviceBindService.AddOrEditDeviceBind(dtolist);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("定时任务，请求特种设备信息刷新Token后获取数据：" + retString);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("定时任务，请求特种设备信息刷新Token后获取数据失败");
                                    }
                                }
                            }
                            else if (resObj != null && resObj.success)
                            {
                                var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                dtolist.ForEach(item =>
                                {
                                    item.Domain = server.domain;
                                    item.DeviceType = "special";
                                });
                                await _deviceBindService.AddOrEditDeviceBind(dtolist);
                            }
                            else
                            {
                                _logger.LogInformation("定时任务，请求特种设备信息返回：" + retString);
                            }
                        }

                        //卸料平台
                        //center请求个服务器设备数据
                        dt = await _deviceBindService.GetBindDatetime(server.domain, "unload");
                        retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/Unload", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });

                        if (!string.IsNullOrEmpty(retString))
                        {
                            var resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCUnloadEntity>>>(retString);
                            if (resObj != null && resObj.Code != 0 && resObj.Code == 401)//有返回code,出错了或者未登录
                            {
                                var rtoken = _netToken.CenterRefresh(server.domain, server.netport.ToString(), token);
                                if (string.IsNullOrEmpty(rtoken))
                                {
                                    _logger.LogInformation("定时任务，请求卸料平台设备信息刷新Token失败");
                                }
                                else
                                {
                                    retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/Unload", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + rtoken } });
                                    if (!string.IsNullOrEmpty(retString))
                                    {
                                        resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCUnloadEntity>>>(retString);
                                        if (resObj != null && resObj.success)
                                        {
                                            var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                            dtolist.ForEach(item =>
                                            {
                                                item.Domain = server.domain;
                                                item.DeviceType = "unload";
                                            });
                                            await _deviceBindService.AddOrEditDeviceBind(dtolist);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("定时任务，请求卸料平台设备信息刷新Token后获取数据：" + retString);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("定时任务，请求卸料平台设备信息刷新Token后获取数据失败");
                                    }
                                }
                            }
                            else if (resObj != null && resObj.success)
                            {
                                var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                dtolist.ForEach(item =>
                                {
                                    item.Domain = server.domain;
                                    item.DeviceType = "unload";
                                });
                                await _deviceBindService.AddOrEditDeviceBind(dtolist);
                            }
                            else
                            {
                                _logger.LogInformation("定时任务，请求卸料平台设备信息返回：" + retString);
                            }
                        }

                        //深基坑
                        //center请求个服务器设备数据
                        dt = await _deviceBindService.GetBindDatetime(server.domain, "deeppit");
                        retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/DeepPit", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });

                        if (!string.IsNullOrEmpty(retString))
                        {
                            var resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCDeepPit>>>(retString);
                            if (resObj != null && resObj.Code != 0 && resObj.Code == 401)//有返回code,出错了或者未登录
                            {
                                var rtoken = _netToken.CenterRefresh(server.domain, server.netport.ToString(), token);
                                if (string.IsNullOrEmpty(rtoken))
                                {
                                    _logger.LogInformation("定时任务，请求深基坑信息刷新Token失败");
                                }
                                else
                                {
                                    retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/DeepPit", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + rtoken } });
                                    if (!string.IsNullOrEmpty(retString))
                                    {
                                        resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCDeepPit>>>(retString);
                                        if (resObj != null && resObj.success)
                                        {
                                            var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                            dtolist.ForEach(item =>
                                            {
                                                item.Domain = server.domain;
                                                item.DeviceType = "deeppit";
                                            });
                                            await _deviceBindService.AddOrEditDeviceBind(dtolist);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("定时任务，请求深基坑信息刷新Token后获取数据：" + retString);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("定时任务，请求深基坑信息刷新Token后获取数据失败");
                                    }
                                }
                            }
                            else if (resObj != null && resObj.success)
                            {
                                var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                dtolist.ForEach(item =>
                                {
                                    item.Domain = server.domain;
                                    item.DeviceType = "deeppit";
                                });
                                await _deviceBindService.AddOrEditDeviceBind(dtolist);
                            }
                            else
                            {
                                _logger.LogInformation("定时任务，请求深基坑信息返回：" + retString);
                            }
                        }

                        //高支模
                        //center请求个服务器设备数据
                        dt = await _deviceBindService.GetBindDatetime(server.domain, "highformwork");
                        retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/HighFormwork", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });

                        if (!string.IsNullOrEmpty(retString))
                        {
                            var resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCHighFormwork>>>(retString);
                            if (resObj != null && resObj.Code != 0 && resObj.Code == 401)//有返回code,出错了或者未登录
                            {
                                var rtoken = _netToken.CenterRefresh(server.domain, server.netport.ToString(), token);
                                if (string.IsNullOrEmpty(rtoken))
                                {
                                    _logger.LogInformation("定时任务，请求深基坑信息刷新Token失败");
                                }
                                else
                                {
                                    retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/HighFormwork", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + rtoken } });
                                    if (!string.IsNullOrEmpty(retString))
                                    {
                                        resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCHighFormwork>>>(retString);
                                        if (resObj != null && resObj.success)
                                        {
                                            var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                            dtolist.ForEach(item =>
                                            {
                                                item.Domain = server.domain;
                                                item.DeviceType = "highformwork";
                                            });
                                            await _deviceBindService.AddOrEditDeviceBind(dtolist);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("定时任务，请求深基坑信息刷新Token后获取数据：" + retString);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("定时任务，请求深基坑信息刷新Token后获取数据失败");
                                    }
                                }
                            }
                            else if (resObj != null && resObj.success)
                            {
                                var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                dtolist.ForEach(item =>
                                {
                                    item.Domain = server.domain;
                                    item.DeviceType = "highformwork";
                                });
                                await _deviceBindService.AddOrEditDeviceBind(dtolist);
                            }
                            else
                            {
                                _logger.LogInformation("定时任务，请求深基坑信息返回：" + retString);
                            }
                        }

                        //智慧电表
                        //center请求个服务器设备数据
                        dt = await _deviceBindService.GetBindDatetime(server.domain, "elecmeter");
                        retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/ElecMeter", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + token } });

                        if (!string.IsNullOrEmpty(retString))
                        {
                            var resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCElecMeterEntity>>>(retString);
                            if (resObj != null && resObj.Code != 0 && resObj.Code == 401)//有返回code,出错了或者未登录
                            {
                                var rtoken = _netToken.CenterRefresh(server.domain, server.netport.ToString(), token);
                                if (string.IsNullOrEmpty(rtoken))
                                {
                                    _logger.LogInformation("定时任务，请求智慧电表信息刷新Token失败");
                                }
                                else
                                {
                                    retString = HttpNetRequest.SendRequest(url + "/api/DeviceBind/ElecMeter", new Dictionary<string, object>() { { "operatedate", dt.ToString("yyyy-MM-dd hh:mm:ss") } }, "GET", new Dictionary<string, string>() { { "Authorization", "Bearer " + rtoken } });
                                    if (!string.IsNullOrEmpty(retString))
                                    {
                                        resObj = JsonConvert.DeserializeObject<DeviceResponse<List<GCElecMeterEntity>>>(retString);
                                        if (resObj != null && resObj.success)
                                        {
                                            var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                            dtolist.ForEach(item =>
                                            {
                                                item.Domain = server.domain;
                                                item.DeviceType = "elecmeter";
                                            });
                                            await _deviceBindService.AddOrEditDeviceBind(dtolist);
                                        }
                                        else
                                        {
                                            _logger.LogInformation("定时任务，请求智慧电表信息刷新Token后获取数据：" + retString);
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation("定时任务，请求智慧电表信息刷新Token后获取数据失败");
                                    }
                                }
                            }
                            else if (resObj != null && resObj.success)
                            {
                                var dtolist = _mapper.Map<List<DeviceBindInputDto>>(resObj.data);
                                dtolist.ForEach(item =>
                                {
                                    item.Domain = server.domain;
                                    item.DeviceType = "elecmeter";
                                });
                                await _deviceBindService.AddOrEditDeviceBind(dtolist);
                            }
                            else
                            {
                                _logger.LogInformation("定时任务，请求智慧电表信息返回：" + retString);
                            }
                        }
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
