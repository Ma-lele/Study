using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Round
{
    public class RoundService:BaseServices<BaseEntity>,IRoundService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _roundRepository;
        public RoundService(IUser user, IBaseRepository<BaseEntity> roundRepository)
        {
            _user = user;
            _roundRepository = roundRepository;
            BaseDal = roundRepository;
        }

        public async Task<int> doDelete(long ROUNDID)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spRoundDelete", new { ROUNDID = ROUNDID, USERID = _user.Id });
        }

        public async Task<int> doFinish(long ROUNDID, string remark)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spRoundFinish", new { ROUNDID = ROUNDID, username = _user.Name, remark = remark });
        }

        public async Task<int> DoRoundTypeInsert(int SITEID, string rtcontent)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spRoundTypeInsert", new { SITEID = SITEID, GROUPID = _user.GroupId, rtcontent = rtcontent, operatoruser= _user.Name });
        }

        public async Task<DataTable> doInsert(object param)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoundInsertForApp", param);
        }

        public async Task<int> doRemarkAdd(long ROUNDID, string remark)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spRoundRemarkAdd", new { ROUNDID = ROUNDID, USERID = _user.Id, remark = remark });
        }

        public async Task<int> doSolve(long ROUNDID,  string solvedremark)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spRoundSolve", new { ROUNDID = ROUNDID, solveduserid = _user.Id, solveduser = _user.Name, solvedremark = solvedremark });
        }

        public async Task<DataSet> GetChartData( string type, int SITEID)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spRoundChart", new { GROUPID = _user.GroupId, USERID = _user.Id, type = type, SITEID = SITEID });
        }

        public async Task<DataTable> getListByDate(int SITEID, DateTime updatedate)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoundListByDate", new { SITEID = SITEID, updatedate = updatedate });
        }

        public async Task<DataTable> GetRoundType(int SITEID, int RTHID)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoundTypeGet", new { SITEID = SITEID, RTHID = RTHID });
        }


        public async Task<DataTable> getListByUser(DateTime updatedate)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoundListByUser", new { USERID = _user.Id, updatedate = updatedate });
        }

        public async Task<DataTable> GetMobileListCount(int datetype = 0, int orderby = 0)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoundCountList", new { GROUPID = _user.GroupId, USERID = _user.Id, datetype= datetype, orderby= orderby });
        }

        public async Task<DataTable> GetOrderList(int siteid, int roundtype, int status, int datetype, int page, int size)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoundListUncloseByUser", new { USERID = _user.Id, SITEID = siteid, roundtype = roundtype, status = status, datetype= datetype,pageindex = page, pagesize = size });
        }

        public async Task<DataTable> GetOrderListWithoutRole(int siteid, int roundtype, int status, int datetype, int page, int size)
        {
            var dt= await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoundListBySiteId", new { SITEID = siteid, roundtype = roundtype, status = status, datetype= datetype, pageindex = page, pagesize = size });

            if(dt!=null && dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    dr["remark"] = dr["remark"]==null?"":HttpUtility.HtmlDecode(dr["remark"].ToString());
                }
            }

            return dt;
        }

        public async Task<DataTable> GetV2RoundDayCount(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2RoundDayCount", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetV2RoundList(int SITEID, int status, string keyword, DateTime startdate, DateTime enddate, int pageindex, int pagesize)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2RoundList", new { SITEID = SITEID, status = status, keyword = keyword, startdate = startdate, enddate = enddate, pageindex= pageindex, pagesize= pagesize });
        }

        public async Task<DataTable> GetV2RoundStats(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2RoundStats", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetV2RoundTypeCount(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _roundRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2RoundTypeCount", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public Task<DataTable> GetV2RoundOne(int ROUNDID)
        {
            throw new NotImplementedException();
        }
    }
}
