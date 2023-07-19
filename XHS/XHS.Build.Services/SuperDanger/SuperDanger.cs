using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SuperDanger
{
    public class SuperDanger : BaseServices<BaseEntity>, ISuperDanger
    {
        private readonly IBaseRepository<BaseEntity> _superDangerRepository;

        public SuperDanger(IBaseRepository<BaseEntity> superDangerRepository)
        {
            _superDangerRepository = superDangerRepository;
        }

        public Task<DataTable> GetFileAsync(string SDID)
        {
            return _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerFile", new { SDID = SDID});
        }

        public Task<DataTable> GetListAsync(int GROUPID, int pageindex, int pagesize, string keyword)
        {
            return _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerList", new { GROUPID = GROUPID, pageindex = pageindex, pagesize = pagesize, keyword = keyword });
        }

        public async Task<DataRow> GetOneAsync(string SDID)
        {

            DataTable dt = await _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerOne", new { SDID = SDID });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public Task<DataTable> GetCount(int REGIONID)
        {
            return _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerCount", new { REGIONID = REGIONID });
        }

        public Task<DataTable> GetTypeCount(int REGIONID)
        {
            return _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerTypeCount", new { REGIONID = REGIONID });
        }

        public Task<DataTable> GetStatusCount(int REGIONID)
        {
            return _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerStatusCount", new { REGIONID = REGIONID });
        }

        public Task<DataTable> GetSiteList(int REGIONID, int pageindex, int pagesize, string keyword)
        {
            return _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerSiteList", new { REGIONID = REGIONID, pageindex = pageindex, pagesize = pagesize, keyword = keyword });
        }

        public Task<DataTable> GetBySite(string siteajcode)
        {
            return _superDangerRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtSuperDangerBySite", new { siteajcode = siteajcode });
        }
    }
}
