using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;

namespace XHS.Build.Aqt365
{
    public class Test
    {
        public async void Tuisong(DataTable dt, string GONGCHENG_CODE)
        {
            if (dt == null || dt.Rows.Count <= 0)
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
                strWhere += " or recordNumber='" + item + "'";
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
                       // await _operateLogService.AddCityUploadApiLog(LogEntity);
                        if (string.IsNullOrEmpty(result))
                        {
                            errcount += errcount;
                        }
                        else
                        {
                            JObject mJObj = JObject.Parse(result);
                            int code = (int)mJObj["code"];
                            if (code == 0)
                            {
                                if (!list.Contains(url + api))
                                {
                                   // await _aqtUploadService.UpdateCityUploadDate(url, api, now);
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
                          //  await _operateLogService.AddCityUploadApiLog(LogEntity);

                        }

                    }
                    catch (Exception ex)
                    {
                   //     _logger.LogError(api + ":" + ex.Message);
                    }
                }
            }
            #endregion

        }
    }
}
