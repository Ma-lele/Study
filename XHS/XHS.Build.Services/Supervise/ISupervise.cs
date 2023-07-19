using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Supervise
{
    public interface ISupervise : IBaseServices<BaseEntity>
    {
        Task<DataTable> GetCloseCountAsync(int GROUPID,int datayear);
      
        Task<DataTable> GetCloseCountListAsync(int GROUPID,string yearmonth);

        Task<DataTable> GetCountAsync(int GROUPID,string datamonth);

        Task<DataTable> GetTypeCountAsync(int GROUPID,int datayear);

        Task<DataTable> GetTypeRankAsync(int GROUPID,string yearmonth);
    }
}
