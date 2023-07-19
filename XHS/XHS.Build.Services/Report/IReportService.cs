using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Report
{
    public interface IReportService:IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 夜间噪声一览
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>结果集</returns>
        Task<DataTable> getNoiseNight(int SITEID, string startdate, string enddate);

        /// <summary>
        /// 噪声小时均值分布
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>结果集</returns>
        Task<DataSet> getNoiseDistribution(string startdate, string enddate);

        /// <summary>
        /// 噪声周报
        /// </summary>
        /// <param name="startdate">开始日期</param>
        /// <returns></returns>
        Task<DataSet> getNoiseWeek(DateTime startdate);

        /// <summary>
        /// PM2.5/PM10平均浓度月报
        /// </summary>
        /// <param name="GROUPID">分组编号</param>
        /// <param name="yearmonth">年月 例：2017-01</param>
        /// <param name="sitetype">监测类型</param>
        /// <returns>结果集</returns>
        Task<DataTable> getPmMonth(string yearmonth, int sitetype);

        /// <summary>
        /// PM2.5/PM10小时均值分布
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>结果集</returns>
        Task<DataSet> getPmDistribution(string startdate, string enddate);


        /// <summary>
        /// app简报
        /// </summary>
        /// <returns>结果集</returns>
        Task<DataTable> getAppSimpleReport();

        /// <summary>
        /// 监测点颗粒物分布
        /// </summary>
        /// <param name="SITEID">监测点IDID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>结果集</returns>
        Task<DataSet> getSitePmDistribution(int SITEID, DateTime startdate, DateTime enddate);
    }
}
