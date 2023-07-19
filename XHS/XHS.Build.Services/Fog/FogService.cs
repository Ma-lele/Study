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

namespace XHS.Build.Services.Fog
{
    public class FogService : BaseServices<GCFogEntity>, IFogService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCFogEntity> _fogRepository;
        public FogService(IUser user, IBaseRepository<GCFogEntity> fogRepository)
        {
            _user = user;
            _fogRepository = fogRepository;
            BaseDal = fogRepository;
        }
        public async Task<int> doCheckin(string fogcode)
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFogCheckin", new { fogcode = fogcode });
        }

        public async Task<int> doCheckout(string fogcode)
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFogCheckout", new { fogcode = fogcode });
        }

        public async Task<int> doLogInsert(object param)
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFogLogInsert", param);
        }

        public async Task<int> doTurnOff(string fogcode, string switchno)
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFogTurnOff", new { fogcode = fogcode, switchno = switchno });
        }

        public async Task<int> doTurnOn(string fogcode, string switchno)
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFogTurnOn", new { fogcode = fogcode, switchno = switchno });
        }

        public async Task<DataSet> getList()
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spFogList");
        }

        public async Task<DataTable> getListBySite(int SITEID)
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spFogListBySite", new { SITEID = SITEID });
        }

        public async Task<DataTable> getListByUser()
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spFogListByUser", new { USERID = _user.Id });
        }

        public async Task<DataTable> getServerList()
        {
            return await _fogRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spFogServerList");
        }

        public async Task<PageOutput<GCFogSitePageListOutput>> GetSiteFogPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _fogRepository.Db.Queryable<GCFogEntity, GCSiteEntity, GcGroupEntity>((f, s, g) => new JoinQueryInfos(JoinType.Left, f.SITEID == s.SITEID, JoinType.Left, s.GROUPID == g.GROUPID))
                .WhereIF(groupid > 0, (f, s, g) => s.GROUPID == groupid && g.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (f, s, g) => s.sitename.Contains(keyword) || s.siteshortname.Contains(keyword) || f.fogname.Contains(keyword) || f.fogcode.Contains(keyword))
                .OrderBy((f, s, g) => new { s.SITEID, f.fogname }, OrderByType.Desc)
            .Select<GCFogSitePageListOutput>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCFogSitePageListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _fogRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("SELECT [GROUPID]  ,[groupname] ,[groupshortname] , (SELECT COUNT(1) FROM T_GC_Fog F WHERE F.GROUPID = g.GROUPID) count,ISNULL((SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE[TABLE_NAME] = 'T_GC_Device' AND[COLUMN_NAME] = 'fogkickline'),0) hasfogkickline FROM[T_GC_Group] g where status = 0 ORDER BY count DESC,[groupshortname]");
        }
    }
}
