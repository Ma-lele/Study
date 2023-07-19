using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace XHS.Build.Common.Extensions
{
    public static class RateLimitServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Ip限流
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="cacheConfig"></param>
        public static void AddIpRateLimit(this IServiceCollection services, IConfiguration configuration)
        {
            #region IP限流
            var config = configuration.GetSection("IpRateLimiting");
            var options = config.Get<IpRateLimitOptions>();
            //将域名转化成IP，配置限流
            services.Configure<IpRateLimitOptions>(
                cfg =>
                {
                    cfg.IpWhitelist = Doamins2IPs(options.IpWhitelist);
                });
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            services.AddMemoryCache();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion
        }

        /// <summary>
        /// 域名转IP
        /// </summary>
        /// <param name="ips"></param>
        /// <returns></returns>
        private static List<string> Doamins2IPs(List<string> ips)
        {
            List<string> result = new List<string>();
            foreach (var item in ips)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                try
                {
                    IPAddress[] IPs = Dns.GetHostAddresses(item);
                    var ip = IPs[0]?.ToString();
                    if (!result.Contains(ip))
                        result.Add(ip);
                }
                catch (System.Exception ex)
                {

                }
            }
            return result;
        }
    }
}
