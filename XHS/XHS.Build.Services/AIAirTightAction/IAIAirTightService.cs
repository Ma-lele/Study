using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.AIAirTightAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AIAirTightAction
{
    public interface IAIAirTightService : IBaseServices<AIAirTightActionEntity>
    {
        Task<int> WarnInsertForAirTight(AirTightProcInputDto input);

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="date">日期</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetAirTightList(int SITEID, DateTime date);

        /// <summary>
        /// 车辆密闭运输记录列表
        /// </summary>
        /// <param name="month">月份</param>
        /// <param name="SiteId">项目编号</param>
        /// <param name="keyword">车牌号模糊</param>
        /// <param name="pageindex">分页</param>
        /// <param name="pagesize">分页</param>
        /// <returns></returns>
        Task<DataTable> GetAiAirtightRecordListAsync(string month,int SiteId, string keyword,int pageindex,int pagesize);

        /// <summary>
        /// 车辆密闭运输对比
        /// </summary>
        /// <param name="SiteId">项目编号</param>
        /// <returns></returns>
        Task<DataRow> GetAiAirtightDataCompareAsync(int SiteId);

        /// <summary>
        /// 车辆密闭运输数据统计
        /// </summary>
        /// <param name="SiteId">项目编号</param>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        Task<DataTable> GetAiAirtightDataCountAsync(int SiteId, int type);

        /// <summary>
        /// 车辆密闭运输时段分析
        /// </summary>
        /// <param name="SiteId">项目编号</param>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        Task<DataTable> GetAiAirtightDuringAnalysisAsync(int SiteId, int type);
    }
}
