using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Attend
{
    public class AttendService : BaseServices<BaseEntity>, IAttendService
    {
        public readonly IUser _user;
        public readonly IBaseRepository<BaseEntity> _attendRepository;
        public AttendService(IUser user, IBaseRepository<BaseEntity> attendRepository)
        {
            _user = user;
            base.BaseDal = attendRepository;
            _attendRepository = attendRepository;
        }

        public async Task<int> DoAttendSwitch(long ATTENDID, int type)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAttendDailySwitch", new { ATTENDID = ATTENDID, type = type });

        }

        public async Task<long> DoInsert(int SITEID, float longitude, float latitude, string address, string remark)
        {
            SgParams sp = new SgParams();
            sp.Add("GROUPID", _user.GroupId);
            sp.Add("SITEID", SITEID);
            sp.Add("USERID", _user.Id);
            sp.Add("longitude", longitude);
            sp.Add("latitude", latitude);
            sp.Add("address", address);
            sp.Add("remark", remark);
            sp.NeetReturnValue();
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spUserAttendInsert", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<int> DoNoteRegist(DateTime attenddate, string operatenote)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spUserAttendNoteRegist", new { USERID = _user.Id, attenddate = attenddate, operatenote = operatenote });
        }

        public async Task<DataTable> GetAttendDaily(int GROUPID, string billdate)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAttendDaily", new { GROUPID = GROUPID, billdate = billdate });
        }

        public async Task<DataTable> GetAttendDailyOne(string billdate)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAttendDailyOne", new { USERID = _user.Id, billdate = billdate });
        }

        public async Task<DataTable> GetAttendMonth(int GROUPID, string datestart, string dateend)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAttendMonth", new { GROUPID = GROUPID, datestart = datestart, dateend = dateend });
        }

        public async Task<DataTable> GetListById(ulong ATTENDID)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUserAttendList", new { ATTENDID = ATTENDID, USERID = _user.Id });
        }

        public async Task<DataTable> GetNoteList(DateTime operatedate)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUserAttendNoteList", new { USERID = _user.Id, operatedate = operatedate });
        }

        public async Task<DataTable> spUserAttendListForUser(DateTime billdate)
        {

            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUserAttendListForUser", new { userid = _user.Id, billdate = billdate });

        }
    }
}
