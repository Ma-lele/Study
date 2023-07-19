using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.DeviceCN
{
    public interface IDeviceCNService:IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 监测对象的历史数据（实时，小时，日均）
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns>结果集</returns>
        Task<DataSet> getSiteDataHis(object param);

        Task<int> rtdInsert(DeviceRtdDataInput input);

        /// <summary>
        /// 获取一个监测点最新一条监测数据
        /// </summary>
        /// <param name="SITEID">分组编号</param>
        /// <returns>结果集</returns>
        Task<DataTable> getSiteRtdApi(int SITEID);

        Task<DataTable> getSiteAllRtdApi(int GROUPID, int pageIndex, int pageSize);

        Task<DataTable> getSiteRtdOneHourApi(int SITEID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<int> doBatch(int timeout);

        /// <summary>
        /// 获取10分钟内需要推送的单条记录
        /// </summary>
        /// <param name="devicecode">设备编号</param>
        /// <returns></returns>
        Task<DataRow> getOneForSend(string devicecode);

        /// <summary>
        /// 检索设备实时信息
        /// </summary>
        /// <returns>设备实时信息数据集</returns>
        DataTable getRtdList();
    }
}
