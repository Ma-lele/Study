using System;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.AIIllegalCarAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AIIllegalCarAction
{
    public interface IAIIllegalCarService : IBaseServices<AIIllegalCarActionEntity>
    {
        Task<int> WarnInsertForIllegalCar(IllegalCarProcInputDto input);

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="date">日期</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetIllegalCarList(int SITEID, DateTime date);


        /// <summary>
        /// 非法车辆进入列表
        /// </summary>
        /// <param name="month">月份</param>
        /// <param name="keyword">车牌号</param>
        /// <param name="SiteId">项目安监号</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="pagesize">每页条数</param>
        /// <returns></returns>
        Task<DataTable> GetAiIllegalRecordListAsync(string month,string keyword, int SiteId, int pageindex,int pagesize);

        /// <summary>
        /// 非法车辆数据对比
        /// </summary>
        /// <param name="SiteId">项目编号</param>
        /// <returns></returns>
        Task<DataRow> GetAiIllegalDataCompareAsync(int SiteId);

        /// <summary>
        /// 非法车辆据统计
        /// </summary>
        /// <param name="SiteId">项目编号</param>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        Task<DataTable> GetAiIllegalDataCountAsync(int SiteId, int type);

        /// <summary>
        /// 非法车辆时段分析
        /// </summary>
        /// <param name="SiteId">项目编号</param>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        Task<DataTable> GetAiIllegalDuringAnalysisAsync(int SiteId, int type);
    }
}
