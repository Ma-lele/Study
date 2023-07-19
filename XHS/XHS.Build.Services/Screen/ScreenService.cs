using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Screen
{
    public class ScreenService : BaseServices<GCScreenEntity>, IScreenService
    {
        private readonly IBaseRepository<GCScreenEntity> _baseRepository;
        public ScreenService(IBaseRepository<GCScreenEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        /// <summary>
        /// 通过大屏编号获取大屏通知
        /// </summary>
        /// <param name="screencode">大屏编号</param>
        /// <returns></returns>
        public async Task<DataRow> getNoticeBycode(string screencode)
        {
            DataTable dt = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spScreenNoticeGetByCode", new SugarParameter("@screencode", screencode));
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataRow> syncNotice(SgParams sp)
        {
            DataTable dt = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spScreenNoticeSync", sp.Params);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("SELECT [GROUPID]  ,[groupname] ,[groupshortname] , (SELECT COUNT(1) FROM[T_GC_Screen] sc WHERE sc.GROUPID = g.GROUPID) count FROM[T_GC_Group] g where status = 0 ORDER BY count DESC,[groupshortname]");
        }

        public async Task<PageOutput<VSiteScreen>> GetSiteScreenPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<VSiteScreen>()
                .WhereIF(groupid > 0, (v) => v.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (v) => v.screenname.Contains(keyword) || v.screencode.Contains(keyword))
                .OrderBy((v) => v.operatedate)
                .Select<VSiteScreen>()
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<VSiteScreen>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

    }
}
