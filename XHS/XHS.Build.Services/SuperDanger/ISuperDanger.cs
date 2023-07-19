using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SuperDanger
{
    public interface ISuperDanger : IBaseServices<BaseEntity>
    {
        Task<DataTable> GetListAsync(int GROUPID,int pageindex,int pagesize, string keyword);

        Task<DataTable> GetFileAsync(string SDID);

        Task<DataRow> GetOneAsync(string SDID);

        Task<DataTable> GetCount(int REGIONID);

        Task<DataTable> GetTypeCount(int REGIONID);

        Task<DataTable> GetStatusCount(int REGIONID);

        Task<DataTable> GetSiteList(int REGIONID, int pageindex, int pagesize, string keyword);

        Task<DataTable> GetBySite(string siteajcode);

    }
}
