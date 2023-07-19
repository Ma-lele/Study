using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SpecialEqp
{
    public class SpecialEqpTypeService : BaseServices<GCSpecialEqpTypeEntity>, ISpecialEqpTypeService
    {
        private readonly IBaseRepository<GCSpecialEqpTypeEntity> _baseRepository;
        public SpecialEqpTypeService(IBaseRepository<GCSpecialEqpTypeEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }
        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,isnull(A.count,0) as count from T_GC_Group G LEFT JOIN (SELECT D.GROUPID, count(1) as count from T_GC_SpecialEqpType D GROUP BY D.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0 ORDER BY G.city, G.district, G.GROUPID");
        }

        public async Task<PageOutput<GCSpecialEqpTypeEntity>> GetSiteSpecialEqpPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCSpecialEqpTypeEntity>().WhereIF(groupid > 0, t => t.GROUPID == groupid).OrderBy("  GROUPID, bdel, SETYPEID ")
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCSpecialEqpTypeEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }
    }
}
