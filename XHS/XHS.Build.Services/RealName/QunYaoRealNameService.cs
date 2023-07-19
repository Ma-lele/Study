using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Common;
using Newtonsoft.Json.Linq;
using XHS.Build.Common.Util;
using XHS.Build.Model.Base;

namespace XHS.Build.Services.RealName
{
    public class QunYaoRealNameService : BaseServices<BaseEntity>, IQunYaoRealNameService
    {
        private readonly IHpSystemSetting _systemSettingService;
        public QunYaoRealNameService(IHpSystemSetting systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }
        public async Task<string> GetInAndOutByPeopleAndDay(string stTm, string itemId)
        {
            string result = string.Empty;
            Dictionary<string, string> postDict = new Dictionary<string, string>();

            try
            {
                JObject mJObj = new JObject();
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("pass", _systemSettingService.getSettingValue(Const.Setting.S142));
                param.Add("cityCode", _systemSettingService.getSettingValue(Const.Setting.S143));
                param.Add("prjNum", itemId);
                param.Add("beginTime", stTm + " 00:00:00");
                //param.Add("endTime", form["tm"] + " 23:59:59");
                string data = getRequestData("DownLoadWorker_Kqxx", param);
                string contentType = "text/xml; charset=utf-8";
                result = UHttp.Post(_systemSettingService.getSettingValue(Const.Setting.S141), data, contentType);
               

            }
            catch (Exception ex)
            {
               
            }
            return result;


        }

        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetProjList()
        {
            string result = string.Empty;
            Dictionary<string, string> postDict = new Dictionary<string, string>();

            try
            {
                JObject mJObj = new JObject();
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("pass", _systemSettingService.getSettingValue(Const.Setting.S142));
                param.Add("cityCode", _systemSettingService.getSettingValue(Const.Setting.S143));
                string data = getRequestData("DownLoadProjectInfo", param);
                string contentType = "text/xml; charset=utf-8";
                result = UHttp.Post(_systemSettingService.getSettingValue(Const.Setting.S141), data, contentType);

            }
            catch (Exception ex)
            {
                return result;
            }
            return result;


        }

        /// <summary>
        /// 获取请求内容(方法1) 适合 .Net
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        private string getRequestData(string methodName, Dictionary<string, string> param)
        {
            StringBuilder requestData = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-16\"?>")
                .Append("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body>")
                .Append("<").Append(methodName)
                .Append(" xmlns=\"http://tempuri.org/\">");
            foreach (KeyValuePair<string, string> item in param)
            {
                requestData.Append("<").Append(item.Key).Append(">")
                .Append(item.Value)
                .Append("</").Append(item.Key).Append(">");
            }
            requestData.Append("</").Append(methodName).Append(">")
            .Append("</soap:Body>")
            .Append("</soap:Envelope>");
            string val = requestData.ToString();
            return val;
        }

    }
}
