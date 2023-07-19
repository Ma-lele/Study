using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Danger
{
    public interface IDangerService : IBaseServices<BaseEntity>
    {
        Task<DataRow> GetSiteCountAsync(int GROUPID);

        Task<DataTable> GetDevCountAsync(int GROUPID);

        Task<DataTable> GetWarnCountAsync(int GROUPID, int type);

        Task<DataTable> GetWarnRankAsync(int GROUPID, int type);

        Task<DataTable> GetWarnAreaCountAsync(int GROUPID, int type);

        Task<DataTable> GetDevList(int GROUPID, int type, int pageindex, int pagesize, string keyword);

        /// <summary>
        /// 危大工程-列表
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2DangerList(int SITEID);

        /// <summary>
        /// 危大工程-文件
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="SDID">危大工程ID</param>
        /// <returns></returns>

        Task<string> spV2DangerFile(int SITEID, string SDID);

        /// <summary>
        /// 危大工程-类型统计分析
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2DangerTypeStats(int SITEID);

        /// <summary>
        /// 危大工程-趋势
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="days">天数</param>
        /// <returns></returns>
        Task<DataTable> spV2DangerTrend(int SITEID, int days);
    }
}
