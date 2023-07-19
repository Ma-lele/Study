using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Unload
{
    public class UnloadRepository : BaseRepository<GCUnloadEntity>, IUnloadRepository
    {
        public UnloadRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// 获取卸料平台列表
        /// </summary>
        /// <returns>数据集</returns>
        public async Task<PageOutput<GCUnloadPageListOutput>> GetSiteUnloadPageList(int groupid,string keyword,int page,int size)
        {

            RefAsync<int> totalCount = 0;
            var list = await Db.Queryable<GCUnloadEntity, GCSiteEntity>((u, s) => new JoinQueryInfos(JoinType.Inner, u.SITEID == s.SITEID))
                .WhereIF(groupid > 0, (u, s) => u.GROUPID == groupid && s.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (u, s) => u.unloadid.Contains(keyword) || u.unloadname.Contains(keyword) || s.siteshortname.Contains(keyword))
                .OrderBy(" ULID desc")
            .Select<GCUnloadPageListOutput>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCUnloadPageListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,isnull(A.count,0) as count from T_GC_Group G LEFT JOIN (SELECT D.GROUPID, count(1) as count from T_GC_Unload D INNER JOIN T_GC_Site S ON D.GROUPID = S.GROUPID AND D.SITEID = S.SITEID GROUP BY D.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0 ORDER BY isnull(A.count,0) desc, G.city, G.district, G.GROUPID");
        }

        public async Task<int> UpdateRtdData(string unloadid, string paramjson)
        {
            return await Db.Ado.ExecuteCommandAsync("UPDATE T_GC_Unload set rtdjson = @rtdjson, rtdtime = GETDATE() where unloadid = @unloadid", new { rtdjson = paramjson, unloadid = unloadid });
        }
    }
}
