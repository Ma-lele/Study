using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.RoleRight
{
    public class RoleRightService : BaseServices<GCRoleRightEntity>, IRoleRightService
    {
        //private readonly IUser _user;
        private readonly IBaseRepository<GCRoleRightEntity> _roleRightRepository;
        private readonly ICache _cache;
        public RoleRightService(IBaseRepository<GCRoleRightEntity> roleRightRepository, ICache cache)
        {
            //_user = user;
            _roleRightRepository = roleRightRepository;
            BaseDal = roleRightRepository;
            _cache = cache;
        }

        /// <summary>
        /// 获取指定角色的权限
        /// </summary>
        /// <param name="ROLEID"></param>
        /// <returns></returns>
        public async Task<DataTable> GetAll(int ROLEID)
        {
            return await _roleRightRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTwoRoleRightMenuGet", new { ROLEID = ROLEID });

        }

        /// <summary>
        /// 保存指定角色的权限
        /// </summary>
        /// <param name="ROLEID"></param>
        /// <param name="MENUIDS"></param>
        /// <returns></returns>
        public async Task<int> Save(string ROLEID, string MENUIDS)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _roleRightRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoRoleRightMenuInsert",
                new SugarParameter("@ROLEID", ROLEID),
                new SugarParameter("@MENUIDS", MENUIDS),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 获取指定角色的权限（市平台）
        /// </summary>
        /// <param name="ROLEID"></param>
        /// <returns></returns>
        public async Task<DataTable> GetCtAll(string ROLEID)
        {
            return await _roleRightRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtRoleRightMenuGet", new { ROLEID = ROLEID });

        }


        public async Task<IList<string>> GetFrontPermissionsAsync(string ROLEID,string prefix = "M")
        {
            var key = string.Format(XHS.Build.Common.Cache.CacheKey.FrontUserPermissions, ROLEID);
            if (await _cache.ExistsAsync(key))
            {
                return await _cache.GetAsync<IList<string>>(key);
            }
            else
            {
                var dt = await _roleRightRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spRoleRightMyMenuList", new { ROLEID = ROLEID, prefix  = prefix });
                List<string> userPermissoins = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    userPermissoins.Add(dt.Rows[i]["MENUID"].ToString());
                }


                await _cache.SetAsync(key, userPermissoins, TimeSpan.FromMinutes(30));
                return userPermissoins;
            }
        }
    }
}
