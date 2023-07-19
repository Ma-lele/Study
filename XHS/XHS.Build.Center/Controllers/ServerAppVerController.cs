using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Center;

namespace XHS.Build.Center.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class ServerAppVerController : ControllerBase
    {
        private readonly IAppVersionService _appVersionService;
        public ServerAppVerController(IAppVersionService appVersionService)
        {
            _appVersionService = appVersionService;
        }

        /// <summary>
        /// 获取app版本信息
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="type">手机类型（ios：苹果；android：安卓）</param>
        [HttpGet]
        public async Task<IResponseOutput> GetAppVersion(string domain,string type)
        {
          
            if (string.IsNullOrEmpty(domain))
            {
                return ResponseOutput.NotOk("请传入域名", 0);
            }

            DataTable dt = await _appVersionService.GetAppVersion(domain, type);
            BnVersion bnVersion = new BnVersion();
            if (dt.Rows.Count > 0)
            {
                DataRow dataRow = dt.AsEnumerable().First<DataRow>();
                bnVersion.domain = Convert.ToString(dataRow["domain"]);
                bnVersion.welcome = Convert.ToInt16(dataRow["welcome"]);
                bnVersion.discription = Convert.ToString(dataRow["discription"]);
                bnVersion.basever = Convert.ToString(dataRow["basever"]);
                if (type == "ios")
                {                   
                    bnVersion.version = Convert.ToString(dataRow["iosver"]);
                    bnVersion.url = Convert.ToString(dataRow["iosurl"]);
                }
                else
                {
                    bnVersion.version = Convert.ToString(dataRow["androidver"]);
                    bnVersion.url = Convert.ToString(dataRow["androidurl"]);
                }
                return ResponseOutput.Ok(bnVersion);
            }
            else
            {
                return ResponseOutput.NotOk("未取到数据");
            }
        }

        /// <summary>
        /// 获取ios版本审核信息
        /// </summary>
        [HttpGet]
        public async Task<IResponseOutput> GetIosCheckVersion()
        {
            //网络文件地址
          // string file_url = @"http://env.update.xhs-sz.com/apk/app18/iossetting.json";
            //实例化唯一文件标识
         //   Uri file_uri = new Uri(file_url);
          
            //返回文件流
         //   Stream stream = WebRequest.Create(file_uri).GetResponse().GetResponseStream();
            //实例化文件内容
         //   StreamReader file_content = new StreamReader(stream);
            FileStream fileStream = new FileStream("./Setting/iossetting.json", FileMode.Open);
            StreamReader file_content = new StreamReader(fileStream);
            //读取文件内容
            string file_content_str = file_content.ReadToEnd();
           
            JObject mJObj = JObject.Parse(file_content_str);
            file_content.Close();
            fileStream.Close();
            if (mJObj != null)
            {
                return ResponseOutput.Ok(mJObj);
            }
            else
            {
                return ResponseOutput.NotOk("未取到数据");
            }
        }
    }
}
