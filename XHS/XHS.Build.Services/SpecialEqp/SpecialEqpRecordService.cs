using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using System.Linq;
using XHS.Build.Model.ModelDtos;
using System.Data;

namespace XHS.Build.Services.SpecialEqp
{
    public class SpecialEqpRecordService : BaseServices<GCSpecialEqpRecordEntity>, ISpecialEqpRecordService
    {
        private readonly IBaseRepository<GCSpecialEqpRecordEntity> _baseRepository;
        public SpecialEqpRecordService(IBaseRepository<GCSpecialEqpRecordEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,A.count as count from T_GC_Group G LEFT JOIN (SELECT D.GROUPID, count(1) as count from T_GC_SpecialEqpRecord D GROUP BY D.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0 and A.count > 0 ORDER BY isnull(A.count,0) desc,G.city, G.district, G.GROUPID");
        }

        public async Task<PageOutput<SpecialEqpRecordListOutput>> GetSiteSpecialEqpPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCSpecialEqpRecordEntity, GCSiteEntity, GCSpecialEqpEntity>((r, s, p) => new JoinQueryInfos(JoinType.Inner, r.SITEID == s.SITEID, JoinType.Left, r.SEID == p.SEID))
                .WhereIF(groupid > 0, (r, s, p) => r.GROUPID == groupid)
                 .WhereIF(!string.IsNullOrEmpty(keyword), (r, s, p) => s.siteshortname.Contains(keyword) || r.recordno.Contains(keyword) || r.rightno.Contains(keyword) || p.secode.Contains(keyword))
                 .Select<SpecialEqpRecordListOutput>()
                 .OrderBy(" SERID DESC").ToPageListAsync(page, size, totalCount);


            var data = new PageOutput<SpecialEqpRecordListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<List<GCSpecialEqpEntity>> GetEqp(int GROUPID, int SITEID, int setype)
        {
            return await _baseRepository.Db.Queryable<GCSpecialEqpEntity>()
                .WhereIF(GROUPID > 0, ii => ii.GROUPID == GROUPID)
                .WhereIF(SITEID > 0, ii => ii.SITEID == SITEID)
                .WhereIF(setype > 0, ii => ii.setype == setype)
                .ToListAsync();
        }

        public async Task<int> AddProof(List<GCSpecialEqpRecordProof> input)
        {
            return await _baseRepository.Db.Insertable(input).ExecuteCommandAsync();
        }

        public async Task<List<ImgDto>> GetImgs(int SERID)
        {
            var data = await _baseRepository.Db.Queryable<GCSpecialEqpRecordProof>()
                .Where(ii => ii.SERID == SERID)
                .Select(ii => new ImgDto
                {
                    name = ii.filename,
                    url = ii.SERPROOFID.ToString()
                })
                .ToListAsync();

            return data;
        }

        public async Task<int> DelImgs(List<GCSpecialEqpRecordProof> list)
        {
            return await _baseRepository.Db.Deleteable(list).ExecuteCommandAsync();
        }

        public async Task<DataSet> GetOneAsync(int SITEID, string secode)
        {
            var ds = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spV2SpecialRecord", new { SITEID = SITEID, secode = secode });
            ds.Tables[0].TableName = "main";
            ds.Tables[1].TableName = "imgs";
            return ds;
        }
    }
}
