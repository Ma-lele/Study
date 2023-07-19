using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AIWash
{
    public interface IAiWashService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 车辆未冲洗列表
        /// </summary>
        /// <param name="month">月份</param>
        /// <param name="platecolor">车牌颜色</param>
        /// <param name="siteajcode">项目编号</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pagesize">每页条数</param>
        /// <returns></returns>
        Task<DataTable> GetAiWashRecordListAsync(string month, string platecolor, int SiteId, int pageindex,int pagesize);

        /// <summary>
        /// 车冲数据对比
        /// </summary>
        /// <param name="siteajcode">项目编号</param>
        /// <returns></returns>
        Task<DataRow> GetAiWashDataCompareAsync(int SiteId);
     
        /// <summary>
        /// 车冲数据统计
        /// </summary>
        /// <param name="siteajcode">项目编号</param>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        Task<DataTable> GetAiWashDataCountAsync(int SiteId, int type);


        /// <summary>
        /// 车冲时段分析
        /// </summary>
        /// <param name="siteajCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<DataTable> GetAiWashDuringAnalysisAsync(int SiteId, int type);
    }
}
