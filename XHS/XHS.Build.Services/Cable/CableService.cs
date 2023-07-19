using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.Cable
{
    public class CableService : BaseServices<GCCableEntity>, ICableService
    {
        private readonly IBaseRepository<GCCableEntity> _cableRepository;
        private readonly IHpSystemSetting _systemSettingService;
        private readonly ICache _cache;
        private string CableApiTokenUrl;
        private string CableApiRealUrl;
        private string CableApiDamageUrl;
        private string CableApiBrokenUrl;
        private string CableApiStatusUrl;

        public CableService(IBaseRepository<GCCableEntity> cableRepository, IHpSystemSetting systemSettingService, ICache cache)
        {
            _systemSettingService = systemSettingService;
            _cableRepository = cableRepository;
            BaseDal = cableRepository;
            _cache = cache;
            CableApiTokenUrl = systemSettingService.getSettingValue(Const.Setting.S121) + "token";
            CableApiRealUrl = systemSettingService.getSettingValue(Const.Setting.S121) + "maxDamage";
            CableApiDamageUrl = systemSettingService.getSettingValue(Const.Setting.S121) + "allDamage";
            CableApiBrokenUrl = systemSettingService.getSettingValue(Const.Setting.S121) + "allBrokenWires";
            CableApiStatusUrl = systemSettingService.getSettingValue(Const.Setting.S121) + "sensorStatus";
        }

        /// <summary>
        /// 获取钢丝绳列表
        /// </summary>
        /// <returns>数据集</returns>
        public DataSet GetList()
        {
            return _cableRepository.Db.Ado.UseStoredProcedure().GetDataSetAll("spCableList");
        }

        /// <summary>
        /// 评估风险等级
        /// </summary>
        /// <param name="sensorid">设备ID</param>
        /// <param name="risklevel">风险等级</param>
        /// <returns></returns>
        public int DoRiskLevelCheck(string sensorid, int risklevel)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            _cableRepository.Db.Ado.UseStoredProcedure().ExecuteCommand("spCableRiskLevelCheck", new SugarParameter("@sensorid", sensorid), new SugarParameter("@risklevel", risklevel), output);
            return output.Value.ObjToInt();
        }

        public async Task<DataTable> GetListForSite(int SITEID)
        {
            return await _cableRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCableListForSite", new { SITEID = SITEID });
        }



        /// <summary>
        /// 获取最大损伤数据
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>实时值数据集</returns>
        public  Task<string> GetMaxDamage(string sensorid)
        {
            string result = "";

            string token = getApiToken();

            JObject jParam = new JObject();
            jParam.Add("SensorId", new JArray(sensorid.Split(',')));

            WebHeaderCollection wheader = new WebHeaderCollection();
            wheader["Authorization"] = "bearer " + token;
            wheader["Content-Type"] = UHttp.CONTENT_TYPE_JSON;
            result = UHttp.Post(CableApiRealUrl, jParam.ToString(), UHttp.CONTENT_TYPE_JSON, wheader);


            // JArray jArray = JArray.Parse(result);
            return Task.FromResult(result);
        }

        /// <summary>
        /// 获取完整的损伤数据
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>数据集</returns>
        public Task<string> GetAllDamage(string sensorid)
        {
            string token = getApiToken();

            JObject jParam = new JObject();
            jParam.Add("SensorId", new JArray(sensorid.Split(',')));

            WebHeaderCollection wheader = new WebHeaderCollection();
            wheader["Authorization"] = "bearer " + token;
            wheader["Content-Type"] = UHttp.CONTENT_TYPE_JSON;
            string result = UHttp.Post(CableApiDamageUrl, jParam.ToString(), UHttp.CONTENT_TYPE_JSON, wheader);

            return Task.FromResult(result);
        }

        /// <summary>
        /// 获取完整断丝预测
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>数据集</returns>
        public Task<string> GetBroken(string sensorid)
        {
            string token = getApiToken();

            JObject jParam = new JObject();
            jParam.Add("SensorId", new JArray(sensorid.Split(',')));

            WebHeaderCollection wheader = new WebHeaderCollection();
            wheader["Authorization"] = "bearer " + token;
            wheader["Content-Type"] = UHttp.CONTENT_TYPE_JSON;
            string result = UHttp.Post(CableApiBrokenUrl, jParam.ToString(), UHttp.CONTENT_TYPE_JSON, wheader);

            return Task.FromResult(result);
        }

        private string getApiToken()
        {
            string result = "";
            string token = "";

            try
            {
                JObject jObj = new JObject();
                Dictionary<string, string> postDict = new Dictionary<string, string>();
                postDict.Add("grant_type", "password");
                postDict.Add("username", _systemSettingService.getSettingValue(Const.Setting.S122));
                postDict.Add("password", _systemSettingService.getSettingValue(Const.Setting.S123));
                result = UHttp.Post(CableApiTokenUrl, quoteParas(postDict), UHttp.CONTENT_TYPE_FORM);

                jObj = new JObject();
                jObj = JObject.Parse(result);
                token = jObj["access_token"].ToString();

            }
            catch (Exception ex)
            {

            }

            return token;
        }

        private string quoteParas(Dictionary<string, string> paras)
        {
            string quotedParas = "";
            bool isFirst = true;
            string val = "";
            foreach (string para in paras.Keys)
            {
                if (paras.TryGetValue(para, out val))
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        quotedParas += para + "=" + HttpUtility.UrlPathEncode(val);
                    }
                    else
                    {
                        quotedParas += "&" + para + "=" + HttpUtility.UrlPathEncode(val);
                    }
                }
                else
                {
                    break;
                }
            }

            return quotedParas;
        }

        public async Task<PageOutput<GCCablePageListOutput>> GetSiteCablePageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _cableRepository.Db.Queryable<GCCableEntity, GCSiteEntity>((u, s) => new JoinQueryInfos(JoinType.Inner, u.SITEID == s.SITEID))
                .WhereIF(groupid > 0, (u, s) => u.GROUPID == groupid && s.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (u, s) => s.sitename.Contains(keyword) || s.siteshortname.Contains(keyword))
                .OrderBy(" CSID desc")
            .Select<GCCablePageListOutput>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCCablePageListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _cableRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,isnull(A.count,0) as count from T_GC_Group G LEFT JOIN (SELECT D.GROUPID, count(1) as count from T_GC_Cable D GROUP BY D.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0 ORDER BY G.city, G.district, G.GROUPID");
        }
    }
}
