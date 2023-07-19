using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Defence
{
    public class DefenceService : BaseServices<DefenceEntity>, IDefenceService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<DefenceEntity> _defenceRepository;
        public DefenceService(IBaseRepository<DefenceEntity> defenceRepository, IUser user)
        {
            _user = user;
            _defenceRepository = defenceRepository;
            BaseDal = defenceRepository;
        }
        public async Task<int> doCheckin(string dfcode)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceCheckin", new { dfcode = dfcode });
        }

        public async Task<int> doCheckout(string dfcode)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceCheckout", new { dfcode = dfcode });
        }

        public async Task<int> doDelete(int DEFENCEID)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceDelete", new { DEFENCEID = DEFENCEID });
        }

        public async Task<int> doDisconnect(string dfcode, string switchno)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceDisconnect", new { dfcode = dfcode, switchno = switchno });
        }


        public async Task<int> doFourDisconnect(string dfcode, string switchno, string defectPosition, DateTime defectDate)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFourDefenceDisconnect", new { dfcode = dfcode, switchno = switchno, defectPosition = defectPosition, defectDate = defectDate });
        }

        public async Task<int> doFourRecover(string dfcode, DateTime recoveryDate)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFourDefenceRecover", new { dfcode = dfcode, recoveryDate = recoveryDate });
        }

        public async Task<int> doInsert(object param)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceInsert", param);
        }

        public async Task<int> doLogInsert(object param)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceLogInsert", param);
        }

        public async Task<int> doRecover(string dfcode)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceRecover", new { dfcode = dfcode });
        }

        public async Task<int> doShield(string dfcode)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceShield", new { dfcode = dfcode, @operator = _user.Name });
        }

        public async Task<int> doUnshield(string dfcode)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceUnshield", new { dfcode = dfcode, @operator = _user.Name });
        }

        public async Task<int> doUpdate(object param)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceUpdate", param);
        }

        public async Task<int> doZoneShield(int SITEID, string dfzone, int bsheild)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDefenceZoneShield", new { GROUPID = _user.GroupId, SITEID = SITEID, dfzone = dfzone, bsheild = bsheild, @operator = _user.Name });
        }

        public async Task<DataSet> getList()
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spDefenceList");
        }

        public async Task<DataTable> getListBySite(int SITEID)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spDefenceListBySite", new { SITEID = SITEID });
        }

        public async Task<DataRow> getOne(string dfcode)
        {
            DataTable dt = await _defenceRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spDefenceGet", new { dfcode = dfcode });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataTable> getServerList()
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spDefenceServerList");
        }

        public async Task<PageOutput<DefenceEntity>> GetSiteDefencePageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _defenceRepository.Db.Queryable<DefenceEntity, GcGroupEntity, GCSiteEntity>((d, g, s) => new JoinQueryInfos(JoinType.Left, d.GROUPID == g.GROUPID, JoinType.Left, d.SITEID == s.SITEID))
                .WhereIF(groupid > 0, (d, g, s) => d.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (d, g, s) => d.dfname.Contains(keyword) || s.siteshortname.Contains(keyword) || d.dfcode.Contains(keyword) || d.dfzone.Contains(keyword))
                .OrderBy((d, g, s) => new { d.SITEID, d.dfname })
                .Select<DefenceEntity>()
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<DefenceEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _defenceRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("SELECT [GROUPID]  ,[groupname]  ,[groupshortname] , (SELECT COUNT(1) FROM T_GC_Defence F WHERE F.GROUPID = g.GROUPID) count FROM[T_GC_Group] g WHERE status = 0 ORDER BY count DESC,[groupshortname]");
        }


        public async Task<DataSet> spV2DefenceStats(string keyword, int dfstatus, int bsheild, int SITEID)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure()
                .GetDataSetAllAsync("spV2DefenceStats",
                new
                {
                    SITEID = SITEID,
                    keyword = keyword,
                    dfstatus = dfstatus,
                    bsheild = bsheild
                });
        }


        public async Task<DataSet> spV2DefenceTotal(int SITEID)
        {
            return await _defenceRepository.Db.Ado.UseStoredProcedure()
                .GetDataSetAllAsync("spV2DefenceTotal", new { SITEID = SITEID });
        }


        
    }
}
