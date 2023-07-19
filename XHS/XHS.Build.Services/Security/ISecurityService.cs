using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Security
{
   public  interface ISecurityService : IBaseServices<GCSecurityEntity>
    {
        /// <summary>
        /// 巡更站点
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="order"></param>
        /// <param name="ordertype"></param>
        /// <returns></returns>
        Task<PageOutput<GCSiteSecurityPageOutput>> GetSecuritySitePageList(string keyword, int page, int size,string order="",string ordertype="");

        /// <summary>
        /// 站点下巡更点
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        Task<List<SecurityListOutput>> GetTodaySecurityListCount(int siteid,DateTime date);

        /// <summary>
        /// 跟据巡更点 查询巡更点详细
        /// </summary>
        /// <param name="securityid"></param>
        /// <returns></returns>
        Task<SiteSecurityOutput> GetSiteSecurityOutput(int securityid);


        /// <summary>
        /// 巡更点插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Insert(GCSecurityEntity entity);

        /// <summary>
        /// 巡更点更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Update(GCSecurityEntity entity);

        /// <summary>
        /// 巡更点删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> Delete(int securityid, string username);

        /// <summary>
        /// 移动巡检-左上角统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataSet> spV2SecurityStats(int SITEID);

        /// <summary>
        /// 移动巡检-巡更点信息
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2SecurityPoint(int SITEID);

        /// <summary>
        /// 巡更次数统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        Task<DataTable> spV2SecurityHisCount(int SITEID, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 历史记录
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="keyword">关键词</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="SECURITYID">点位ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        Task<DataTable> spV2SecurityHisList(int SITEID, string keyword, DateTime startDate, DateTime endDate,
            int SECURITYID, int pageIndex, int pageSize);

        /// <summary>
        /// 巡检照片
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="SCHISID">巡检记录id</param>
        /// <returns></returns>
        Task<DataTable> spV2SecurityHisImage(int SITEID, int SCHISID);
    }
}
