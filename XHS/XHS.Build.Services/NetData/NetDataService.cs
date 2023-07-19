using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Common;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.NetData
{
    public class NetDataService: INetDataService
    {
        private readonly ICache _cache;
        private readonly IHpSystemSetting _hpSystemSetting;
        public NetDataService(ICache cache, IHpSystemSetting hpSystemSetting)
        {
            _cache = cache;
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 获取晨会交底token
        /// </summary>
        /// <returns></returns>
        public string getPlateToken()
        {
            string key = CacheKey.MorningMeetingKey;
            if (_cache.Exists(key) && !string.IsNullOrEmpty(_cache.Get(key)))
            {
                return _cache.Get(key);
            }
            else
            {
                var url = _hpSystemSetting.getSettingValue(Const.Setting.S133);
                if (string.IsNullOrEmpty(url))
                {
                    return string.Empty;
                }
                var strResponse = HttpNetRequest.HttpGet(url);
                if (!string.IsNullOrEmpty(strResponse))
                {
                    var retObj = JsonConvert.DeserializeObject<dynamic>(strResponse);
                    if (retObj.status == "success")
                    {
                        _cache.Set(key, retObj.token, TimeSpan.FromMinutes(60));
                        return retObj.token;
                    }
                }
                return string.Empty;
            }
        }
    }
}
