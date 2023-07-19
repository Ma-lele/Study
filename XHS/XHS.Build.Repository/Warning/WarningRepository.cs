using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Warning
{
    public class WarningRepository : BaseRepository<BaseEntity>, IWarningRepository
    {
        public WarningRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<int> doCarWashOffline(string parkkey, string gatename)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForCarWashOffline", new SugarParameter("@parkkey", parkkey), new SugarParameter("@gatename", gatename), output);
            return output.Value.ObjToInt();
        }

        public async Task<int> doCarWashOfflineTimeout(string parkkey, string gatename, int type)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForCarWashOfflineTimeout", new SugarParameter("@parkkey", parkkey), new SugarParameter("@gatename", gatename), new SugarParameter("@type", type), output);
            return output.Value.ObjToInt();
        }

        public async Task<int> doChangeStatus(int WARNID, int warnstatus, string username)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnChangeStatus", new SugarParameter("@WARNID", WARNID), new SugarParameter("@warnstatus", warnstatus), new SugarParameter("@username", username), output);
            return output.Value.ObjToInt();
        }


        public async Task<int> doUpdate(int WARNID, int type)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            return await Db.Ado.UseStoredProcedure().GetIntAsync("spWarnUpdate", new { WARNID = WARNID, type = type });
        }

        public async Task<int> doUpdateCmd(int WARNID)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            return await Db.Ado.UseStoredProcedure().GetIntAsync("spWarnCmdUpdate", new { WARNID = WARNID });
        }

        public async Task<DataTable> getAISendList(int WARNID)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnSendListByID", new { WARNID = WARNID, warntype = 0 });
        }

        public async Task<DataTable> getListApp(int SITEID, DateTime startdate, DateTime enddate)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnListForApp", new { SITEID = SITEID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> getListPrime(int GROUPID, int SITEID, string USERID, DateTime startdate, DateTime enddate)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnListPrime", new { GROUPID = GROUPID, SITEID = SITEID, USERID = USERID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> getListType(int GROUPID, int SITEID, string USERID, int type, DateTime startdate, DateTime enddate)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnListType", new { GROUPID = GROUPID, SITEID = SITEID, USERID = USERID, type = type, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> getListByType(int GROUPID, int SITEID, string USERID, int type, DateTime startdate, DateTime enddate)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnListByType", new { GROUPID = GROUPID, SITEID = SITEID, USERID = USERID, type = type, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> getOne(int WARNID)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnGet", new { WARNID = WARNID });
        }

        public async Task<DataTable> getSendCmdList()
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnSendCmdList");
        }

        public async Task<DataTable> getSendList()
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnSendList");
        }

        public async Task<DataTable> getSendList(int WARNID)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnSendListByID", new { WARNID = WARNID, warntype = 6 });
        }

        public async Task<DataTable> getSumList(int GROUPID, string USERID, DateTime startdate, DateTime enddate)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnSumList", new { GROUPID = GROUPID, USERID = USERID, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> getTodayList(int SITEID)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnTodayList", new { SITEID = SITEID });
        }

        public async Task<DataTable> GetWarnListByCondition(string userid, string groupid, DateTime startdate, DateTime enddate, int normalai, int warntype, int warnstatus, string wpcode, int pageindex, int pagesize)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnListByCondition", new { GROUPID = groupid, USERID = userid, startdate = startdate, enddate = enddate, normalai = normalai, warntype = warntype, warnstatus = warnstatus, wpcode = wpcode, pageIndex = pageindex, pageSize = pagesize });
        }

        public async Task<int> doNjjyWarnAddRemark(object param)
        {
            return await Db.Ado.UseStoredProcedure().GetIntAsync("spWarnRemarkAdd", param);
        }

        public async Task<DataTable> doNjjyWarnSolve(object param)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnSolve", param);
        }

        public async Task<int> doNjjyWarnFinish(object param)
        {
            return await Db.Ado.UseStoredProcedure().GetIntAsync("spWarnFinish", param);
        }

        /// <summary>
        /// 插入提醒
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doInsert(object param)
        {
            return await Db.Ado.UseStoredProcedure().GetIntAsync("spSmsQueueInsert", param);
        }

        /// <summary>
        /// 获取某条告警对应的监测对象对应的警告接收者（南京建邺专用）
        /// </summary>
        /// <param name="WARNID">告警编号</param>
        /// <returns>结果集</returns>
        public async Task<DataSet> getNjjyWarnDetail(int WARNID, int type)
        {
            return await Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spNjjyWarnDetail", new { WARNID = WARNID, type = type });
        }

        /// <summary>
        /// 获取一个工地AI的30天和1年的统计件数
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警,65:区域闯入,66:黄土裸露,67:密闭运输</param>
        /// <returns>结果集</returns>
        public async Task<DataSet> getSiteAiCount(int SITEID, int type)
        {
            return await Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteAiCount", new { SITEID = SITEID, type = type });
        }

        /// <summary>
        /// 获取一个工地指定日的统计件数
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警,65:区域闯入,66:黄土裸露,67:密闭运输</param>
        /// <param name="billdate">指定日</param>
        /// <returns>结果集</returns>
        public async Task<DataTable> getSiteAiDayCount(int SITEID, int type, DateTime billdate)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteAiDayCount", new { SITEID = SITEID, type = type, billdate = billdate });
        }

        /// <summary>
        /// 获取工地列表
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警,65:区域闯入,66:黄土裸露,67:密闭运输</param>
        /// <param name="ENDWARNID">本页最后一条数据的WARNID（用于手机分页）</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">一页件数</param>
        /// <returns>结果集</returns>
        public async Task<DataSet> getSiteAiList(int SITEID, int type, DateTime billdate, int ENDWARNID, int pageindex, int pagesize)
        {
            return await Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteAiList", new
            {
                SITEID = SITEID,
                type = type,
                billdate = billdate,
                ENDWARNID = ENDWARNID,
                pageindex = pageindex,
                pagesize = pagesize
            });
        }

        public async Task<DataTable> spWarnCountForAppTop(string userid, int groupid, DateTime dt)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnCountForAppTop", new { userid = userid, groupid = groupid, billdate = dt });
        }

        public async Task<DataTable> GetWarnListForTaihu(int groupid, int SITEID, DateTime startdate, DateTime? enddate)
        {
            return await Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnSumListForTaihu", new { GROUPID = groupid, SITEID = SITEID, startdate = startdate, enddate = enddate });
        }
    }
}
