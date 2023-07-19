using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Njjy
{
    public interface INjjyService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 获取监测对象的施工阶段的修改履历(njjy版)
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="operatedate">最后更新时间</param>
        /// <returns>结果集</returns>
        Task<DataTable> getPhaseHisList(int SITEID, DateTime operatedate);

        /// <summary>
        /// 获取监测对象的施工阶段的修改履历(建邺专用)
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns>结果集</returns>
        Task<DataTable> getPhaseHisList(int SITEID);
    }
}
