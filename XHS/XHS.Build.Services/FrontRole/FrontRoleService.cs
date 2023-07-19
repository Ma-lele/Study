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

namespace XHS.Build.Services.FrontRole
{
    public class FrontRoleService : BaseServices<GCRoleEntity>, IFrontRoleService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCRoleEntity> _frontRoleRepository;
        public FrontRoleService(IUser user, IBaseRepository<GCRoleEntity> frontRoleRepository)
        {
            _user = user;
            _frontRoleRepository = frontRoleRepository;
            BaseDal = frontRoleRepository;
        }

        /// <summary>
        /// 获取一页角色
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PageOutput<GCRoleEntity>> GetPageList(string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _frontRoleRepository.Db.Queryable<GCRoleEntity>()
                .WhereIF(!string.IsNullOrEmpty(keyword), (f) => f.rolename.Contains(keyword))
                .Where((f) => f.ROLEID > 1)
                .OrderBy((f) => new { f.ROLEID }, OrderByType.Asc)
                .Select<GCRoleEntity>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCRoleEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }
    }
}
