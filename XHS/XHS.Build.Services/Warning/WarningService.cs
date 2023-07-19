using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Warning;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Warning
{
    public class WarningService : BaseServices<BaseEntity>, IWarningService
    {
        private readonly IUser _user;
        private readonly IWarningRepository _warningRepository;
        public WarningService(IWarningRepository warningRepository, IUser user)
        {
            _user = user;
            _warningRepository = warningRepository;
            BaseDal = warningRepository;
        }

        public async Task<int> doCarUnWashed(SgParams sp)
        {
            await _warningRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForCarUnWashed", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<int> CarWashInsert(CarWashInsertDto dto)
        {
            List<SugarParameter> param = new List<SugarParameter>();
            foreach (PropertyInfo p in dto.GetType().GetProperties())
            {
                param.Add(new SugarParameter("@" + p.Name, p.GetValue(dto)));
            }
            param.Add(new SugarParameter("@jsonparam", JsonConvert.SerializeObject(dto)));
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            param.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spCarWashInsert", param);
            return output.Value.ObjToInt();
        }

        public async Task<int> doCarWashOffline(string parkkey, string gatename)
        {
            return await _warningRepository.doCarWashOffline(parkkey, gatename);
        }

        public async Task<int> doCarWashOfflineTimeout(string parkkey, string gatename, int type)
        {
            return await _warningRepository.doCarWashOfflineTimeout(parkkey, gatename, type);
        }

        public async Task<int> doChangeStatus(int WARNID, int warnstatus)
        {
            return await _warningRepository.doChangeStatus(WARNID, warnstatus, _user.Name);
        }

        public async Task<int> doFire(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForFire", ps);
            return output.Value.ObjToInt();
        }
        public async Task<int> doSmoke(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForSmoke", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doVest(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForVest", ps);
            return output.Value.ObjToInt();
        }


        public async Task<int> doElectric(EmeterWarnDataInput input)
        {
            SgParams sp = new SgParams();
            sp.SetParams(input);
            sp.Add("jsonall", JsonConvert.SerializeObject(input));
            sp.NeetReturnValue();
            await _warningRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForElectric", sp.Params);
            return sp.ReturnValue;           
        }

        public async Task<int> doHelmet(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForHelmet", ps);
            return output.Value.ObjToInt();
        }
        public async Task<int> doHighFormwork(HighFormworkAlarmInfoDto dto)
        {
            SgParams sp = new SgParams();
            sp.SetParams(dto);
            sp.Remove("recordNumber");
            sp.NeetReturnValue();
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForHighFormwork", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<int> doDeepPit(SgParams sp)
        {
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForDeepPit", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<int> doOverload(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForLiftOver", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doStranger(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForStranger", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doTrespasser(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _warningRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnInsertForTrespasser", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doUpdate(int WARNID, int type)
        {
            return await _warningRepository.doUpdate(WARNID, type);
        }

        public async Task<int> doUpdateCmd(int WARNID)
        {
            return await _warningRepository.doUpdateCmd(WARNID);
        }

        public async Task<DataTable> getAISendList(int WARNID)
        {
            return await _warningRepository.getAISendList(WARNID);
        }

        public async Task<DataTable> getListApp(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _warningRepository.getListApp(SITEID, startdate, enddate);
        }

        public async Task<DataTable> getListPrime(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await _warningRepository.getListPrime(_user.GroupId, SITEID, _user.Id, startdate, enddate);
        }

        public async Task<DataTable> getListType(int SITEID, int type, DateTime startdate, DateTime enddate)
        {
            return await _warningRepository.getListType(_user.GroupId, SITEID, _user.Id, type, startdate, enddate);
        }


        public async Task<DataTable> getListByType(int SITEID, int type, DateTime startdate, DateTime enddate)
        {
            return await _warningRepository.getListByType(_user.GroupId, SITEID, _user.Id, type, startdate, enddate);
        }


        public async Task<DataTable> getListByDevicecode(string devicecode, int type, DateTime startdate, DateTime enddate)
        {

            return await _warningRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnListByDevicecode", new { GROUPID = _user.GroupId, devicecode = devicecode, type = type, startdate = startdate, enddate = enddate }); ;
        }

        public async Task<DataRow> getOne(int WARNID)
        {
            DataTable dt = await _warningRepository.getOne(WARNID);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }

        }

        public async Task<DataTable> getSendCmdList()
        {
            return await _warningRepository.getSendCmdList();
        }

        public async Task<DataTable> getSendList()
        {
            return await _warningRepository.getSendList();
        }

        public async Task<DataTable> getSendList(int WARNID)
        {
            return await _warningRepository.getSendList(WARNID);
        }

        public async Task<DataTable> getSumList(DateTime startdate, DateTime enddate)
        {
            return await _warningRepository.getSumList(_user.GroupId, _user.Id, startdate, enddate);
        }

        public async Task<DataTable> getTodayList(int SITEID)
        {
            return await _warningRepository.getTodayList(SITEID);
        }

        public async Task<DataTable> GetWarnListByCondition(DateTime startdate, DateTime enddate, int normalai, int warntype, int warnstatus, string wpcode, int pageindex, int pagesize)
        {
            DataTable dt = await _warningRepository.GetWarnListByCondition(_user.Id, _user.GroupId.ToString(), startdate, enddate, normalai, warntype, warnstatus, wpcode, pageindex, pagesize);

            return dt;

        }

        public async Task<int> doNjjyWarnAddRemark(object param)
        {
            return await _warningRepository.doNjjyWarnAddRemark(param);
        }

        public async Task<DataTable> doNjjyWarnSolve(object param)
        {
            return await _warningRepository.doNjjyWarnSolve(param);
        }

        public async Task<int> doNjjyWarnFinish(object param)
        {
            return await _warningRepository.doNjjyWarnFinish(param);
        }

        /// <summary>
        /// 插入提醒
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doInsert(object param)
        {
            return await _warningRepository.doInsert(param);
        }

        /// <summary>
        /// 获取某条告警对应的监测对象对应的警告接收者（南京建邺专用）
        /// </summary>
        /// <param name="WARNID">告警编号</param>
        /// <returns>结果集</returns>
        public async Task<DataSet> getNjjyWarnDetail(int WARNID, int type)
        {
            DataSet ds = await _warningRepository.getNjjyWarnDetail(WARNID, type);
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["remark"] = dr["remark"] == null ? "" : HttpUtility.HtmlDecode(dr["remark"].ToString());
                        dr["remarkall"] = dr["remarkall"] == null ? "" : HttpUtility.HtmlDecode(dr["remarkall"].ToString());
                    }
                }
            }
            return ds;
        }

        public async Task<DataSet> getSiteAiCount(int SITEID, int type)
        {
            return await _warningRepository.getSiteAiCount(SITEID, type);
        }


        public async Task<DataTable> getSiteAiDayCount(int SITEID, int type, DateTime billdate)
        {
            return await _warningRepository.getSiteAiDayCount(SITEID, type, billdate);
        }

        public async Task<DataSet> getSiteAiList(int SITEID, int type, DateTime billdate, int ENDWARNID, int pageindex, int pagesize)
        {
            return await _warningRepository.getSiteAiList(SITEID, type, billdate, ENDWARNID, pageindex, pagesize);
        }

        public async Task<DataTable> spWarnCountForAppTop(DateTime dt)
        {
            return await _warningRepository.spWarnCountForAppTop(_user.Id, _user.GroupId, dt);
        }

        public async Task<DataTable> GetWarnListForTaihu(int SITEID, DateTime startdate, DateTime? enddate)
        {
            return await _warningRepository.GetWarnListForTaihu(_user.GroupId, SITEID, startdate, enddate);
        }

        public async Task<int> FallProtectionInsert(CCWarning input)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" insert into T_CC_Warning (GROUPID,SITEID,devicecode,type,content) values ({0},{1},'{2}',{3},'{4}')",
                input.GROUPID, input.SITEID, input.devicecode, input.type, input.content);
            return await _warningRepository.Db.Ado.ExecuteCommandAsync(sql.ToString());
        }

        public async Task<DataSet> spV2WarnStats(string startTime, string endTime, int SITEID)
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
                .GetDataSetAllAsync("spV2WarnStats", new { startTime = startTime, endTime = endTime, SITEID = SITEID });
        }

        public async Task<DataTable> spV2WarnStatsDaily(string startTime, string endTime, int SITEID)
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
                .GetDataTableAsync("spV2WarnStatsDaily", new { startTime = startTime, endTime = endTime, SITEID = SITEID });
        }

        public async Task<DataTable> spV2WarnTypeDaily(string startTime, string endTime, int SITEID, int type)
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
                .GetDataTableAsync("spV2WarnTypeDaily", new { startTime = startTime, endTime = endTime, SITEID = SITEID, type = type });
        }
        public async Task<DataTable> spV2WarnTypeRank(string startDate, string endDate, int SITEID)
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
                .GetDataTableAsync("spV2WarnTypeRank", new { startDate = startDate, endDate = endDate, SITEID = SITEID});
        }

        public async Task<DataTable> spV2WarnLiveList(int type, int SITEID, string keyword, DateTime startTime, DateTime endTime, int pageSize, int pageIndex)
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2WarnLiveList",
               new
               {
                   type = type,
                   SITEID = SITEID,
                   keyword = keyword,
                   starttime = startTime,
                   endtime = endTime,
                   pagesize = pageSize,
                   pageindex = pageIndex
               });
        }

        public async Task<DataSet> spV2WarnTypeStats(int SITEID, DateTime startTime, DateTime endTime)
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
               .GetDataSetAllAsync("spV2WarnTypeStats",
               new
               {
                   SITEID = SITEID,
                   startDate = startTime,
                   endDate = endTime
               });
        }

        public async Task<DataTable> spV2WarnLiveSelect()
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2WarnLiveSelect");
        }

        public async Task<DataTable> spV2WarnTypeLive(int SITEID, string keyword, int MPID, DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {
            return await _warningRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2WarnTypeLive",
               new
               {
                   SITEID = SITEID,
                   MPID = MPID,
                   startDate = startDate,
                   endDate = endDate,
                   keyword = keyword,
                   pageSize = pageSize,
                   pageIndex = pageIndex
               });
        }
    }
}
