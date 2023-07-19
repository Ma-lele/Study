using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Project
{
    public class ProjectService : BaseServices<BaseEntity>, IProjectService
    {
        private readonly IBaseRepository<BaseEntity> _projectRepository;

        public ProjectService(IBaseRepository<BaseEntity> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<DataRow> GetAnalyseCountAsync(int GROUPID)
        {
            DataTable dt = await _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectAnalyseCount", new { GROUPID = GROUPID });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public Task<DataTable> GetDevListAsync(int GROUPID, int pageindex, int pagesize, string keyword)
        {
            return _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectDevList", new { GROUPID = GROUPID, pageindex = pageindex, pagesize = pagesize, keyword = keyword });
        }

        public Task<DataTable> GetListAsync(int GROUPID, int pageindex, int pagesize, string keyword, string company)
        {
            return _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectList", new { GROUPID = GROUPID, pageindex = pageindex, pagesize = pagesize, keyword = keyword, company = company });

        }

        public Task<DataTable> GetAnalyseOver90Async(int GROUPID)
        {
            return _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectAnalyseOver90", new { GROUPID = GROUPID });

        }

        public Task<DataTable> GetAnalyseTypeCountAsync(int GROUPID)
        {
            return _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectAnalyseTypeCount", new { GROUPID = GROUPID });

        }

        public Task<DataTable> GetAnalyseYearCountAsync(int GROUPID, int datayear)
        {
            return _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectAnalyseYearCount", new { GROUPID = GROUPID, datayear = datayear });

        }

        public async Task<DataTable> GettAnalyseSiteListAsync(int GROUPID)
        {
            return await _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectAnalyseSiteList", new { GROUPID = GROUPID });
        }


        public Task<DataTable> GetAnalyseSiteListByTypeAsync(int GROUPID, int type)
        {
            return _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectAnalyseSiteListByType", new { GROUPID = GROUPID, type = type });

        }

        public Task<DataTable> GetIntegratorListAsync(int GROUPID, DateTime startdate, DateTime enddate, int pageindex, int pagesize, string keyword)
        {
            return _projectRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtProjectIntegratorList", new { GROUPID = GROUPID, startdate = startdate, enddate = enddate, pageindex = pageindex, pagesize = pagesize, keyword = keyword });

        }

    }
}
