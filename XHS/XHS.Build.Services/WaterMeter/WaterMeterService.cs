using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.WaterMeter
{
    public class WaterMeterService:BaseServices<GCWaterMeterEntity>,IWaterMeterService
    {
        private readonly IBaseRepository<GCWaterMeterEntity> _baseRepository;
        public WaterMeterService(IBaseRepository<GCWaterMeterEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        public async Task<DataTable> GetGroupWaterMeterCount()
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGroupWaterMeterCountList");

        }

        public async Task<PageOutput<GCWaterMeterPageListOutput>> GetList(int GROUPID, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCWaterMeterEntity, GCSiteEntity>((e, s) => new JoinQueryInfos(JoinType.Left, e.SITEID == s.SITEID))
                .Where((e, s) => e.bdel == 0)
                .WhereIF(GROUPID > 0, (e, s) => e.GROUPID == GROUPID && s.GROUPID == GROUPID)
                .WhereIF(!string.IsNullOrEmpty(keyword), (e, s) => s.sitename.Contains(keyword) || s.siteshortname.Contains(keyword) || e.wmetercode.Contains(keyword) || e.wmetername.Contains(keyword))
                .OrderBy((e, s) => new { s.siteshortname, e.wmetercode })
            .Select<GCWaterMeterPageListOutput>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCWaterMeterPageListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<GCWaterMeterEntity>> GetListForJob()
        {
            var list = await _baseRepository.Db.Queryable<GCWaterMeterEntity>()
                .Where(e => e.bdel == 0).ToListAsync();

            return list;
        }
    }
}
