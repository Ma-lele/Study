using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AmDisclose
{
    public interface IAmDiscloseService:IBaseServices<GCAmDiscloseEntity>
    {

        /// <summary>
        /// 获取一个月的天统计
        /// </summary>
        /// <param name="projId">项目ID</param>
        /// <returns></returns>
        Task<string> GetCount(string projId);

        /// <summary>
        /// 获取某天的晨会交底数据
        /// </summary>
        /// <param name="projId">项目ID</param>
        /// <param name="date">时间格式（yyyy-MM-dd）</param>
        /// <param name="page">页数</param>
        /// <param name="limit">每页条数</param>
        /// <returns></returns>
        Task<string> GetAmDiscloseByDay(string projId, string date, int page, int limit);
    }
}
