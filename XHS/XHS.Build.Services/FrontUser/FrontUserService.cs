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

namespace XHS.Build.Services.FrontUser
{
    public class FrontUserService : BaseServices<GCUserEntity>, IFrontUserService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCUserEntity> _frontUserRepository;
        public FrontUserService(IUser user, IBaseRepository<GCUserEntity> frontUserRepository)
        {
            _user = user;
            _frontUserRepository = frontUserRepository;
            BaseDal = frontUserRepository;
        }

        /// <summary>
        /// 获取一页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        public async Task<PageOutput<GCUserListEntity>> GetPageList(string keyword, int page, int size, int GROUPID)
        {
            RefAsync<int> totalCount = 0;
            var list = await _frontUserRepository.Db.Queryable<GCUserEntity, GcGroupEntity, GCSiteEntity, GCRoleEntity>
                ((f, g, s, r) => new JoinQueryInfos(
                    JoinType.Inner, f.GROUPID == g.GROUPID,
                    JoinType.Left, f.SITEID == s.SITEID,
                    JoinType.Inner, f.ROLEID == r.ROLEID
                    ))
                .WhereIF(!string.IsNullOrEmpty(keyword), (f, g, s, r) =>
                    f.username.Contains(keyword) ||
                    f.company.Contains(keyword) ||
                    s.siteshortname.Contains(keyword) ||
                    r.rolename.Contains(keyword) ||
                    f.mobile.Contains(keyword) ||
                    f.usersitetype == 0 && keyword == "全部项目" ||
                    f.usersitetype == 2 && keyword == "多个项目")
                .WhereIF(GROUPID > 0, (f) => f.GROUPID == GROUPID)
                .Where((f) => f.bdel == 0)
                .OrderBy((f) => new { f.status }, OrderByType.Desc)
                .OrderBy((f) => new { f.username }, OrderByType.Asc)
               .Select((f, g, s, r) => new GCUserListEntity
               {
                   username = f.username,
                   SITEID = s.SITEID,
                   siteshortname = s.siteshortname,
                   groupshortname = g.groupshortname,
                   USERID = f.USERID,
                   ROLEID = f.ROLEID,
                   rolename = r.rolename,
                   status = f.status,
                   usersitetype = f.usersitetype,
                   position = f.position,
                   company = f.company,
                   mobile = f.mobile,
                   GROUPID = f.GROUPID

               })
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCUserListEntity>()
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
        public async Task<int> UserAdd(GCUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _frontUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoUserInsert",
                new SugarParameter("@GROUPID", input.GROUPID),
                new SugarParameter("@SITEID", input.SITEID),
                new SugarParameter("@ROLEID", input.ROLEID),
                new SugarParameter("@username", input.username),
                //new SugarParameter("@pwd", input.pwd),
                new SugarParameter("@operator", input.@operator),
                new SugarParameter("@mobile", input.mobile),
                new SugarParameter("@position", input.position),
                new SugarParameter("@company", input.company),
                new SugarParameter("@usersitetype", input.usersitetype),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> UserUpdate(GCUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _frontUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoUserUpdate",
                new SugarParameter("@USERID", input.USERID),
                new SugarParameter("@GROUPID", input.GROUPID),
                new SugarParameter("@SITEID", input.SITEID),
                new SugarParameter("@ROLEID", input.ROLEID),
                new SugarParameter("@username", input.username),
                //new SugarParameter("@pwd", input.pwd),
                new SugarParameter("@operator", input.@operator),
                new SugarParameter("@mobile", input.mobile),
                new SugarParameter("@position", input.position),
                new SugarParameter("@company", input.company),
                new SugarParameter("@usersitetype", input.usersitetype),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> UserDelete(GCUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _frontUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoUserDelete",
                new SugarParameter("@USERID", input.USERID),
                new SugarParameter("@operator", input.@operator),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 获取每个分组用户数量
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> GetGroupCount()
        {
            return await _frontUserRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGroupUserCount");
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> ResetPwd(GCUserEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _frontUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoUserPwdReset",
                new SugarParameter("@USERID", input.USERID),
                new SugarParameter("@operator", input.@operator),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 获取单个工地的负责人列表（勾和没勾都获取）
        /// </summary>
        /// <param name="USERID"></param>
        /// <returns></returns>
        public async Task<DataTable> getUserSiteList(int USERID)
        {
            DataTable dt = await _frontUserRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTwoUserSiteList", new { USERID = USERID });

            return dt;
        }

        /// <summary>
        /// 保存单个工地的负责人
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> saveUserSite(UserSiteInput input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);

            var param = new List<SugarParameter>();

            foreach (PropertyInfo p in input.GetType().GetProperties())
            {
                param.Add(new SugarParameter("@" + p.Name, p.GetValue(input)));
            }
            param.Add(returnvalue);

            await _frontUserRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoUserSiteRegist", param);
            return returnvalue.Value.ObjToInt();
        }
    }
}
