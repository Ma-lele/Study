using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.SmsQueue
{
    public interface ISmsQueueRepository : IBaseRepository<SmsQueueEntity>
    {
        /// <summary>
        /// 检索该监测点下的提醒
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns>提醒信息数据集</returns>
        Task<DataTable> getList(int SITEID);

        /// <summary>
        /// 检索待发送提醒
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getSendList();

        /// <summary>
        /// 更新提醒状态
        /// </summary>
        /// <param name="SQID">提醒ID</param>
        /// <param name="resultmessage">提醒结果</param>
        /// <returns></returns>
        Task<int> doUpdate(long SQID, string resultmessage);

        /// <summary>
        /// 插入提醒
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doInsert(params SugarParameter[] param);
    }
}
