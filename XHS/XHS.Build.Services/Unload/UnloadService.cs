using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Repository.Unload;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.Unload
{
    public class UnloadService : BaseServices<GCUnloadEntity>, IUnloadService
    {
        private readonly IUnloadRepository _unloadRepository;
        private readonly IHpSystemSetting _systemSettingService;
        private readonly ICache _cache;
        public const string UnloadToken = "UnloadToken";//Token
        private readonly IUser _user;
        private readonly IMongoDBRepository<UnloadInput> _unloadInputService;

        public UnloadService(IUnloadRepository unloadRepository, IMongoDBRepository<UnloadInput> unloadInputService, IHpSystemSetting systemSettingService, ICache cache, IUser user)
        {
            _systemSettingService = systemSettingService;
            _unloadRepository = unloadRepository;
            _unloadInputService = unloadInputService;
            BaseDal = unloadRepository;
            _cache = cache;
        }


        public async Task<int> AddRealData(UnloadInput unloadInput)
        {

            int ret = await _unloadRepository.UpdateRtdData(unloadInput.unload_id, JsonConvert.SerializeObject(unloadInput));
            if (ret <= 0) return -27;

            await _unloadInputService.InsertAsync(unloadInput);
            if (unloadInput.upstate == 2 || unloadInput.upstate == 3)
            {
                if (unloadInput.upstate == 3 && unloadInput.weight >= unloadInput.early_warning_weight)
                {
                    unloadInput.upstate = 4;
                }
                doWarn(new SugarParameter("@unloadid", unloadInput.unload_id), new SugarParameter("@upstate", unloadInput.upstate), new SugarParameter("@weight", unloadInput.weight), new SugarParameter("@bias", unloadInput.bias), new SugarParameter("@electric", unloadInput.electric_quantity), new SugarParameter("@paramjson", JsonConvert.SerializeObject(unloadInput)));
            }
            return ret;
        }


        public async Task<DataTable> getListForSite(int SITEID)
        {
            return await _unloadRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUnloadListForSite", new { SITEID = SITEID });
        }

        /// <summary>
        /// 报警
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doWarn(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _unloadRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnInsertForUnload", ps);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// 获取卸料平台列表
        /// </summary>
        /// <returns>数据集</returns>
        public async Task<PageOutput<GCUnloadPageListOutput>> GetSiteUnloadPageList(int groupid, string keyword, int page, int size)
        {
            return await _unloadRepository.GetSiteUnloadPageList(groupid, keyword, page, size);
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _unloadRepository.GetGroupCount();
        }


        /// <summary>
        /// 获取获取实时值
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <returns>实时值数据集</returns>
        public async Task<string> GetListRealUnloadData(string unloadid)
        {
            var userName = _systemSettingService.getSettingValue(Const.Setting.S125);
            var url = _systemSettingService.getSettingValue(Const.Setting.S124);
            var YKToken = "";
            if (_cache.Exists(UnloadToken) && !string.IsNullOrEmpty(_cache.Get(UnloadToken)))
            {
                YKToken = _cache.Get(UnloadToken);
            }
            if (string.IsNullOrEmpty(YKToken))
            {
                YKToken = GetToken();
            }

            string param = "userName=" + userName + "&token=" + YKToken + "&listUnloadId=" + unloadid;
            var retString = UHttp.Post(url + "UnloadingPlatform/v2/listRealUnloadData.action", param);

            JObject mJObj = new JObject();
            mJObj = JObject.Parse(retString);
            if (mJObj["status"].ToInt() < 0)
            {
                YKToken = GetToken();
                retString = UHttp.Post(url + "UnloadingPlatform/v2/listRealUnloadData.action", param);

            }
            return retString;
        }

        public string GetToken()
        {
            string token = "";
            var userName = _systemSettingService.getSettingValue(Const.Setting.S125);
            var password = _systemSettingService.getSettingValue(Const.Setting.S126);
            var url = _systemSettingService.getSettingValue(Const.Setting.S124);

            string param = "userName=" + userName + "&password=" + password;
            var retString = UHttp.Post(url + "SmartSite/v2/getToken.action", param);

            JObject mJObj = new JObject();
            mJObj = JObject.Parse(retString);
            if (mJObj["status"].ToInt() > 0)
            {
                JObject mJdata = (JObject)mJObj["data"];
                token = mJdata["token"].ToString();
                _cache.Set(UnloadToken, token);
            }
            return token;
        }
        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="startTime">开始时间, yyyyMMddHHmmss格式</param>
        /// <param name="endTime">结束时间, yyyyMMddHHmmss格式</param>
        /// <returns>数据集</returns>
        public async Task<string> GetListUnloadDataScheduleCurrent(string unloadid, string startTime, string endTime)
        {
            var userName = _systemSettingService.getSettingValue(Const.Setting.S125);
            var url = _systemSettingService.getSettingValue(Const.Setting.S124);
            var YKToken = "";
            if (_cache.Exists(UnloadToken) && !string.IsNullOrEmpty(_cache.Get(UnloadToken)))
            {
                YKToken = _cache.Get(UnloadToken);
            }
            if (string.IsNullOrEmpty(YKToken))
            {
                YKToken = GetToken();
            }
            string param = "userName=" + userName + "&token=" + YKToken + "&unload_id=" + unloadid + "&startTime=" + startTime + "&endTime=" + endTime;
            var retString = UHttp.Post(url + "UnloadingPlatform/v2/listUnloadDataScheduleCurrent.action", param);

            JObject mJObj = new JObject();
            mJObj = JObject.Parse(retString);
            if (mJObj["status"].ToInt() < 0)
            {
                YKToken = GetToken();
                retString = UHttp.Post(url + "UnloadingPlatform/v2/listUnloadDataScheduleCurrent.action", param);

            }
            return retString;
        }

        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="startTime">开始时间, yyyyMMddHHmmss格式</param>
        /// <param name="endTime">结束时间, yyyyMMddHHmmss格式</param>
        /// <returns>数据集</returns>
        public async Task<List<UnloadInput>> GetListUnloadDataCurrent(string unloadid, DateTime startTime, DateTime endTime)
        {
            FilterDefinition<UnloadInput> filter = Builders<UnloadInput>.Filter.Empty;
            filter = filter & Builders<UnloadInput>.Filter.Eq(a => a.unload_id, unloadid);
            filter = filter & Builders<UnloadInput>.Filter.Gte(a => a.createtime, startTime);
            filter = filter & Builders<UnloadInput>.Filter.Lt(a => a.createtime, endTime);
            SortDefinition<UnloadInput> sort = Builders<UnloadInput>.Sort.Descending(a => a.createtime);
            List <UnloadInput> retString = (List<UnloadInput>)_unloadInputService.FindByFilterDefinition(filter, sort);
            return retString;
        }

        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="updatetime">时间</param>
        /// <returns>数据集</returns>
        public async Task<List<GCUnloadEntity>> GetDistinctUnloadList(DateTime updatetime)
        {
            return await _unloadRepository.Db.Queryable<GCUnloadEntity>().Where(a => SqlFunc.Subqueryable<GCUnloadEntity>().Where(b => b.operatedate >= updatetime).Any()).ToListAsync();
        }

        public async Task<DataTable> spV2UnloadStats(int SITEID)
        {
            return await _unloadRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2UnloadStats",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2UnloadSelect(int SITEID)
        {
            return await _unloadRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2UnloadSelect",
               new
               {
                   SITEID = SITEID
               });
        }

        public List<UnloadInput> UnloadHistory(string unloadid, DateTime searchDate, int pageIndex, int pageSize,ref long total)
        {
            FilterDefinition<UnloadInput> filter = Builders<UnloadInput>.Filter.Empty;
            filter = filter & Builders<UnloadInput>.Filter.Eq(a => a.unload_id, unloadid);
            filter = filter & Builders<UnloadInput>.Filter.Gte(a => a.createtime, searchDate);
            filter = filter & Builders<UnloadInput>.Filter.Lt(a => a.createtime, searchDate.AddDays(1));
            SortDefinition<UnloadInput> sort = Builders<UnloadInput>.Sort.Descending(a => a.createtime);
            List<UnloadInput> retString = (List<UnloadInput>)_unloadInputService.FindByFilterWithPageTotal(filter, pageIndex, ref total, pageSize, sort);
            return retString;
        }

        public List<UnloadInput> UnloadTrend(string unloadid, DateTime startDate, DateTime endDate)
        {
            FilterDefinition<UnloadInput> filter = Builders<UnloadInput>.Filter.Empty;
            filter = filter & Builders<UnloadInput>.Filter.Eq(a => a.unload_id, unloadid);
            filter = filter & Builders<UnloadInput>.Filter.Gte(a => a.createtime, startDate);
            filter = filter & Builders<UnloadInput>.Filter.Lt(a => a.createtime, endDate.AddDays(1));
            SortDefinition<UnloadInput> sort = Builders<UnloadInput>.Sort.Descending(a => a.createtime);

            List<UnloadInput> retString = (List<UnloadInput>)_unloadInputService.FindByFilterDefinition(filter, sort);
            return retString;
        }
    }
}
