using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.CtRole
{
    public class CtRoleService : BaseServices<CTRoleEntity>, ICtRoleService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<CTRoleEntity> _ctRoleRepository;
        public CtRoleService(IUser user, IBaseRepository<CTRoleEntity> ctRoleRepository)
        {
            _user = user;
            _ctRoleRepository = ctRoleRepository;
            BaseDal = ctRoleRepository;
        }

        /// <summary>
        /// 获取一页角色
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PageOutput<CTRoleEntity>> GetPageList(string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _ctRoleRepository.Db.Queryable<CTRoleEntity>()
                .WhereIF(!string.IsNullOrEmpty(keyword), (f) => f.rolename.Contains(keyword))
                .OrderBy((f) => new { f.ROLEID }, OrderByType.Asc)
                .Select<CTRoleEntity>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<CTRoleEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        /// <summary>
        /// 新建角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> CtRoleAdd(CTRoleEntity input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _ctRoleRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spCtRoleInsert",
                new SugarParameter("@rolename", input.rolename),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }
    }
}
