using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Company
{
    public class CompanyService : BaseServices<BaseEntity>, ICompanyService
    {
        private readonly IBaseRepository<BaseEntity> _companyRepository;

        public CompanyService(IBaseRepository<BaseEntity> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public Task<DataTable> GetPunishInfoAsync(string creditCode, string keyword, int pageindex, int pagesize)
        {
            return null; 
        }

        public Task<DataTable> GetrRelationProjectAsync(string creditCode, string keyword, int pageindex,int pagesize)
        {
            return null;
        }

        public Task<DataTable> GetSiteAsync(string projectCode)
        {
            return _companyRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtCompanySite", new { sitecodes = projectCode });
        }

        public async Task<DataRow> GetUnitCountAsync(string city, string district)
        {
            DataTable dt = null;
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataRow> GetUnitInfoAsync(string creditCode)
        {
            DataTable dt = null;
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public Task<DataTable> GetUnitStatisticsAsync(string city, string district, string keyword, int pageindex, int pagesize)
        {
            return null;
        }
    }
}
