using AutoMapper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.User
{
    public class SysUserService : BaseServices<SysUserEntity>, ISysUserService
    {
        private readonly IUser _user;
        private readonly ICache _cache;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<SysUserEntity> _userRepository;
        private readonly IBaseRepository<UserRoleEntity> _userRoleRepository;
        private readonly IBaseRepository<RolePermissionApiEntity> _rolePermissionApiRepository;

        public SysUserService(
            IUser user,
            ICache cache,
            IMapper mapper,
            IBaseRepository<SysUserEntity> userRepository,
            IBaseRepository<UserRoleEntity> userRoleRepository,
            IBaseRepository<RolePermissionApiEntity> rolePermissionApiRepository
        )
        {
            BaseDal = userRepository;
            _user = user;
            _cache = cache;
            _mapper = mapper;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _rolePermissionApiRepository = rolePermissionApiRepository;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="passwordOld">原密码</param>
        /// <param name="passwordNew">新密码</param>
        /// <returns></returns>
        public async Task<int> ResetPassword(string id, string passwordOld, string passwordNew)
        {
            var result = await _userRepository.Db.Updateable<SysUserEntity>()
                    .SetColumns(it => it.Password == passwordNew)
                    .Where(it => it.Id == id)
                    .Where(it => it.Password == passwordOld)
                    .ExecuteCommandAsync();
            return result;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _userRepository.Db.Queryable<GcGroupEntity>().Select(g => new GroupHelmetBeaconCount()
            {
                count = SqlFunc.Subqueryable<SysUserEntity>().Where(u => u.GroupId == g.GROUPID).Count(),
                GROUPID = g.GROUPID,
                groupname = g.groupname,
                groupshortname = g.groupshortname
            }).ToListAsync();
        }

        public async Task<IList<string>> GetPermissionsAsync()
        {
            var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, _user.Id);
            if (await _cache.ExistsAsync(key))
            {
                return await _cache.GetAsync<IList<string>>(key);
            }
            else
            {
                //var userPermissoins = await _rolePermissionApiRepository.Db.Queryable<SysApisEntity, RolePermissionApiEntity, UserRoleEntity>((a, b, c) =>
                // new JoinQueryInfos(
                //     JoinType.Inner, a.Id == b.ApiId,
                // JoinType.Inner, b.RoleId == c.Roleid)).Where((a, b, c) => c.Userid == _user.Id).Select((a, b, c) => a.ApiUrl).ToListAsync();
                //var userPermissoins = await _rolePermissionApiRepository.Db.Queryable<UserRoleEntity, RolePermissionApiEntity, SysPermissionEntity>((a, b, c) =>
                // new JoinQueryInfos(JoinType.Inner, b.RoleId == a.Roleid, JoinType.Inner, b.PermissionId == c.Id)).Where((a, b, c) => a.Userid == _user.Id && b.ApiId != null).Select((a, b, c) => c.ApiId).ToListAsync();
                var dt = await _userRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spUserApis", new { USERID = _user.Id });
                List<string> userPermissoins = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    userPermissoins.Add(dt.Rows[i]["apiurl"].ToString());
                }
                await _cache.SetAsync(key, userPermissoins, TimeSpan.FromMinutes(30));

                return userPermissoins;
            }
        }

        public async Task<IList<string>> GetFrontPermissionsAsync()
        {
            var key = string.Format(XHS.Build.Common.Cache.CacheKey.UserPermissions, _user.Id);
            if (await _cache.ExistsAsync(key))
            {
                return await _cache.GetAsync<IList<string>>(key);
            }
            else
            {
                _userRepository.CurrentDb = "XJ_Env";
                var apis = await _userRepository.Db.Queryable<GCRoleRightEntity, GCPermission>((role, per) => new JoinQueryInfos(
                         JoinType.Inner, role.MENUID == per.MenuID
                     ))
                    .Where((role, per) => role.ROLEID.ToString() == _user.Roleid && !SqlFunc.IsNullOrEmpty(SqlFunc.Trim(role.MENUID)))
                    .Select((role, per) => per.ApiUri)
                    .Distinct()
                    .ToListAsync();


                await _cache.SetAsync(key, apis, TimeSpan.FromMinutes(30));
                return apis;
            }
        }



        /// <summary>
        /// 更新验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="verifycode">验证码</param>
        /// <returns></returns>
        public async Task<int> SetVerifyCode(string mobile, string verifycode)
        {
            var result = await _userRepository.Db.Updateable<SysUserEntity>()
                    .SetColumns(it => it.Verifycode == verifycode)
                    .SetColumns(it => it.Verifytime == DateTime.Now)
                    .Where(it => it.Mobile == mobile)
                    .ExecuteCommandAsync();
            return result;
        }
    }
}
