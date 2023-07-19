using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Elevator
{
    public interface IElevatorService : IBaseServices<ElevatorEntity>
    {
        Task<int> doCheckin(string secode);

        DataTable getAll();

        Task<int> doPartInsert(string secode, int setype);
        Task<int> doRtdInsert(SgParams sp);
        Task<int> doParamUpdate(SgParams sp);

        Task<int> doTipOverWarn(string secode, string starttime, string endtime, string tipOverData);
        Task<int> doCheckout(string secode);

        /// <summary>
        /// 插入塔吊数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doDyCraneRtdInsert(List<SugarParameter> param);


        /// <summary>
        /// 插入升降机数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doDyLiftRtdInsert(List<SugarParameter> param);


        /// <summary>
        /// 插入卸料平台数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doDyUnloadRtdInsert(List<SugarParameter> param);
    }
}
