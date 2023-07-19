using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Company
{
    public interface ICompanyService : IBaseServices<BaseEntity>
    {
        Task<DataTable> GetUnitStatisticsAsync(string city, string district,string keyword, int pageindex,int pagesize);

        Task<DataRow> GetUnitInfoAsync(string creditCode);

        Task<DataTable> GetrRelationProjectAsync(string creditCode, string keyword, int pageindex,int pagesize);

        Task<DataTable> GetPunishInfoAsync(string creditCode, string keyword, int pageindex,int pagesize);

        Task<DataRow> GetUnitCountAsync(string city, string district);

        Task<DataTable> GetSiteAsync(string projectCode);
    }
}
