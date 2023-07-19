using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Phase
{
    public interface IPhaseService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 获取施工阶段
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getAll();

        /// <summary>
        /// 获取监测对象的施工阶段的修改履历
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="operatedate">最后更新时间</param>
        /// <returns>结果集</returns>
        Task<DataTable> getHisList(int SITEID, DateTime operatedate);


        /// <summary>
        /// 工程履历照片列表
        /// </summary>
        /// <param name="PHASEHISID">工程履历ID</param>
        /// <returns></returns>
        Task<DataTable> getPhotoList(long PHASEHISID);

        /// <summary>
        /// 更新监测点施工阶段
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="constructphase">施工阶段ID</param>
        /// <param name="phasepercent">施工进度百分比</param>
        /// <param name="phasedate">施工进度日期</param>
        /// <returns></returns>
        Task<long> doUpdate(int SITEID,  int constructphase, int phasepercent, DateTime phasedate);

        /// <summary>
        /// 获取监测对象的施工进度
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetSitePhaseList(int SITEID);

        /// <summary>
        /// 更新监测对象的施工进度
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="PHASEIDS">进度ID</param>
        /// <param name="phasedates">更新时间</param>
        /// <param name="phasedatetype">更新类型</param>
        /// <returns>结果集</returns>
        Task<int> UpdateSitePhase(int SITEID,string PHASEIDS,string phasedates,int phasedatetype);

    }
}
