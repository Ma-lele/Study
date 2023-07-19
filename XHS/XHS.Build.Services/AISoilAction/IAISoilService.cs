using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.AISoilAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AISoilAction
{
    public interface IAISoilService:IBaseServices<AISoilActionEntity>
    {
        /// <summary>
        /// 获取项目最新 裸土覆盖信息
        /// </summary>
        /// <param name="projid"></param>
        /// <returns></returns>
        Task<AISoilActionEntity> GetSoilLastSoilrate(string projid);

        /// <summary>
        /// 裸土覆盖 报警
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> WarnInsertForSoil(AISoilProcInputDto input);

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="date">日期</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetSoilList(int SITEID, DateTime date);

        /// <summary>
        /// 裸土覆盖数据列表
        /// </summary>
        /// <param name="month">按月份筛选</param>
        /// <param name="pageindex">分页</param>
        /// <param name="pagesize">分页</param>
        /// <returns></returns>
        Task<DataTable> GetAiSoilRecordListAsync(int SiteId,string month,int pageindex,int pagesize);

        /// <summary>
        /// 裸土覆盖数据比对
        /// </summary>
        /// <returns></returns>
        Task<DataRow> GetAiSoilDataCompareAsync(int SiteId);


        /// <summary>
        /// 裸土覆盖
        /// </summary>
        /// <param name="SiteId">编号</param>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        Task<DataTable> GetAiSoilDataCountAsync(int SiteId,int type);



        /// <summary>
        /// 裸土时段分析
        /// </summary>
        /// <param name="SiteId">编号</param>
        /// <param name="type">0:月统计;1:年统计</param>
        /// <returns></returns>
        Task<DataTable> GetAiSoilDuringAnalysisAsync(int SiteId,int type);

    }
}
