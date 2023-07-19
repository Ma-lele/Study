using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Services.DeviceCN;
using XHS.Build.Services.SystemSetting;
using XHS.Build.TaskJobs.Jobs;

namespace XHS.Build.TaskJobs
{
    public class BolangJob : JobBase, IJob
    {
        private readonly ILogger<BolangJob> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IDeviceCNService _deviceCNService;
        public BolangJob(ILogger<BolangJob> logger,IConfiguration configuration, IHpSystemSetting hpSystemSetting, IDeviceCNService deviceCNService)
        {
            _logger = logger;
            _configuration = configuration;
            _hpSystemSetting = hpSystemSetting;
            _deviceCNService = deviceCNService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, Convert.ToInt32(jobId)));
        }

        public async Task Run(IJobExecutionContext context, int jobid)
        {
            string filename = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S024), "bolang", "device.csv.true");
            if (!string.IsNullOrEmpty(filename) && UFile.IsExistFile(filename))
            {
                //先把文件读好
                List<string[]> list = new List<string[]>();
                using (var fs = System.IO.File.OpenRead(filename))
                using (var reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        list.Add(values);
                    }
                }
                int okCount = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        string bacode = list[i][0];
                        ///token地址
                        string urlToken = string.Format("http://122.97.130.226:8088/api/Yc/GetToken?bacode={0}&username={1}&password={2}",
                            bacode, list[i][1], list[i][2]);
                        string result = UHttp.Post(urlToken, string.Empty);
                        JObject jobject = JObject.Parse(result);
                        string AccessToken = Convert.ToString(jobject["data"]["AccessToken"]);

                        //定义header
                        WebHeaderCollection header = new WebHeaderCollection();
                        string timestamp = Convert.ToString((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
                        string nonce = DateTime.Now.ToString("mmssfffffff");//随机数
                        string jdzch = list[i][3];//项目注册号
                                                  //实时数据
                        DataRow dr = await _deviceCNService.getOneForSend(list[i][4]);
                        if (dr == null)
                            continue;
                        //拼xml
                        string data = string.Format("<sbbm>{0}</sbbm><PM25>{1}</PM25><PM10>{2}</PM10><Wd>{3}</Wd><Sd>{4}</Sd>" +
                            "<Fx>{5}</Fx><Fs>{6}</Fs><TSP>{7}</TSP><Noise>{8}</Noise><pressure>{9}</pressure>",
                            list[i][4], dr["pm2_5"], dr["pm2_5"], dr["pm10"], dr["temperature"],
                            dr["dampness"], dr["direction"], dr["speed"], dr["noise"], dr["atmos"]);

                        //upload地址
                        string urlUpload = string.Format("http://122.97.130.226:8088/api/Yc/UploadData?bacode={0}&token={1}&data={2}",
                            bacode, AccessToken, data);
                        string signature = getSignature(timestamp, nonce, jdzch, AccessToken, data);
                        header["jdzch"] = jdzch;
                        header["timestamp"] = timestamp;
                        header["nonce"] = nonce;
                        header["signature"] = signature.ToUpper();//转大写
                        result = UHttp.Post(urlUpload, string.Empty, header);
                        jobject = JObject.Parse(result);
                        if (string.IsNullOrEmpty(Convert.ToString(jobject["ErrorCode"])) && Convert.ToBoolean(jobject["result"]))
                        {
                            okCount++;
                            _logger.LogInformation(string.Format("推送了编号 {0} 的数据.", list[i][4]));
                        }
                        else
                        {
                            _logger.LogInformation(string.Format("推送编号 {0} 的数据失败. {1}{2}", list[i][4], Environment.NewLine, jobject.ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                }
            }
        }

        private static string getSignature(string timestamp, string nonce, string jdzch, string token, string data)
        {
            string str = timestamp + nonce + jdzch + token + data;      //拼一起
            char[] intArray = str.ToCharArray();                        //字符数组
            Array.Sort(intArray);                                       //排序
            str = string.Join(string.Empty, intArray);                  //再拼一起
            string result = UEncrypter.EncryptByMD5(str);               //MD5加密
            return result;
        }
    }
}
