using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Report
{
    public class ReportService:BaseServices<BaseEntity>,IReportService
    {
        public readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _reportRepository;
        public ReportService(IBaseRepository<BaseEntity> reportRepository, IUser user)
        {
            _user = user;
            _reportRepository = reportRepository;
            BaseDal = reportRepository;
        }

        public async Task<DataSet> getNoiseDistribution(string startdate, string enddate)
        {
            return await _reportRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spNoiseDistributionReport", new { GROUPID = _user.GroupId, startdate = startdate, enddate = enddate, USERID = _user.Id });
        }

        public async  Task<DataTable> getNoiseNight(int SITEID, string startdate, string enddate)
        {
            return await _reportRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNoiseNightReport", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataSet> getNoiseWeek(DateTime startdate)
        {
            return await _reportRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spNoiseWeekReport", new { GROUPID = _user.GroupId, startdate = startdate, USERID = _user.Id });
        }

        public async  Task<DataSet> getPmDistribution(string startdate, string enddate)
        {
            return await _reportRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spPmDistributionReport", new { GROUPID = _user.GroupId, startdate = startdate, enddate = enddate, USERID = _user.Id });
        }

        public async  Task<DataTable> getPmMonth(string yearmonth, int sitetype)
        {
            return await _reportRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spPmMonthReport", new { GROUPID = _user.GroupId, yearmonth = yearmonth, sitetype = sitetype, USERID = _user.Id });
        }


        public async Task<DataTable> getAppSimpleReport()
        {
            return await _reportRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAppReport", new { GROUPID = _user.GroupId, USERID = _user.Id });
        }


        public async Task<DataSet> getSitePmDistribution(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _reportRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSitePmDistribution", new { SITEID = SITEID, startdate = startdate, enddate = enddate, USERID = _user.Id });
        }
    }
}
