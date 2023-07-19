using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Inspection
{
    public interface ICaRoundService : IBaseServices<InspectionEntity>
    {

        /// <summary>
        /// 资产巡检插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<string> doInsert(List<SugarParameter> param);


        /// <summary>
        /// 资产巡检检查项插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doDetailInsert(List<SugarParameter> param);


        /// <summary>
        /// 资产巡检问题项插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doDetailOkInsert(List<SugarParameter> param);


        /// <summary>
        /// 资产巡检删除
        /// </summary>
        /// <param name="INSPID">资产巡检编号</param>
        /// <returns></returns>
        Task<int> doDelete(string roundcode);

        /// <summary>
        /// 资产巡检信息
        /// </summary>
        /// <param name="INSPID">资产巡检编号</param>
        /// <returns></returns>
        Task<DataSet> getOne(string roundcode);

        /// <summary>
        /// 资产巡检问题解决
        /// </summary>
        /// <param name="INSPID">资产巡检ID</param>
        /// <param name="solvedremark">解决日志</param>
        /// <returns></returns>
        Task<int> doSolve(List<SugarParameter> param);

        /// <summary>
        /// 资产巡检问题添加批示
        /// </summary>
        /// <param name="INSPID">资产巡检ID</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        Task<int> doRemarkAdd(List<SugarParameter> param);

        /// <summary>
        /// 结束资产巡检问题
        /// </summary>
        /// <param name="INSPID">资产巡检ID</param>
        /// <returns></returns>
        Task<int> doFinish(List<SugarParameter> param);


        /// <summary>
        /// 资产巡检统计列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="userid"></param>
        /// <param name="siteid"></param>
        /// <param name="teid"></param>
        /// <param name="keyword"></param>
        /// <param name="sort"></param>
        /// <param name="tenanttype"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<DataTable> GetOrderTenantList(DateTime startdate, DateTime enddate, int siteid = 0, int teid = 0, string keyword = "", int sort = 1, int tenanttype = 0, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 资产巡检单（待验收或租户别）
        /// </summary>
        /// <param name="teid"></param>
        /// <param name="status"></param>
        /// <param name="keyword"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<DataTable> GetOrderList(DateTime startdate, DateTime enddate, int teid = 0, int isoverhouronly = 0, int status = 0, string keyword = "", int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 获取租户检查项
        /// </summary>
        /// <param name="teid"></param>
        /// <returns></returns>
        Task<DataTable> GetCheckListByTenant(int teid);
    }
}
