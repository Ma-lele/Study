using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Helmet
{
    public interface IHelmetService : IBaseServices<GCHelmetEntity>
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<HelmetOutputList>> GetHelmetPage(int groupid, string keyword, int page, int size);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetGroupHelmetBuildCount();

        /// <summary>
        /// 获取安全帽列表
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        Task<DataSet> GetListBySiteId(int SITEID);

        /// <summary>
        /// 安全帽未佩戴列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="month"></param>
        /// <param name="siteid"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<DataTable> GetAiHelmetRecordListAsync(string keyword,string  month,int siteid,int pageindex,int pagesize);

        /// <summary>
        /// 安全帽未佩戴数据对比
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        Task<DataRow> GetAiHelmetDataCompareAsync(int siteid);

        /// <summary>
        /// 安全帽数据统计
        /// </summary>
        /// <param name="type"></param>
        /// <param name="siteid"></param>
        /// <returns></returns>
        Task<DataTable> GetAiHelmetDataCountAsync(int type,int siteid);

        /// <summary>
        /// 安全帽未佩戴时段分析
        /// </summary>
        /// <param name="type"></param>
        /// <param name="siteid"></param>
        /// <returns></returns>
        Task<DataTable> GetAiHelmetDuringAnalysisAsync(int type,int siteid);

        /// <summary>
        /// 未佩戴安全帽位置列表
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetAiHelmentLocationListAsync(int siteid);

    }
}
