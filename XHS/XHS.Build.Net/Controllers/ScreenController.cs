using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Net.Attributes;
using XHS.Build.Services.Screen;

namespace XHS.Build.Net.Controllers
{
    /// <summary>
    /// 大屏
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class ScreenController : ControllerBase
    {
        private readonly IScreenService _screenService;
        public ScreenController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        /// <summary>
        /// 获取公告
        /// </summary>
        /// <param name="screencode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetNotice(string screencode)
        {
            if (string.IsNullOrEmpty(screencode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            try
            {
                DataRow dr = await _screenService.getNoticeBycode(screencode);
                if(dr==null || dr.Table == null)
                {
                    return ResponseOutput.Ok();
                }
                StringBuilder theResult = new StringBuilder();
                bool isFirst = true;
                DataTable dt = dr.Table;
                string theName = null;
                string theText = null;
                theResult.Append("{");
                foreach (DataColumn dc in dt.Columns)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        theResult.Append(",");
                    theName = dc.ColumnName.Trim().ToLower();
                    theText = dr[dc.ColumnName].ToString();
                    theResult.Append(" \""+theName+"\":\""+theText+"\"");
                }
                theResult.Append("}");

                string json = theResult.ToString();// JsonConvert.SerializeObject(dr.Table.Rows[0]);// JsonTransfer.dataRow2Json(dr);
                JObject jo = JObject.Parse(json);

                json = HttpUtility.HtmlDecode(Convert.ToString(dr["jsonparam"]));
                if (!string.IsNullOrEmpty(json))
                {
                    JObject jsonparam = JObject.Parse(json);
                    jo["projectctrllevel"] = jsonparam["projectctrllevel"];
                    jo["projectopenstate"] = jsonparam["projectopenstate"];
                }
                jo.Remove("jsonparam");
                return ResponseOutput.Ok(jo);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("获取数据错误");
            }
        }

        /// <summary>
        /// 同步公告
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SyncNotice(ScreenSyncNoticeInput json)
        {
            if (json==null || string.IsNullOrEmpty(json.screencode))
            {
                return ResponseOutput.NotOk("参数错误", 0);
            }

            try
            {
             

                var resp = JsonConvert.SerializeObject(json);
                JObject jo = (JObject)JsonConvert.DeserializeObject(resp);// new JObject(JsonConvert.SerializeObject(json));
                jo.Remove("screencode");
                jo.Remove("defaultnotice");
                SgParams sp = new SgParams();
                sp.Add("screencode", json.screencode);
                sp.Add("defaultnotice", json.defaultnotice);
                sp.Add("jsonparam", JsonConvert.SerializeObject(jo));

                DataRow dr = await _screenService.syncNotice(sp);

                var dynamicTable = dr.Table.ToDynamicList();
                return ResponseOutput.Ok(dynamicTable.FirstOrDefault());
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk("获取数据错误");
            }
        }
    }
}
