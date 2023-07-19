using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Env
{
    public interface IEnvService : IBaseServices<BaseEntity>
    {
        Task<DataRow> GetSiteCountAsync(int GROUPID);

        Task<DataTable> GetWarnCountAsync(int GROUPID, int type);

        Task<DataSet> GetWarnCountTotalAsync(int GROUPID);

        Task<DataRow> GetWashCountAsync(int GROUPID, int type);

        Task<DataRow> GetAirTightCountAsync(int GROUPID, int type);
        
        Task<DataRow> GetSoilCountAsync(int GROUPID, int type);

        Task<DataTable> GetSitePmAsync(int GROUPID);

        Task<DataTable> GetPmRtdRankAsync(int GROUPID);

        Task<DataTable> GetPmHourRankAsync(int GROUPID);

        Task<DataTable> GetPmDailyRankAsync(int GROUPID);

        Task<DataTable> GetPmDailyListAsync(int GROUPID,string billdate,string keyword,int pageindex,int pagesize);

        Task<DataTable> GetPmHourListAsync(int GROUPID,string billdate,string keyword,int pageindex,int pagesize);

        Task<DataTable> GetCtEnvWarnListAsync(int GROUPID, int SITEID, DateTime startdate, DateTime enddate, int type, int pageindex, int pagesize);


        #region  项目端2.0

        //获取监测点最新扬尘实时数据
        Task<DataTable> GetPmSiteRtdData(int SITEID);

        //获取监测点扬尘历史数据
        Task<DataTable> GetSiteDataHis(int SITEID, DateTime startdate, DateTime enddate, int datatype, int pageindex, int pagesize);

        //获取监测点臭氧历史数据
        Task<DataTable> GetSiteO3His(int SITEID, DateTime startdate, DateTime enddate, int datatype, int pageindex, int pagesize);

        //获取监测点扬尘实时，小时，日均数据
        Task<DataSet> GetSiteO3Chart(int SITEID);

        //获取监测点扬尘实时，小时，日均数据
        Task<DataSet> GetPmSiteChart(int SITEID);

        //获取监测点超标告警统计
        Task<DataTable> GetPmSiteDayWarnCount(int SITEID,DateTime startdate, DateTime enddate);

        //获取监测点离线柱状图
        Task<DataTable> GetPmOnlineBarChart(int SITEID, DateTime startdate, DateTime enddate);
        #endregion
    }
}
