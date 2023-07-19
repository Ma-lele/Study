using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Schedule
{
    public interface IScheduleService : IBaseServices<BaseEntity>
    {/// <summary>
     /// 取得工程进度统计信息
     /// </summary>
     /// <param name="SITEID">监测点ID</param>
     /// <returns>结果</returns>
        Task<DataSet> getSum(int SITEID);

        /// <summary>
        /// 检索工程进度
        /// <returns>结果集</returns>
        Task<DataSet> getList(int SITEID);

        /// <summary>
        /// 工程进度更新
        /// </summary>
        /// <param name="param">分组情报</param>
        /// <returns>结果集</returns>
        Task<int> doUpdate(object param);
    }
}
