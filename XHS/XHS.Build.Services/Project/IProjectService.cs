using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Project
{
    public interface IProjectService: IBaseServices<BaseEntity>
    {
        Task<DataRow> GetAnalyseCountAsync(int GROUPID);

        Task<DataTable> GetAnalyseOver90Async(int GROUPID);

        Task<DataTable> GetAnalyseTypeCountAsync(int GROUPID);
        Task<DataTable> GetAnalyseYearCountAsync(int GROUPID, int datayear);
        Task<DataTable> GetListAsync(int GROUPID, int pageindex, int pagesize, string keyword, string company);

        Task<DataTable> GetDevListAsync(int GROUPID,int pageindex,int pagesize,string keyword);

        Task<DataTable> GettAnalyseSiteListAsync(int GROUPID);

        /// <summary>
        /// 获取各种类型的项目列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="type">0:全部，1: 待审, 2: 在建, 3: 停工, 4: 终止安监, 5: 竣工 6:本月标化考评项目 7:超90天未竣工项目</param>
        /// <returns></returns>
        Task<DataTable> GetAnalyseSiteListByTypeAsync(int GROUPID, int type);

        /// <summary>
        /// 获取集成商在线率列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<DataTable> GetIntegratorListAsync(int GROUPID, DateTime startdate, DateTime enddate, int pageindex, int pagesize, string keyword);
    }
}
