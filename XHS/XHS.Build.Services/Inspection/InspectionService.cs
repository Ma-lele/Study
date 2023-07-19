using System;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Inspection
{
    public class InspectionService : BaseServices<InspectionEntity>, IInspectionService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<InspectionEntity> _InspectionRepository;
        public InspectionService(IUser user, IBaseRepository<InspectionEntity> InspectionRepository)
        {
            _user = user;
            _InspectionRepository = InspectionRepository;
            BaseDal = InspectionRepository;
        }

        public async Task<int> doDelete(string inspcode)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionDelete", new { inspcode = inspcode, USERID = _user.Id });
        }

        public async Task<DataSet> getOne(string inspcode)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spInspectionGet", new { inspcode = inspcode });
        }

        public async Task<int> doFinish(object param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionFinish", param);
        }

        public async Task<int> doFourFinish(SgParams sp)
        {
            await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownFourRectifyContentInfo", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<string> doInsert(object param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetStringAsync("spInspectionInsert", param);
        }

        public async Task<int> doFourInsert(SgParams sp)
        {
            await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownFourInspectContentInfo", sp.Params);
            return sp.ReturnValue;
        }


        public async Task<int> doUpdate(object param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionUpdate", param);
        }


        public async Task<int> reformAdd(object param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionReformAdd", param);
        }

        public async Task<int> doRemarkAdd(object param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionRemarkAdd", param);
        }


        public async Task<int> deduct(object param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionDeduct", param);
        }

        public async Task<int> doSolve(object param)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spInspectionSolve", param);
        }

        public async Task<DataTable> GetMobileListCount()
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spInspectionCountList", new { GROUPID = _user.GroupId, USERID = _user.Id });

        }

        public async Task<DataTable> GetOrderSiteList(int siteid = 0, long INSPID = 0, int datetype = 1, string insplevel = "", int status = 0, int page = 1, int size = 10)
        {
            DateTime enddate = DateTime.Now.AddDays(1);
            DateTime startdate = DateTime.Now.AddDays(-1);
            if (datetype == 2)
            {
                startdate = DateTime.Now.AddDays(-7);
            }
            else if (datetype == 3)
            {
                startdate = DateTime.Now.AddDays(-30);
            }
            else if (datetype == 4 || datetype == 0)
            {
                startdate = DateTime.Now.AddDays(-90);
            }
            if (insplevel == "0")
            {
                insplevel = "";
            }
            var dt = await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spInspectionListBySiteId", new { USERID = _user.Id, SITEID = siteid, INSPID = INSPID, status = status, insplevel = insplevel, startdate = startdate, enddate = enddate, pageindex = page, pagesize = size });
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["processjson"] = dr["processjson"] == null ? "" : HttpUtility.HtmlDecode(dr["processjson"].ToString());
                }
            }
            return dt;
        }

        public async Task<DataTable> GetOrderListUnSolve(int siteid = 0, long INSPID = 0, int datetype = 1, string insplevel = "", int status = 0, int page = 1, int size = 10)
        {
            DateTime enddate = DateTime.Now.AddDays(1);
            DateTime startdate = DateTime.Now.AddDays(-1);
            if (datetype == 2)
            {
                startdate = DateTime.Now.AddDays(-7);
            }
            else if (datetype == 3)
            {
                startdate = DateTime.Now.AddDays(-30);
            }
            else if (datetype == 4 || datetype == 0)
            {
                startdate = DateTime.Now.AddDays(-90);
            }
            if (insplevel == "0")
            {
                insplevel = "";
            }
            var dt = await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spInspectionListUncloseByUser", new { USERID = _user.Id, SITEID = siteid, INSPID = INSPID, status = status, insplevel = insplevel, startdate = startdate, enddate = enddate, pageindex = page, pagesize = size });

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr["processjson"] = dr["processjson"] == null ? "" : HttpUtility.HtmlDecode(dr["processjson"].ToString());
                }
            }

            return dt;
        }


        public Task<DataTable> GetCountAsync(int GROUPID, string datamonth)
        {
            return _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtInspectCount", new { GROUPID = GROUPID, datamonth = datamonth });
        }

        public Task<DataTable> GetMonthReviewAsync(int GROUPID, int datayear)
        {
            return _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtInspectMonthReview", new { GROUPID = GROUPID, datayear = datayear });

        }

        public Task<DataTable> GetRoundCountAsync(int GROUPID, string yearmonth)
        {
            return _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtInspectRoundCount", new { GROUPID = GROUPID, yearmonth = yearmonth });
        }

        public Task<DataTable> GetSafetyStandardAsync(int GROUPID, int datayear)
        {
            return _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtInspectSafetyStandard", new { GROUPID = GROUPID, datayear = datayear });
        }

        public Task<DataTable> GetSelfInspectAsync(int GROUPID, int datayear)
        {
            return _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtInspectSelfInspect", new { GROUPID = GROUPID, datayear = datayear });
        }
        public async Task<DataTable> GetV2InspectDayCount(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2InspectionDayCount", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetV2InspectList(int SITEID, int USERID, int INSPID, int processstatus, string keyword, DateTime startdate, DateTime enddate, int pageindex, int pagesize)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2InspectionList", new { SITEID = SITEID, USERID = USERID, INSPID= INSPID, processstatus = processstatus, keyword = keyword, startdate = startdate, enddate = enddate, pageindex = pageindex, pagesize = pagesize });
        }

        public async Task<DataTable> GetV2InspectStats(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2InspectionStats", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetV2InspectTypeCount(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2InspectionTypeCount", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetV2InspectOne(string inspcode)
        {
            return await _InspectionRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spInspectionGet", new { inspcode = inspcode });
        }
    }
}
