using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Round
{
    public interface IRoundService : IBaseServices<BaseEntity>
    {

        /// <summary>
        /// 获取巡查报表数据
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetChartData(string type, int SITEID);

        /// <summary>
        /// 检索巡查
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="updatedate">最后更新时间</param>
        /// <returns></returns>
        Task<DataTable> getListByDate(int SITEID, DateTime updatedate);

        /// <summary>
        /// 检索巡查
        /// </summary>
        ///<param name="SITEID">监测点ID</param>
        /// <param name="RTHID">问题类型历史ID（-1时，最新有效）</param>
        /// <returns></returns>
        Task<DataTable> GetRoundType(int SITEID, int RTHID);

        /// <summary>
        /// 根据用户检索巡查
        /// </summary>
        /// <param name="updatedate">最后更新时间</param>
        /// <returns></returns>
        Task<DataTable> getListByUser( DateTime updatedate);

        /// <summary>
        /// 巡查插入
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<DataTable> doInsert(object param);

        /// <summary>
        /// 巡查删除
        /// </summary>
        /// <param name="ROUNDID">巡查编号</param>
        /// <returns></returns>
        Task<int> doDelete(long ROUNDID);

        /// <summary>
        /// 巡查问题解决
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <param name="solvedremark">解决日志</param>
        /// <returns></returns>
        Task<int> doSolve(long ROUNDID,  string solvedremark);

        /// <summary>
        /// 巡查问题添加批示
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        Task<int> doRemarkAdd(long ROUNDID, string remark);

        /// <summary>
        /// 结束巡查问题
        /// </summary>
        /// <param name="ROUNDID">巡查ID</param>
        /// <returns></returns>
        Task<int> doFinish(long ROUNDID, string remark);

        /// <summary>
        /// 修改监测点问题项
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="rtcontent">问题项</param>
        /// <returns></returns>
        Task<int> DoRoundTypeInsert(int SITEID, string rtcontent);


        /// <summary>
        /// 获取手机移动巡检首页列表
        /// </summary>
        /// <param name="datetype">0:全部，1：天，2：周，3：月，4：3月</param>
        /// <param name="orderby">0:综合排序，1：待整改，2：检查单，3：整改次数，4：累计问题</param>
        /// <returns></returns>
        Task<DataTable> GetMobileListCount(int datetype = 0,int orderby = 0);

        /// <summary>
        /// 移动巡检 待办工单
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="siteid"></param>
        /// <param name="roundtype"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<DataTable> GetOrderList(int siteid, int roundtype, int status,int datetype, int page, int size);
        Task<DataTable> GetOrderListWithoutRole(int siteid, int roundtype, int status, int datetype, int page, int size);

        /// <summary>
        /// 智慧工地2.0项目端-随手拍-左上角统计
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="startdate">开始日期</param>
        ///  <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2RoundStats(int SITEID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 智慧工地2.0项目端-随手拍-问题类型统计
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="startdate">开始日期</param>
        ///  <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2RoundTypeCount(int SITEID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 智慧工地2.0项目端-随手拍-每日次数统计
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="startdate">开始日期</param>
        ///  <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2RoundDayCount(int SITEID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 智慧工地2.0项目端-随手拍-列表
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="status"></param>
        /// <param name="keyword"></param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <param name="pageindex">结束日期</param>
        /// <param name="pagesize">结束日期</param>
        /// <returns></returns>
        Task<DataTable> GetV2RoundList(int SITEID,int status,string keyword, DateTime startdate, DateTime enddate,int pageindex, int pagesize);

        /// <summary>
        /// 智慧工地2.0项目端-随手拍-详情
        /// </summary>
        /// <param name="ROUNDID"></param>
        /// <returns></returns>
        Task<DataTable> GetV2RoundOne(int ROUNDID);
    }
}
