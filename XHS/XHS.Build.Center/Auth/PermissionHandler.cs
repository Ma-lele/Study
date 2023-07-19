using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Configs;

namespace XHS.Build.Center.Auth
{
    [SingleInstance]
    public class PermissionHandler : IPermissionHandler
    {
        private readonly ConfigHelper _configHelper;
        private readonly IUserKey _user;
        private readonly ICache _cache;
        public PermissionHandler(IUserKey user, ICache cache)
        {
            _configHelper = new ConfigHelper();
            _user = user;
            _cache = cache;
        }
        /// <summary>
        /// api接口权限验证
        /// </summary>
        /// <param name="api">接口路径</param>
        /// <param name="ip">请求ip</param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string api, string ip)
        {
            //待放数据库配置
            //从数据库获取 可以放缓存里获取权限配置

            //所有api
            //var listApis = _configHelper.Get<List<Apis>>("apisconfig","",true);
            //配置的key和api关系
            //var keyApis = _configHelper.Get<List<KeyApis>>("keyapiconfig","",true);

            var keyApis = await _cache.GetAsync<KeySecretList>("keysecretconfig");
            if (keyApis == null)
            {
                keyApis = _configHelper.Get<KeySecretList>("keysecretconfig", "", true);
                await _cache.SetAsync("keysecretconfig", keyApis, TimeSpan.FromHours(1));
            }
            var isValid = keyApis.Items.Where(k => k.Key == _user.Key).Any(m =>
            {
                if (m.Apis == null || m.Apis.Length == 0)
                {
                    return false;
                }
                var existApi = false;

                //先看是否直接等于包含
                if (m.Apis.Contains(api, StringComparer.OrdinalIgnoreCase))
                    return true;

                if (m.Apis.Any(_api =>
                {
                    //_api = _api.ToLower();
                    if (api.Contains(_api, StringComparison.OrdinalIgnoreCase))
                    {
                        string remain = api.Replace(_api, string.Empty, StringComparison.OrdinalIgnoreCase);
                        if (remain.Length > 0 && remain[0] == '/')
                            return true;
                    }

                    return false;
                }
                    ))
                    existApi = true;

                if (existApi)
                {
                    return m != null && m.Key == _user.Key;
                }
                else
                {
                    return false;
                }

            });
            return isValid;
        }
    }
}
