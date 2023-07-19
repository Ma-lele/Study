using System;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.UserService
{
    public class UserService : BaseServices<GCUserEntity>, IUserService
    {
        private readonly IBaseRepository<GCUserEntity> _userRepository;
        private readonly IUser _user;
        public UserService(IBaseRepository<GCUserEntity> userRepository, IUser user)
        {
            base.BaseDal = userRepository;
            _userRepository = userRepository;
            _user = user;
        }

        public async Task<DataTable> GetUserByLogin(LoginRequest loginRequest)
        {
            DataTable dt = await _userRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUserMobileLoginCheck", new { username = loginRequest.UserName, pwd = loginRequest.Password });
            if (dt != null && dt.Rows.Count != 1)
            {
                return null;
            }
            else
            {
                return dt;
            }
        }

        public async Task<DataTable> GetTenantUserByLogin(LoginRequest loginRequest)
        {
            DataTable dt = await _userRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTenantUserMobileLoginCheck", new { username = loginRequest.UserName, pwd = loginRequest.Password });
            if (dt != null && dt.Rows.Count != 1)
            {
                return null;
            }
            else
            {
                return dt;
            }
        }

        public async Task<int> changePwd(string USERID, string pwd)
        {
            return await _userRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spUserPwdReset", new { USERID = USERID, pwd = pwd });
            //return await _userRepository.changePwd(USERID, pwd);
        }

        public async Task<int> changeTenantUserPwd(string USERID, string pwd)
        {
            return await _userRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spTenantUserPwdReset", new { USERID = USERID, pwd = pwd });
            //return await _userRepository.changePwd(USERID, pwd);
        }

        public async Task<int> UserJpushIdSave(string RegistrationId, string USERUUID)
        {
            int result = await _userRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spUserJPushInsert", new { USERUUID = USERUUID, RegistrationId = RegistrationId });
            return result;
        }


        public async Task<string> GetUserJpushIds(string uuids)
        {
            string result = await _userRepository.Db.Ado.UseStoredProcedure().GetStringAsync("spUserJPushIdsGet", new { uuids = uuids });
            return result;
        }

        public async Task<DataTable> GetUserList(int GROUPID, DateTime? updatetime)
        {
            return await _userRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUserListWithUpdatetime", new { GROUPID = GROUPID, updatetime = updatetime });
            
        }

        public async Task<GCUserEntity> GetUserByUUID(string uuid)
        {
            _userRepository.CurrentDb = "XJ_Env";
            return await _userRepository.Db.Queryable<GCUserEntity>()
                .Where(ii => ii.UUID == uuid && ii.bdel == 0)
                .FirstAsync();
        }


        public async Task<GCUserEntity> QueryUser(string loginName, string password)
        {
            _userRepository.CurrentDb = "XJ_Env";
            return await _userRepository.Db.Queryable<GCUserEntity>()
                .Where(ii => ii.username == loginName && ii.pwd == password && ii.bdel == 0 && ii.status == 1)
                .FirstAsync();
        }

    }
}
