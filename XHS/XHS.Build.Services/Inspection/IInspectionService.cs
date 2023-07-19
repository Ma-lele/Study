using System;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Inspection
{
    public interface IInspectionService : IBaseServices<InspectionEntity>
    {

        /// <summary>
        /// 移动执法插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<string> doInsert(object param);

        /// <summary>
        /// 移动执法插入（省4号文）
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doFourInsert(SgParams sp);

        /// <summary>
        /// 移动执法完成（省4号文）
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doFourFinish(SgParams sp);

        /// <summary>
        /// 移动执法巡查更新
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdate(object param);

        /// <summary>
        /// 创建整改单
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> reformAdd(object param);

        /// <summary>
        /// 整改单扣分
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> deduct(object param);

        /// <summary>
        /// 移动执法删除
        /// </summary>
        /// <param name="INSPID">移动执法编号</param>
        /// <returns></returns>
        Task<int> doDelete(string inspcode);

        /// <summary>
        /// 移动执法信息
        /// </summary>
        /// <param name="INSPID">移动执法编号</param>
        /// <returns></returns>
        Task<DataSet> getOne(string inspcode);

        /// <summary>
        /// 移动执法问题解决
        /// </summary>
        /// <param name="INSPID">移动执法ID</param>
        /// <param name="solvedremark">解决日志</param>
        /// <returns></returns>
        Task<int> doSolve(object param);

        /// <summary>
        /// 移动执法问题添加批示
        /// </summary>
        /// <param name="INSPID">移动执法ID</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        Task<int> doRemarkAdd(object param);

        /// <summary>
        /// 结束移动执法问题
        /// </summary>
        /// <param name="INSPID">移动执法ID</param>
        /// <returns></returns>
        Task<int> doFinish(object param);



        /// <summary>
        /// 获取手机移动巡检首页列表
        /// </summary>
        /// <param name="groupid">分组ID</param>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        Task<DataTable> GetMobileListCount();

        /// <summary>
        /// 检查单列表
        /// </summary>
        /// <param name="siteid">监测点ID</param>
        /// <param name="INSPID">检查单ID</param>
        /// <param name="datetype">1：当日；2：一周；3：一月；4：三月</param>
        /// <param name="status">状态</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页条数</param>
        /// <returns></returns>
        Task<DataTable> GetOrderSiteList(int siteid = 0, long INSPID = 0, int datetype = 1, string insplevel = "", int status = 0, int page = 1, int size = 10);

        /// <summary>
        /// 检查单 待办工单
        /// </summary>
        /// <param name="siteid">监测点ID</param>
        /// <param name="INSPID">检查单ID</param>
        /// <param name="datetype">1：当日；2：一周；3：一月；4：三月</param>
        /// <param name="status">状态</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页条数</param>
        /// <returns></returns>
        Task<DataTable> GetOrderListUnSolve(int siteid = 0, long INSPID = 0,  int datetype = 1, string insplevel = "", int status = 0, int page = 1, int size = 10);
        
        #region 市平台接口

        Task<DataTable> GetCountAsync(int GROUPID, string datamonth);

        Task<DataTable> GetMonthReviewAsync(int GROUPID, int datayear);

        Task<DataTable> GetRoundCountAsync(int GROUPID, string yearmonth);

        Task<DataTable> GetSafetyStandardAsync(int GROUPID, int datayear);

        Task<DataTable> GetSelfInspectAsync(int GROUPID, int datayear);
        #endregion

        /// <summary>
        /// 智慧工地2.0项目端-检查单-左上角统计
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="startdate">开始日期</param>
        ///  <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2InspectStats(int SITEID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 智慧工地2.0项目端-检查单-问题类型统计
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="startdate">开始日期</param>
        ///  <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2InspectTypeCount(int SITEID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 智慧工地2.0项目端-检查单-每日次数统计
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="startdate">开始日期</param>
        ///  <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2InspectDayCount(int SITEID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 智慧工地2.0项目端-检查单列表
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="USERID">用户ID</param>
        /// <param name="INSPID">一页最后一条数据的key</param>
        /// <param name="processstatus">2：等待整改，3：等待确认，4：完成整改</param>
        /// <param name="keyword">单号，模糊查询</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <param name="pageindex">结束日期</param>
        /// <param name="pagesize">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2InspectList(int SITEID, int USERID, int INSPID, int processstatus, string keyword, DateTime startdate, DateTime enddate, int pageindex, int pagesize);

        /// <summary>
        /// 智慧工地2.0项目端-检查单详情
        /// </summary>
        /// <param name="inspcode">检查单号</param>
        /// <returns></returns>
        Task<DataTable> GetV2InspectOne(string inspcode);
    }
}
