using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SpecialEqpDoc
{
    public interface ISpecialEqpDocService:IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 获取特种设备文件列表
        /// </summary>
        /// <param name="SEID">特种设备ID</param>
        /// <returns>特种设备数据集</returns>
        Task<DataSet> getList(int SEID);

        /// <summary>
        /// 插入特种设备文件
        /// </summary>
        /// <param name="param">特种设备文件信息</param>
        /// <returns>设置结果</returns>
        string doInsert(object param);

        /// <summary>
        /// 删除特种设备文件
        /// </summary>
        /// <param name="SEDOCID">特种设备文件ID</param>
        /// <returns>结果</returns>
        DataRow doDelete(string SEDOCID);

        /// <summary>
        /// 工地特种设备（南京建邺专用）
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns>结果集</returns>
        DataSet getNjjySiteSpec(int SITEID);

        DataSet getRealOne(int SEID);

        Task<List<SpecialEqpData>> GetListDataHis(string SeCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize = 20);
    }
}
