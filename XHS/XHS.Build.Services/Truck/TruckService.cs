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

namespace XHS.Build.Services.Truck
{
    public class TruckService : BaseServices<GCSiteTruckEntity>, ITruckService
    {
        private readonly IBaseRepository<GCSiteTruckEntity> _baseRepository;
        public TruckService(IBaseRepository<GCSiteTruckEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        //public async Task<DataTable> GetGroupTruckCount()
        //{
        //    return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGroupTruckCountList");

        //}

        public async Task<PageOutput<GCTruckPageListOutput>> GetList(string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCSiteTruckEntity, GCSiteEntity>((e, s) => new JoinQueryInfos(JoinType.Inner, e.SITEID == s.SITEID))
                .WhereIF(!string.IsNullOrEmpty(keyword), (e, s) => s.sitename.Contains(keyword) || s.siteshortname.Contains(keyword)
                || e.transcomp.Contains(keyword) || e.disposeno.Contains(keyword) || e.truckno.Contains(keyword))
                .OrderBy((e, s) => new { s.siteshortname, e.truckno })
            .Select<GCTruckPageListOutput>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCTruckPageListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

    }
}
