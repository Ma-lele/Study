using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using System.Reflection;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.CtUser
{
    public class CtUserService : BaseServices<CTUserEntity>, ICtUserService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<CTUserEntity> _ctUserRepository;
        public CtUserService(IUser user, IBaseRepository<CTUserEntity> ctUserRepository)
        {
            _user = user;
            _ctUserRepository = ctUserRepository;
            BaseDal = ctUserRepository;
        }

        /// <summary>
        /// 获取一页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        public async Task<PageOutput<CTUserListEntity>> GetPageList(string keyword, int page, int size, int GROUPID)
        {
            RefAsync<int> totalCount = 0;
            var list = await _ctUserRepository.Db.Queryable<CTUserEntity, GcGroupEntity, CTRoleEntity>
                ((f, g, r) => new JoinQueryInfos(
                    JoinType.Inner, f.GROUPID == g.GROUPID,
                    JoinType.Inner, f.ROLEID == r.ROLEID
                    ))
                .WhereIF(!string.IsNullOrEmpty(keyword), (f, g, r) =>
                    f.username.Contains(keyword) ||
                    r.rolename.Contains(keyword) ||
                    f.mobile.Contains(keyword))
                .WhereIF(GROUPID > 0, (f) => f.GROUPID == GROUPID)
                .Where((f) => f.status <= 1)
                .OrderBy((f) => new { f.status }, OrderByType.Asc)
                .OrderBy((f) => new { f.username }, OrderByType.Asc)
               .Select((f, g, r) => new CTUserListEntity
               {
                   username = f.username,
                   USERID = f.USERID,
                   ROLEID = f.ROLEID,
                   rolename = r.rolename,
                   status = f.status,
                   mobile = f.mobile,
                   GROUPID = f.GROUPID,
                   userregion = f.userregion

               })
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<CTUserListEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> UserAdd(CTUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _ctUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spCtUserInsert",
                new SugarParameter("@GROUPID", input.GROUPID),
                new SugarParameter("@ROLEID", input.ROLEID),
                new SugarParameter("@username", input.username),
                new SugarParameter("@mobile", input.mobile),
                new SugarParameter("@userregion", input.userregion),
                new SugarParameter("@operator", _user.Name),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> UserUpdate(CTUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _ctUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spCtUserUpdate",
                new SugarParameter("@USERID", input.USERID),
                new SugarParameter("@GROUPID", input.GROUPID),
                new SugarParameter("@ROLEID", input.ROLEID),
                new SugarParameter("@username", input.username),
                new SugarParameter("@mobile", input.mobile),
                new SugarParameter("@userregion", input.userregion),
                new SugarParameter("@operator", _user.Name),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> UserDelete(CTUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _ctUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spCtUserDelete",
                new SugarParameter("@USERID", input.USERID),
                new SugarParameter("@operator", _user.Name),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 获取每个分组用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> GetGroupCount()
        {
            return await _ctUserRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGroupCtUserCount");
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> ResetPwd(CTUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _ctUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spCtUserPwdReset",
                new SugarParameter("@USERID", input.USERID),
                new SugarParameter("@operator", _user.Name),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> ChangePwd(CTUserPwd input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _ctUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spCtUserPwdChange",
                new SugarParameter("@USERID", _user.Id),
                new SugarParameter("@newpwd", input.newpwd),
                new SugarParameter("@oldpwd", input.oldpwd),
                new SugarParameter("@operator", _user.Name),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }
        /// <summary>
        /// 获取分组下一级的区域列表（分组到市，返回区列表；分组到区，返回街道列表）
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        public async Task<DataTable> GetRegions(int GROUPID)
        {
            DataTable result = await _ctUserRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtGetGroupRegions", new { GROUPID = GROUPID });
            return result;

        }

        // <summary>
        /// 更新验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="verifycode">验证码</param>
        /// <returns></returns>
        public async Task<int> SetVerifyCode(string mobile, string verifycode)
        {
            var result = await _ctUserRepository.Db.Updateable<CTUserEntity>()
                    .SetColumns(it => it.verifycode == verifycode)
                    .SetColumns(it => it.verifytime == DateTime.Now)
                    .Where(it => it.mobile == mobile)
                    .ExecuteCommandAsync();
            return result;
        }
        
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<DataTable> GetUserLogin(string username,string pwd)
        {
            return await _ctUserRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtUserLoginCheck", new { username = username, pwd = pwd });
        }
    }
}
