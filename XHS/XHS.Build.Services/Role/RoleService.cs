using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Role
{
    public class RoleService : BaseServices<SysRoleEntity>, IRoleService
    {
        private readonly IBaseRepository<SysRoleEntity> _baseRepository;
        public RoleService(IBaseRepository<SysRoleEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            base.BaseDal = baseRepository;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select a.GROUPID,a.groupshortname,sum(t.rcount) as 'COUNT' from T_GC_Group a left join (select groupid,count(1) as 'rcount' from T_SysRole where isdeleted = 0 group by groupid ) t on a.GROUPID = t.groupid or (t.groupid = 0 and a.GROUPID != t.groupid) group by a.groupid,a.groupshortname");

        }


        public async Task<PageOutput<SysRoleEntityDto>> QueryRolePage(Expression<Func<SysRoleEntity, GcGroupEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<SysRoleEntity, GcGroupEntity>((r, g) => new JoinQueryInfos(
                       JoinType.Left, r.GROUPID == g.GROUPID
                   ))
             .WhereIF(whereExpression != null, whereExpression)
             .Select(
                (r, g) => new SysRoleEntityDto
                {
                    Id = r.Id,
                    GROUPID = r.GROUPID,
                    CreateTime = r.CreateTime,
                    Enabled = r.Enabled,
                    GroupShortName = g.groupshortname,
                    IsDeleted = r.IsDeleted,
                    issys = r.issys,
                    Name = r.Name
                }
             )
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .ToPageListAsync(intPageIndex, intPageSize, totalCount);
             
            return new PageOutput<SysRoleEntityDto>() { data = list, dataCount = totalCount };
        }
    }
}
