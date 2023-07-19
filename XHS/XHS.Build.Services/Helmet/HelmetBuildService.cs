using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Helmet
{
    public class HelmetBuildService : BaseServices<GCHelmetBuildEntity>, IHelmetBuildService
    {
        private readonly IBaseRepository<GCHelmetBuildEntity> _baseRepository;
        public HelmetBuildService(IBaseRepository<GCHelmetBuildEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupHelmetBuildCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,isnull(A.count,0) as count from T_GC_Group G LEFT JOIN (SELECT D.GROUPID, count(1) as count from T_GC_HelmetBuild D INNER JOIN T_GC_Site S ON D.GROUPID = S.GROUPID AND D.SITEID = S.SITEID GROUP BY D.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0 ORDER BY G.city, G.district, G.GROUPID");
        }

        public async Task<PageOutput<HelmetBuildOutputList>> GetHelmetBuildPage(int groupid, string keyword, int page, int size, string order = "", string ordertype = "")
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCHelmetBuildEntity, GCSiteEntity>((h, s) => new JoinQueryInfos(JoinType.Inner, h.SITEID == s.SITEID))
                .WhereIF(groupid > 0, (h, s) => h.GROUPID == groupid && s.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (h, s) => h.buildname.Contains(keyword) || s.sitename.Contains(keyword) || s.siteshortname.Contains(keyword))
                .OrderByIF(!string.IsNullOrWhiteSpace(order), order + " " + ordertype)
            .Select<HelmetBuildOutputList>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<HelmetBuildOutputList>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }
    }
}
