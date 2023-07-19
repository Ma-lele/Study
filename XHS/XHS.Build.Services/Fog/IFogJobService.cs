using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Fog
{
    public interface IFogJobService : IBaseServices<GCFogEntity>
    {
        Task<DataTable> GetFogKickerDataTable();
    }
}
